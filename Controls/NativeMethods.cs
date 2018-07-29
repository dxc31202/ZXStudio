using System;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

public class NativeMethods
{
    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    public extern static int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

    public static Guid IID_IShellFolder = new Guid(
        "{000214E6-0000-0000-C000-000000000046}"
        );

    public enum SHCONTF
    {
        SHCONTF_FOLDERS = 0x0020,
        SHCONTF_NONFOLDERS = 0x0040,
        SHCONTF_INCLUDEHIDDEN = 0x0080,
        SHCONTF_INIT_ON_FIRST_NEXT = 0x0100,
        SHCONTF_NETPRINTERSRCH = 0x0200,
        SHCONTF_SHAREABLE = 0x0400,
        SHCONTF_STORAGE = 0x0800
    }

    [Flags]
    public enum SHCIDS : uint
    {
        SHCIDS_ALLFIELDS = 0x80000000,
        SHCIDS_CANONICALONLY = 0x10000000,
        SHCIDS_BITMASK = 0xFFFF0000,
        SHCIDS_COLUMNMASK = 0x0000FFFF
    }

    [Flags]
    public enum FOLDERFLAGS
    {
        FWF_AUTOARRANGE = 0x1,
        FWF_ABBREVIATEDNAMES = 0x2,
        FWF_SNAPTOGRID = 0x4,
        FWF_OWNERDATA = 0x8,
        FWF_BESTFITWINDOW = 0x10,
        FWF_DESKTOP = 0x20,
        FWF_SINGLESEL = 0x40,
        FWF_NOSUBFOLDERS = 0x80,
        FWF_TRANSPARENT = 0x100,
        FWF_NOCLIENTEDGE = 0x200,
        FWF_NOSCROLL = 0x400,
        FWF_ALIGNLEFT = 0x800,
        FWF_NOICONS = 0x1000,
        FWF_SHOWSELALWAYS = 0x2000,
        FWF_NOVISIBLE = 0x4000,
        FWF_SINGLECLICKACTIVATE = 0x8000,
        FWF_NOWEBVIEW = 0x10000,
        FWF_HIDEFILENAMES = 0x20000,
        FWF_CHECKSELECT = 0x40000
    }

    public enum FOLDERVIEWMODE
    {
        FVM_FIRST = 1,
        FVM_ICON = 1,
        FVM_SMALLICON = 2,
        FVM_LIST = 3,
        FVM_DETAILS = 4,
        FVM_THUMBNAIL = 5,
        FVM_TILE = 6,
        FVM_THUMBSTRIP = 7,
        FVM_LAST = 7
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct STRRET
    {
        [FieldOffset(0)]
        UInt32 uType;
        [FieldOffset(4)]
        IntPtr pOleStr;
        [FieldOffset(4)]
        IntPtr pStr;
        [FieldOffset(4)]
        UInt32 uOffset;
        [FieldOffset(4)]
        IntPtr cStr;
    }

