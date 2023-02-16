using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeAI
{
    public class NeuralNet
    {

        int iNodes, hNodes, oNodes, hLayers;
        Matrix[] weights;

        public NeuralNet(int input, int hidden, int output, int hiddenLayers)
        {
            iNodes = input;
            hNodes = hidden;
            oNodes = output;
            hLayers = hiddenLayers;

            weights = new Matrix[hLayers + 1];
            weights[0] = new Matrix(hNodes, iNodes + 1);
            for (int i = 1; i < hLayers; i++)
            {
                weights[i] = new Matrix(hNodes, hNodes + 1);
            }
            weights[weights.Length - 1] = new Matrix(oNodes, hNodes + 1);

            foreach (Matrix w in weights)
            {
                w.randomize();
            }
        }

        public void mutate(double mr)
        {
            foreach (Matrix w in weights)
            {
                w.mutate(mr);
            }
        }

        public double[] output(double[] inputsArr)
        {
            Matrix inputs = weights[0].singleColumnMatrixFromArray(inputsArr);

            Matrix curr_bias = inputs.addBias();

            for (int i = 0; i < hLayers; i++)
            {
                Matrix hidden_ip = weights[i].dot(curr_bias);
                Matrix hidden_op = hidden_ip.activate();
                curr_bias = hidden_op.addBias();
            }

            Matrix output_ip = weights[weights.Length - 1].dot(curr_bias);
            Matrix output = output_ip.activate();

            return output.toArray();
        }

        public NeuralNet crossover(NeuralNet partner)
        {
            NeuralNet child = new NeuralNet(iNodes, hNodes, oNodes, hLayers);
            for (int i = 0; i < weights.Length; i++)
            {
                child.weights[i] = weights[i].crossover(partner.weights[i]);
            }
            return child;
        }

        public NeuralNet clone()
        {
            NeuralNet clone = new NeuralNet(iNodes, hNodes, oNodes, hLayers);
            for (int i = 0; i < weights.Length; i++)
            {
                clone.weights[i] = weights[i].clone();
            }

            return clone;
        }

        public void load(Matrix[] weight)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = weight[i];
            }
        }

        public Matrix[] pull()
        {
            Matrix[] model = new Matrix[weights.Length];//weights.clone();
            for (int x = 0; x < weights.Length; x++)
                model[x] = weights[x].clone();
            return model;
        }

        public void show(double x, double y, double w, double h, double[] vision, double[] decision)
        {
            double space = 5;
            double nSize = (h - (space * (iNodes - 2))) / iNodes;
            double nSpace = (w - (weights.Length * nSize)) / weights.Length;
            double hBuff = (h - (space * (hNodes - 1)) - (nSize * hNodes)) / 2;
            double oBuff = (h - (space * (oNodes - 1)) - (nSize * oNodes)) / 2;

            int maxIndex = 0;
            for (int i = 1; i < decision.Length; i++)
            {
                if (decision[i] > decision[maxIndex])
                {
                    maxIndex = i;
                }
            }

            int lc = 0;  //Layer Count

            //DRAW NODES
            for (int i = 0; i < iNodes; i++)
            {  //DRAW INPUTS
                if (vision[i] != 0)
                {
                    App.fill(0, 255, 0);
                }
                else
                {
                    App.fill(255);
                }
                App.stroke(0);
                App.ellipseMode(App.CORNER);
                App.ellipse(x, y + (i * (nSize + space)), nSize, nSize);
                App.textSize(nSize / 2);
                App.textAlign(App.CENTER, App.CENTER);
                App.fill(0);
                App.text(i, x + (nSize / 2), y + (nSize / 2) + (i * (nSize + space)));
            }

            lc++;

            for (int a = 0; a < hLayers; a++)
            {
                for (int i = 0; i < hNodes; i++)
                {  //DRAW HIDDEN
                    App.fill(255);
                    App.stroke(0);
                    App.ellipseMode(App.CORNER);
                    App.ellipse(x + (lc * nSize) + (lc * nSpace), y + hBuff + (i * (nSize + space)), nSize, nSize);
                }
                lc++;
            }

            for (int i = 0; i < oNodes; i++)
            {  //DRAW OUTPUTS
                if (i == maxIndex)
                {
                    App.fill(0, 255, 0);
                }
                else
                {
                    App.fill(255);
                }
                App.stroke(0);
                App.ellipseMode(App.CORNER);
                App.ellipse(x + (lc * nSpace) + (lc * nSize), y + oBuff + (i * (nSize + space)), nSize, nSize);
            }

            lc = 1;

            //DRAW WEIGHTS
            for (int i = 0; i < weights[0].rows; i++)
            {  //INPUT TO HIDDEN
                for (int j = 0; j < weights[0].cols - 1; j++)
                {
                    if (weights[0].matrix[i,j] < 0)
                    {
                        App.stroke(255, 0, 0);
                    }
                    else
                    {
                        App.stroke(0, 0, 255);
                    }
                    App.line(x + nSize, y + (nSize / 2) + (j * (space + nSize)), x + nSize + nSpace, y + hBuff + (nSize / 2) + (i * (space + nSize)));
                }
            }

            lc++;

            for (int a = 1; a < hLayers; a++)
            {
                for (int i = 0; i < weights[a].rows; i++)
                {  //HIDDEN TO HIDDEN
                    for (int j = 0; j < weights[a].cols - 1; j++)
                    {
                        if (weights[a].matrix[i,j] < 0)
                        {
                            App.stroke(255, 0, 0);
                        }
                        else
                        {
                            App.stroke(0, 0, 255);
                        }
                        App.line(x + (lc * nSize) + ((lc - 1) * nSpace), y + hBuff + (nSize / 2) + (j * (space + nSize)), x + (lc * nSize) + (lc * nSpace), y + hBuff + (nSize / 2) + (i * (space + nSize)));
                    }
                }
                lc++;
            }

            for (int i = 0; i < weights[weights.Length - 1].rows; i++)
            {  //HIDDEN TO OUTPUT
                for (int j = 0; j < weights[weights.Length - 1].cols - 1; j++)
                {
                    if (weights[weights.Length - 1].matrix[i,j] < 0)
                    {
                        App.stroke(255, 0, 0);
                    }
                    else
                    {
                        App.stroke(0, 0, 255);
                    }
                    App.line(x + (lc * nSize) + ((lc - 1) * nSpace), y + hBuff + (nSize / 2) + (j * (space + nSize)), x + (lc * nSize) + (lc * nSpace), y + oBuff + (nSize / 2) + (i * (space + nSize)));
                }
            }

            App.fill(0);
            App.textSize(15);
            App.textAlign(App.CENTER, App.CENTER);
            App.text("U", x + (lc * nSize) + (lc * nSpace) + nSize / 2, y + oBuff + (nSize / 2));
            App.text("D", x + (lc * nSize) + (lc * nSpace) + nSize / 2, y + oBuff + space + nSize + (nSize / 2));
            App.text("L", x + (lc * nSize) + (lc * nSpace) + nSize / 2, y + oBuff + (2 * space) + (2 * nSize) + (nSize / 2));
            App.text("R", x + (lc * nSize) + (lc * nSpace) + nSize / 2, y + oBuff + (3 * space) + (3 * nSize) + (nSize / 2));
        }
    }

}
