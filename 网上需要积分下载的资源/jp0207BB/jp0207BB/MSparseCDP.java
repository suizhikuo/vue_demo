// MSparseCDP.java
//
// Copyright (c) 2001 Brian Boyter
// All rights reserved
//
// This software is released subject to the GNU Public License.  See
// the full license included with this distribution.
//
// Methods:	parseCDP()
//		ASNgetSequence()
//		ASNgetContextValue()
//		ASNtagLength()
//		ASNtagSize()
//
//
// This class ASN-parses the CDP (CRL Distribution Point) extension
//       of an X.509 certificate.
//
// The CDP starts out as a byte array.
// It then has two nested sequences.
// Each sequence has two contexts.
// If a sequence has a DistributionPointName.GeneralNames,
//    then that URL is converted into a String.
// An array of URLs is returned to the calling program.
//
// -----------------------------------------------------------------
// This is an extract from RFC2459 para 4.2.1.14  (CRL Distribution Points)
//
// cRLDistributionPoints ::= {
//      CRLDistPointsSyntax }
//
// CRLDistPointsSyntax ::= SEQUENCE SIZE (1..MAX) OF DistributionPoint
//
// DistributionPoint ::= SEQUENCE {
//      distributionPoint       [0]     DistributionPointName OPTIONAL,
//      reasons                 [1]     ReasonFlags OPTIONAL,
//      cRLIssuer               [2]     GeneralNames OPTIONAL }
//
// DistributionPointName ::= CHOICE {
//      fullName                [0]     GeneralNames,
//      nameRelativeToCRLIssuer [1]     RelativeDistinguishedName }
// ------------------------------------------------------------------
//
//


package com.boyter.mscrypto;

import java.util.ArrayList;
import java.lang.*;


public class MSparseCDP
{
    static boolean debug = (System.getProperty("mscrypto.debug") != null);

//****************************************************************************
    public static String[] parseCDP(byte[] CDPblob) {
	byte[] seq1 = null;
	byte[] seq2 = null;
	ArrayList urlList = new ArrayList();

	// check to make sure this is a byte array
	if (CDPblob[0] != 0x04) {
		if (debug) System.out.println("parseCDP: ASN parse error: CDP is not an Octet String");
		return null;	// blob must be an octet string
	}

	// check to make sure we have all of the bytes we should have
	if (ASNtagLength(CDPblob, 0) != CDPblob.length - 2) {
		if (debug) System.out.println("parseCDP: ASN parse error: CDP wrong size"
			+ CDPblob.length + " - should be " + ASNtagLength(CDPblob, 0) + " - abort");
		return null;	// blob has the wrong length
	}

	// skip over the first tag
	int index1 = ASNtagSize(CDPblob, 0);

	while ((seq1 = ASNgetSequence(CDPblob, index1)) != null) {

		// move pointer to next sequence
		index1 = ASNtagSize(CDPblob, index1) + seq1.length + index1;

		int index2 = 0;
		while ((seq2 = ASNgetSequence(seq1, index2)) != null) {

			// move pointer to next sequence
			index2 = ASNtagSize(CDPblob, index2) + seq2.length + index2;

			// there should be a context specific tag next
			int tagType = seq2[0] & 0xe0;
			if (tagType != 0x00a0) {
				if (debug ) System.out.println("parseCDP: parse error - context specific tag 1 missing - skip");
				continue;
			}

			// get the first context value (looking for 0 which is a distribution point)
			int contextVal = ASNgetContextValue(seq2, 0);

			if (contextVal != 0)
				switch (contextVal) {
				case 1:				// reasonFlags	
					if (debug ) System.out.println("parseCDP: reasonFlag found - skip");
					continue;
				case 2:				// CRLissuer
					if (debug ) System.out.println("parseCDP: CRLissuer found - skip");
					continue;
				default:			// unknown
					if (debug ) System.out.println("parseCDP: parse error - unknown DistributionPoint type - skip");
					continue;
				}

			// This sequence contains a DistributionPointName

			// there should be a context specific tag next
			tagType = seq2[2] & 0xe0;
			if (tagType != 0x00a0) {
				if (debug ) System.out.println("parseCDP: parse error - context specific tag 2 missing - skip");
				continue;
			}

			// get the second context value (looking for 0 which is a fullName)
			contextVal = ASNgetContextValue(seq2, 2);

			if (contextVal != 0)
				switch (contextVal) {
				case 1:				// rfc822Name
					if (debug ) System.out.println("parseCDP: nameRelativeToCRLIssuer found - skip");
					continue;
				default:			// unknown
					if (debug ) System.out.println("parseCDP: parse error - unknown DistributionPointName type - skip");
					continue;
				}

			// there should be a context specific OID tag next
			tagType = seq2[4];
			if (tagType != -122) {
				if (debug ) System.out.println("parseCDP: parse error - context specific OID tag missing - skip");
				continue;
			}

			// copy the CRL's URL into a new byte array
			int URLsize = ASNtagLength(seq2, 4);
			String URL = new String(seq2, ASNtagSize(seq2, 4)+4, URLsize);
			if (debug ) System.out.println("parseCDP: found CDP URL: " + URL);

			urlList.add(URL);
		}

	}

	String[] urls = new String[urlList.size()];
	urlList.toArray(urls);

	return urls;
    }

//****************************************************************************
// returns a new byte array containing this sequence
    static byte[] ASNgetSequence(byte[] blob, int index) {
	if (index > (blob.length)-1)
		return null;

	// get the size of this seq
	int size = ASNtagLength(blob, index);

	// copy this seq into a new byte array
	byte[] out = new byte[size];
	java.lang.System.arraycopy(blob, index+ASNtagSize(blob, index), out, 0, size);

	return out;
    }

//****************************************************************************
// returns the context value
    static int ASNgetContextValue(byte[] blob, int index) {
	return (int)(blob[index] & 0x1f);
    }

//****************************************************************************
// returns the size of the data block which follows this tag
    static int ASNtagLength(byte[] blob, int index) {
	byte b1 = blob[index+1];
	if ((b1 & 0x10) != 0) {	// the length is > 127
		int n = b1 & 0x7f;	// the number of bytes that make up the length
		int t = 0;
		for (int i=0; i<n; i++)
			t = (t << 8) + blob[2+i];
          	return t;
	} else				// length is < 128
		return (int)b1;
    }

//****************************************************************************
// returns the size of this tag
    static int ASNtagSize(byte[] blob, int index) {
	byte b1 = blob[index+1];
	if ((b1 & 0x10) != 0) {		// the length is > 127
		int n = b1 & 0x7f;	// the number of bytes that make up the length
          	return n+2;
	} else				// length is < 128
		return 2;
    }

}