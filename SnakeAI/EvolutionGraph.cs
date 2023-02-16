using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeAI
{
    public class EvolutionGraph
    {

        public EvolutionGraph() {
            //App.super();
            //App.runSketch(new String[] { this.getClass().getSimpleName() }, this);
            App.runSketch(this);
        }
   
   void settings() {
            App.size(900, 600);
        }
   
   void setup() {
            App.background(150);
            App.frameRate(30);
        }
   
   void draw() {
            App.background(150);
            App.fill(0);
            App.strokeWeight(1);
            App.textSize(15);
            App.textAlign(App.CENTER, App.CENTER);
            App.text("Generation", App.width / 2, App.height - 10);
            App.translate(10, App.height / 2);
            App.rotate(Math.PI / 2);
            App.text("Score", 0, 0);
            App.rotate(-Math.PI / 2);
            App.translate(-10, -App.height / 2);
            App.textSize(10);
            double x = 50;
            double y = App.height - 35;
            double xbuff = (App.width - 50) / 51.0;
            double ybuff = (App.height - 50) / 200.0;
            for (int i = 0; i <= 50; i++)
            {
                App.text(i, x, y);
                x += xbuff;
            }
            x = 35;
            y = App.height - 50;
            double ydif = ybuff * 10.0;
            for (int i = 0; i < 200; i += 10)
            {
                App.text(i, x, y);
                App.line(50, y, App.width, y);
                y -= ydif;
            }
            App.strokeWeight(2);
            App.stroke(255, 0, 0);
            int score = 0;
            for (int i = 0; i < SnakeAI.evolution.Count; i++)
            {
                int newscore = SnakeAI.evolution[i];
                App.line(50 + (i * xbuff), App.height - 50 - (score * ybuff), 50 + ((i + 1) * xbuff), App.height - 50 - (newscore * ybuff));
                score = newscore;
            }
            App.stroke(0);
            App.strokeWeight(5);
            App.line(50, 0, 50, App.height - 50);
            App.line(50, App.height - 50, App.width, App.height - 50);
        }
   
   void exit() {
            //dispose();
            SnakeAI.graph = null;
        }
    }

}
