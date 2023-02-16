using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeAI
{
    public static class App
    {
        public static Random random = new Random();


        //https://stackoverflow.com/questions/218060/random-gaussian-variables
        public static double randomGaussian()
        {
            const double mean = 0.0;
            const double stdDev = 1.0;

            double u1 = 1.0 - random.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;
        }


        public static Table loadTable(string f, string h)
        {
            return null;
        }

        public static void saveTable(Table t, string f)
        {

        }


        private static Bitmap backbuffer = null;
        private static Graphics gfx = null;

        public static int width = 0;
        public static int height = 0;
        public static void size(int w, int h)
        {
            if(gfx != null)
            {
                gfx.Dispose();
                gfx = null;
            }

            if(backbuffer != null)
            {
                backbuffer.Dispose();
                backbuffer = null;
            }

            width = w;
            height = h;

            backbuffer = new Bitmap(width, height);
            gfx = Graphics.FromImage(backbuffer);
        }

        public static Bitmap GenerateBitmap()
        {
            gfx.Flush(System.Drawing.Drawing2D.FlushIntention.Flush);
            return (Bitmap)backbuffer.Clone();
        }

        public static void frameRate(int f)
        {
            Program.form.timer1.Interval = (int)(1.0 / (double)f * 1000.0);
        }

        public static void background(int g)
        {
            background(g, g, g);
        }

        public static void background(int r, int g, int b)
        {
            gfx.FillRectangle(new SolidBrush(Color.FromArgb(r, g, b)), new Rectangle(0, 0, width, height));
        }

        public static void runSketch(EvolutionGraph e)
        {
            // call settings/setup/etc?

            // then loop and draw for framerate?   implement to Timer?

        }

        public static void print(string s)
        {

        }

        public static void println(string s=null)
        {

        }

        public const int CENTER = 0;
        public const int CORNER = 1;
        public const int LEFT = 2;

        public static Font createFont(string s, double h)
        {
            return new Font(s, (float)h);
        }

        public static void textFont(Font f)
        {
            fontName = f.Name;
            fontHeight = f.Height;
        }

        public static void stroke(int g)
        {
            stroke(g, g, g);
        }

        static Color? strokeClr = null;
        static float strokeWidth = 1.0f;

        public static void stroke(int r, int g, int b)
        {
            strokeClr = Color.FromArgb(r, g, b);
        }

        static Color? fillClr = null;

        public static void noFill()
        {
            fillClr = null;
        }

        public static void fill(int g)
        {
            fill(g, g, g);
        }

        public static void fill(int r, int g, int b)
        {
            fillClr = Color.FromArgb(r, g, b);
        }

        public static void rect(double x, double y, double w, double h)
        {
            RectangleF rc = new RectangleF((float)x, (float)y, (float)w, (float)h);

            if (_rectMode == CENTER)
            {
                rc.X -= rc.Width / 2.0f;
                rc.Y -= rc.Height / 2.0f;
            }


            if(fillClr != null)
            {
                gfx.FillRectangle(new SolidBrush(fillClr.Value), rc);
            }

            if(strokeClr != null)
            {
                gfx.DrawRectangle(new Pen(strokeClr.Value, strokeWidth), rc.X, rc.Y, rc.Width, rc.Height);
            }
        }

        static int _rectMode = CORNER;

        public static void rectMode(int i)
        {
            _rectMode = i;
        }

        public static void textSize(double s)
        {
            fontHeight = (float)s;
        }

        public static void textAlign(int h)
        {
            textAlign(h, LEFT);
        }
        public static void textAlign(int h, int v)
        {
            if (h == LEFT)
                textAlignH = StringAlignment.Near;
            else
                textAlignH = StringAlignment.Center;

            if (v == LEFT)
                textAlignV = StringAlignment.Near;
            else
                textAlignV = StringAlignment.Center;
        }

        public static void noStroke()
        {
            strokeClr = null;
        }

        static string fontName = null;
        static float fontHeight = 12.0f;
        static StringAlignment textAlignH = StringAlignment.Near;
        static StringAlignment textAlignV = StringAlignment.Near;

        public static void text(string s, double x, double y)
        {
            if (fontName == null || fillClr == null)
                return;

            StringFormat sf = new StringFormat();
            sf.Alignment = textAlignH;
            sf.LineAlignment = textAlignV;
            gfx.DrawString(s, new Font(fontName, fontHeight, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(fillClr.Value), (float)x, (float)y, sf);
        }

        public static void text(int i, double x, double y)
        {
            text(i.ToString(), x, y);
        }

        static int _ellipseMode = CORNER;

        public static void ellipseMode(int i)
        {
            _ellipseMode = i;
        }

        public static void ellipse(double x, double y, double w, double h)
        {
            RectangleF rc = new RectangleF((float)x, (float)y, (float)w, (float)h);

            if (_ellipseMode == CENTER)
            {
                rc.X -= rc.Width / 2.0f;
                rc.Y -= rc.Height / 2.0f;
            }


            if (fillClr != null)
            {
                gfx.FillEllipse(new SolidBrush(fillClr.Value), rc);
            }

            if (strokeClr != null)
            {
                gfx.DrawEllipse(new Pen(strokeClr.Value, strokeWidth), rc.X, rc.Y, rc.Width, rc.Height);
            }
        }

        public static void point(double x, double y)
        {

        }

        public static void line(double x0, double y0, double x1, double y1)
        {
            if (strokeClr == null)
                return;

            gfx.DrawLine(new Pen(strokeClr.Value, strokeWidth), (float)x0, (float)y0, (float)x1, (float)y1);
        }

        public static void strokeWeight(double w)
        {
            strokeWidth = (float)w;
        }

        public static void translate(double x, double y)
        {

        }

        public static void rotate(double r)
        {

        }
    }
}
