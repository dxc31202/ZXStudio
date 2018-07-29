using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace ZXStudio.Controls
{
    [Serializable]

    public class FastLabel : Panel
    {
        [DllImport("user32.dll")]
        public static extern int DrawText(IntPtr hdc, string lpStr, int nCount, [MarshalAs(UnmanagedType.Struct)] ref RECT lpRect, int wFormat);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("GDI32.dll")]
        public static extern bool DeleteObject(IntPtr objectHandle);

        [DllImport("gdi32.dll")]
        public static extern uint SetTextColor(IntPtr hdc, int crColor);

        [DllImport("gdi32.dll")]
        public static extern uint SetBkColor(IntPtr hdc, int crColor);

        public FastLabel()
        {
            this.Resize += new EventHandler(FastLabel_Resize);
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.OptimizedDoubleBuffer,
                true);
            rect.Top = 0;
            rect.Left = 0;
            rect.Right = this.Width;
            rect.Bottom = this.Height;

        }

        Color paintBackColor = SystemColors.Control;
        Color paintForeColor = Color.Black;

        Color changedBackColor = Color.Salmon;
        Color changedForeColor = Color.White;
        public Color ChangedBackColor { get { return changedBackColor; } set { changedBackColor = value; } }
        public Color ChangedForeColor { get { return changedForeColor; } set { changedForeColor = value; } }
        void FastLabel_Resize(object sender, EventArgs e)
        {
            rect.Top = 0;
            rect.Left = 0;
            rect.Right = this.Width;
            rect.Bottom = this.Height;
        }

        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }
        string text = "";
        [Category("User"), Description("Text value")]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]

        public new string Text
        {
            set
            {
                if (value != text)
                {
                    text = value;
                    paintBackColor = changedBackColor;
                    paintForeColor = changedForeColor;
                    Invalidate();
                    return;
                }
                paintBackColor = BackColor;
                paintForeColor = ForeColor;
                Invalidate();

            }
            get { return text; }
        }
        RECT rect = new RECT();
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                win32back = ColorTranslator.ToWin32(value);
                base.BackColor = value;
            }
        }
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                win32Fore = ColorTranslator.ToWin32(value);
                base.ForeColor = value;
            }
        }
        int win32back;
        int win32Fore;

        protected override void OnPaint(PaintEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(paintForeColor))
            {
                //e.Graphics.Clear(paintBackColor);
                e.Graphics.DrawString(text, Font, brush, new PointF(0, 0));
            }

        }
    }
}