// MSTrustMgrProvider.java
//
// Copyright (c) 2001 Brian Boyter
// All rights reserved
//
// This software is released subject to the GNU Public License.  See
// the full license included with this distribution.
//
// Methods:	MSTrustMgrProvider() 
//		install() 
//

package com.boyter.mscrypto;

final public class MSTrustMgrProvider extends java.security.Provider { 
	private static boolean isInstalled; 

	public MSTrustMgrProvider() { 
		super("MSTMF", 1.0, "MSTMF implements MS Trust Factory"); 
		put("TrustManagerFactory.MSTMF", "com.boyter.mscrypto.MSTrustManagerFactoryImpl"); 
		isInstalled = true;
	}


/** Installs the Custom provider. */ 
	public static synchronized void install() { 
		if (!isInstalled) {
			int position =
			   java.security.Security.insertProviderAt (new MSTrustMgrProvider(), 1);
			System.out.println("MSTrustMgrProvider installed at position " + position);
		}
	} 
}

