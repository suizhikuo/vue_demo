// MSValidCertificate.java
//
// Copyright (c) 2001 Brian Boyter
// All rights reserved
//
// This software is released free of cost subject to
// the license included with this distribution.
//
// Methods:	isCertValid()
//		isCertChainValid()
//		getCACerts()
//		


package com.boyter.mscrypto;
  
import java.security.Principal;
import java.security.cert.CertificateFactory;
import java.security.cert.X509Certificate;
import java.security.cert.Certificate;
import java.security.PublicKey;
import java.security.SignatureException;
import java.io.InputStreamReader;
import java.io.ByteArrayInputStream;
import java.io.InputStream;
import java.security.cert.CollectionCertStoreParameters;
import java.security.cert.CertStore;
import java.security.cert.X509CertSelector;
import java.util.Date;
import java.util.Collection;
import java.util.List;
import java.util.ArrayList;
import java.util.Iterator;


final public class MSValidCertificate { 

   static MSCryptoFunctions MSF = new MSCryptoFunctions();
   static boolean debug = (System.getProperty("mscrypto.debug") != null);
   static CertStore CACerts=null;



//****************************************************************************
// Returns true if the cert is valid.
//
//   Certificate is valid if:
//   a. Has a chain of trust back to a trusted root CA.
//   b. The certs in the cert chain are not expired.
//   c. The certs in the cert chain have not been revoked.
//   d. The cert is valid for the purpose it is being used.
//
//   The DontKnowFlag tells this routine what to do if the
//   revocation status of the cert cannot be established:
//   0 = reject the cert
//   1 = accept the cert anyway
//   2 = prompt the user what he wants to do
//

