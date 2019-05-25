using KeePass.Plugins;
using KeePassLib;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace KeepassPinentry
{
    public sealed class KeepassPinentryExt : Plugin
    {

        private EntryDB db;
        private LocalServer server;

        public override bool Initialize(IPluginHost host)
        {
            var unlocker = new Unlocker { Host = host };

            db = new EntryDB { Host = host };            

            var pinentry = new Pinentry {
                PasswordFetcher = (keyInfo) => {
                    return db.Get("GPG").Strings.ReadSafe(PwDefs.PasswordField);
                },
            };

            int port = -1;
            if (!int.TryParse(Environment.GetEnvironmentVariable("STDPROXY_PORT"), out port))
            {
                port = 500;
            }

            server = new LocalServer() {
                Address = "127.0.0.1",
                Port = port,
                Handler = pinentry.Handle,
                Certificate = () => {
                    var entry = db.Get("TLSKEY");
                    var data = entry.Binaries.Get("certificate.p12").ReadData();
                    var pwd = entry.Strings.ReadSafe(PwDefs.PasswordField);
                    return new X509Certificate2(data, pwd);
                },
            };

            server.Start();

            return true;
        }

        public override void Terminate()
        {
            server.Stop();
        }
    }
}