using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Collections;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KeepassPinentry
{
    internal class EntryDBException : Exception
    {
        public EntryDBException() { }

        public EntryDBException(string message)
            : base(message) { }
    }

    internal class EntryDB
    {
        public IPluginHost Host { get; set; }

        public PwEntry GetByTitle(string text)
        {
            return Get(new SearchParameters { SearchInTitles = true, SearchString = text});
        }

        public PwEntry GetByUserName(string text)
        {
            return Get(new SearchParameters { SearchInUserNames = true, SearchString = text });
        }

        public PwEntry GetByTitleAndUserName(string title, string name)
        {
            var param = new SearchParameters
            {
                SearchInTitles = true,
                SearchString = title
            };

            return Get(
                param,
                entry =>
                {
                    string currentName = entry.Strings.ReadSafe(PwDefs.UserNameField);

                    return string.Equals(currentName, name, StringComparison.OrdinalIgnoreCase);
                });
        }

        public PwEntry Get(SearchParameters param, Func<PwEntry, bool> predicate = null)
        {
            try
            {
                IEnumerable<PwEntry> entries = GetAll(param);
                PwEntry entry = predicate is null ? entries.First() : entries.First(predicate);
                NotifyReadEntry(entry);

                return entry;
            }
            catch (InvalidOperationException)
            {
                var message = $"Unable to find entry with the search string {param.SearchString}.";
                ShowBalloonNotification(message);

                throw new EntryDBException(message);
            }
        }

        private IEnumerable<PwEntry> GetAll(SearchParameters param)
        {
            var unlocker = new Unlocker { Host = Host };
            unlocker.Unlock();

            var databases = Host.MainWindow.DocumentManager.GetOpenDatabases();
            foreach (var database in databases)
            {
                var list = new PwObjectList<PwEntry>();
                database.RootGroup.SearchEntries(param, list);

                foreach (var entry in list)
                    yield return entry;
            }
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

        private void NotifyReadEntry(PwEntry entry)
        {
            var fullTitle = entry.Strings.ReadSafe(PwDefs.TitleField);
            var fullName = entry.Strings.ReadSafe(PwDefs.UserNameField);
            ShowBalloonNotification($"Entry {fullTitle} ({fullName}) was read.");
        }
    }
}