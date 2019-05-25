using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeepassPinentry
{

    public delegate void HandlerFunc(TextReader r, TextWriter w);

    public class LocalServer
    {
        private TcpListener listener;

        public string Address { get; set; }

        public int Port { get; set; }

        public Func<X509Certificate> Certificate { get; set; }

        public HandlerFunc Handler { get; set; }

        public void Start()
        {
            IPAddress ipAddress = IPAddress.Parse(Address);
            listener = new TcpListener(ipAddress, Port);
            listener.Start();

            Task.Run(async () => {
                while (true)
                {
                    Socket client = await listener.AcceptSocketAsync();
                    var res = Task.Run(() => {
                        SslStream sslStream;
                        try
                        {
                            sslStream = new SslStream(new NetworkStream(client), false);
                            sslStream.AuthenticateAsServer(Certificate());
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                            return;
                        }

                        var reader = new StreamReader(sslStream, Encoding.ASCII);
                        var writer = new StreamWriter(sslStream, Encoding.ASCII)
                        {
                            AutoFlush = true
                        };

                        Handler(reader, writer);

                        sslStream.Close();
                        client.Close();
                    });
                }
            });
        }

        public void Stop()
        {
            if (listener != null)
            {
                listener.Stop();
            }
        }
    }
}