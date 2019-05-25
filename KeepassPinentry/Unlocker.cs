using KeePass.Plugins;
using System.Windows.Forms;

namespace KeepassPinentry
{
    public class Unlocker {

        public IPluginHost Host { get; set; }

        public void Unlock()
        {
            var mainWindow = Host.MainWindow;

            mainWindow.Invoke((MethodInvoker)delegate ()
            {
                foreach (var document in mainWindow.DocumentManager.Documents)
                {
                    if (mainWindow.IsFileLocked(document))
                    {
                        mainWindow.OpenDatabase(document.LockedIoc, null, false);
                    }
                }
            });
        }
    }
}