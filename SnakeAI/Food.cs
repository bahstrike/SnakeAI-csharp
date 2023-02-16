using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace SnakeAI
{
    public class Food
    {
        public PointF pos;

        public Food()
        {
            int x = (int)(400 + SnakeAI.SIZE + Math.Floor(App.random.NextDouble() * 38) * SnakeAI.SIZE);
            int y = (int)(SnakeAI.SIZE + Math.Floor(App.random.NextDouble() * 38) * SnakeAI.SIZE);
            pos = new PointF(x, y);
        }

        public void show()
        {
            App.stroke(0);
            App.fill(255, 0, 0);
            App.rect(pos.X, pos.Y, SnakeAI.SIZE, SnakeAI.SIZE);
        }

        public Food clone()
        {
            Food clone = new Food();
            clone.pos.X = pos.X;
            clone.pos.Y = pos.Y;

            return clone;
        }
    }

}
