// MSRSASignFactoryImpl.java
//
// Copyright (c) 2001 Brian Boyter
// All rights reserved
//
// This software is released subject to the GNU Public License.  See
// the full license included with this distribution.
//
// Methods:	setMessageDigestType();
//		engineSetParameter()
//		engineInitSign()
//		engineSign()
//		engineUpdate()
//		engineInitVerify()
//		engineVerify()

package com.boyter.mscrypto;
import java.security.AlgorithmParameters;
import java.security.SignatureSpi;
import java.security.Signature;
import java.security.PrivateKey;
import java.security.PublicKey;
import java.security.interfaces.RSAPublicKey;
import java.security.spec.AlgorithmParameterSpec;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;


public class MSRSASignFactoryImpl extends SignatureSpi {

	static MSCryptoFunctions MSF = new MSCryptoFunctions();
	static boolean debug = (System.getProperty("mscrypto.debug") != null);
	static RSAPublicKey rsaPublicKey;
	static boolean signOpInProgress   = false;
	static boolean verifyOpInProgress = false;
	static Signature jsse;
	static MessageDigest MD;
	static String MessageDigestType;


//****************************************************************************
	protected void setMessageDigestType(String MDType) {
		try {
			MD = MessageDigest.getInstance(MDType);
			MessageDigestType = MDType;
		} catch (NoSuchAlgorithmException e) {
			e.printStackTrace();
		}

		if (debug) System.out.println("MSRSASignFactoryImpl:setMessageDigestType " + MDType);
	}


//****************************************************************************
	protected Object engineGetParameter(String param) {
		System.out.println("MSSHARSASignFactoryImpl: engineGetParameter: not implemented");
		return null;
	}


//****************************************************************************
// This method is overridden by providers to initialize this signature
// engine with the specified parameter set. 
	protected  void engineSetParameter(AlgorithmParameterSpec params) {
		System.out.println("MSSHARSASignFactoryImpl: engineSetParameter: not implemented");
	}


//****************************************************************************
// Deprecated. Replaced by engineSetParameter(AlgorithmParameterSpec params)
	protected void engineSetParameter(String param, Object value) {
		System.out.println("MSSHARSASignFactoryImpl: engineSetParameter: not implemented");
	}
 


//****************************************************************************
// Initializes this signature object with the specified private key for
// signing operations.
	protected void engineInitSign(PrivateKey privateKey) {

		if (debug) System.out.println("MSSHARSASignFactoryImpl: engineInitSign: entered");

		try {
			signOpInProgress   = true;
			verifyOpInProgress = false;
		} catch (Exception e) {
			e.printStackTrace(); 
		}
	}



//****************************************************************************
// Returns the signature bytes of all the data updated so far. 
	protected byte[] engineSign() {

		if (debug) System.out.println("MSSHARSASignFactoryImpl: engineSign: entered");

		if (!signOpInProgress) {
			System.out.println("MSSHARSASignFactoryImpl: error - throw exception");
			return null;
		}

		byte[] hash  = MD.digest();
		byte[] mssig = MSF.MSrsaSignHash(hash, (byte[])null, MessageDigestType);
		signOpInProgress = false;
		return mssig;
	}



//****************************************************************************
// Finishes this signature operation and stores the resulting signature
// bytes in the provided buffer outbuf, starting at offset.
// returns the number of bytes placed into outbuf 
	protected  int engineSign(byte[] outbuf, int offset, int len) {

		if (debug) System.out.println("MSSHARSASignFactoryImpl: engineSign: entered");

		if (!signOpInProgress) {
			System.out.println("MSSHARSASignFactoryImpl: error - throw exception");
			return 0;
		}

		byte[] hash  = MD.digest();
		byte[] mssig = MSF.MSrsaSignHash(hash, (byte[])null, MessageDigestType);
		java.lang.System.arraycopy((Object)mssig, 0, (Object)outbuf, offset, mssig.length);
		signOpInProgress = false;
		return mssig.length;
	}



//****************************************************************************
// Updates the data to be signed or verified using the specified byte.
	protected void engineUpdate(byte b) {

		if (debug) System.out.println("MSSHARSASignFactoryImpl: engineUpdate: entered");

		try {
			if (signOpInProgress) {
				MD.update(b);
			} else

			if (verifyOpInProgress) {
				jsse.update(b);
			}
		} catch (Exception e) {
			e.printStackTrace(); 
		}
	}



//****************************************************************************
// Updates the data to be signed or verified, using the specified array
// of bytes, starting at the specified offset.
	protected void engineUpdate(byte[] data, int off, int len) {
		int i;
		if (debug) System.out.println("MSSHARSASignFactoryImpl: engineUpdate: entered");

		try {
			if (signOpInProgress) {
				MD.update(data, off, len);
			} else

			if (verifyOpInProgress) {
				jsse.update(data, off, len);
			}
		} catch (Exception e) {
			e.printStackTrace(); 
		}
	}



 
//****************************************************************************
// Initializes this signature object with the specified public key for verification operations.
	protected void engineInitVerify(PublicKey publicKey) {

		if (debug) System.out.println("MSSHARSASignFactoryImpl: engineInitVerify: entered");

		try {
			String SignatureAlg = MessageDigestType + "withRSA";
			jsse = Signature.getInstance(SignatureAlg, "SunJSSE");
			jsse.initVerify(publicKey);
		} catch (Exception e) {
			e.printStackTrace(); 
		}

		signOpInProgress   = false;
		verifyOpInProgress = true;
	}
 
//****************************************************************************
// Verifies the passed-in signature.
	protected boolean engineVerify(byte[] sigBytes) {
		boolean verifyresult=false;
		if (debug) System.out.println("MSSHARSASignFactoryImpl: engineVerify: entered");

		if (!verifyOpInProgress) {
			System.out.println("MSSHARSASignFactoryImpl: error - throw exception");
			return false;
		}

		try {
			verifyresult = jsse.verify(sigBytes);
		} catch (Exception e) {
			e.printStackTrace(); 
		}
		return verifyresult;
	}
 
//****************************************************************************
// Verifies the passed-in signature in the specified array of bytes,
// starting at the specified offset. 
	protected boolean engineVerify(byte[] sig, int off, int len) {
		boolean verifyresult=false;
		if (debug) System.out.println("MSSHARSASignFactoryImpl: engineVerify: entered");

		if (!verifyOpInProgress) {
			System.out.println("MSSHARSASignFactoryImpl: error - throw exception");
			return false;
		}

		try {
			verifyresult = jsse.verify(sig, off, len);
		} catch (Exception e) {
			e.printStackTrace(); 
		}
		return verifyresult;
	}
}