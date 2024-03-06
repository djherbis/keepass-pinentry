using System.Diagnostics;
using System.IO;

namespace KeepassPinentry
{
    public delegate string PasswordFetcher(string key);

    public class Pinentry
    {

        public PasswordFetcher PasswordFetcher { get; set; }

        public void Handle(TextReader reader, TextWriter writer)
        {
            // See https://www.gnupg.org/documentation/manuals/assuan.pdf
            string keyInfo = "";

            writer.WriteLine("OK Ready.");

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("OPTION")) {
                    // do nothing.

                } else if (line.StartsWith("GETINFO")) {
                    var args = line.Substring("GETINFO".Length).Trim();
                    switch (args)
                    {
                        case "flavor":
                            writer.WriteLine("D KeepassPinentry");
                            break;

                        case "version":
                            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                            string version = fvi.FileVersion;
                            writer.WriteLine("D " + version);
                            break;

                        case "pid":
                            Process currentProcess = Process.GetCurrentProcess();
                            string pid = currentProcess.Id.ToString();
                            writer.WriteLine("D " + pid);
                            break;

                        default:
                            writer.WriteLine("ERR Unknown Command");
                            continue;
                    }

                } else if (line.StartsWith("SETKEYINFO")) {
                    var args = line.Substring("SETKEYINFO".Length).Trim();
                    keyInfo = args == "--clear" ? "" : args;

                } else if (line.StartsWith("GETPIN")) {
                    try
                    {
                        writer.WriteLine("D " + PasswordFetcher(keyInfo));
                    }
                    catch (EntryDBException e)
                    {
                        // https://dev.gnupg.org/source/libgpg-error/browse/master/src/err-codes.h.in
                        // https://dev.gnupg.org/source/libgpg-error/browse/master/doc/errorref.txt
                        writer.WriteLine($"ERR 86 {e.Message}"); // GPG_ERR_PIN_ENTRY
                    }
                } else {
                    // do nothing
                }

                writer.WriteLine("OK");
            }
        }
    }
}