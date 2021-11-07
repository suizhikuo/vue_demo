// MSTrustManagerImpl.java
//
// Copyright (c) 2001 Brian Boyter
// All rights reserved
//
// This software is released subject to the GNU Public License.  See
// the full license included with this distribution.
//
// Methods:	getAcceptedIssuers()
//		checkClientTrusted()
//		checkServerTrusted()
//		


package com.boyter.mscrypto;

import java.security.KeyStore; 
import java.security.KeyStoreException;
import java.security.cert.X509Certificate;
import javax.net.ssl.X509TrustManager;
import java.security.cert.CollectionCertStoreParameters;
import java.security.cert.CertStore;
import java.security.cert.X509CertSelector;
import java.util.Date;
import java.util.Collection;


final class MSTrustManagerImpl implements X509TrustManager { 
    MSTrustManagerImpl(KeyStore ks) throws KeyStoreException { }

	static MSCryptoFunctions MSF = new MSCryptoFunctions();
	static boolean debug = (System.getProperty("mscrypto.debug") != null);


//****************************************************************************
// Reads Microsoft certificate store
// Returns array of trusted root CA certificates 
    public X509Certificate[] getAcceptedIssuers() {

	Object[] objarray = null;
	X509Certificate[] CAArray = null;

	try {
		if (debug) System.out.println("getAcceptedIssuers: entered\n");

		CertStore CACerts = com.boyter.mscrypto.MSValidCertificate.getCACerts();

		X509CertSelector xcs = new X509CertSelector();			
		xcs.setCertificateValid(new Date());

		Collection certcollection = CACerts.getCertificates(xcs);
		if (debug) System.out.println(
			"getAcceptedIssuers: " + certcollection.size() + " certs found");

		CAArray = new X509Certificate[certcollection.size()];
		CAArray = (X509Certificate[]) certcollection.toArray(CAArray);

	} catch (Exception e) {
		e.printStackTrace();
	}

//	if (debug) for (int i=0; i<CAArray.length; i++)
//		System.out.println(i + "  " + CAArray[i].getSubjectDN());

	return CAArray;
    }


//****************************************************************************
// Returns true if the client is authorized to access the server.
    public void checkClientTrusted(X509Certificate chain[], String authType) {

//	int DontKnowFlag=0;	// reject the cert if we cannot tell if it is revoked
//	int DontKnowFlag=1;	// accept the cert anyway
	int DontKnowFlag=2;	// ask the user

	if (debug) System.out.println("checkClientTrusted: Entered");

	if (com.boyter.mscrypto.MSValidCertificate.isCertChainValid(chain, DontKnowFlag))
		return;

	// client cert is not trusted
	System.out.println("Client Certificate is not Trusted - aborting");
	System.exit(2);
    }


//****************************************************************************
// Returns true if the server is authorized to access the client. 
    public void checkServerTrusted(X509Certificate chain[], String authType) {

	int DontKnowFlag=2;	// ask the user

	if (debug) System.out.println("checkServerTrusted: Entered");

	if (com.boyter.mscrypto.MSValidCertificate.isCertChainValid(chain, DontKnowFlag))
		return;

	// server cert is not trusted
	System.out.println("Server Certificate is not Trusted - aborting");
	System.exit(2);
    }
} 

