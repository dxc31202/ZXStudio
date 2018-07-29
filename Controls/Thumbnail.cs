using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[StructLayout(LayoutKind.Sequential)]
public struct SIZE
{
    public int cx;
    public int cy;

    public SIZE(int cx, int cy)
    {
        this.cx = cx;
        this.cy = cy;
    }
}
[Flags]
public enum SIIGBF
{
    SIIGBF_RESIZETOFIT = 0x00,
    SIIGBF_BIGGERSIZEOK = 0x01,
    SIIGBF_MEMORYONLY = 0x02,
    SIIGBF_ICONONLY = 0x04,
    SIIGBF_THUMBNAILONLY = 0x08,
    SIIGBF_INCACHEONLY = 0x10,
    SIIGBF_CROPTOSQUARE = 0x00000020,
    SIIGBF_WIDETHUMBNAILS = 0x00000040,
    SIIGBF_ICONBACKGROUND = 0x00000080,
    SIIGBF_SCALEUP = 0x00000100,


}

public enum SIGDN : uint
{
    NORMALDISPLAY = 0,
    PARENTRELATIVEPARSING = 0x80018001,
    PARENTRELATIVEFORADDRESSBAR = 0x8001c001,
    DESKTOPABSOLUTEPARSING = 0x80028000,
    PARENTRELATIVEEDITING = 0x80031001,
    DESKTOPABSOLUTEEDITING = 0x8004c000,
    FILESYSPATH = 0x80058000,
    URL = 0x80068000
}
[ComImportAttribute()]
[GuidAttribute("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
public interface IShellItemImageFactory
{
    void GetImage(
    [In, MarshalAs(UnmanagedType.Struct)] SIZE size,
    [In] SIIGBF flags,
    [Out] out IntPtr phbm);
}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
public interface IShellItem
{
    void BindToHandler(IntPtr pbc,
        [MarshalAs(UnmanagedType.LPStruct)]Guid bhid,
        [MarshalAs(UnmanagedType.LPStruct)]Guid riid,
        out IntPtr ppv);

    void GetParent(out IShellItem ppsi);

    void GetDisplayName(SIGDN sigdnName, out IntPtr ppszName);

    void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);

    void Compare(IShellItem psi, uint hint, out int piOrder);
};



public static class Thumbnail
{
    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteObject(IntPtr hObject);

    // GUID of IShellItem.

    static Guid uuid = new Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe");
    public static int Size;
    public static int Height;

    static string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();
    public static IntPtr GetThumbNail(IntPtr pidl, int size)
    {
        if (pidl == IntPtr.Zero) return IntPtr.Zero;

        Size = size;
        IShellItem ppsi = null;

        IntPtr hbitmap = IntPtr.Zero;
        NativeMethods.SHCreateItemFromIDList(pidl, uuid, out ppsi);
        try
        {
            if (size < 32)
                ((IShellItemImageFactory)ppsi).GetImage(new SIZE(size, size), SIIGBF.SIIGBF_ICONONLY, out hbitmap);
            else
                ((IShellItemImageFactory)ppsi).GetImage(new SIZE(size, size), SIIGBF.SIIGBF_THUMBNAILONLY, out hbitmap);
            return hbitmap;
        }
        catch (Exception ex)
        {
            try
            {
                ((IShellItemImageFactory)ppsi).GetImage(new SIZE(size, size), SIIGBF.SIIGBF_ICONONLY, out hbitmap);
                return hbitmap;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }
    }

    public static IntPtr GetIcon(IntPtr pidl, int size)
    {
        if (pidl == IntPtr.Zero) return IntPtr.Zero;

        Size = size;
        IShellItem ppsi = null;

        IntPtr hbitmap = IntPtr.Zero;
        NativeMethods.SHCreateItemFromIDList(pidl, uuid, out ppsi);
        try
        {
            ((IShellItemImageFactory)ppsi).GetImage(new SIZE(size, size), SIIGBF.SIIGBF_ICONONLY, out hbitmap);
            return hbitmap;
        }
        catch
        {
            return IntPtr.Zero;
        }
    }
    public static Bitmap GetBitmapFromHBitmap(IntPtr nativeHBitmap)
    {
        Bitmap bmp = Bitmap.FromHbitmap(nativeHBitmap);

        if (Bitmap.GetPixelFormatSize(bmp.PixelFormat) < 32)
            return bmp;

        return CreateAlphaBitmap(bmp, PixelFormat.Format32bppArgb);
    }

    public static Bitmap CreateAlphaBitmap(Bitmap srcBitmap, PixelFormat targetPixelFormat)
    {
        Bitmap result = new Bitmap(srcBitmap.Width, srcBitmap.Height, targetPixelFormat);

        Rectangle bmpBounds = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);

        BitmapData srcData = srcBitmap.LockBits(bmpBounds, ImageLockMode.ReadOnly, srcBitmap.PixelFormat);

        bool isAlplaBitmap = false;

        try
        {
            for (int y = 0; y <= srcData.Height - 1; y++)
            {
                for (int x = 0; x <= srcData.Width - 1; x++)
                {
                    Color pixelColor = Color.FromArgb(
                        Marshal.ReadInt32(srcData.Scan0, (srcData.Stride * y) + (4 * x)));

                    if (pixelColor.A > 0 & pixelColor.A < 255)
                    {
                        isAlplaBitmap = true;
                    }

                    result.SetPixel(x, y, pixelColor);
                }
            }
        }
        finally
        {
            srcBitmap.UnlockBits(srcData);
        }

        if (isAlplaBitmap)
        {
            return result;
        }
        else
        {
            return result;
            // return srcBitmap;
        }
    }

    public static Bitmap ScaleImage(Bitmap image, int width, int height)
    {
        return ScaleImage(image, width, height, false);

    }
    static Bitmap bmp;

    public static Bitmap ScaleImage(Bitmap image, int width, int height, bool highlight)
    {
        double ratio;

        if (image.Width > image.Height)
            ratio = width / (double)image.Width;
        else
            ratio = height / (double)image.Height;
        int scaleWidth = (int)(image.Width * ratio);
        int scaleHeight = (int)(image.Height * ratio);
        bmp = new Bitmap(width, height);
        //bmp.SetResolution(width, height);

        using (Graphics g = Graphics.FromImage(bmp))
        {
            // Centre the result 
            //if (highlight)                    g.Clear(Color.FromArgb(0xE5, 0xF3, 0xFF));
            //g.Clear(t);

            int x = (width - scaleWidth) / 2;
            int y = (width - scaleHeight) / 2;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            g.DrawImage(image, x, y, scaleWidth, scaleHeight);
            //bmp.MakeTransparent(t);

        }

        return bmp;
    }

    public static Bitmap CentreImage(Bitmap image, int width, int height)
    {
        double ratio;

        if (image.Width > image.Height)
            ratio = width / (double)image.Width;
        else
            ratio = height / (double)image.Height;
        int scaleWidth = (int)(image.Width * ratio);
        int scaleHeight = (int)(image.Height * ratio);
        bmp = new Bitmap(width, height);
        //bmp.SetResolution(width, height);

        using (Graphics g = Graphics.FromImage(bmp))
        {
            // Centre the result 
            //if (highlight)                    g.Clear(Color.FromArgb(0xE5, 0xF3, 0xFF));
            //g.Clear(t);

            int x = (width - image.Width) / 2;
            int y = (width - image.Height) / 2;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            g.DrawImage(image, x, y, image.Width, image.Height);
            //bmp.MakeTransparent(t);

        }

        return bmp;
    }


    public static Bitmap CentreImage(Bitmap image, int height)
    {
        double ratio;

        ratio = 16 / (double)height;
        int scaleWidth = (int)(image.Width * ratio);
        int scaleHeight = (int)(image.Height * ratio);
        bmp = new Bitmap(16, 16);
        //bmp.SetResolution(width, height);

        using (Graphics g = Graphics.FromImage(bmp))
        {
            // Centre the result 
            //if (highlight)                    g.Clear(Color.FromArgb(0xE5, 0xF3, 0xFF));
            //g.Clear(t);

            int x = 0;
            int y = (height - 16) / 2;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            g.DrawImage(image, x, y, image.Width, image.Height);
            //bmp.MakeTransparent(t);

        }

        return bmp;
    }


}

static class ThumbDB
{
    #region Public Members
    //public static string ThumDBFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Treemap\ThumbsDB.xml";
    public static string ThumDBFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Treemap\ThumbsDB.db";
    public static string ThumDBFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Treemap";

    public static void SaveBitmap(IntPtr ppidl,Bitmap bmp)
    {
        using (PIDL pidl = new PIDL(ppidl))
            SaveBitmap(pidl.PhysicalPath , pidl.DisplayName, bmp);

    }
    public static void SaveBitmap(string fileName,string displayname, Bitmap bmp)
    {
        //string file = Path.GetFileName(fileName);
        string path=Path.GetDirectoryName(fileName);
        string key = path.Replace(':', '_');
        key= key.Replace('\\', '_') + ".thumbdb";
        char[] newline = { '[', 'E', 'O', 'L', ']', '\r', '\n' };
        try
        {
            byte[] bb = ByteArrayFromBitmap(ref bmp);
            string ss = Convert.ToBase64String(bb);
            _readWriteLock.EnterWriteLock();
            if (!File.Exists(ThumDBFolder + @"\" + key))
            {
                // Create a file to write to.
                if (!Directory.Exists(ThumDBFolder)) Directory.CreateDirectory(ThumDBFolder);
                using (StreamWriter sw = File.CreateText(ThumDBFolder + @"\" + key))
                {
                    sw.WriteLine(fileName + "|" + DateTime.Now.ToShortDateString() + "|" + DateTime.Now.ToShortTimeString() + "|" + bmp.Width.ToString() + "|" + bmp.Height.ToString());
                    sw.WriteLine(ss);
                }
                return;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            using (StreamReader sr = new StreamReader(ThumDBFolder + @"\" + key))
            {
                string s1 = sr.ReadToEnd();//.Replace(Environment.NewLine, "").Trim(); 
                string[] lines = s1.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                for (int i = 0; i < lines.Length - 1; i += 2)
                {
                    if (!lines[i].ToString().StartsWith(fileName))
                    {
                        sb.AppendLine(lines[i]);
                        sb.AppendLine(lines[i + 1]);
                    }
                }
            }
            using (StreamWriter sw = new StreamWriter(ThumDBFolder + @"\" + key, false))
            {
                sw.Write(sb.ToString());
                sw.WriteLine(fileName + "|" + DateTime.Now.ToShortDateString() + "|" + DateTime.Now.ToShortTimeString() + "|" + bmp.Width.ToString() + "|" + bmp.Height.ToString() );
                sw.WriteLine(ss );
            }
        }
        catch (Exception ex )
        {
            Console.WriteLine(ex.Message);
        }

        finally
        {
            _readWriteLock.ExitWriteLock();
        }


        //Task t = Task.Run(() =>
        //{

        //    OpenFile(ThumDBFile, "Images");
        //    XElement root = xdoc.Root;
        //    byte[] bb = ByteArrayFromBitmap(ref bmp);
        //    string ss = Convert.ToBase64String(bb);
        //    XElement ele = new XElement("Image",
        //        new XAttribute("Filename", fileName),
        //        new XAttribute("Date", DateTime.Now.ToShortDateString()),
        //        new XAttribute("Time", DateTime.Now.ToShortTimeString()),
        //        new XAttribute("Width", bmp.Width.ToString()),
        //        new XAttribute("Height", bmp.Height.ToString()),
        //        new XAttribute("BitmapData", ss)
        //        );
        //    ReplaceOrAdd(xdoc.Root, ele, fileName);
        //    CloseFile(ThumDBFile);
        //});

        //t.Wait();
    }

    public static Bitmap LoadBitmap(IntPtr ppidl)
    {
        using (PIDL pidl = new PIDL(ppidl))
            return LoadBitmap(pidl.PhysicalPath + @"\" + pidl.DisplayName);

    }
    public static Bitmap LoadBitmap(string fileName)
    {
        try
        {
            OpenFile(ThumDBFile, "Images");
            XElement root = xdoc.Root;

            IEnumerable<XElement> file = from g in root.Elements("Image")
                                         where g.Attributes("Filename").Any(att => att.Value == fileName)
                                         select g;

            XElement foundElement = file.LastOrDefault();
            if (foundElement != null)
            {
                byte[] ba = Convert.FromBase64String(foundElement.Attribute("BitmapData").Value);
                return BitmapFromByteArray(ba);
            }
            return null;
        }
        catch { }
        finally
        {
            CloseFile(ThumDBFile);
        }
        return null;
    }

    public static void DeleteBitmap(IntPtr ppidl)
    {
        using (PIDL pidl = new PIDL(ppidl))
            DeleteBitmap(pidl.PhysicalPath + @"\" + pidl.DisplayName);

    }
    public static void DeleteBitmap(string fileName)
    {
        try
        {
            OpenFile(ThumDBFile, "Images");
            XElement root = xdoc.Root;

            IEnumerable<XElement> file = from g in root.Elements("Image")
                                         where g.Attributes("Filename").Any(att => att.Value == fileName)
                                         select g;

            XElement foundElement = file.LastOrDefault();
            if (foundElement != null)
            {
                foundElement.Remove();
            }
        }
        catch { }
        finally
        {
            CloseFile(ThumDBFile);
        }
    }

    public static List<string> GetFilenames()
    {
        try
        {
            OpenFile(ThumDBFile, "Images");
            XElement root = xdoc.Root;


            IEnumerable<XElement> children = root.Elements();
            if (children != null)
            {
                List<string> filenames = new List<string>();
                foreach (XElement element in children)
                {
                    filenames.Add(element.Attribute("Filename").Value);
                }
                return filenames;
            }
            return null;
        }
        catch { }
        finally
        {
            CloseFile(ThumDBFile);
        }
        return null;
    }
    #endregion Public Members

    #region Private Members
    static XDocument xdoc = null;
    static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

    static void OpenFile(string xmlFilename, string root)
    {
        _readWriteLock.EnterWriteLock();

        try
        {
            xdoc = XDocument.Load(xmlFilename);
        }
        catch
        {
            if (!Directory.Exists(ThumDBFolder))
                Directory.CreateDirectory(ThumDBFolder);
            xdoc = new XDocument();
            xdoc.Add(new XElement(root));
            xdoc.Save(xmlFilename);
        }

    }

    static void CloseFile(string xmlFilename)
    {
        xdoc.Save(xmlFilename);
        _readWriteLock.ExitWriteLock();

    }

    static void ReplaceOrAdd(this XElement source, XElement node, string name)
    {
        IEnumerable<XElement> file = from g in source.Elements("Image")
                                     where g.Attributes("Filename").Any(att => att.Value == name)
                                     select g;

        XElement n = file.LastOrDefault();

        if (n == null)
            source.Add(node);
        else
            n.ReplaceWith(node);
    }

    static byte[] ByteArrayFromBitmap(ref Bitmap bitmap)
    {
        // The bitmap contents are coded with Width and Height followed by pixel colors (uint)
        byte[] b = new byte[4 * (bitmap.Height * bitmap.Width + 2)];
        int n = 0;
        // encode the width
        uint x = (uint)bitmap.Width;
        int y = (int)x;
        b[n] = (byte)(x / 0x1000000);
        x = x % (0x1000000);
        n++;
        b[n] = (byte)(x / 0x10000);
        x = x % (0x10000);
        n++;
        b[n] = (byte)(x / 0x100);
        x = x % 0x100;
        n++;
        b[n] = (byte)x;
        n++;
        // encode the height
        x = (uint)bitmap.Height;
        y = (int)x;
        b[n] = (byte)(x / 0x1000000);
        x = x % (0x1000000);
        n++;
        b[n] = (byte)(x / 0x10000);
        x = x % (0x10000);
        n++;
        b[n] = (byte)(x / 0x100);
        x = x % 0x100;
        n++;
        b[n] = (byte)x;
        n++;
        // Loop through each row
        for (int j = 0; j < bitmap.Height; j++)
        {
            // Loop through the pixel on this row
            for (int i = 0; i < bitmap.Width; i++)
            {
                x = (uint)bitmap.GetPixel(i, j).ToArgb();
                y = (int)x;
                b[n] = (byte)(x / 0x1000000);
                x = x % (0x1000000);
                n++;
                b[n] = (byte)(x / 0x10000);
                x = x % (0x10000);
                n++;
                b[n] = (byte)(x / 0x100);
                x = x % 0x100;
                n++;
                b[n] = (byte)x;
                n++;
            }
        }
        return b;
    }

    static Bitmap BitmapFromByteArray(byte[] byteArray)
    {
        int n = 0;
        // Get the width
        uint x = (((uint)byteArray[n] * 256 + (uint)byteArray[n + 1]) * 256 + (uint)byteArray[n + 2]) * 256 + (uint)byteArray[n + 3];
        int width = (int)x;
        n += 4;
        // Get the height
        x = (((uint)byteArray[n] * 256 + (uint)byteArray[n + 1]) * 256 + (uint)byteArray[n + 2]) * 256 + (uint)byteArray[n + 3];
        int height = (int)x;
        n += 4;
        // Create the Bitmmap object
        Bitmap bmp = new Bitmap(width, height);
        // The pixels are stored in order by rows
        for (int j = 0; j < height; j++)
        {
            // Read the pixels for each row
            for (int i = 0; i < width; i++)
            {
                x = (((uint)byteArray[n] * 256 + (uint)byteArray[n + 1]) * 256 + (uint)byteArray[n + 2]) * 256 + (uint)byteArray[n + 3];
                bmp.SetPixel(i, j, Color.FromArgb((int)x));
                n += 4;
            }
        }
        return bmp;
    }
    #endregion Private Members
}

