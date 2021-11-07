// MSRSASignProvider.java
//
// Copyright (c) 2001 Brian Boyter
// All rights reserved
//
// This software is released subject to the GNU Public License.  See
// the full license included with this distribution.
//
// Methods:	MSRSASignProvider()
//		install()
//

package com.boyter.mscrypto;

final public class MSRSASignProvider extends java.security.Provider { 
	private static boolean isInstalled; 

	public MSRSASignProvider() { 
		super("MicrosoftRSASign", 1.0, "MSRSASignProvider implements Microsoft RSA Signature"); 
		put("Signature.MD5withRSA",  "com.boyter.mscrypto.MSMD5RSASignature");
		put("Signature.SHA1withRSA", "com.boyter.mscrypto.MSSHARSASignature");
		isInstalled = true;
	}


// Installs the Custom provider.
	public static synchronized void install() { 
		if (!isInstalled) {
			int position =
			   java.security.Security.insertProviderAt (new MSRSASignProvider(), 1);
			System.out.println("MSRSASignProvider installed at position " + position);
		}
	} 
}