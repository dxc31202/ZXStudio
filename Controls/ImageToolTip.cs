using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
namespace ZXStudio.Controls
{
    class ImageToolTip : ToolTip
    {

        public string Filename;
        public int ImageSize = 96;
        Bitmap thumbnail;

        string lastFilename;
        public ImageToolTip()
        {
            
            this.Popup += new PopupEventHandler(ImageToolTip_Popup);
            this.Draw += new DrawToolTipEventHandler(ImageToolTip_Draw);
            OwnerDraw = true;
            
            //st.DesiredSize = new Size(150, 150);

        }
        public string Caption;

        ~ImageToolTip()
        {
            this.Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        Bitmap icon;
        void ImageToolTip_Popup(object sender, PopupEventArgs e)
        {

            try
            {
                
                Size oldSize = e.ToolTipSize;
                Filename = Filename.Replace(@"\\", @"\");
                if (lastFilename != Filename)
                {
                    
                    using (PIDL pidl = new PIDL(Filename))
                    {

                        IntPtr hicon = Thumbnail.GetIcon(pidl.Pidl, 32);
                        using (Bitmap bmp = Thumbnail.GetBitmapFromHBitmap(hicon))
                            icon = Thumbnail.ScaleImage(bmp, 32, 32);
                        Thumbnail.DeleteObject(hicon);

                        IntPtr himage = Thumbnail.GetThumbNail(pidl.Pidl, ImageSize);
                        using (Bitmap bmp = Thumbnail.GetBitmapFromHBitmap(himage))
                        {
                            if ((bmp.Width != ImageSize && bmp.Height != ImageSize))
                                thumbnail = Thumbnail.CentreImage(bmp, ImageSize, ImageSize);
                            else
                                if (bmp.Width != ImageSize || bmp.Height != ImageSize)
                                    thumbnail = Thumbnail.ScaleImage(bmp, ImageSize, ImageSize);
                            else
                                thumbnail = Thumbnail.GetBitmapFromHBitmap(himage);
                            //ThumbDB.SaveBitmap(pidl.Pidl, thumbnail);
                        }
                        Thumbnail.DeleteObject(himage);
                    }
                }
                string[] texts = Caption.Split(new char[] { '\r', '\n' });
                using (Graphics g = Graphics.FromImage(thumbnail))
                {
                    foreach (string s in texts)
                    {
                        int width = (int)g.MeasureString(s, e.AssociatedControl.Font).Width+35;
                        if (width > oldSize.Width) oldSize.Width = width;
                    }
                }
                if (ImageSize != 16)
                    if (thumbnail != null) oldSize.Height += ImageSize +3;
                oldSize.Width = oldSize.Width;
                if (oldSize.Width < ImageSize + 18)
                    oldSize.Width = ImageSize + 18;
               
                e.ToolTipSize = oldSize;
                lastFilename = Filename;
            }

            catch (Exception ex) { }

        }

        
        void ImageToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {

            string caption = e.ToolTipText;
            
            Rectangle bounds = e.Bounds;
            //e.DrawBackground();
            // Draw the custom background.
            e.Graphics.FillRectangle(SystemBrushes.Info, bounds);

            // Draw the standard border.
            //e.DrawBorder();
            // Draw the custom border to appear 3-dimensional.
            e.Graphics.DrawLines(SystemPens.ControlLightLight, new Point[] { new Point(0, bounds.Height - 1), new Point(0, 0), new Point(bounds.Width - 1, 0) });
            e.Graphics.DrawLines(SystemPens.ControlDarkDark, new Point[] { new Point(0, bounds.Height - 1), new Point(bounds.Width - 1, bounds.Height - 1), new Point(bounds.Width - 1, 0) });

            // Specify custom text formatting flags.
            //TextFormatFlags textFormatFlags = TextFormatFlags.Top |
            //                     TextFormatFlags.Left |
            //                     TextFormatFlags.NoFullWidthCharacterBreak;
            // Draw the standard text with customized formatting options.
            //e.DrawText(textFormatFlags);
            using (StringFormat sf = new StringFormat())
            {
                sf.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoClip;
                
                e.Graphics.DrawString(caption, e.Font, SystemBrushes.InfoText, bounds.X+25,bounds.Y, sf);
            }


            try
            {
                if (thumbnail != null)
                {
                    e.Graphics.DrawImage(icon, new Rectangle(1, 1, 24, 24));
                    if (ImageSize != 16)
                    {
                        int middle = bounds.Width / 2;
                        int left = middle - (thumbnail.Width / 2);
                        e.Graphics.DrawImage(thumbnail, new Rectangle(left, bounds.Height - ImageSize - 4, thumbnail.Width, thumbnail.Height));
                        //e.Graphics.DrawRectangle(SystemPens.ControlDarkDark, new Rectangle(left , bounds.Height - imageSize-6, thumbnail.Width , thumbnail.Height+3 ));
                    }
                }
            }
            catch { }
        }
        
    }

}
