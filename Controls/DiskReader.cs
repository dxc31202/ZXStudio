using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Data;
using System.Linq;
using System.Drawing;

namespace ZXStudio.Controls
{
    public delegate void DiskReadCompleteEvent(DiskReader reader, long Elapsed, ListViewItem item);
    public delegate void ProgressEvent(float value, ListViewItem item);
    public class DiskReader : DiskItem
    {
        Stopwatch stopwatch = new Stopwatch();
        public event DiskReadCompleteEvent DiskReadComplete;
        public event ProgressEvent Progress;
        long FileSize = 0;
        public string Root;
        public DriveInfo DriveInfo;
        ListViewItem item;
        public long Max
        {
            get
            {
                return DriveInfo.TotalSize - DriveInfo.TotalFreeSpace;
            }
        }
        public DiskReader(string disk, ListViewItem item)
            : base(disk)
        {
            this.item = item;
            DriveInfo = new DriveInfo(disk);

            Root = disk;
            ThreadPool.QueueUserWorkItem(ThreadPoolCallback, disk);
        }

        public void OnProgress(float value, ListViewItem item)
        {
            if (Progress != null)
                Progress(value,item);
        }
        public void OnDiskReadComplete()
        {
            stopwatch.Stop();
            DiskReadComplete?.Invoke(this, stopwatch.ElapsedMilliseconds, item);

        }

        private int MagicFindFileCount(string strDirectory, string strFilter)
        {
            int nFiles = 0;
            try {
                nFiles = Directory.GetFiles(strDirectory, strFilter).Length;
            }
            catch { return 0; }
            foreach (String dir in Directory.GetDirectories(strDirectory))
            {
                nFiles += MagicFindFileCount(dir, strFilter);
            }

            return nFiles;
        }
        public void ThreadPoolCallback(Object threadContext)
        {
            if (!DriveInfo.IsReady) return;
            stopwatch.Start();
            Cancel = false;
                TotalSize = DriveInfo.TotalSize- DriveInfo.TotalFreeSpace;//SizeDirectory(threadContext.ToString());
            
            RecurseDirectory(threadContext.ToString(), this);
            OnDiskReadComplete();
        }
        public long TotalSize;
        public bool Cancel = false;
        long SizeDirectory(string directory)
        {
            IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
            NativeMethods.WIN32_FIND_DATA findData;
            IntPtr findHandle;
            if (Cancel) return 0;
            long size = 0;
            // please note that the following line won't work if you try this on a network folder, like \\Machine\C$
            // simply remove the \\?\ part in this case or use \\?\UNC\ prefix
            string slash = directory.EndsWith(@"\") ? "" : @"\";
            string folder = directory + (slash);
            try
            {
                findHandle = NativeMethods.FindFirstFile(folder + @"*.*", out findData);
                int s = Marshal.GetLastWin32Error();
                if (findHandle != INVALID_HANDLE_VALUE)
                {
                    do
                    {
                        if (Cancel) return 0;
                        if ((findData.dwFileAttributes & (int)FileAttributes.Directory) != 0)
                        {

                            if (findData.cFileName == ".") continue;
                            if (findData.cFileName == "..") continue;
                            {
                                if (((findData.dwFileAttributes & (int)FileAttributes.System) == 0) && ((findData.dwFileAttributes & (int)FileAttributes.Hidden) == 0))
                                {
                                    string subdirectory = directory + (directory.EndsWith(@"\") ? "" : @"\") + findData.cFileName;
                                    size += SizeDirectory(subdirectory);
                                }
                            }
                        }
                        else {
                            if (((findData.dwFileAttributes & (int)FileAttributes.System) == 0) && ((findData.dwFileAttributes & (int)FileAttributes.Hidden) == 0))
                            {
                                //size += (findData.nFileSizeLow | (findData.nFileSizeHigh << 8));
                                //FileSize += size;
                                size++;

                            }
                        }
                    }
                    while (NativeMethods.FindNextFile(findHandle, out findData));
                    NativeMethods.FindClose(findHandle);

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return size;

        }
        int lastPCT = -1;
        long CurrentSize = 0;
        void RecurseDirectory(string directory, DiskItem parent)
        {
            IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
            NativeMethods.WIN32_FIND_DATA findData;
            IntPtr findHandle;
            if (Cancel) return;
            // please note that the following line won't work if you try this on a network folder, like \\Machine\C$
            // simply remove the \\?\ part in this case or use \\?\UNC\ prefix
            string slash = directory.EndsWith(@"\") ? "" : @"\";
            string folder = directory + (slash);
            try
            {
                findHandle = NativeMethods.FindFirstFile(folder + @"*.*", out findData);
                int s = Marshal.GetLastWin32Error();
                if (findHandle != INVALID_HANDLE_VALUE)
                {
                    do
                    {
                        if (Cancel) return;
                        if ((findData.dwFileAttributes & (int)FileAttributes.Directory) != 0)
                        {

                            if (findData.cFileName == ".") continue;
                            if (findData.cFileName == "..") continue;
                            {
                                if (((findData.dwFileAttributes & (int)FileAttributes.System) == 0) && ((findData.dwFileAttributes & (int)FileAttributes.Hidden) == 0))
                                {
                                    string subdirectory = directory + (directory.EndsWith(@"\") ? "" : @"\") + findData.cFileName;
                                    DiskItem di = new DiskItem(parent, findData.cFileName);
                                    RecurseDirectory(subdirectory, parent.Add(di));
                                    float percent = (float)FileSize / (float)TotalSize * 100f;
                                    if(lastPCT != (int)percent)
                                        OnProgress(percent, item);
                                    lastPCT = (int)percent;
                                }
                            }
                        }
                        else {
                            if (((findData.dwFileAttributes & (int)FileAttributes.System) == 0) && ((findData.dwFileAttributes & (int)FileAttributes.Hidden) == 0))
                            {
                                long size = (findData.nFileSizeLow | (findData.nFileSizeHigh << 8));
                                FileSize += size;
                                CurrentSize ++;
                                parent.Add(new DiskItem(parent, findData.cFileName, size));
                            }
                        }
                    }
                    while (NativeMethods.FindNextFile(findHandle, out findData));
                    NativeMethods.FindClose(findHandle);

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

        }


    }
}