    [Flags()]
    public enum SFGAO : uint
    {
        SFGAO_CANCOPY = 0x000000001,
        SFGAO_CANMOVE = 0x000000002,
        SFGAO_CANLINK = 0x000000004,
        SFGAO_STORAGE = 0x000000008,
        SFGAO_CANRENAME = 0x00000010,
        SFGAO_CANDELETE = 0x00000020,
        SFGAO_HASPROPSHEET = 0x00000040,
        SFGAO_DROPTARGET = 0x00000100,
        SFGAO_CAPABILITYMASK = 0x00000177,
        SFGAO_ENCRYPTED = 0x00002000,
        SFGAO_ISSLOW = 0x00004000,
        SFGAO_GHOSTED = 0x00008000,
        SFGAO_LINK = 0x00010000,
        SFGAO_SHARE = 0x00020000,
        SFGAO_READONLY = 0x00040000,
        SFGAO_HIDDEN = 0x00080000,
        SFGAO_DISPLAYATTRMASK = 0x000FC000,
        SFGAO_FILESYSANCESTOR = 0x10000000,
        SFGAO_FOLDER = 0x20000000,
        SFGAO_FILESYSTEM = 0x40000000,
        SFGAO_HASSUBFOLDER = 0x80000000,
        SFGAO_CONTENTSMASK = 0x80000000,
        SFGAO_VALIDATE = 0x01000000,
        SFGAO_REMOVABLE = 0x02000000,
        SFGAO_COMPRESSED = 0x04000000,
        SFGAO_BROWSABLE = 0x08000000,
        SFGAO_NONENUMERATED = 0x00100000,
        SFGAO_NEWCONTENT = 0x00200000,
        SFGAO_CANMONIKER = 0x00400000,
        SFGAO_HASSTORAGE = 0x00400000,
        SFGAO_STREAM = 0x00400000,
        SFGAO_STORAGEANCESTOR = 0x00800000,
        SFGAO_STORAGECAPMASK = 0x70C50008,
        SFGAO_SYSTEM = 0x00001000,
    }

