// MSRSACipherFactoryImpl.java
//
// Copyright (c) 2001 Brian Boyter
// All rights reserved
//
// This software is released subject to the GNU Public License.  See
// the full license included with this distribution.
//
// Methods:	engineDoFinal()
//		engineGetBlockSize() 
//		engineGetIV()
//		engineGetKeySize()
//		engineGetOutputSize()
//		engineGetParameters()
//		engineInit()
//		engineSetMode()
//		engineSetPadding()
//		engineUnwrap()
//		engineUpdate()
//		engineWrap()
//

package com.boyter.mscrypto;

import java.security.AlgorithmParameters;
import javax.crypto.CipherSpi;
import javax.crypto.Cipher;
import java.security.Key;
import java.security.SecureRandom;
import java.security.spec.AlgorithmParameterSpec;
import javax.crypto.NoSuchPaddingException;
import java.security.NoSuchAlgorithmException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.BadPaddingException;
import javax.crypto.ShortBufferException;
import java.io.ByteArrayOutputStream;


public final class MSRSACipherFactoryImpl extends CipherSpi {

	static String PaddingAlgorithm = "PKCS1";
	static int ciphermode = 0;
	static MSCryptoFunctions MSF = new MSCryptoFunctions();
	static boolean debug = (System.getProperty("mscrypto.debug") != null);
	static int KeySize = MSF.MSrsaGetKeysize() / 8;
	private ByteArrayOutputStream buffer = new ByteArrayOutputStream();


	protected byte[] engineDoFinal(byte[] input, int inputOffset, int inputLen)
		throws IllegalBlockSizeException, BadPaddingException {
		if (debug) System.out.println("\nMSRSACipherFactoryImpl: engineDoFinal entered\n");

		byte[] outputData=null;
		buffer.write(input, inputOffset, inputLen);
		byte[] inputData = buffer.toByteArray();

		if (ciphermode == Cipher.DECRYPT_MODE) {
			if (KeySize != inputData.length)
				throw new IllegalBlockSizeException(
				"MSRSA length of data to be decrypted must equal keysize "
					+ KeySize + "  " + inputData.length);
			outputData = MSF.MSrsaDecrypt(PaddingAlgorithm, inputData);
		}

		if (ciphermode == Cipher.ENCRYPT_MODE) {
			if (KeySize < inputData.length)
				throw new IllegalBlockSizeException(
				"MSRSA length of data to be decrypted must be <= keysize "
					+ KeySize + "  " + inputData.length);
			outputData = MSF.MSrsaEncrypt(PaddingAlgorithm, inputData);
		}

		buffer.reset();
		return outputData;
	}


	protected int engineDoFinal(byte[] input, int inputOffset,
		int inputLen, byte[] output, int outputOffset)
		throws ShortBufferException, IllegalBlockSizeException, BadPaddingException {
		if (debug) System.out.println("\nMSRSACipherFactoryImpl: engineDoFinal entered\n");

		byte[] outputData = engineDoFinal(input, inputOffset, inputLen);
		System.arraycopy(outputData, 0, output, outputOffset, outputData.length);
		return outputData.length;
	}

	protected int engineGetBlockSize() {
		if (debug) System.out.println("\nMSRSACipherFactoryImpl: engineGetBlockSize entered\n");
		return KeySize;
	}

	protected byte[] engineGetIV() {
		if (debug) System.out.println("\nMSRSACipherFactoryImpl: engineGetIV entered\n");
		return null;
	}

	protected  int engineGetKeySize(Key key) {
		if (debug) System.out.println("\nMSRSACipherFactoryImpl: engineGetKeySize entered\n");
		return KeySize;		// keysize is in bytes
	}
 
	protected int engineGetOutputSize(int inputLen) {
		if (debug) System.out.println("\nMSRSACipherFactoryImpl: engineOutputSize entered\n");
		return KeySize;
	}

	protected AlgorithmParameters engineGetParameters() {
		if (debug) System.out.println("\nMSRSACipherFactoryImpl: engineGetParameters entered\n");
		return null;
	}

	protected void engineInit(int opmode, Key key,
			AlgorithmParameterSpec params, SecureRandom random)
			throws InvalidAlgorithmParameterException, InvalidKeyException {
		if (debug) System.out.println("\nMSRSACipherFactoryImpl: engineInit entered\n");

		engineInit(opmode, key, random);
	}

	protected void engineInit(int opmode, Key key, AlgorithmParameters params, 
		SecureRandom random) throws InvalidAlgorithmParameterException {
		if (debug) System.out.println("\nMSRSACipherFactoryImpl: engineInit entered\n");
		buffer.reset();
		throw new InvalidAlgorithmParameterException(
			"MSRSA does not accept AlgorithmParameterSpec");
	}

	protected void engineInit(int opmode, Key key, SecureRandom random) throws InvalidKeyException{
		if (debug) System.out.println("\nMSRSACipherFactoryImpl: engineInit entered\n");

		buffer.reset();
		if (opmode != Cipher.ENCRYPT_MODE && opmode != Cipher.DECRYPT_MODE)
			throw new InvalidKeyException ("MSRSA opmode must be either encrypt or decrypt");
		ciphermode = opmode;
	}

	protected void engineSetMode(String mode) throws NoSuchAlgorithmException {
		System.out.println("\nMSRSACipherFactoryImpl: engineSetMode entered\n");
		if (!mode.equalsIgnoreCase("ECB")) {
			throw new NoSuchAlgorithmException("MSRSA supports only ECB mode");
		}
	}

	protected void engineSetPadding(String padding) throws NoSuchPaddingException {
		if (debug) System.out.println("\nMSRSACipherFactoryImpl: engineSetPadding entered\n");

		if( padding.substring(0, 5).equalsIgnoreCase("PKCS1") ) {
			PaddingAlgorithm = "PKCS1";
		} else {
			throw new NoSuchPaddingException("MSRSA only supports PKCS1 Padding ("
				+ padding + ")");
		}
	}

	protected byte[] engineUpdate(byte[] input, int inputOffset, int inputLen) {
		if (debug) System.out.println("\nMSRSACipherFactoryImpl: engineUpdate entered\n");

		buffer.write(input, inputOffset, inputLen);
		return null;
	}

	protected int engineUpdate(byte[] input, int inputOffset,
		int inputLen, byte[] output, int outputOffset) {
		if (debug) System.out.println("\nMSRSACipherFactoryImpl: engineUpdate entered\n");
		return 0;
	}
 
}