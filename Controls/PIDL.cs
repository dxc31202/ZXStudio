using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;

public class PIDL : IDisposable
{
    static readonly NativeMethods.IShellFolder desktopFolder;
    static readonly IntPtr desktopPIDL;
    IntPtr pidl;
    public IntPtr ParentPidl;
    string displayName;

    string typeName;
    string physicalPath;
    NativeMethods.SFGAO m_attributes;

    public IntPtr Pidl { get { return this.pidl; } }
    public string DisplayName { get { return displayName; } set { displayName = value; } }

    public string TypeName { get { return typeName; } }
    public string PhysicalPath { get { return physicalPath; } }

    public bool CanRename { get { return (m_attributes & NativeMethods.SFGAO.SFGAO_CANRENAME) != 0; } }
    public bool CanMove { get { return (m_attributes & NativeMethods.SFGAO.SFGAO_CANMOVE) != 0; } }
    public bool CanDelete { get { return (m_attributes & NativeMethods.SFGAO.SFGAO_CANDELETE) != 0; } }
    public bool CanCopy { get { return (m_attributes & NativeMethods.SFGAO.SFGAO_CANCOPY) != 0; } }
    public bool IsReadOnly { get { return (m_attributes & NativeMethods.SFGAO.SFGAO_READONLY) != 0; } }
    public bool IsEncrypted { get { return (m_attributes & NativeMethods.SFGAO.SFGAO_ENCRYPTED) != 0; } }
    public bool IsLink { get { return (m_attributes & NativeMethods.SFGAO.SFGAO_LINK) != 0; } }
    public NativeMethods.SFGAO Attributes { get { return (m_attributes); } }
    public uint IntAttributes { get { return (uint)m_attributes; } }
    public bool IsHidden { get { return (m_attributes & NativeMethods.SFGAO.SFGAO_HIDDEN) != 0; } }
    public bool IsSlow { get { return (m_attributes & NativeMethods.SFGAO.SFGAO_ISSLOW) != 0; } }
    public bool IsGhosted { get { return (m_attributes & NativeMethods.SFGAO.SFGAO_GHOSTED) != 0; } }
    public bool IsCompressed { get { return ((m_attributes & NativeMethods.SFGAO.SFGAO_COMPRESSED) != 0)|| PhysicalPath.ToLower().EndsWith(".zip"); } }
    public bool IsRemovable { get { return (m_attributes & NativeMethods.SFGAO.SFGAO_REMOVABLE) != 0; } }
    public bool IsShared { get { return (m_attributes & NativeMethods.SFGAO.SFGAO_SHARE) != 0; } }
    public bool IsFolder { get { return (m_attributes & NativeMethods.SFGAO.SFGAO_FOLDER) != 0; } }
    public bool IsFileSystem { get { return (m_attributes & NativeMethods.SFGAO.SFGAO_FILESYSTEM) != 0; } }
    public bool HasSubfolders { get { return (m_attributes & NativeMethods.SFGAO.SFGAO_HASSUBFOLDER) != 0; } }
    public bool IsBrowsable { get { return (m_attributes & NativeMethods.SFGAO.SFGAO_BROWSABLE) != 0; } }
    public bool IsDesktop { get { return desktopFolder.CompareIDs(NativeMethods.SHCIDS.SHCIDS_CANONICALONLY, this.pidl, desktopPIDL) == 0; } }

    public bool IsDisk
    {
        get
        {
            return ((uint)m_attributes == 4034920797);
        }
    }