   final public static boolean isCertValid(X509Certificate cert, int DontKnowFlag) {

	if (debug) System.out.println("isCertValid: Entered");

	// Get the certificate chain
	X509Certificate[] CertChain = getCertChain(cert);
	if (CertChain == null)
		return false;

	return isCertChainValid(CertChain, DontKnowFlag);
   }




//****************************************************************************
//
   public static boolean isCertChainValid(X509Certificate[] certchain, int DontKnowFlag) {

	if (debug) System.out.println("isCertChainValid: Entered");

	for (int i = 0; i < certchain.length; i++) {
		X509Certificate cert = certchain[i];
		Principal subjectDN = cert.getSubjectDN();
		if (debug) System.out.println("  CertChain: " + subjectDN.toString());
		Principal issuerDN  = cert.getIssuerDN();
		X509Certificate issuercert = cert;

		if (i < certchain.length-1)		// true if not a root CA cert
			issuercert = certchain[i+1];	// issuer is next cert in the chain

		// is cert within validity period?
		if (!isCertWithinValidityPeriod(cert))
			return false;

		// verify the signature on the cert
		if (!verifySignature(issuercert, cert))
			return false;

		// is cert revoked?
		if (subjectDN.equals(issuerDN)) {	// test if this is a root CA cert
			if (debug) System.out.println("\n Assume root CA certs are never revoked\n");
			continue;	// skip the root CA
		}
		if (isCertRevoked(cert, DontKnowFlag))
			return false;

	} 

	if (debug) System.out.println("isCertChainValid: yes - cert chain is trusted");
	return true; 
    }



//****************************************************************************
// check if the cert is revoked
   final static boolean isCertRevoked(X509Certificate cert, int DontKnowFlag) {

	byte[] certblob = null;

	if (debug) System.out.println("isCertRevoked: Entered   flag: " + DontKnowFlag);

	try {
		certblob = cert.getEncoded();
	} catch (Exception e) {
		e.printStackTrace();
	}

	// Does the cert have a CDP (CRL distribution point)???
	byte[] CDPblob = cert.getExtensionValue("2.5.29.31");
	if (CDPblob == null) {
		if (debug) System.out.println("isCertRevoked: cert does not contain a CDP");
		System.out.println("Cannot determine if certificate is revoked (no CDP)");
		return AskUserWhatHeWantsToDo(DontKnowFlag);
	}

	// yes there is a CDP - ASN parse the CDP
	String[] URLarray = MSparseCDP.parseCDP(CDPblob);
	boolean CRLdownloadOK = false;
	if (URLarray != null) {
		for (int i=0; i<URLarray.length; i++) {
			String URL = URLarray[i];

			// go fetch that CRL
			if (debug) System.out.println("isCertRevoked: fetching the CRL, URL: " + URL);
			if (MSF.MSgetCRL(URL)) {
				CRLdownloadOK = true;
				break;	// url was fetched correctly
			} else
				System.out.println("Download  failed for CRL: " + URL);
		}
		if (!CRLdownloadOK)
			return AskUserWhatHeWantsToDo(DontKnowFlag);
	}

	// is the cert revoked???
	int revocationStatus = MSF.MSVerifyCertRevocation(certblob);

	if (debug) System.out.println("isCertRevoked: revocationStatus: " + revocationStatus );
	switch (revocationStatus) {
	case 0:
		// cert is revoked
		System.out.println("Certificate " + cert.getSubjectDN().toString() + "is revoked");
		return AskUserWhatHeWantsToDo(DontKnowFlag);
	case 1:
		// cert is not revoked
		if (debug) System.out.println("isCertRevoked: the cert has not been revoked");
		return false;
	default:
	}

	System.out.println("Cannot determine if certificate " + cert.getSubjectDN().toString() + "is revoked");
	return AskUserWhatHeWantsToDo(DontKnowFlag);
    }



//****************************************************************************
// helper function
    final static boolean AskUserWhatHeWantsToDo(int DontKnowFlag) {

	switch(DontKnowFlag) {
	case 0:		// accept the cert anyway
		return false;
	case 1:		// reject the cert
		return true;
	default:	// ask the user what he wants to do
	}

	InputStreamReader in = new InputStreamReader(System.in);
	while(true)
	try {
		System.out.print("Do you want to (a)accept the cert anyway or (r)reject the cert? :");
		int c = in.read();	// read a character
		switch(c) {
		case 'a':
			in.close();
			System.out.println("certificate accepted");
			return false;
		case 'r':
			in.close();
			System.out.println("certificate rejected");
			return true;
		default:	// prompt user again for option
		}
	} catch (Exception e) {
		e.printStackTrace();
	}
   }


//****************************************************************************
// helper function
   final static boolean isCertWithinValidityPeriod(X509Certificate cert) {

	// check the validity period
	try {
		cert.checkValidity();
	} catch (java.security.cert.CertificateExpiredException e) {
		if (debug) System.out.println("isCertWithinValidityPeriod: NO - cert is expired");
		return false;
	} catch (java.security.cert.CertificateNotYetValidException e) {
		if (debug) System.out.println("isCertWithinValidityPeriod: NO - cert is notYetValid");
		return false;
	} catch (Exception e) {
		e.printStackTrace();
		return false;
	}

	return true;
   }



//****************************************************************************
// helper function
   final static boolean verifySignature(X509Certificate issuercert, X509Certificate cert) {
	PublicKey key;

	key = issuercert.getPublicKey();
	try {
		cert.verify(key);
	} catch (SignatureException e) {
		if (debug) System.out.println("verifySignature: NO - cert has been corrupted");
		return false;
	} catch (Exception e) {
		e.printStackTrace(); 
	}

	if (debug) System.out.println("verifySignature: Yes - cert signature verified");
	return true;
   }


//****************************************************************************
// Reads Microsoft certificate store
// Returns array of trusted root CA certificates 
    final public static CertStore getCACerts() { 

	if (debug) System.out.println("getCACerts: entered\n");

	if (CACerts != null)
		return CACerts;

	try {
		String[] encodedCerts = MSF.MSgetCACerts();
		if (debug) System.out.println("getCACerts: " + encodedCerts.length + " CA certs found");
		ArrayList CAList = new ArrayList(encodedCerts.length);

		CertificateFactory cf = CertificateFactory.getInstance("X.509");

		for (int i=0; i<encodedCerts.length; i++) {
			InputStream input = new ByteArrayInputStream(encodedCerts[i].getBytes("UTF-8"));
			X509Certificate cert = (X509Certificate) cf.generateCertificate(input);
			input.close();
			CAList.add(cert);
		}

		if (debug) System.out.println("getCACerts: get list of CRL URLs");

		CollectionCertStoreParameters CAccsp = 
			new CollectionCertStoreParameters(CAList);
		CACerts = CertStore.getInstance("Collection", CAccsp);

	} catch (Exception e) {
		e.printStackTrace(); 
	}

	if (debug) System.out.println("getCACerts: normal exit");
	return CACerts;
    }


//****************************************************************************
// Returns the certificate chain to validate the given alias.        
   final public static X509Certificate[] getCertChain(X509Certificate cert) {

	if (debug) System.out.println("getCertChain: entered");
	List CertChainList = new ArrayList();
	X509Certificate issuercert;
	boolean match;
	X509Certificate[] issuerArray = null;

	try {
		getCACerts();

		Principal subject = cert.getSubjectDN();
		Principal issuer  = cert.getIssuerDN();
		if (debug) System.out.println("  add to cert chain: " + subject + "\n");
		CertChainList.add(cert);

		while (!(issuer.equals(subject))) {	// stop if issuer==subject (root CA)
			match = false;

			X509CertSelector xcs = new X509CertSelector();			
			xcs.setCertificateValid(new Date());
//			xcs.setSubject( issuer.toString() );
			Collection certcollection = CACerts.getCertificates(xcs);

// using setSubject on the X509CertSelector is broken.
// Note that it is commented out above.
// This got broken in JDK 1.4.0-rc and may work again in the future....
// In the meantime, the workaround is below.  Instead of using CertSelector
// to find certs with a specific subject, we must loop thru the certcollection
// and remove certs that do not match the desired subject
//
			Iterator iter = certcollection.iterator();
			while ( iter.hasNext() ) {
				X509Certificate cacert = (X509Certificate) (iter.next());
				if ( !cacert.getSubjectDN().equals(issuer) )
					iter.remove();
			}

			if (debug) System.out.println(certcollection.size() + " certs found");
			issuerArray = new X509Certificate[certcollection.size()];
			issuerArray = (X509Certificate[]) certcollection.toArray(issuerArray);

			for (int i=0; i<issuerArray.length; i++)
				if (verifySignature(issuerArray[i], cert)) {
					match = true;
					cert = issuerArray[i];
					subject = cert.getSubjectDN();
					issuer  = cert.getIssuerDN();
					if (debug) System.out.println("  add to cert chain: " + subject + "\n");
					CertChainList.add(cert);
					break;
				}
			if (!match) {
				if (debug) System.out.println("\nERROR - certchain is broken\n");
				return null;
			}
		}
	} catch (Exception e) {
		e.printStackTrace(); 
	}

	X509Certificate[] CertChain = new X509Certificate[CertChainList.size()];
	CertChainList.toArray(CertChain);

	if (debug) System.out.println(CertChain.length + " certs in cert chain");

	for(int i=0; i<CertChain.length; i++)
		if (debug) System.out.println("getCertChain: " + CertChain[i].getSubjectDN() + "\n");
	return CertChain; 
    }
}