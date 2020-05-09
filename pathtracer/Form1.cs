using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pathtracer
{
    public partial class Form1 : Form
    {
        //public PictureBox pb { get { return pictureBox1; } set { pictureBox1 = value; } }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PathTracer pt = new PathTracer();
            pt.Render((Bitmap)pictureBox1.Image);
        }
    }
}
