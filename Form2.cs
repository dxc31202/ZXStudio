using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ZXCassetteDeck;
namespace ZXStudio
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        
        private void Form2_Load(object sender, EventArgs e)
        {
             //CassetteController.LoadFile(@"D:\\Users\\Dave\\Documents\\_Progs\\Games\\B\\Brainstorm (1983)(Micromega).tzx");


            ProcessFiles(@"D:\Users\Dave\Documents\_Progs\Games");

        }

        void ProcessFiles(string root)
        {
            foreach (string dir in Directory.GetDirectories(root))
            {
                ProcessFiles(dir);
            }

            foreach (string filename in Directory.GetFiles(root,"*.tzx"))
            {
                CassetteController.LoadFile(filename);
            }
        }


    }
}
