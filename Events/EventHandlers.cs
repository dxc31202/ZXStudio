using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    public delegate void ErrorLoggedEventHandler(object sender, LogErrorEventArgs e);
    public delegate void AssembledLineEventHandler(object sender, AssembledLineEventArgs e);
    public delegate void AsseblyCompleteEventHandler(object sender, AsseblyCompleteEventArgs e);
    public delegate void AsseblyStartEventHandler(object sender, AsseblyStartEventArgs e);
    public delegate void LogSourceErrorEventHandler(object sender, LogErrorEventArgs e);
    public delegate void HeaderCloseEventHandler(object sender, CloseEventArgs e);
    public delegate object SelectFileEventHandler(object sender, SelectFileEventArgs e);
    public delegate void RunInEmulatorEventHandler(object sender, RunInEmulatorEventArgs e);
    public delegate void LoadInEmulatorEventHandler(object sender, LoadInEmulatorEventArgs e);
    public delegate void FileCreatedEventHandler(object sender, FileCreatedEventArgs e);
    public delegate void EditTreeNodeEventHandler(object sender, EditTreeNodeEventArgs e);
    public delegate void SelectErrorEventHandler(object sender, SelectErrorEventArgs e);
    public delegate void InfoEventHandler(object sender, InfoEventArgs e);

    public delegate void TraceEventHandler(object sender, TraceEventArgs e);


}
