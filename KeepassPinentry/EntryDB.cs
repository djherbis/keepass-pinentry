using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Collections;
using System.Windows.Forms;

namespace KeepassPinentry
{
    internal class EntryDB
    {
        public IPluginHost Host { get; set; }

        public PwEntry Get(string text)
        {
            var unlocker = new Unlocker { Host = Host };
            unlocker.Unlock();

            var p = new SearchParameters
            {
                SearchInTitles = true,
                SearchString = text
            };

            var list = new PwObjectList<PwEntry>();
            Host.Database.RootGroup.SearchEntries(p, list);

            var e = list.GetAt(0); // TODO(djherbis): might throw
            var title = e.Strings.ReadSafe(PwDefs.TitleField);
            ShowBalloonNotification($"Entry {title} was read.");
            return e;
        }

        public void ShowBalloonNotification(string aMessage)
        {
            MethodInvoker invoker = delegate () {
                if (Host.MainWindow.MainNotifyIcon != null)
                {
                    Host.MainWindow.MainNotifyIcon.ShowBalloonTip(
                        5000, "KeepassPinentry",
                      aMessage, ToolTipIcon.Info);
                }
            };
            InvokeMainWindow(invoker, true);
        }

        private void InvokeMainWindow(MethodInvoker aInvoker, bool aAsync)
        {
            if (Host.MainWindow.InvokeRequired)
            {
                if (aAsync)
                {
                    Host.MainWindow.BeginInvoke(aInvoker);
                }
                else
                {
                    Host.MainWindow.Invoke(aInvoker);
                }
            }
            else
            {
                aInvoker.Invoke();
            }
        }
    }
}