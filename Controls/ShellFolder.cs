using System;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;

[ComVisible(false)]
public class ShellFolder : IDisposable
{

    public static readonly NativeMethods.IShellFolder desktopFolder;
    NativeMethods.IShellFolder folder;

    public NativeMethods.IShellFolder Folder
    {
        get { return folder; }
        set { folder = value; }
    }
    PIDL pidl;

    public PIDL Pidl
    {
        get { return this.pidl; }
    }

    public object Interface
    {
        get { return folder; }
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    static ShellFolder()
    {
        NativeMethods.SHGetDesktopFolder(out desktopFolder);
    }

    public ShellFolder()
    {

    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public ShellFolder(Environment.SpecialFolder specialFolder)
    {
        Open(specialFolder);
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public ShellFolder(string fullPath)
    {
        Open(fullPath);
    }

    ~ShellFolder()
    {
        Dispose();
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public ShellFolder(PIDL pidl)
    {
        Open(pidl);
    }

    public void Dispose()
    {
        Close();
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public void Open(Environment.SpecialFolder specialFolder)
    {
        Close();
        this.pidl = new PIDL(specialFolder);
        InitializeFolder();
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public void Open(string fullPath)
    {
        this.pidl = new PIDL(fullPath);
        InitializeFolder();
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public void Open(PIDL pidl)
    {
        this.pidl = (PIDL)pidl.Clone();
        InitializeFolder();
    }

    public void Close()
    {
        if (this.pidl != null)
            this.pidl.Dispose();
        this.pidl = null;
        if (folder != null)
            Marshal.ReleaseComObject(folder);
        folder = null;
    }
    public bool Cancel;
    public delegate bool FoundChildDelegate(PIDL pidl);
    public event FoundChildDelegate FoundChild;
    NativeMethods.IEnumIDList enumList = null;
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public ArrayList GetChildren(bool showHiddenObjects, bool showNonFolders, bool sortResults)
    {
        Cancel = false;
        ArrayList children = new ArrayList();

        if (this.pidl == null || !this.pidl.IsFolder || folder == null) return children;
        int hresult = folder.EnumObjects(IntPtr.Zero, NativeMethods.SHCONTF.SHCONTF_FOLDERS | (showNonFolders ? NativeMethods.SHCONTF.SHCONTF_NONFOLDERS : 0) | (showHiddenObjects ? NativeMethods.SHCONTF.SHCONTF_INCLUDEHIDDEN : 0), out enumList);
        if (hresult != 0)
            return null;
        IntPtr pidl = IntPtr.Zero;
        int fetched = 0;
        while (enumList.Next(1, ref pidl, out fetched) == 0 && fetched == 1)
        {
            if (Cancel) break;
            children.Add(new PIDL(this.pidl.Pidl, pidl));
        }
        if (enumList != null)
            Marshal.ReleaseComObject(enumList);
        enumList = null;
        return children;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public List<PIDL> GetFolders(bool showHiddenObjects)
    {
        Cancel = false;
        List<PIDL> children = new List<PIDL>();

        if (this.pidl == null || !this.pidl.IsFolder || folder == null) return children;
        int hresult = folder.EnumObjects(IntPtr.Zero, NativeMethods.SHCONTF.SHCONTF_FOLDERS | (showHiddenObjects ? NativeMethods.SHCONTF.SHCONTF_INCLUDEHIDDEN : 0), out enumList);
        if (hresult != 0)
            return null;
        IntPtr pidl = IntPtr.Zero;
        int fetched = 0;
        while (enumList.Next(1, ref pidl, out fetched) == 0 && fetched == 1)
        {
            if (Cancel) break;
            children.Add(new PIDL(this.pidl.Pidl, pidl));
        }
        if (enumList != null)
            Marshal.ReleaseComObject(enumList);
        enumList = null;
        return children;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public List<PIDL> GetFiles(bool showHiddenObjects)
    {
        Cancel = false;
        List<PIDL> children = new List<PIDL>();

        if (this.pidl == null || !this.pidl.IsFolder || folder == null) return children;
        int hresult = folder.EnumObjects(IntPtr.Zero, NativeMethods.SHCONTF.SHCONTF_NONFOLDERS | (showHiddenObjects ? NativeMethods.SHCONTF.SHCONTF_INCLUDEHIDDEN : 0), out enumList);
        if (hresult != 0)
            return null;
        IntPtr pidl = IntPtr.Zero;
        int fetched = 0;
        while (enumList.Next(1, ref pidl, out fetched) == 0 && fetched == 1)
        {
            if (Cancel) break;
            children.Add(new PIDL(this.pidl.Pidl, pidl));
        }
        if (enumList != null)
            Marshal.ReleaseComObject(enumList);
        enumList = null;
        return children;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public List<PIDL> GetChildren1(bool showHiddenObjects, bool showNonFolders, bool sortResults)
    {
        Cancel = false;
        List<PIDL> children = new List<PIDL>();

        if (this.pidl == null || !this.pidl.IsFolder || folder == null) return children;
        int hresult = folder.EnumObjects(IntPtr.Zero, NativeMethods.SHCONTF.SHCONTF_FOLDERS | (showNonFolders ? NativeMethods.SHCONTF.SHCONTF_NONFOLDERS : 0) | (showHiddenObjects ? NativeMethods.SHCONTF.SHCONTF_INCLUDEHIDDEN : 0), out enumList);
        if (hresult != 0)
            return null;
        IntPtr pidl = IntPtr.Zero;
        int fetched = 0;
        while (enumList.Next(1, ref pidl, out fetched) == 0 && fetched == 1)
        {
            if (Cancel) break;
            children.Add(new PIDL(this.pidl.Pidl, pidl));
        }
        if (enumList != null)
            Marshal.ReleaseComObject(enumList);
        enumList = null;
        return children;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public bool GetChildrenAsync(bool showHiddenObjects, bool showNonFolders, bool sortResults, FoundChildDelegate callback)
    {
        if (this.pidl == null || !this.pidl.IsFolder || folder == null) return true;
        int hresult = folder.EnumObjects(IntPtr.Zero, NativeMethods.SHCONTF.SHCONTF_FOLDERS | (showNonFolders ? NativeMethods.SHCONTF.SHCONTF_NONFOLDERS : 0) | (showHiddenObjects ? NativeMethods.SHCONTF.SHCONTF_INCLUDEHIDDEN : 0), out enumList);
        if (hresult != 0) return true;
        IntPtr pidl = IntPtr.Zero;
        int fetched = 0;
        while (enumList.Next(1, ref pidl, out fetched) == 0 && fetched == 1)
            if (callback(new PIDL(this.pidl.Pidl, pidl)))
            {
                if (enumList != null)
                    Marshal.ReleaseComObject(enumList);
                enumList = null;
                return true;

            }
        if (enumList != null)
            Marshal.ReleaseComObject(enumList);
        enumList = null;
        return false;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public bool FindChildren(bool showHiddenObjects, bool showNonFolders, bool sortResults)
    {
        //ArrayList children = new ArrayList();
        HasChildren = false;
        if (this.pidl == null || !this.pidl.IsFolder || folder == null) return false;
        int hresult = folder.EnumObjects(IntPtr.Zero, NativeMethods.SHCONTF.SHCONTF_FOLDERS | (showNonFolders ? NativeMethods.SHCONTF.SHCONTF_NONFOLDERS : 0) | (showHiddenObjects ? NativeMethods.SHCONTF.SHCONTF_INCLUDEHIDDEN : 0), out enumList);
        if (hresult != 0)
            return false;
        HasChildren = true;
        return true;
    }
    public bool HasChildren = false;
    public PIDL NextChild()
    {
        int fetched = 0;
        IntPtr pidl = IntPtr.Zero;
        if (enumList.Next(1, ref pidl, out fetched) == 0 && fetched == 1)
        {
            return new PIDL(this.pidl.Pidl, pidl);
        }
        HasChildren = false;
        if (enumList != null)
            Marshal.ReleaseComObject(enumList);
        enumList = null;
        return null;
    }

    public void CancelFind()
    {
        if (enumList != null)
            Marshal.ReleaseComObject(enumList);
        enumList = null;
    }


    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public int CountChildren(bool showHiddenObjects, bool showNonFolders)
    {
        int result = 0;
        NativeMethods.IEnumIDList enumList = null;
        if (this.pidl == null || !this.pidl.IsFolder || folder == null)
            return 0;
        int hr = folder.EnumObjects(IntPtr.Zero, NativeMethods.SHCONTF.SHCONTF_FOLDERS | (showNonFolders ? NativeMethods.SHCONTF.SHCONTF_NONFOLDERS : 0) | (showHiddenObjects ? NativeMethods.SHCONTF.SHCONTF_INCLUDEHIDDEN : 0), out enumList);
        if (hr != 0) return 0;
        IntPtr pidl = IntPtr.Zero;
        int fetched = 0;
        while (enumList.Next(1, ref pidl, out fetched) == 0 && fetched == 1)
        {
            result++;
        }
        if (enumList != null)
            Marshal.ReleaseComObject(enumList);
        enumList = null;
        return result;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    void InitializeFolder()
    {
        if (this.pidl.IsDesktop)
            NativeMethods.SHGetDesktopFolder(out folder);
        else
        {
            NativeMethods.IShellFolder parentFolder = null;
            try
            {
                IntPtr ptr;
                int hr = desktopFolder.BindToObject(this.pidl.Pidl, IntPtr.Zero, ref NativeMethods.IID_IShellFolder, out ptr);
                if (hr != 0)
                    return;
                folder = (NativeMethods.IShellFolder)Marshal.GetTypedObjectForIUnknown(ptr, typeof(NativeMethods.IShellFolder));
            }
            finally
            {
                if (parentFolder != null)
                    Marshal.ReleaseComObject(parentFolder);
                parentFolder = null;
            }
        }
    }
}
