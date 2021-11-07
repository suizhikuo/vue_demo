
// MSKeyManagerImpl.java
//
// Copyright (c) 2001 Brian Boyter
// All rights reserved
//
// This software is released subject to the GNU Public License.  See
// the full license included with this distribution.
//
// Methods:	chooseClientAlias()
//		chooseServerAlias()
//		getCertificateChain()
//		getClientAliases()
//		getServerAliases()
//		getPrivateKey()
//		

package com.boyter.mscrypto;

import java.io.ByteArrayInputStream;
import java.net.Socket;
import java.util.List;
import java.util.ArrayList;
import java.util.Iterator;
import java.security.KeyStore; 
import java.security.PublicKey;
import java.security.PrivateKey;
import java.security.interfaces.RSAPrivateKey;
import java.security.interfaces.RSAPrivateCrtKey;
import java.security.interfaces.RSAPublicKey;
import java.security.Principal;
import java.security.KeyFactory;
import java.security.KeyStoreException;
import java.security.cert.CertificateFactory;
import java.security.cert.X509Certificate;
import java.security.cert.Certificate;
import java.security.spec.RSAPrivateKeySpec;
import java.security.spec.RSAPrivateCrtKeySpec;
import java.security.cert.CertificateNotYetValidException;
import java.security.cert.CertificateExpiredException;
import java.math.BigInteger;
import javax.net.ssl.X509KeyManager;


