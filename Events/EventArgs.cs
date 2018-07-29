using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ZXStudio
{
    public class LogErrorEventArgs : EventArgs
    {
        public Token Token;
        public string Message;
        public int Column;
        public int Line;
        public LogErrorEventArgs(Token token, string message, int column, int line)
        {
            Token = token;
            Message = message;
            Column = column;
            Line = line;
        }
    }

    public class CloseEventArgs : EventArgs
    {
        private int nTabIndex = -1;
        public CloseEventArgs(int nTabIndex)
        {
            this.nTabIndex = nTabIndex;
        }
        /// <summary>
        /// Get/Set the tab index value where the close button is clicked
        /// </summary>
        public int TabIndex
        {
            get
            {
                return this.nTabIndex;
            }
            set
            {
                this.nTabIndex = value;
            }
        }

    }

    public class SelectFileEventArgs : EventArgs
    {
        public object File;
        public string FileType
        {
            get
            {
                return Path.GetExtension(File.ToString());
            }
        }

        public SelectFileEventArgs(object file)
        {
            File = file;
        }
    }

    public class RunInEmulatorEventArgs : EventArgs
    {
        public int[] Memory;
        public ushort StartAddress;
        public bool Debug;
        public RunInEmulatorEventArgs(int[] memory, ushort startAddress, bool debug)
        {
            Memory = memory;
            StartAddress = startAddress;
            Debug = debug;
        }
    }

    public class LoadInEmulatorEventArgs : EventArgs
    {
        public int[] Memory;
        public ushort StartAddress;
        public LoadInEmulatorEventArgs(int[] memory, ushort startAddress)
        {
            Memory = memory;
            StartAddress = startAddress;
        }
    }

    public class AssembledLineEventArgs : EventArgs
    {
        public int StartAddress;
        public int Length;
        public AssembledLineEventArgs(int startaddress, int length)
        {
            StartAddress = startaddress;
            Length = length;
        }

    }

    public class SelectErrorEventArgs : EventArgs
    {
        public string FileName;
        public int Line;
        public int Column;
        public SelectErrorEventArgs(string fileName, int line,int column)
        {
            FileName = fileName;
            Line = line;
            Column = column;
        }

    }

    public class InfoEventArgs:EventArgs
    {
        public string Message;
        public InfoEventArgs(string message)
        {
            Message = message;
        }
    }

    public class EditTreeNodeEventArgs : EventArgs
    {
        public TreeNode Node;
        public EditTreeNodeEventArgs(TreeNode node)
        {
            Node= node;
        }

    }
    
    public class AsseblyStartEventArgs : EventArgs
    {
        public AsseblyStartEventArgs()
        {
        }
    }
    public class AsseblyCompleteEventArgs : EventArgs
    {
        public bool Success;
        public AsseblyCompleteEventArgs(bool success)
        {
            Success = success;
        }
    }

    public class FileCreatedEventArgs : EventArgs
    {
        public FileCreatedEventArgs() : base()
        { }
    }


    public class TraceEventArgs : EventArgs
    {
        public bool Go;

        public TraceEventArgs(bool go)
        {
            Go = go;
        }
    }
}
