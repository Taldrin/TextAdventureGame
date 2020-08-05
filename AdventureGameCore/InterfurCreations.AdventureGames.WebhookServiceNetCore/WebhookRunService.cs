using CERTENROLLLib;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Webhook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.WebhookService
{
    public class WebhookRunService : IWebhookService
    {
        public string StartWebhookService(int port = 44368)
        {
            string externalip = new WebClient().DownloadString("http://icanhazip.com").TrimEnd('\n');

            Log.LogMessage("Identified IP as: " + externalip);

            Log.LogMessage("Generating SSL Cert...");
            var cert = GenerateCert("FurryAdventureBot", TimeSpan.FromDays(2000));

            Log.LogMessage("Binding SSL cert to port: " + port);
            RegisterSslOnPort(port, cert.Thumbprint, externalip);



            Log.LogMessage("Starting up webhook owin reciever");

            WebHoster.StartWebHost();


            string webhookUrl = $"https://{externalip}:{port}/api/webhooks/incoming/genericjson?code=3d62de47-5f0f-45cb-b538-fa3ecc342ad6";
            Log.LogMessage("Webhook reciever started, with webhook URL: " + webhookUrl);
            return webhookUrl;
        }

        private X509Certificate2 GenerateCert(string certName, TimeSpan expiresIn)
        {
            var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            var store2 = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store2.Open(OpenFlags.ReadWrite);
            var existingCert = store.Certificates.Find(X509FindType.FindBySubjectName, certName, false);
            var existingCert2 = store2.Certificates.Find(X509FindType.FindBySubjectName, certName, false);
            bool localCertFound = false;
            bool userCertFound = false;
            if (existingCert.Count > 0)
            {
                store.Close();
                userCertFound = true;
            }
            if (existingCert2.Count > 0)
            {
                store2.Close();
                localCertFound = true;
            }
            if(!localCertFound || !userCertFound)
            {
                var cert = CreateSelfSignedCertificate(certName, expiresIn);

                if(!localCertFound)
                {
                    store.Add(cert);
                    store.Close();
                }
                if(!userCertFound)
                {
                    store2.Add(cert);
                    store2.Close();
                }

                return cert;
            } else
            {
                return existingCert[0];
            }
        }

        private void RegisterSslOnPort(int port, string certThumbprint, string ip)
        {
            var appId = Guid.NewGuid();
            string arguments = $"http add sslcert ipport=0.0.0.0:{port} certhash={certThumbprint} appid={{{appId}}}";
            ProcessStartInfo procStartInfo = new ProcessStartInfo("netsh", arguments);

            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;

            var process = Process.Start(procStartInfo);
            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }

            process.WaitForExit();
        }

        private X509Certificate2 CreateSelfSignedCertificate(string subjectName, TimeSpan expiresIn)
        {
            // create DN for subject and issuer
            var dn = new CX500DistinguishedName();
            dn.Encode("CN=" + subjectName, X500NameFlags.XCN_CERT_NAME_STR_NONE);

            // create a new private key for the certificate
            CX509PrivateKey privateKey = new CX509PrivateKey();
            privateKey.ProviderName = "Microsoft Base Cryptographic Provider v1.0";
            privateKey.MachineContext = true;
            privateKey.Length = 2048;
            privateKey.KeySpec = X509KeySpec.XCN_AT_SIGNATURE; // use is not limited
            privateKey.ExportPolicy = X509PrivateKeyExportFlags.XCN_NCRYPT_ALLOW_PLAINTEXT_EXPORT_FLAG;
            privateKey.Create();

            // Use the stronger SHA512 hashing algorithm
            var hashobj = new CObjectId();
            hashobj.InitializeFromAlgorithmName(ObjectIdGroupId.XCN_CRYPT_HASH_ALG_OID_GROUP_ID,
                ObjectIdPublicKeyFlags.XCN_CRYPT_OID_INFO_PUBKEY_ANY,
                AlgorithmFlags.AlgorithmFlagsNone, "SHA512");

            // add extended key usage if you want - look at MSDN for a list of possible OIDs
            var oid = new CObjectId();
            oid.InitializeFromValue("1.3.6.1.5.5.7.3.1"); // SSL server
            var oidlist = new CObjectIds();
            oidlist.Add(oid);
            var eku = new CX509ExtensionEnhancedKeyUsage();
            eku.InitializeEncode(oidlist);

            // Create the self signing request
            var cert = new CX509CertificateRequestCertificate();
            cert.InitializeFromPrivateKey(X509CertificateEnrollmentContext.ContextMachine, privateKey, "");
            cert.Subject = dn;
            cert.Issuer = dn; // the issuer and the subject are the same
            cert.NotBefore = DateTime.Now.Subtract(TimeSpan.FromDays(1));
            // this cert expires immediately. Change to whatever makes sense for you
            cert.NotAfter = DateTime.Now.Add(expiresIn);
            cert.X509Extensions.Add((CX509Extension)eku); // add the EKU
            cert.HashAlgorithm = hashobj; // Specify the hashing algorithm
            cert.Encode(); // encode the certificate

            // Do the final enrollment process
            var enroll = new CX509Enrollment();
            enroll.InitializeFromRequest(cert); // load the certificate
            enroll.CertificateFriendlyName = subjectName; // Optional: add a friendly name
            string csr = enroll.CreateRequest(); // Output the request in base64
            // and install it back as the response
            enroll.InstallResponse(InstallResponseRestrictionFlags.AllowUntrustedCertificate,
                csr, EncodingType.XCN_CRYPT_STRING_BASE64, ""); // no password
            // output a base64 encoded PKCS#12 so we can import it back to the .Net security classes
            var base64encoded = enroll.CreatePFX("", // no password, this is for internal consumption
                PFXExportOptions.PFXExportChainWithRoot);

            // instantiate the target class with the PKCS#12 data (and the empty password)
            return new System.Security.Cryptography.X509Certificates.X509Certificate2(
                System.Convert.FromBase64String(base64encoded), "",
                // mark the private key as exportable (this is usually what you want to do)
                System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable
            );
        }
    }
}
