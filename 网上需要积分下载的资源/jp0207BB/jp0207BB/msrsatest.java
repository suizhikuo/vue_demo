import com.boyter.mscrypto.MSKeyMgrProvider;
import com.boyter.mscrypto.MSTrustMgrProvider;
import com.boyter.mscrypto.MSRSASignProvider;
import com.boyter.mscrypto.MSRSACipherProvider;
import com.boyter.mscrypto.MSValidCertificate;
import javax.net.ssl.KeyManagerFactory;
import javax.net.ssl.TrustManagerFactory;
import javax.net.ssl.X509KeyManager;
import javax.net.ssl.X509TrustManager;
import javax.crypto.Cipher;
import java.security.PrivateKey;
import java.security.KeyStore;
import java.security.cert.X509Certificate;
import java.security.Principal;
import java.security.Signature;
import java.security.PublicKey;

public class msrsatest {
	public static void main(String[] args) throws Exception {
		System.setProperty("mscrypto.debug","true");
		MSTrustMgrProvider.install();
		MSKeyMgrProvider.install();
		MSRSACipherProvider.install();
		MSRSASignProvider.install();

		TrustManagerFactory tmf = TrustManagerFactory.getInstance("MSTMF");
		tmf.init((KeyStore)null);
		KeyManagerFactory kmf = KeyManagerFactory.getInstance("MSKMF");
		kmf.init(null, null);

		X509TrustManager xtm = (X509TrustManager)tmf.getTrustManagers()[0];
		X509KeyManager xkm = (X509KeyManager)kmf.getKeyManagers()[0];

		X509Certificate[] issuerCerts = xtm.getAcceptedIssuers();
		System.out.println("number of CA certs found: " + issuerCerts.length);
		Principal[] issuers = new Principal[issuerCerts.length];

		for (int i=0; i<issuerCerts.length; i++)
			issuers[i] = issuerCerts[i].getSubjectDN();
		String[] aliases = xkm.getClientAliases("X509", issuers);
		System.out.println("number of aliases found: " + aliases.length);
		PrivateKey privkey = xkm.getPrivateKey(aliases[0]);
		X509Certificate[] chain = xkm.getCertificateChain(aliases[0]);
		PublicKey pubkey = chain[0].getPublicKey();

//		System.out.println("test if cert is valid");
//		if (MSValidCertificate.isCertValid(chain[0], 0) )
//			System.out.println("cert is valid");
//		   else
//			System.out.println("cert is not valid");


		String message = "RSA cipher test - OK";
		byte[] messageBytes = message.getBytes("UTF8");

		Cipher rc = Cipher.getInstance("RSA/ECB/PKCS1Padding","MSRSACipher");
		System.out.println("using provider: " + rc.getProvider().getName());

		System.out.println("\n\nMSrsaEncypt test");
		rc.init( Cipher.ENCRYPT_MODE, pubkey );
		byte[] encryptedMessage = rc.doFinal(messageBytes);

		System.out.println("\n\nMSrsaDecrypt test");
		rc.init( Cipher.DECRYPT_MODE, privkey );
		byte[] decryptedMessage = rc.doFinal(encryptedMessage);
		String decryptedMessageString = new String(decryptedMessage,"UTF8");
		System.out.println("\nDecrypted message: " + decryptedMessageString);


		System.out.println("\n\nMD5withRSA Signature test");
		Signature rsa = Signature.getInstance("MD5withRSA");
		System.out.println("using provider: " + rsa.getProvider().getName());
		rsa.initSign(privkey);
		rsa.update(messageBytes);
		byte[] sig = rsa.sign();
		System.out.println("signature OK - length: " + sig.length);

		System.out.println("\n\nMD5withRSA Signature verify test");
		rsa.initVerify(pubkey);
		rsa.update(messageBytes);
		if (rsa.verify(sig))
			System.out.println("signature verify OK");
		  else
			System.out.println("signature verify failed");



		System.out.println("\n\nSHA1withRSA Signature test");
		rsa = Signature.getInstance("SHA1withRSA");
		System.out.println("using provider: " + rsa.getProvider().getName());
		rsa.initSign(privkey);
		rsa.update(messageBytes);
		sig = rsa.sign();
		System.out.println("signature OK - length: " + sig.length);

		System.out.println("\n\nSHA1withRSA Signature verify test");
		rsa.initVerify(pubkey);
		rsa.update(messageBytes);
		if (rsa.verify(sig))
			System.out.println("signature verify OK");
		  else
			System.out.println("signature verify failed");

	}
}