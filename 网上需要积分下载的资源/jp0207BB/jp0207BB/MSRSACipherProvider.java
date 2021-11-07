// MSRSACipherProvider.java
//
// Copyright (c) 2001 Brian Boyter
// All rights reserved
//
// This software is released subject to the GNU Public License.  See
// the full license included with this distribution.
//
// Methods:	MSRSACipherProvider()
//		install()
//

package com.boyter.mscrypto;

final public class MSRSACipherProvider extends java.security.Provider { 
	private static boolean isInstalled; 

	public MSRSACipherProvider() { 
		super("MSRSACipher", 1.0, "MSRSACipherProvider implements Microsoft RSA Decryption");
		put("Cipher.RSA/ECB/PKCS1Block02Pad", "com.boyter.mscrypto.MSRSACipherFactoryImpl");
		put("Cipher.RSA/ECB/PKCS1Padding", "com.boyter.mscrypto.MSRSACipherFactoryImpl");
		put("Cipher.RSA/ECB/PKCS1", "com.boyter.mscrypto.MSRSACipherFactoryImpl");
		isInstalled = true;
	}


// Installs the Custom provider.
	public static synchronized void install() { 
		if (!isInstalled) {
			int position =
			   java.security.Security.insertProviderAt (new MSRSACipherProvider(), 1);
			System.out.println("MSRSACipherProvider installed at position " + position);
		}
	} 
}