final class MSKeyManagerImpl implements X509KeyManager { 
   MSKeyManagerImpl(KeyStore ks, char[] passphrase) throws KeyStoreException { }
	static MSCryptoFunctions MSF = new MSCryptoFunctions();
	static boolean debug = (System.getProperty("mscrypto.debug") != null);


//****************************************************************************
// Choose an alias to authenticate the client side of a secure socket
// given the public key type and the list of certificate issuer
// authorities recognized by the peer (if any). 
   public String chooseClientAlias(String[] keyType, Principal[] issuers, Socket socket) {
	String alias=null;

	if (debug) System.out.println("chooseClientAlias: entered");

	try {
		String[] aliases = getClientAliases(keyType[0], issuers);
		if (aliases == null) {
			if (debug) System.out.println("\nchooseClientAlias: something wrong - no aliases\n");
			return null;
		}
		alias = aliases[0];
	} catch (Exception e) {
		e.printStackTrace(); 
	}

	if (debug) System.out.println("\nchooseClientAlias: " + alias);
	return alias;
   }


//****************************************************************************
// Choose an alias to authenticate the server side of a secure socket
// given the public key type and the list of certificate issuer authorities
// recognized by the peer (if any). 
   public String chooseServerAlias(String keyType, Principal[] issuers, Socket socket) {
	String alias=null;

	if (debug) System.out.println("chooseServerAlias: return server alias");

	try {
		String[] aliases = getServerAliases(keyType, issuers);
		if (aliases == null) {
			if (debug) System.out.println("\nchooseServerAlias: something wrong - no aliases\n");
			return null;
		}
		alias = aliases[0];
	} catch (Exception e) {
		e.printStackTrace(); 
	}

	if (debug) System.out.println("\nchooseServerAlias: " + alias);
	return alias;
   }


//****************************************************************************
// Returns the certificate chain to validate the given alias.        
   public X509Certificate[] getCertificateChain(String alias) {

	if (debug) System.out.println("getCertificateChain: entered, alias:" + alias);

	X509Certificate[] CertChain = null;
	X509Certificate cert=null;

	try {
		byte[] certblob = MSF.MSgetCert("My", alias);

		CertificateFactory cf = CertificateFactory.getInstance("X.509");
		ByteArrayInputStream bais = new ByteArrayInputStream(certblob);
		cert = (X509Certificate) cf.generateCertificate(bais);
		bais.close();

		CertChain = com.boyter.mscrypto.MSValidCertificate.getCertChain(cert);

	} catch (Exception e) {
		e.printStackTrace(); 
	}

	return CertChain; 
    }


//****************************************************************************
// Get the matching aliases for authenticating the client side of
// a secure socket given the public key type and the list of certificate
// issuer authorities recognized by the peer (if any).   
   public String[] getClientAliases(String keyType, Principal[] issuers) {
	String[] ValidAliases=null;
	if (debug) System.out.println("getClientAliases: entered");
	int i;

	try {
		String[] aliases = MSF.MSgetAliases("My");
		if (aliases == null) {
			System.out.println("\nNo client aliases found - fatal error\n");
			java.lang.System.exit(1);
		}

		// now throw out any aliases not signed by an approved issuer,
		// expired, or revoked
		if (debug) System.out.println("Number of accepted issuers: " + issuers.length);
		ValidAliases = CheckAlias(aliases, issuers);

	} catch (Exception e) {
		e.printStackTrace(); 
	}

	if (debug) {
		System.out.println("aliases found: " + ValidAliases.length);
			for (i=0; i<ValidAliases.length; i++)
		System.out.println("getClientAliases: alias: " + ValidAliases[i]);
	}
	return ValidAliases; 
    }


//****************************************************************************
// Get the matching aliases for authenticating the server side of
// a secure socket given the public key type and the list of
// certificate issuer authorities recognized by the peer (if any).          
   public String[] getServerAliases(String keyType, Principal[] issuers) {

	String[] ValidAliases=null;
	if (debug) System.out.println("getServerAliases: return array of aliases ");

	try {
		String[] aliases = MSF.MSgetAliases("My");
		if (aliases == null) {
			if (debug) System.out.println("\nNo server aliases found\n");
			return null;
		}

		// now throw out any aliases not signed by an approved issuer,
		// expired, or revoked
		ValidAliases = CheckAlias(aliases, issuers);

	} catch (Exception e) {
		e.printStackTrace(); 
	}

	if (debug) {
		System.out.println("aliases found: " + ValidAliases.length);
		for (int i=0; i<ValidAliases.length; i++)
			System.out.println("getServerAliases: alias: " + ValidAliases[i]);
	}
	return ValidAliases; 
    }



//****************************************************************************
// returns the RSA private key for the given alias
   public PrivateKey getPrivateKey(String alias) {
	RSAPrivateKey rsaprivkey=null;
	RSAPrivateCrtKey rsaprivcrtkey=null;
	BigInteger mod=null;
	BigInteger exp=null;
	BigInteger coeff=null;
	BigInteger p=null;
	BigInteger q=null;
	BigInteger expp=null;
	BigInteger expq=null;
	BigInteger pubexp=null;
	byte[] pubexpblob = new byte[4];
	byte[] keysizeblob = new byte[4];
	int keysize;

	if (debug) System.out.println("getPrivateKey: entered, alias: " + alias);

	try {
	   int i;
	   byte[] keyblob = MSF.MSgetPrivateKey(alias);
	   KeyFactory kf = KeyFactory.getInstance("RSA");

	   if (keyblob == null) {		// generate a dummy key
		byte[] modblob = new byte[128];
		for(i=0; i<128; i++) 
			modblob[i] = 127;
		mod = new BigInteger(modblob);
		exp = mod;

		RSAPrivateKeySpec privKeySpec = new RSAPrivateKeySpec(mod, exp);
	        rsaprivkey = (RSAPrivateKey) kf.generatePrivate( privKeySpec );

		if (debug) System.out.println("getPrivateKey: normal exit");
		return rsaprivkey;

	   } else {			// use the key that got exported
		for(i=0; i<4; i++) {
			pubexpblob[i]  = keyblob[19-i];
			keysizeblob[i] = keyblob[15-i];
		}
		BigInteger bigkeysize = new BigInteger(keysizeblob);
		keysize = bigkeysize.intValue();
		if (debug) System.out.println("keysize: " + keysize);

		byte[] modblob   = new byte[ (keysize/8) ];
		byte[] expblob   = new byte[ (keysize/8) ];
		byte[] pblob     = new byte[ keysize/16 ];
		byte[] qblob     = new byte[ keysize/16 ];
		byte[] exppblob  = new byte[ keysize/16 ];
		byte[] expqblob  = new byte[ keysize/16 ];
		byte[] coefblob  = new byte[ keysize/16 ];

		for(i=0; i<keysize/8; i++) {
			modblob[i] = keyblob[19-i+(keysize/16)*2];
			expblob[i] = keyblob[19-i+(keysize/16)*9];
		}

		for(i=0; i<keysize/16; i++) {
			pblob[i]    = keyblob[19-i+(keysize/16)*3];
			qblob[i]    = keyblob[19-i+(keysize/16)*4];
			exppblob[i] = keyblob[19-i+(keysize/16)*5];
			expqblob[i] = keyblob[19-i+(keysize/16)*6];
			coefblob[i] = keyblob[19-i+(keysize/16)*7];
		}

		mod    = new BigInteger(1, modblob);
		exp    = new BigInteger(1, expblob);
		coeff  = new BigInteger(1, coefblob);
		p      = new BigInteger(1, pblob);
		q      = new BigInteger(1, qblob);
		expp   = new BigInteger(1, exppblob);
		expq   = new BigInteger(1, expqblob);
		pubexp = new BigInteger(1, pubexpblob);

		RSAPrivateCrtKeySpec privCrtKeySpec = 
		  new RSAPrivateCrtKeySpec(mod, pubexp, exp, p, q, expp, expq, coeff);
      		rsaprivcrtkey = (RSAPrivateCrtKey) kf.generatePrivate( (RSAPrivateKeySpec)privCrtKeySpec );
	   }
	} catch (Exception e) {
		e.printStackTrace(); 
	}


//	System.out.println("mod:     " + rsaprivcrtkey.getModulus());
//	System.out.println("pubexp:  " + rsaprivcrtkey.getPublicExponent());
//	System.out.println("privexp: " + rsaprivcrtkey.getPrivateExponent());
//	System.out.println("p:       " + rsaprivcrtkey.getPrimeP());
//	System.out.println("q:       " + rsaprivcrtkey.getPrimeQ());
//	System.out.println("expp:    " + rsaprivcrtkey.getPrimeExponentP());
//	System.out.println("expq:    " + rsaprivcrtkey.getPrimeExponentQ());
//	System.out.println("coeff:   " + rsaprivcrtkey.getCrtCoefficient());

	if (debug) System.out.println("getPrivateKey: normal exit");
	return rsaprivcrtkey;
   }



//****************************************************************************
// helper function
// remove any aliases not signed by an approved issuer,
// expired, or revoked
    static String[] CheckAlias(String[] aliases, Principal[] issuers) {

	X509Certificate cert=null;
	List AliasList = new ArrayList();
	List IssuerList = new ArrayList();

	for (int i=0; i<aliases.length; i++)
		AliasList.add(aliases[i]);

	if (issuers != null)
		for (int i=0; i<issuers.length; i++)
			IssuerList.add(issuers[i].toString());
	try {
		// iterate thru the list of aliases
		Iterator iter = AliasList.iterator();
		while (iter.hasNext()) {
			String alias = (String) iter.next();

			// get the cert for this alias
			byte[] certblob = MSF.MSgetCert("My", alias);
			CertificateFactory cf = CertificateFactory.getInstance("X.509");
			ByteArrayInputStream input = new ByteArrayInputStream(certblob);
			cert = (X509Certificate) cf.generateCertificate(input);
			input.close();

			// is this alias's cert signed by an approved issuer?
			if (issuers != null) {
				String certIssuer = cert.getIssuerDN().toString();
				if (!IssuerList.contains(certIssuer)) {
					iter.remove();
					if (debug)
						System.out.println("CheckAlias: no issuer found for alias " + alias);
					continue;
				}
			}

			if (!com.boyter.mscrypto.MSValidCertificate.isCertValid(cert, 1)) {
				iter.remove();
				if (debug)
					System.out.println("CheckAlias: cert is expired or revoked for alias " + alias);
				continue;
			}

			if (debug) System.out.println("CheckAlias: alias is valid " + alias);
		}

	aliases = new String[AliasList.size()];
	AliasList.toArray(aliases);	

	} catch (Exception e) {
		e.printStackTrace(); 
	}
	return aliases;
    }
}