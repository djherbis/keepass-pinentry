using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Collections;
using System.Windows.Forms;
using System;

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

            var databases = Host.MainWindow.DocumentManager.GetOpenDatabases();
            foreach (var database in databases)
            {
                var list = new PwObjectList<PwEntry>();
                database.RootGroup.SearchEntries(p, list);
                if (list.UCount > 0) 
                {
                    var e = list.GetAt(0);
                    var title = e.Strings.ReadSafe(PwDefs.TitleField);
                    ShowBalloonNotification($"Entry {title} was read.");
                    return e;
                }
            }
            throw new Exception("No " + text + " entry found!");
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