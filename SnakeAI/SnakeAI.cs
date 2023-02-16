using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeAI
{
    static class SnakeAI
    {
        public static int SIZE = 20;
        public static int hidden_nodes = 16;
        public static int hidden_layers = 2;
        public static int fps = 100;  //15 is ideal for self play, increasing for AI does not directly increase speed, speed is dependant on processing power

        public static int highscore = 0;

        public static double mutationRate = 0.05;
        public static double defaultmutation = mutationRate;

        public static bool humanPlaying = false;  //false for AI, true to play yourself
        public static bool replayBest = true;  //shows only the best of each generation
        public static bool seeVision = false;  //see the snakes vision
        public static bool modelLoaded = false;

        public static Font font;

        public static List<int> evolution;

        public static Button graphButton;
        public static Button loadButton;
        public static Button saveButton;
        public static Button increaseMut;
        public static Button decreaseMut;

        public static EvolutionGraph graph;

        public static Snake snake;
        public static Snake model;

        public static Population pop;

        public static void settings()
        {
            App.size(1200, 800);
        }

        public static void setup()
        {
            font = App.createFont("Verdana"/*"agencyfb-bold.ttf"*/, 32);
            evolution = new List<int>();
            graphButton = new Button(349, 15, 100, 30, "Graph");
            loadButton = new Button(249, 15, 100, 30, "Load");
            saveButton = new Button(149, 15, 100, 30, "Save");
            increaseMut = new Button(340, 85, 20, 20, "+");
            decreaseMut = new Button(365, 85, 20, 20, "-");
            App.frameRate(fps);
            if (humanPlaying)
            {
                snake = new Snake();
            }
            else
            {
                pop = new Population(2000); //adjust size of population
            }
        }

        public static void draw()
        {
            App.background(0);
            App.noFill();
            App.stroke(255);
            App.line(400, 0, 400, App.height);
            App.rectMode(App.CORNER);
            App.rect(400 + SIZE, SIZE, App.width - 400 - 40, App.height - 40);
            App.textFont(font);
            if (humanPlaying)
            {
                snake.move();
                snake.show();
                App.fill(150);
                App.textSize(20);
                App.text("SCORE : " + snake.score, 500, 50);
                if (snake.dead)
                {
                    snake = new Snake();
                }
            }
            else
            {
                if (!modelLoaded)
                {
                    if (pop.done())
                    {
                        highscore = pop.bestSnake.score;
                        pop.calculateFitness();
                        pop.naturalSelection();
                    }
                    else
                    {
                        pop.update();
                        pop.show();
                    }
                    App.fill(150);
                    App.textSize(25);
                    App.textAlign(App.LEFT);
                    App.text("GEN : " + pop.gen, 120, 60);
                    //text("BEST FITNESS : "+pop.bestFitness,120,50);
                    //text("MOVES LEFT : "+pop.bestSnake.lifeLeft,120,70);
                    App.text("MUTATION RATE : " + mutationRate * 100 + "%", 120, 90);
                    App.text("SCORE : " + pop.bestSnake.score, 120, App.height - 45);
                    App.text("HIGHSCORE : " + highscore, 120, App.height - 15);
                    increaseMut.show();
                    decreaseMut.show();
                }
                else
                {
                    model.look();
                    model.think();
                    model.move();
                    model.show();
                    model.brain.show(0, 0, 360, 790, model.vision, model.decision);
                    if (model.dead)
                    {
                        Snake newmodel = new Snake();
                        newmodel.brain = model.brain.clone();
                        model = newmodel;

                    }
                    App.textSize(25);
                    App.fill(150);
                    App.textAlign(App.LEFT);
                    App.text("SCORE : " + model.score, 120, App.height - 45);
                }
                App.textAlign(App.LEFT);
                App.textSize(18);
                App.fill(255, 0, 0);
                App.text("RED < 0", 120, App.height - 75);
                App.fill(0, 0, 255);
                App.text("BLUE > 0", 200, App.height - 75);
                graphButton.show();
                loadButton.show();
                saveButton.show();
            }

        }

        public static void fileSelectedIn(/*File selection*/)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Load Snake Model";
            

            if (ofd.ShowDialog() != DialogResult.OK)//selection == null)
            {
                App.println("Window was closed or the user hit cancel.");
            }
            else
            {
                String path = ofd.FileName;//selection.getAbsolutePath();
                Table modelTable = App.loadTable(path, "header");
                Matrix[] weights = new Matrix[modelTable.getColumnCount() - 1];
                double[,] fin = new double[hidden_nodes,25];
                for (int i = 0; i < hidden_nodes; i++)
                {
                    for (int j = 0; j < 25; j++)
                    {
                        fin[i,j] = modelTable.getFloat(j + i * 25, "L0");
                    }
                }
                weights[0] = new Matrix(fin);

                for (int h = 1; h < weights.Length - 1; h++)
                {
                    double[,] hid = new double[hidden_nodes,hidden_nodes + 1];
                    for (int i = 0; i < hidden_nodes; i++)
                    {
                        for (int j = 0; j < hidden_nodes + 1; j++)
                        {
                            hid[i,j] = modelTable.getFloat(j + i * (hidden_nodes + 1), "L" + h);
                        }
                    }
                    weights[h] = new Matrix(hid);
                }

                double[,] fout = new double[4,hidden_nodes + 1];
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < hidden_nodes + 1; j++)
                    {
                        fout[i,j] = modelTable.getFloat(j + i * (hidden_nodes + 1), "L" + (weights.Length - 1));
                    }
                }
                weights[weights.Length - 1] = new Matrix(fout);

                evolution = new List<int>();
                int g = 0;
                int genscore = modelTable.getInt(g, "Graph");
                while (genscore != 0)
                {
                    evolution.Add(genscore);
                    g++;
                    genscore = modelTable.getInt(g, "Graph");
                }
                modelLoaded = true;
                humanPlaying = false;
                model = new Snake(weights.Length - 1);
                model.brain.load(weights);
            }
        }

        public static void fileSelectedOut(/*File selection*/)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save Snake Model";

            if (sfd.ShowDialog() != DialogResult.OK)//selection == null)
            {
                App.println("Window was closed or the user hit cancel.");
            }
            else
            {
                String path = sfd.FileName;//selection.getAbsolutePath();
                Table modelTable = new Table();
                Snake modelToSave = pop.bestSnake.clone();
                Matrix[] modelWeights = modelToSave.brain.pull();
                double[][] weights = new double[modelWeights.Length][];
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] = modelWeights[i].toArray();
                }
                for (int i = 0; i < weights.Length; i++)
                {
                    modelTable.addColumn("L" + i);
                }
                modelTable.addColumn("Graph");
                int maxLen = weights[0].Length;
                for (int i = 1; i < weights.Length; i++)
                {
                    if (weights[i].Length > maxLen)
                    {
                        maxLen = weights[i].Length;
                    }
                }
                int g = 0;
                for (int i = 0; i < maxLen; i++)
                {
                    TableRow newRow = modelTable.addRow();
                    for (int j = 0; j < weights.Length + 1; j++)
                    {
                        if (j == weights.Length)
                        {
                            if (g < evolution.Count)
                            {
                                newRow.setInt("Graph", evolution[g]);
                                g++;
                            }
                        }
                        else if (i < weights[j].Length)
                        {
                            newRow.setFloat("L" + j, weights[j][i]);
                        }
                    }
                }
                App.saveTable(modelTable, path);

            }
        }

        public static void mousePressed(double mouseX, double mouseY)
        {
            if (graphButton.collide(mouseX, mouseY))
            {
                graph = new EvolutionGraph();
            }
            if (loadButton.collide(mouseX, mouseY))
            {
                SnakeAI.fileSelectedIn();//selectInput("Load Snake Model", "fileSelectedIn");
            }
            if (saveButton.collide(mouseX, mouseY))
            {
                SnakeAI.fileSelectedOut();//selectOutput("Save Snake Model", "fileSelectedOut");
            }
            if (increaseMut.collide(mouseX, mouseY))
            {
                mutationRate *= 2;
                defaultmutation = mutationRate;
            }
            if (decreaseMut.collide(mouseX, mouseY))
            {
                mutationRate /= 2;
                defaultmutation = mutationRate;
            }
        }


        public static void keyPressed(Keys keyCode)
        {
            if (humanPlaying)
            {
                //if (key == App.CODED)
                {
                    switch (keyCode)
                    {
                        case Keys.Up:
                            snake.moveUp();
                            break;
                        case Keys.Down:
                            snake.moveDown();
                            break;
                        case Keys.Left:
                            snake.moveLeft();
                            break;
                        case Keys.Right:
                            snake.moveRight();
                            break;
                    }
                }
            }
        }

    }
}
