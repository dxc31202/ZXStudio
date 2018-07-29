using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ZXStudio.Controls
{
    class IconListBox:ListBox
    {
        public event ItemSelectedEventHandler ItemSelectedEvent;
        SysImageList sysimagelist = new SysImageList(SysImageListSize.smallIcons);
        public IconListBox():base()
        {
            DoubleBuffered = true;
            //this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //UpdateStyles();
            DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            ItemHeight = 22;
            BorderStyle = BorderStyle.None;
            
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            ItemSelectedEvent?.Invoke(this, new ItemSelectedEventArgs((PIDL)SelectedItem));
            base.OnSelectedIndexChanged(e);
        }
        int x, y;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point point = PointToClient(Cursor.Position);
            int index = IndexFromPoint(point);
            //if (index < 0) return;
            //Do any action with the item
            if (MouseOver == index) return;
            PaintItem(MouseOver, BackColor);
            MouseOver = index;
            PaintItem(index, Hilight);
            base.OnMouseMove(e);
        }


        void PaintItem(int index, Color color)
        {
            if (index < 0) return;
            if (index >= Items.Count)
                return;
            using (Graphics g = Graphics.FromHwnd(Handle))
            {
                int iconindex = sysimagelist.IconIndex(((PIDL)Items[index]).Pidl);
                using (Brush brush = new SolidBrush(color))
                    g.FillRectangle(brush, GetItemRectangle(index));
                g.DrawImage(sysimagelist.Icon(iconindex).ToBitmap(), new Rectangle(8, GetItemRectangle(index).Y + 3, 16, 16));

                if (Items[index].Equals(SelectedItem))
                    using (Font boldfont = new Font(Font.Name, Font.Size, FontStyle.Bold))
                        g.DrawString(Items[index].ToString(), boldfont, Brushes.Black, 36, GetItemRectangle(index).Y + 5);
                else
                    g.DrawString(Items[index].ToString(), Font, Brushes.Black, 36, GetItemRectangle(index).Y + 5);
            }
        }
        public int MouseOver = 0;
        int Index;
        Color Hilight = Color.FromArgb(0xC3, 0xDE, 0xF5);
      
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (Items.Count == 0) return;
            if (e.Index < 0) return;
            Index = e.Index;
            Color backg = BackColor;
            if (e.Index == MouseOver)
                backg = Hilight;
            PaintItem(e.Index, backg);

            //e.DrawBackground();
            using (Graphics g = Graphics.FromHwnd(Handle))
            {
                int iconindex = sysimagelist.IconIndex(((PIDL)Items[e.Index]).Pidl);
                using (Brush brush = new SolidBrush(backg))
                    g.FillRectangle(brush, GetItemRectangle(e.Index));
                g.DrawImage(sysimagelist.Icon(iconindex).ToBitmap(), new Rectangle(8, GetItemRectangle(e.Index).Y + 3, 16, 16));
                if (Items[e.Index].Equals(SelectedItem))
                    using (Font fontbold = new Font(Font.Name, Font.Size, FontStyle.Bold))
                        g.DrawString(Items[e.Index].ToString(), fontbold, Brushes.Black, 36, GetItemRectangle(e.Index).Y + 5);
                else
                    g.DrawString(Items[e.Index].ToString(), Font, Brushes.Black, 36, GetItemRectangle(e.Index).Y + 5);
            }
            e.DrawFocusRectangle();
            base.OnDrawItem(e);
        }

        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
    }
}
