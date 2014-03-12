using System;
using System.Collections.ObjectModel;
using LogViewer.Model;

namespace LogViewer.Infrastructure
{
    public interface ILogSource
    {
        ObservableCollection<LogEntryViewModel> Entries { get; set; }
        LogEntryViewModel Selected { get; set; }
        int CurrentIndex { get; set; }
        LogEntryCounter Counter { get; set; }

        void SelectNextEntry(Func<LogEntryViewModel, bool> accept);
        void SelectPreviousEntry(Func<LogEntryViewModel, bool> accept);
        void SelectTop();
        void SelectBottom();
    }
}