    [Flags()]
    public enum SHGDN
    {
        SHGDN_NORMAL = 0,
        SHGDN_INFOLDER = 1,
        SHGDN_FORADDRESSBAR = 16384,
        SHGDN_FORPARSING = 32768
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public RECT(Rectangle r)
        {
            left = r.Left;
            top = r.Top;
            right = r.Right;
            bottom = r.Bottom;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FOLDERSETTINGS
    {
        public FOLDERFLAGS ViewMode;
        public FOLDERVIEWMODE fFlags;
    }

    public enum SVUIA_STATUS
    {
        SVUIA_DEACTIVATE = 0,
        SVUIA_ACTIVATE_NOFOCUS = 1,
        SVUIA_ACTIVATE_FOCUS = 2,
        SVUIA_INPLACEACTIVATE = 3
    }

    [ComImportAttribute()]
    [GuidAttribute(
    "000214F2-0000-0000-C000-000000000046")]
    [InterfaceTypeAttribute(
    ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumIDList
    {
        [PreserveSig]
        int Next(
            int celt,
            ref IntPtr
            rgelt, out
            int pceltFetched
            );
        [PreserveSig]
        int Skip(int celt);
        [PreserveSig]
        int Reset();
        [PreserveSig]
        int Clone(
            ref IEnumIDList ppenum
            );
    }

    [ComImport()]
    [Guid(
    "000214E2-0000-0000-C000-000000000046")]
    [InterfaceType(
    ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellBrowser
    {
        [PreserveSig]
        int GetWindow(out IntPtr phwnd);
        [PreserveSig]
        int ContextSensitiveHelp(bool fEnterMode);
        [PreserveSig]
        int InsertMenusSB(
            IntPtr hmenuShared,
            ref IntPtr lpMenuWidths
            );
        [PreserveSig]
        int SetMenuSB(
            IntPtr hmenuShared,
            IntPtr holemenuRes,
            IntPtr hwndActiveObject
            );
        [PreserveSig]
        int RemoveMenusSB(
            IntPtr hmenuShared
            );
        [PreserveSig]
        int SetStatusTextSB(
            string pszStatusText
            );
        [PreserveSig]
        int EnableModelessSB(
            bool fEnable
            );
        [PreserveSig]
        int TranslateAcceleratorSB(
            IntPtr pmsg,
            short wID
            );
        [PreserveSig]
        int BrowseObject(
            IntPtr pidl,
            uint wFlags
            );
        [PreserveSig]
        int GetViewStateStream(
            long grfMode,
            ref IStream ppStrm
            );
        [PreserveSig]
        int GetControlWindow(
            uint id,
            ref IntPtr phwnd
            );
        [PreserveSig]
        int SendControlMsg(
            uint id,
            uint uMsg,
            short wParam,
            long lParam,
            ref long pret
            );
        [PreserveSig]
        int QueryActiveShellView(
            ref IShellView ppshv
            );
        [PreserveSig]
        int OnViewWindowActive(
            IShellView pshv
            );
        [PreserveSig]
        int SetToolbarItems(
            IntPtr lpButtons,
            uint nButtons,
            uint uFlags
            );
    }

    [ComImport()]
    [Guid("000214E3-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellView
    {
        [PreserveSig]
        int GetWindow(out IntPtr phwnd);
        [PreserveSig]
        int ContextSensitiveHelp(bool fEnterMode);
        [PreserveSig]
        int TranslateAcceleratorA(IntPtr pmsg);
        [PreserveSig]
        int EnableModeless(bool fEnable);
        [PreserveSig]
        int UIActivate(SVUIA_STATUS uState);
        [PreserveSig]
        int Refresh();
        [PreserveSig]
        int CreateViewWindow(
            IShellView psvPrevious,
            ref FOLDERSETTINGS pfs,
            IShellBrowser psb,
            ref RECT prcView,
            out IntPtr phWnd
            );
        [PreserveSig]
        int DestroyViewWindow();
        [PreserveSig]
        int GetCurrentInfo(ref FOLDERSETTINGS pfs);
        [PreserveSig]
        int AddPropertySheetPages(
            long dwReserved,
             ref IntPtr pfnPtr,
             int lparam
             );
        [PreserveSig]
        int SaveViewState();
        [PreserveSig]
        int SelectItem(IntPtr pidlItem, uint uFlags);
        [PreserveSig]
        int GetItemObject(
            uint uItem,
            ref Guid riid,
            ref IntPtr ppv
            );
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214E6-0000-0000-C000-000000000046")]
    public interface IShellFolder
    {
        [PreserveSig]
        Int32 ParseDisplayName(
            IntPtr hwnd,
            IntPtr pbc,
            [MarshalAs(UnmanagedType.LPWStr)]
                string pszDisplayName,
            ref UInt32 pchEaten,
            out IntPtr ppidl,
            ref UInt32 pdwAttributes
            );
        [PreserveSig]
        Int32 EnumObjects(
            IntPtr hwnd,
            SHCONTF grfFlags,
            out IEnumIDList ppenumIDList
            );
        [PreserveSig]
        Int32 BindToObject(
            IntPtr pidl,
            IntPtr pbc,
            ref Guid riid,
            out IntPtr ppv
            );
        [PreserveSig]
        Int32 BindToStorage(
            IntPtr pidl,
            IntPtr pbc,
            ref Guid riid,
            out IntPtr ppv
            );
        [PreserveSig]
        Int32 CompareIDs(
            SHCIDS lParam,
            IntPtr pidl1,
            IntPtr pidl2
            );
        [PreserveSig]
        Int32 CreateViewObject(
            IntPtr hwndOwner,
            ref Guid riid,
            out IShellView ppv
            );
        [PreserveSig]
        Int32 GetAttributesOf(
            UInt32 cidl,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
                IntPtr[] apidl,
            ref SFGAO rgfInOut
            );
        [PreserveSig]
        Int32 GetUIObjectOf(
            IntPtr hwndOwner,
            UInt32 cidl,
            IntPtr[] apidl,
            ref Guid riid,
            UInt32 rgfReserved,
            out IntPtr ppv
            );
        [PreserveSig]
        Int32 GetDisplayNameOf(
            IntPtr pidl,
            SHGDN uFlags,
            out STRRET pName
            );
        [PreserveSig]
        Int32 SetNameOf(
            IntPtr hwnd,
            IntPtr pidl,
            [MarshalAs(UnmanagedType.LPWStr)]
                string pszName,
            UInt32 uFlags,
            out IntPtr ppidlOut
            );
    }

    [Flags]
    public enum SHGFI
    {
        SHGFI_ICON = 0x000000100,
        SHGFI_DISPLAYNAME = 0x000000200,
        SHGFI_TYPENAME = 0x000000400,
        SHGFI_ATTRIBUTES = 0x000000800,
        SHGFI_ICONLOCATION = 0x000001000,
        SHGFI_EXETYPE = 0x000002000,
        SHGFI_SYSICONINDEX = 0x000004000,
        SHGFI_LINKOVERLAY = 0x000008000,
        SHGFI_SELECTED = 0x000010000,
        SHGFI_ATTR_SPECIFIED = 0x000020000,
        SHGFI_LARGEICON = 0x000000000,
        SHGFI_SMALLICON = 0x000000001,
        SHGFI_OPENICON = 0x000000002,
        SHGFI_SHELLICONSIZE = 0x000000004,
        SHGFI_PIDL = 0x000000008,
        SHGFI_USEFILEATTRIBUTES = 0x000000010,
        SHGFI_ADDOVERLAYS = 0x000000020,
        SHGFI_OVERLAYINDEX = 0x000000040
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public Int32 iIcon;
        public UInt32 dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    [Flags]
    public enum CSIDL
    {
        CSIDL_FLAG_CREATE = 0x8000,
        CSIDL_ADMINTOOLS = 0x0030,
        CSIDL_ALTSTARTUP = 0x001d,
        CSIDL_APPDATA = 0x001a,
        CSIDL_BITBUCKET = 0x000a,
        CSIDL_CDBURN_AREA = 0x003b,
        CSIDL_COMMON_ADMINTOOLS = 0x002f,
        CSIDL_COMMON_ALTSTARTUP = 0x001e,
        CSIDL_COMMON_APPDATA = 0x0023,
        CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019,
        CSIDL_COMMON_DOCUMENTS = 0x002e,
        CSIDL_COMMON_FAVORITES = 0x001f,
        CSIDL_COMMON_MUSIC = 0x0035,
        CSIDL_COMMON_PICTURES = 0x0036,
        CSIDL_COMMON_PROGRAMS = 0x0017,
        CSIDL_COMMON_STARTMENU = 0x0016,
        CSIDL_COMMON_STARTUP = 0x0018,
        CSIDL_COMMON_TEMPLATES = 0x002d,
        CSIDL_COMMON_VIDEO = 0x0037,
        CSIDL_CONTROLS = 0x0003,
        CSIDL_COOKIES = 0x0021,
        CSIDL_DESKTOP = 0x0000,
        CSIDL_DESKTOPDIRECTORY = 0x0010,
        CSIDL_DRIVES = 0x0011,
        CSIDL_FAVORITES = 0x0006,
        CSIDL_FONTS = 0x0014,
        CSIDL_HISTORY = 0x0022,
        CSIDL_INTERNET = 0x0001,
        CSIDL_INTERNET_CACHE = 0x0020,
        CSIDL_LOCAL_APPDATA = 0x001c,
        CSIDL_MYDOCUMENTS = 0x000c,
        CSIDL_MYMUSIC = 0x000d,
        CSIDL_MYPICTURES = 0x0027,
        CSIDL_MYVIDEO = 0x000e,
        CSIDL_NETHOOD = 0x0013,
        CSIDL_NETWORK = 0x0012,
        CSIDL_PERSONAL = 0x0005,
        CSIDL_PRINTERS = 0x0004,
        CSIDL_PRINTHOOD = 0x001b,
        CSIDL_PROFILE = 0x0028,
        CSIDL_PROFILES = 0x003e,
        CSIDL_PROGRAM_FILES = 0x0026,
        CSIDL_PROGRAM_FILES_COMMON = 0x002b,
        CSIDL_PROGRAMS = 0x0002,
        CSIDL_RECENT = 0x0008,
        CSIDL_SENDTO = 0x0009,
        CSIDL_STARTMENU = 0x000b,
        CSIDL_STARTUP = 0x0007,
        CSIDL_SYSTEM = 0x0025,
        CSIDL_TEMPLATES = 0x0015,
        CSIDL_WINDOWS = 0x0024
    }


    [DllImport("shell32.dll", EntryPoint = "#18")]
    public static extern IntPtr ILClone(
        IntPtr pidl
        );

    [DllImport("shell32.dll", EntryPoint = "#25")]
    public static extern IntPtr ILCombine(
        IntPtr pidlA,
        IntPtr pidlB
        );

    [DllImport("shell32.dll", EntryPoint = "#17")]
    public static extern bool ILRemoveLastID(
        IntPtr pidl
        );

    [DllImport("shell32.dll")]
    public static extern Int32 SHGetDesktopFolder(
        out IShellFolder ppshf
        );

    [DllImport("shell32.dll")]
    public static extern Int32 SHGetFolderLocation(
        IntPtr hwndOwner,
        CSIDL nFolder,
        IntPtr hToken,
        UInt32 dwReserved,
        out IntPtr ppidl
        );

    [DllImport("shell32.dll", CharSet = CharSet.Ansi)]
    public static extern IntPtr SHGetFileInfo(
        string fileName,
        uint dwFileAttributes,
        ref SHFILEINFO psfi,
        int cbSizeFileInfo,
        SHGFI flags
        );

    [DllImport("shell32.dll", CharSet = CharSet.Ansi)]
    public static extern IntPtr SHGetFileInfo(
        IntPtr pidl,
        uint dwFileAttributes,
        ref SHFILEINFO psfi,
        int cbSizeFileInfo,
        SHGFI flags
        );

    [DllImport("shell32.dll")]
    public static extern bool SHGetPathFromIDList(
        IntPtr pidl,
        StringBuilder pszPath
        );

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    internal static extern void SHCreateItemFromParsingName(
     [In][MarshalAs(UnmanagedType.LPWStr)] string pszPath,
     [In] IntPtr pbc,
     [In][MarshalAs(UnmanagedType.LPStruct)] Guid riid,
     [Out][MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] out IShellItem ppv);

    [DllImport("shell32.dll", PreserveSig = false)]
    internal static extern void SHCreateItemFromIDList(
         [In] IntPtr pidl,
         [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
         [Out, MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] out IShellItem ppv);

    public const uint CLR_NONE = 0xFFFFFFFF;
    public const uint CLR_DEFAULT = 0xFF000000;

    [Flags]
    public enum ImageListDrawingStyle : uint
    {
        Normal = 0x00000000,
        Transparent = 0x00000001,
        Blend25 = 0x00000002,
        Blend50 = 0x00000004,
        Mask = 0x00000010,
        Image = 0x00000020,
        Rop = 0x00000040,
        OverlayMask = 0x00000F00,
        PreserveAlpha = 0x00001000, // This preserves the alpha channel in dest
        Scale = 0x00002000, // Causes the image to be scaled to cx, cy instead of clipped
        DpiScale = 0x00004000,
        Async = 0x00008000,
    }

    [DllImport("comctl32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public extern static bool ImageList_SetOverlayImage(IntPtr himl, int iImage, int iOverlay);

    [DllImport("comctl32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public extern static bool ImageList_DrawEx(IntPtr himl, int i, IntPtr hdcDst, int x, int y, int dx, int dy, uint rgbBk, uint rgbFg, ImageListDrawingStyle fStyle);

    // The CharSet must match the CharSet of the corresponding PInvoke signature
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WIN32_FIND_DATA
    {
        public uint dwFileAttributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
        public uint dwReserved0;
        public uint dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternateFileName;
    }

    [DllImport("kernel32.dll")]
    public static extern bool FindClose(IntPtr hFindFile);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
    out ulong lpFreeBytesAvailable,
    out ulong lpTotalNumberOfBytes,
    out ulong lpTotalNumberOfFreeBytes);

    [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
    public static extern long StrFormatByteSize(
    long fileSize,
    [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer,
    int bufferSize);

}
