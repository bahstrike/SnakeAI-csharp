using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeAI
{
    public class Matrix
    {

        public int rows, cols;
        public double[,] matrix;

        public Matrix(int r, int c)
        {
            rows = r;
            cols = c;
            matrix = new double[rows,cols];
        }

        public Matrix(double[,] m)
        {
            matrix = m;
            rows = matrix.GetLength(0);
            cols = matrix.GetLength(1);
        }

        public void output()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    App.print(matrix[i,j] + " ");
                }
                App.println();
            }
            App.println();
        }

        public Matrix dot(Matrix n)
        {
            Matrix result = new Matrix(rows, n.cols);

            if (cols == n.rows)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < n.cols; j++)
                    {
                        double sum = 0;
                        for (int k = 0; k < cols; k++)
                        {
                            sum += matrix[i,k] * n.matrix[k,j];
                        }
                        result.matrix[i,j] = sum;
                    }
                }
            }
            return result;
        }

        public void randomize()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = App.random.NextDouble() * 2.0 - 1.0;
                }
            }
        }

        public Matrix singleColumnMatrixFromArray(double[] arr)
        {
            Matrix n = new Matrix(arr.Length, 1);
            for (int i = 0; i < arr.Length; i++)
            {
                n.matrix[i,0] = arr[i];
            }
            return n;
        }

        public double[] toArray()
        {
            double[] arr = new double[rows * cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    arr[j + i * cols] = matrix[i,j];
                }
            }
            return arr;
        }

        public Matrix addBias()
        {
            Matrix n = new Matrix(rows + 1, 1);
            for (int i = 0; i < rows; i++)
            {
                n.matrix[i,0] = matrix[i,0];
            }
            n.matrix[rows,0] = 1;
            return n;
        }

        public Matrix activate()
        {
            Matrix n = new Matrix(rows, cols);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    n.matrix[i,j] = relu(matrix[i,j]);
                }
            }
            return n;
        }

        public double relu(double x)
        {
            return Math.Max(0, x);
        }

        public void mutate(double mutationRate)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double rand = App.random.NextDouble();
                    if (rand < mutationRate)
                    {
                        matrix[i,j] += App.randomGaussian() / 5;

                        if (matrix[i,j] > 1)
                        {
                            matrix[i,j] = 1;
                        }
                        if (matrix[i,j] < -1)
                        {
                            matrix[i,j] = -1;
                        }
                    }
                }
            }
        }

        public Matrix crossover(Matrix partner)
        {
            Matrix child = new Matrix(rows, cols);

            int randC = (int)Math.Floor(App.random.NextDouble() * cols);
            int randR = (int)Math.Floor(App.random.NextDouble() * rows);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if ((i < randR) || (i == randR && j <= randC))
                    {
                        child.matrix[i,j] = matrix[i,j];
                    }
                    else
                    {
                        child.matrix[i,j] = partner.matrix[i,j];
                    }
                }
            }
            return child;
        }

        public Matrix clone()
        {
            Matrix clone = new Matrix(rows, cols);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    clone.matrix[i,j] = matrix[i,j];
                }
            }
            return clone;
        }
    }

}
