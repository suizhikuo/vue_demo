// MSSHARSASignature.java
//
// Copyright (c) 2001 Brian Boyter
// All rights reserved
//
// This software is released subject to the GNU Public License.  See
// the full license included with this distribution.
//


package com.boyter.mscrypto;
import com.boyter.mscrypto.MSRSASignFactoryImpl;


public final class MSSHARSASignature extends MSRSASignFactoryImpl {

	public MSSHARSASignature() {
		super();
		super.setMessageDigestType("SHA1");
	}

}