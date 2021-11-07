import java.io.InputStream;
import java.io.FileInputStream;
import java.io.InputStreamReader;
import java.io.BufferedReader;
import java.io.IOException;
import java.util.*;
import java.io.*;
import java.net.*;

import javax.net.ssl.*;
import javax.security.cert.X509Certificate; 
import javax.security.*; 
import java.security.KeyStore; 
import java.security.*;
import javax.net.ssl.SSLSocket;
import javax.net.ssl.SSLServerSocket;
import javax.net.ssl.SSLServerSocketFactory;
import com.boyter.mscrypto.MSKeyMgrProvider;
import com.boyter.mscrypto.MSTrustMgrProvider;
import javax.crypto.Cipher;

public class EchoServer
{
  public static void main(String [] arstring) {
//	System.setProperty("javax.net.debug","ssl");
	System.setProperty("mscrypto.debug","true");
	SSLServerSocketFactory factory = null;
	SSLContext ctx=null; 
	KeyManagerFactory kmf;
	TrustManagerFactory tmf;

	try {
		MSTrustMgrProvider.install();
		MSKeyMgrProvider.install();

		kmf = KeyManagerFactory.getInstance("MSKMF");
		kmf.init(null, null); 
		tmf = TrustManagerFactory.getInstance("MSTMF");
		System.out.println("TrustManagerProvider name: " + tmf.getProvider().getInfo());

		tmf.init((KeyStore)null); 
		ctx = SSLContext.getInstance("TLS"); 
		ctx.init(kmf.getKeyManagers(),tmf.getTrustManagers() ,null); 
		factory = ctx.getServerSocketFactory();
	} 
	catch (Exception e) {
		e.printStackTrace(); 
	} 

	int threadCount = 1;

	try {
		SSLServerSocket sslserversocket =
			(SSLServerSocket)factory.createServerSocket(9999);
		sslserversocket.setNeedClientAuth(true);

		while(true) {
			SSLSocket sslsocket = (SSLSocket)sslserversocket.accept();
			System.out.println("connection made - make new thread number "+ threadCount);
			new ThreadedEchoHandler(sslsocket, threadCount).start();
			threadCount++;

		}
	}
		catch (Exception exception)
	{
		exception.printStackTrace();
	}
  }
}



class ThreadedEchoHandler extends Thread
{  public ThreadedEchoHandler(Socket s, int count) {
	socket=s;
	counter=count;
   }


   public void run() {
   try
      {  BufferedReader in = new BufferedReader
            (new InputStreamReader(socket.getInputStream()));
	PrintWriter out = new PrintWriter
            (socket.getOutputStream(), true /* autoFlush */);

	String string = null;
	while ((string = in.readLine()) != null) {
		if (string == null)
			break;
		System.out.println(counter + ": " + string);
		System.out.flush();
	}

	socket.close();
   }
      catch (Exception e)
	{  System.out.println(e); }
   }
   private Socket socket;
   private int counter;

}


