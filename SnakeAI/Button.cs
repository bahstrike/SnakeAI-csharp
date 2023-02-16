using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeAI
{
    public class Button
    {
        public double X, Y, W, H;
        public String text;
        public Button(double x, double y, double w, double h, String t)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
            text = t;
        }

        public bool collide(double x, double y)
        {
            if (x >= X - W / 2 && x <= X + W / 2 && y >= Y - H / 2 && y <= Y + H / 2)
            {
                return true;
            }
            return false;
        }

        public void show()
        {
            App.fill(255);
            App.stroke(0);
            App.rectMode(App.CENTER);
            App.rect(X, Y, W, H);
            App.textSize(22);
            App.textAlign(App.CENTER, App.CENTER);
            App.fill(0);
            App.noStroke();
            App.text(text, X, Y - 3);
        }
    }

}
