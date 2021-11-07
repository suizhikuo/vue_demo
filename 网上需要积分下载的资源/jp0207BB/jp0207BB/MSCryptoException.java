// MSCryptoFunctions.java
//
// Copyright (c) 2001 Brian Boyter
// All rights reserved
//
// This software is released subject to the GNU Public License.  See
// the full license included with this distribution.
//


package com.boyter.mscrypto;

public class MSCryptoException extends Exception
{
	public MSCryptoException() {}
	public MSCryptoException(String gripe)	{
		super(gripe);
	}
}