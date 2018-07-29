using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
namespace ZXStudio
{
    public class Globals
    {
        [DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr hWnd, uint Msg);

        public const uint SW_RESTORE = 0x09;

        public static void EnsureVisible(IntPtr handle)
        {
            ShowWindow(handle, SW_RESTORE);
        }
        static IMemory memory;
        public static IMemory Memory
        {
            get
            {
                return memory;
            }
            set
            {
                memory = value;
            }
        }

        public static string romfile;
        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr hwnd;
            public int message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public int pointX;
            public int pointY;
        }

        public const int PM_REMOVE = 0x01;

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool PeekMessage(out MSG msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool TranslateMessage([In] ref MSG m);

        [DllImport("user32.dll")]
        public static extern bool DispatchMessage([In] ref MSG m);
        public bool DesignMode
        {
            get
            {
                return (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv");
            }
        }

        public static void DoEvents()
        {
            while (Globals.PeekMessage(out Globals.MSG msg, IntPtr.Zero, 0, 0, Globals.PM_REMOVE))
            {
                Globals.TranslateMessage(ref msg);
                Globals.DispatchMessage(ref msg);
            }
        }
        static object currentDocument;
        public static object CurrentDocument
        {
            get
            {
                return currentDocument;
            }
            set
            {
                currentDocument = value;
            }
        }
        public static string GetFilename(string location, string name, string type, bool directory = false)
        {
            string fileName;
            int count = 1;

            if (directory)
                fileName = location + "\\" + name + count.ToString();
            else
                fileName = location + "\\" + name + count.ToString() + "." + type;
            while (true)
            {
                if (directory)
                {
                    if (!Directory.Exists(fileName)) break;
                }
                else
                {
                    if (!File.Exists(fileName)) break;
                }
                count++;
                if (directory)
                    fileName = location + "\\" + name + count.ToString();
                else
                    fileName = location + "\\" + name + count.ToString() + "." + type;
            }
            return fileName;

        }

        public static byte[] ToByteArray(string str)
        {
            return System.Text.ASCIIEncoding.Default.GetBytes(str);
        }
        public static string ToString(byte[] bytes)
        {

            return System.Text.ASCIIEncoding.Default.GetString(bytes);
        }

        public static string StartAddress { get; set; }
    }
}
