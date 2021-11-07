README   24 Jan 2002

EchoClient and EchoServer are two
JSSE demo programs.  They illustrate how
the Java JSSE can be made to use the
Windows keystore and Windows certstore.

msrsatest is a test program.
it demonstrates the use of the Microsoft CAPI
RSA signature and encryption interface.

mscryptofunctions.c can be compiled to create the
mscryptofunctions.dll (included)
mscryptofunctions.dll contains all of the Java Native
Interfaces needed to access the Windows key store,
Windows certificate store, and the Windows RSA
signature and encryption functions.

The following java files can be compiled with
javac, then jar'd to create the mscrypto.jar (included)
  MSCryptoException.java
  MSCryptoFunctions.java
  MSKeyManagerFactoryImpl.java
  MSKeyManagerImpl.java
  MSKeyMgrProvider.java
  MSMD5RSASignature.java
  MSparseCDP.java
  MSRSACipherFactoryImpl.java
  MSRSACipherProvider.java
  MSRSASignFactoryImpl.java
  MSRSASignProvider.java
  MSSHARSASignature.java
  MSTrustManagerFactoryImpl.java
  MSTrustManagerImpl.java
  MSTrustMgrProvider.java
  MSValidCertificate.java

mscrypto.jar must be signed with a cert issued by
JavaSoft if you want to use mscrypto.jar for
RSA encryption.  The command to sign the jar
file will look something like this:
 jarsigner -keystore keyfile -storepass pwd mscrypto.jar javajcesigner

Good luck
Brian Boyter




