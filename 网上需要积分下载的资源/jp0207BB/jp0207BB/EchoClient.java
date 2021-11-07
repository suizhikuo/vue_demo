import java.io.InputStream;
import java.io.FileInputStream;
import java.io.OutputStream;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.BufferedReader;
import java.io.StringWriter;
import java.io.IOException;

import java.security.KeyStore; 
import java.security.*;
import javax.net.ssl.*;
import javax.net.ssl.SSLSocket;
import javax.net.ssl.SSLSocketFactory;
import javax.net.ssl.SSLContext;
import com.boyter.mscrypto.MSTrustMgrProvider;
import com.boyter.mscrypto.MSKeyMgrProvider;

public class EchoClient {

  public static void main(String [] arstring) {
//	System.setProperty("javax.net.debug","ssl");
	System.setProperty("mscrypto.debug","true");
	SSLSocketFactory factory = null;
	SSLContext ctx=null; 
	KeyManagerFactory kmf;
	TrustManagerFactory tmf;

	try {
		Security.addProvider(new com.sun.net.ssl.internal.ssl.Provider());
		MSTrustMgrProvider.install();
		MSKeyMgrProvider.install();

		kmf = KeyManagerFactory.getInstance("MSKMF");
		kmf.init(null ,null ); 

		tmf = TrustManagerFactory.getInstance("MSTMF");
		System.out.println("TrustManagerProvider name: " + tmf.getProvider().getInfo());
		tmf.init((KeyStore) null);

		ctx = SSLContext.getInstance("TLS"); 
		ctx.init(kmf.getKeyManagers(),tmf.getTrustManagers() ,null); 
		factory = ctx.getSocketFactory();
	} 
	catch (Exception e) {
		e.printStackTrace(); 
	} 


    try
    {
	Security.addProvider(new com.sun.net.ssl.internal.ssl.Provider());
      SSLSocket sslsocket = (SSLSocket)factory.createSocket("localhost", 9999);

      InputStream inputstream = System.in;
      InputStreamReader inputstreamreader = new InputStreamReader(inputstream);
      BufferedReader bufferedreader = new BufferedReader(inputstreamreader);

      OutputStream outputstream = sslsocket.getOutputStream();
      OutputStreamWriter writer = new OutputStreamWriter(outputstream);

	// send a line to the server
	String testtext = "\n****Connection opened from EchoClient****\n\n";
	writer.write( testtext, 0, testtext.length() );
	writer.flush();
	

      String string = null;
      while ((string = bufferedreader.readLine()) != null)
      {
		string = string + '\n';
		writer.write( string, 0, string.length()  );
		writer.flush();
      }
    }
    catch (Exception exception)
    {
      exception.printStackTrace();
    }
  }
}

