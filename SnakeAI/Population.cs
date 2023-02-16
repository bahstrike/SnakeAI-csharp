using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeAI
{
    public class Population
    {

        Snake[] snakes;
        public Snake bestSnake;

        int bestSnakeScore = 0;
        public int gen = 0;
        int samebest = 0;

        double bestFitness = 0;
        double fitnessSum = 0;

        public Population(int size)
        {
            snakes = new Snake[size];
            for (int i = 0; i < snakes.Length; i++)
            {
                snakes[i] = new Snake();
            }
            bestSnake = snakes[0].clone();
            bestSnake.replay = true;
        }

        public bool done()
        {  //check if all the snakes in the population are dead
            for (int i = 0; i < snakes.Length; i++)
            {
                if (!snakes[i].dead)
                    return false;
            }
            if (!bestSnake.dead)
            {
                return false;
            }
            return true;
        }

        public void update()
        {  //update all the snakes in the generation
            if (!bestSnake.dead)
            {  //if the best snake is not dead update it, this snake is a replay of the best from the past generation
                bestSnake.look();
                bestSnake.think();
                bestSnake.move();
            }
            for (int i = 0; i < snakes.Length; i++)
            {
                if (!snakes[i].dead)
                {
                    snakes[i].look();
                    snakes[i].think();
                    snakes[i].move();
                }
            }
        }

        public void show()
        {  //show either the best snake or all the snakes
            if (SnakeAI.replayBest)
            {
                bestSnake.show();
                bestSnake.brain.show(0, 0, 360, 790, bestSnake.vision, bestSnake.decision);  //show the brain of the best snake
            }
            else
            {
                for (int i = 0; i < snakes.Length; i++)
                {
                    snakes[i].show();
                }
            }
        }

        void setBestSnake()
        {  //set the best snake of the generation
            double max = 0;
            int maxIndex = 0;
            for (int i = 0; i < snakes.Length; i++)
            {
                if (snakes[i].fitness > max)
                {
                    max = snakes[i].fitness;
                    maxIndex = i;
                }
            }
            if (max > bestFitness)
            {
                bestFitness = max;
                bestSnake = snakes[maxIndex].cloneForReplay();
                bestSnakeScore = snakes[maxIndex].score;
                //samebest = 0;
                //mutationRate = defaultMutation;
            }
            else
            {
                bestSnake = bestSnake.cloneForReplay();
                /*
                samebest++;
                if(samebest > 2) {  //if the best snake has remained the same for more than 3 generations, raise the mutation rate
                   mutationRate *= 2;
                   samebest = 0;
                }*/
            }
        }

        Snake selectParent()
        {  //selects a random number in range of the fitnesssum and if a snake falls in that range then select it
            double rand = App.random.NextDouble() * fitnessSum;
            double summation = 0;
            for (int i = 0; i < snakes.Length; i++)
            {
                summation += snakes[i].fitness;
                if (summation > rand)
                {
                    return snakes[i];
                }
            }
            return snakes[0];
        }

        public void naturalSelection()
        {
            Snake[] newSnakes = new Snake[snakes.Length];

            setBestSnake();
            calculateFitnessSum();

            newSnakes[0] = bestSnake.clone();  //add the best snake of the prior generation into the new generation
            for (int i = 1; i < snakes.Length; i++)
            {
                Snake child = selectParent().crossover(selectParent());
                child.mutate();
                newSnakes[i] = child;
            }
            snakes = new Snake[newSnakes.Length];//newSnakes.clone();
            for (int x = 0; x < newSnakes.Length; x++)
                snakes[x] = newSnakes[x].clone();
            SnakeAI.evolution.Add(bestSnakeScore);
            gen += 1;
        }

        void mutate()
        {
            for (int i = 1; i < snakes.Length; i++)
            {  //start from 1 as to not override the best snake placed in index 0
                snakes[i].mutate();
            }
        }

        public void calculateFitness()
        {  //calculate the fitnesses for each snake
            for (int i = 0; i < snakes.Length; i++)
            {
                snakes[i].calculateFitness();
            }
        }

        void calculateFitnessSum()
        {  //calculate the sum of all the snakes fitnesses
            fitnessSum = 0;
            for (int i = 0; i < snakes.Length; i++)
            {
                fitnessSum += snakes[i].fitness;
            }
        }
    }

}
