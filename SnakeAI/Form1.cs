using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeAI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            SnakeAI.settings();
            pictureBox1.Width = App.width;
            pictureBox1.Height = App.height;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SnakeAI.setup();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            SnakeAI.mousePressed(e.X, e.Y);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            SnakeAI.keyPressed(e.KeyCode);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SnakeAI.draw();
            pictureBox1.Image = App.GenerateBitmap();

            pictureBox1.Update();
        }
    }
}
