// MSCryptoFunctions.java
//
// Copyright (c) 2001 Brian Boyter
// All rights reserved
//
// This software is released subject to the GNU Public License.  See
// the full license included with this distribution.
//


package com.boyter.mscrypto;

public class MSCryptoFunctions
{
	public native int	MSVerifyCertRevocation(byte[] Cert);
	public native String[]	MSgetAliases(String certStore);
	public native byte[]	MSgetPrivateKey(String alias);
	public native byte[]	MSgetCert(String certStore, String alias);
	public native void	MSrsaSignInit(byte[] privatekey, String hashalg);
	public native void	MSrsaSignUpdate(byte[] data);
	public native byte[]	MSrsaSign();
	public native byte[]	MSrsaSignHash(byte[] hash, byte[] privatekey, String hashalg);
	public native String[]	MSgetCACerts();
	public native byte[]	MSrsaDecrypt(String padalg, byte[] data);
	public native byte[]	MSrsaEncrypt(String padalg, byte[] data);
	public native int	MSrsaGetKeysize();
	public native boolean	MSgetCRL(String url);

	static {
		try {
			System.loadLibrary("mscryptofunctions");
		} catch(UnsatisfiedLinkError e)	{
			System.out.println("cannot load file mscryptofunctions.dll");
			System.exit(1);
		}
	}
}