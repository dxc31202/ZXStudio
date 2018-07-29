using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ZXStudio.Controls
{
    public class TabControlEx : TabControl
    {
        public TabControlEx() : base()
        {
            DrawMode = TabDrawMode.OwnerDrawFixed;
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            pevent.Graphics.Clear(ColorTranslator.FromHtml("#FF293955"));
            base.OnPaintBackground(pevent);
        }
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Rectangle paddedBounds = e.Bounds;
            //paddedBounds.Inflate(1, 1);
            //e.Graphics.Clear(ColorTranslator.FromHtml("#FF293955"));
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                if (SelectedIndex == e.Index)
                {
                    e.Graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#FFFFF29D")), e.Bounds);
                    e.Graphics.DrawString(TabPages[e.Index].Text, this.Font, Brushes.Black, e.Bounds, sf);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#FF4D6082")), e.Bounds);
                    e.Graphics.DrawString(TabPages[e.Index].Text, this.Font, Brushes.White, e.Bounds, sf);
                }
            }

            //base.OnDrawItem(e);
        }

    }
}