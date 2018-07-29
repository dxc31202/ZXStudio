using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZXStudio.Controls
{
    public class HexView : Panel
    {
        RichTextBox hexview;
        public HexView()
        {
            hexview = new RichTextBox();
            Controls.Add(hexview);
            hexview.WordWrap = false;
            hexview.Dock = DockStyle.Fill;
            hexview.Font = new System.Drawing.Font("Consolas", 10);
        }
        public void Clear()
        {
            hexview.Text = "";
        }
        public void Load(byte[] data, int fromaddress, int toaddress)
        {
            int[] idata = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                 idata[i] = data[i];
            Load(idata, fromaddress, toaddress);

        }
        public void Load(int[] data, int fromaddress, int toaddress)
        {
            StringBuilder sb = new StringBuilder();
            
            int width = 0;
            string chars = "";
            string line = "";
            //line = (fromaddress).ToString("X4") + "\t";
            for (int address = 0; fromaddress < toaddress; address++)
            {
                if (address >= toaddress) break;
                if (data[address] == -1)
                {
                    if (line.Length > 0)
                    {
                        sb.Append(Justify(line, 54) + '\t' + chars + Environment.NewLine);
                        sb.Append(Environment.NewLine);

                    }
                    chars = "";
                    width = 0;
                    line = "";
                    continue;
                }
                if(line.Length==0)
                    line = (address).ToString("X4") + "\t";
                if (width == 16)
                {
                    width = 0;
                    sb.Append(Justify(line, 54) + '\t' + chars + Environment.NewLine);
                    line = address.ToString("X4") + "\t";
                    chars = "";
                }
                int value = data[address];
                if (value < 32)
                    chars += ".";
                else
                {
                    if (value > 127)
                        chars += ".";
                    else
                        chars += ((char)value).ToString();
                }
                line += value.ToString("X2").Substring(0, 2) + " ";
                width++;
            }
            if (chars.Length > 0)
                sb.Append(Justify(line, 54) + '\t' + chars);
            hexview.Text = sb.ToString();
        }

        string Justify(string value,int length)
        {
            return (value + new String(' ', length)).Substring(0, length);
        }
    }
}