    public bool HasChildren
    {
        get
        {
            return Children(false, false);
        }
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public bool Children(bool showHiddenObjects, bool showNonFolders)
    {
        using (ShellFolder sf = new ShellFolder(this))
        {
            bool children = false;
            NativeMethods.IEnumIDList enumList = null;
            try
            {
                if (!this.IsFolder || sf.Folder == null)
                    return children;
                int hr = sf.Folder.EnumObjects(
                    IntPtr.Zero,
                    NativeMethods.SHCONTF.SHCONTF_FOLDERS |
                    (showNonFolders ? NativeMethods.SHCONTF.SHCONTF_NONFOLDERS : 0) |
                    (showHiddenObjects ? NativeMethods.SHCONTF.SHCONTF_INCLUDEHIDDEN : 0),
                    out enumList
                    );
                if (hr != 0)
                    return children;
                IntPtr pidl = IntPtr.Zero;
                int fetched = 0;
                while (enumList.Next(1, ref pidl, out fetched) == 0 && fetched == 1)
                {
                    Marshal.FreeCoTaskMem(pidl);
                    pidl = IntPtr.Zero;
                    fetched = 0;
                    children = true;
                    break;
                }
            }
            finally
            {
                if (enumList != null)
                    Marshal.ReleaseComObject(enumList);

                enumList = null;
            }
            return children;
        }
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    static PIDL()
    {
        NativeMethods.SHGetDesktopFolder(out desktopFolder);
        IntPtr hToken = new IntPtr(-1);
        int hr = NativeMethods.SHGetFolderLocation(IntPtr.Zero, NativeMethods.CSIDL.CSIDL_DESKTOP, hToken, 0, out desktopPIDL);
    }

    public PIDL()
    {

    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public PIDL(IntPtr pidl)
    {
        Open(pidl);
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public PIDL(IntPtr parentPidl, IntPtr pidl)
    {
        Open(parentPidl, pidl);
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public PIDL(Environment.SpecialFolder specialFolder)
    {
        Open(specialFolder);
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public PIDL(string fullPath)
    {
        Open(fullPath);
    }

    ~PIDL()
    {
        Dispose();

    }

    public void Dispose()
    {
        Dispose(true);

        // Use SupressFinalize in case a subclass 
        // of this type implements a finalizer.
        GC.SuppressFinalize(this);
    }
    bool disposed = false;
    protected virtual void Dispose(bool disposing)
    {
        // If you need thread safety, use a lock around these  
        // operations, as well as in your methods that use the resource. 
        if (!disposed)
        {
            if (disposing)
            {
                Close();
            }

        }
    }


    public override bool Equals(object obj)
    {
        PIDL pidl = obj as PIDL;
        if (pidl == null)
            return false;
        if (desktopFolder.CompareIDs(NativeMethods.SHCIDS.SHCIDS_CANONICALONLY, this.pidl, pidl.pidl) == 0) return true;
        if (desktopFolder.CompareIDs(NativeMethods.SHCIDS.SHCIDS_ALLFIELDS, this.pidl, pidl.pidl) == 0) return true;
        if (desktopFolder.CompareIDs(NativeMethods.SHCIDS.SHCIDS_BITMASK, this.pidl, pidl.pidl) == 0) return true;
        if (desktopFolder.CompareIDs(NativeMethods.SHCIDS.SHCIDS_COLUMNMASK, this.pidl, pidl.pidl) == 0) return true;
        return false;

    }

    public override int GetHashCode()
    {
        return base.GetHashCode() ^ this.pidl.GetHashCode();
    }

    public void Open(IntPtr pidl)
    {
        this.pidl = NativeMethods.ILClone(pidl);
        InitializeObject();

    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public void Open(IntPtr parentPidl, IntPtr pidl)
    {

        this.ParentPidl = parentPidl;
        this.pidl = NativeMethods.ILCombine(parentPidl, pidl);
        InitializeObject();
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public void Open(Environment.SpecialFolder specialFolder)
    {
        IntPtr hToken = new IntPtr(-1);
        int hr = NativeMethods.SHGetFolderLocation(IntPtr.Zero, SpecialFolderToCSIDL(specialFolder), hToken, 0, out this.pidl);
        if (hr != 0)
            Marshal.ThrowExceptionForHR(hr);
        InitializeObject();
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public void Open(string fullPath)
    {
        uint attr = 0;
        uint pchEaten = 0;
        int hr = desktopFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fullPath, ref pchEaten, out this.pidl, ref attr);
        InitializeObject();
    }

    public void Close()
    {
        if (this.pidl != IntPtr.Zero)
            Marshal.FreeCoTaskMem(this.pidl);
        this.pidl = IntPtr.Zero;
        disposed = true;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public PIDL GetParentFolder()
    {
        if (this.pidl == IntPtr.Zero)
            return null;
        if (desktopFolder.CompareIDs(NativeMethods.SHCIDS.SHCIDS_ALLFIELDS, this.pidl, desktopPIDL) == 0)
            return null;
        IntPtr parentPidl = NativeMethods.ILClone(this.pidl);
        if (!NativeMethods.ILRemoveLastID(parentPidl))
        {
            Marshal.FreeCoTaskMem(parentPidl);
            parentPidl = IntPtr.Zero;
            return null;
        }
        return new PIDL(parentPidl);
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public System.Collections.ArrayList GetAncestors()
    {
        System.Collections.ArrayList list = new System.Collections.ArrayList();
        if (this.pidl == IntPtr.Zero)
            return list;
        PIDL pidl = (PIDL)Clone();
        while (pidl != null)
        {
            list.Add(pidl);
            pidl = pidl.GetParentFolder();
        }
        return list;
    }
    int iconIndex;
    public int IconIndex { get { return iconIndex; } }

    public NativeMethods.SHFILEINFO FileInto(NativeMethods.SHGFI fileinfo)
    {
        NativeMethods.SHFILEINFO shfi = new NativeMethods.SHFILEINFO();
        NativeMethods.SHGetFileInfo(this.pidl, 0, ref shfi, Marshal.SizeOf(shfi), fileinfo);
        return shfi;
    }
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    void InitializeObject()
    {
        NativeMethods.SHFILEINFO shfi = new NativeMethods.SHFILEINFO();
        IntPtr result = IntPtr.Zero;
        try
        {
            result = NativeMethods.SHGetFileInfo(this.pidl, 0, ref shfi, Marshal.SizeOf(shfi),
                NativeMethods.SHGFI.SHGFI_PIDL |
                NativeMethods.SHGFI.SHGFI_DISPLAYNAME |
                NativeMethods.SHGFI.SHGFI_ATTRIBUTES |
                NativeMethods.SHGFI.SHGFI_TYPENAME 
            );
        }
        catch
        {
            System.Diagnostics.Debug.Write("Error in PIDL.InitializeObject");
        }
        displayName = shfi.szDisplayName;
        iconIndex = shfi.iIcon;
        typeName = shfi.szTypeName;

        m_attributes = (NativeMethods.SFGAO)shfi.dwAttributes;
        StringBuilder sb = new StringBuilder(260);
        NativeMethods.SHGetPathFromIDList(this.pidl, sb);
        physicalPath = sb.ToString();
    }

    static NativeMethods.CSIDL SpecialFolderToCSIDL(Environment.SpecialFolder sf)
    {
        switch (sf)
        {
            case Environment.SpecialFolder.ApplicationData: return NativeMethods.CSIDL.CSIDL_APPDATA;
            case Environment.SpecialFolder.CommonApplicationData: return NativeMethods.CSIDL.CSIDL_COMMON_APPDATA;
            case Environment.SpecialFolder.CommonProgramFiles: return NativeMethods.CSIDL.CSIDL_COMMON_PROGRAMS;
            case Environment.SpecialFolder.Cookies: return NativeMethods.CSIDL.CSIDL_COOKIES;
            case Environment.SpecialFolder.Desktop: return NativeMethods.CSIDL.CSIDL_DESKTOP;
            case Environment.SpecialFolder.DesktopDirectory: return NativeMethods.CSIDL.CSIDL_DESKTOPDIRECTORY;
            case Environment.SpecialFolder.Favorites: return NativeMethods.CSIDL.CSIDL_FAVORITES;
            case Environment.SpecialFolder.History: return NativeMethods.CSIDL.CSIDL_HISTORY;
            case Environment.SpecialFolder.InternetCache: return NativeMethods.CSIDL.CSIDL_INTERNET_CACHE;
            case Environment.SpecialFolder.LocalApplicationData: return NativeMethods.CSIDL.CSIDL_LOCAL_APPDATA;
            case Environment.SpecialFolder.MyComputer: return NativeMethods.CSIDL.CSIDL_DRIVES;
            case Environment.SpecialFolder.MyMusic: return NativeMethods.CSIDL.CSIDL_MYMUSIC;
            case Environment.SpecialFolder.MyPictures: return NativeMethods.CSIDL.CSIDL_MYPICTURES;
            case Environment.SpecialFolder.Personal: return NativeMethods.CSIDL.CSIDL_PERSONAL;
            case Environment.SpecialFolder.ProgramFiles: return NativeMethods.CSIDL.CSIDL_PROGRAM_FILES;
            case Environment.SpecialFolder.Programs: return NativeMethods.CSIDL.CSIDL_PROGRAMS;
            case Environment.SpecialFolder.Recent: return NativeMethods.CSIDL.CSIDL_RECENT;
            case Environment.SpecialFolder.SendTo: return NativeMethods.CSIDL.CSIDL_SENDTO;
            case Environment.SpecialFolder.StartMenu: return NativeMethods.CSIDL.CSIDL_STARTMENU;
            case Environment.SpecialFolder.Startup: return NativeMethods.CSIDL.CSIDL_STARTUP;
            case Environment.SpecialFolder.System: return NativeMethods.CSIDL.CSIDL_SYSTEM;
            case Environment.SpecialFolder.Templates: return NativeMethods.CSIDL.CSIDL_TEMPLATES;
            default: return NativeMethods.CSIDL.CSIDL_DESKTOP;
        }


    }

    public int CompareTo(object obj)
    {
        PIDL pidl = obj as PIDL;
        if (pidl == null)
            return 0;
        return desktopFolder.CompareIDs(NativeMethods.SHCIDS.SHCIDS_CANONICALONLY, pidl.Pidl, this.pidl);
    }

    public object Clone()
    {
        PIDL pidl = new PIDL();
        pidl.pidl = NativeMethods.ILClone(this.pidl);
        pidl.displayName = displayName;
        pidl.iconIndex = iconIndex;
        pidl.typeName = typeName;
        pidl.m_attributes = m_attributes;
        pidl.physicalPath = physicalPath;
        return pidl;
    }
    public override string ToString()
    {
        return displayName;
    }
}