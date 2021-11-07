// MSKeyMgrProvider.java
//
// Copyright (c) 2001 Brian Boyter
// All rights reserved
//
// This software is released subject to the GNU Public License.  See
// the full license included with this distribution.
//
// Methods:	MSKeyMgrProvider()
//		install()
//

package com.boyter.mscrypto;

final public class MSKeyMgrProvider extends java.security.Provider { 
	private static boolean isInstalled; 

	public MSKeyMgrProvider() { 
		super("MSKMF", 1.0, "MSKMF implements MS Key Factory"); 
		put("KeyManagerFactory.MSKMF", "com.boyter.mscrypto.MSKeyManagerFactoryImpl"); 
		isInstalled = true;
	}


/** Installs the Custom provider. */ 
	public static synchronized void install() { 
		if (!isInstalled) {
			int position =
			   java.security.Security.insertProviderAt (new MSKeyMgrProvider(), 1);
			System.out.println("MSKeyMgrProvider installed at position " + position);
		}
	} 
}

