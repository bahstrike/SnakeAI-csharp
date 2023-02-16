using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SnakeAI
{
    public class Snake
    {

        public int score = 1;
        int lifeLeft = 200;  //amount of moves the snake can make before it dies
        int lifetime = 0;  //amount of time the snake has been alive
        int xVel, yVel;
        int foodItterate = 0;  //itterator to run through the foodlist (used for replay)

        public double fitness = 0;

        public bool dead = false;
        public bool replay = false;  //if this snake is a replay of best snake

        public double[] vision;  //snakes vision
        public double[] decision;  //snakes decision

        PointF head;

        List<PointF> body;  //snakes body
        List<Food> foodList;  //list of food positions (used to replay the best snake)

        Food food;
        public NeuralNet brain;

        public Snake()
            : this(SnakeAI.hidden_layers)
        {
            
        }

        public Snake(int layers)
        {
            head = new PointF(800, App.height / 2);
            food = new Food();
            body = new List<PointF>();
            if (!SnakeAI.humanPlaying)
            {
                vision = new double[24];
                decision = new double[4];
                foodList = new List<Food>();
                foodList.Add(food.clone());
                brain = new NeuralNet(24, SnakeAI.hidden_nodes, 4, layers);
                body.Add(new PointF(800, (App.height / 2) + SnakeAI.SIZE));
                body.Add(new PointF(800, (App.height / 2) + (2 * SnakeAI.SIZE)));
                score += 2;
            }
        }

        Snake(List<Food> foods)
        {  //this constructor passes in a list of food positions so that a replay can replay the best snake
            replay = true;
            vision = new double[24];
            decision = new double[4];
            body = new List<PointF>();
            foodList = new List<Food>(foods.Count);
            foreach (Food f in foods)
            {  //clone all the food positions in the foodlist
                foodList.Add(f.clone());
            }
            food = foodList[foodItterate];
            foodItterate++;
            head = new PointF(800, App.height / 2);
            body.Add(new PointF(800, (App.height / 2) + SnakeAI.SIZE));
            body.Add(new PointF(800, (App.height / 2) + (2 * SnakeAI.SIZE)));
            score += 2;
        }

        bool bodyCollide(double x, double y)
        {  //check if a position collides with the snakes body
            for (int i = 0; i < body.Count; i++)
            {
                if (x == body[i].X && y == body[i].Y)
                {
                    return true;
                }
            }
            return false;
        }

        bool foodCollide(double x, double y)
        {  //check if a position collides with the food
            if (x == food.pos.X && y == food.pos.Y)
            {
                return true;
            }
            return false;
        }

        bool wallCollide(double x, double y)
        {  //check if a position collides with the wall
            if (x >= App.width - (SnakeAI.SIZE) || x < 400 + SnakeAI.SIZE || y >= App.height - (SnakeAI.SIZE) || y < SnakeAI.SIZE)
            {
                return true;
            }
            return false;
        }

        public void show()
        {  //show the snake
            food.show();
            App.fill(255);
            App.stroke(0);
            for (int i = 0; i < body.Count; i++)
            {
                App.rect(body[i].X, body[i].Y, SnakeAI.SIZE, SnakeAI.SIZE);
            }
            if (dead)
            {
                App.fill(150);
            }
            else
            {
                App.fill(255);
            }
            App.rect(head.X, head.Y, SnakeAI.SIZE, SnakeAI.SIZE);
        }

        public void move()
        {  //move the snake
            if (!dead)
            {
                if (!SnakeAI.humanPlaying && !SnakeAI.modelLoaded)
                {
                    lifetime++;
                    lifeLeft--;
                }
                if (foodCollide(head.X, head.Y))
                {
                    eat();
                }
                shiftBody();
                if (wallCollide(head.X, head.Y))
                {
                    dead = true;
                }
                else if (bodyCollide(head.X, head.Y))
                {
                    dead = true;
                }
                else if (lifeLeft <= 0 && !SnakeAI.humanPlaying)
                {
                    dead = true;
                }
            }
        }

        void eat()
        {  //eat food
            int len = body.Count - 1;
            score++;
            if (!SnakeAI.humanPlaying && !SnakeAI.modelLoaded)
            {
                if (lifeLeft < 500)
                {
                    if (lifeLeft > 400)
                    {
                        lifeLeft = 500;
                    }
                    else
                    {
                        lifeLeft += 100;
                    }
                }
            }
            if (len >= 0)
            {
                body.Add(body[len]);
            }
            else
            {
                body.Add(head);
            }
            if (!replay)
            {
                food = new Food();
                while (bodyCollide(food.pos.X, food.pos.Y))
                {
                    food = new Food();
                }
                if (!SnakeAI.humanPlaying)
                {
                    foodList.Add(food);
                }
            }
            else
            {  //if the snake is a replay, then we dont want to create new random foods, we want to see the positions the best snake had to collect
                food = foodList[foodItterate];
                foodItterate++;
            }
        }

        void shiftBody()
        {  //shift the body to follow the head
            PointF temp = head;
            head.X += xVel;
            head.Y += yVel;
            PointF temp2;
            for (int i = 0; i < body.Count; i++)
            {
                temp2 = body[i];
                body[i] = temp;
                temp = temp2;
            }
        }

        public Snake cloneForReplay()
        {  //clone a version of the snake that will be used for a replay
            Snake clone = new Snake(foodList);
            clone.brain = brain.clone();
            return clone;
        }

        public Snake clone()
        {  //clone the snake
            Snake clone = new Snake(SnakeAI.hidden_layers);
            clone.brain = brain.clone();
            return clone;
        }

        public Snake crossover(Snake parent)
        {  //crossover the snake with another snake
            Snake child = new Snake(SnakeAI.hidden_layers);
            child.brain = brain.crossover(parent.brain);
            return child;
        }

        public void mutate()
        {  //mutate the snakes brain
            brain.mutate(SnakeAI.mutationRate);
        }

        public void calculateFitness()
        {  //calculate the fitness of the snake
            if (score < 10)
            {
                fitness = Math.Floor((double)(lifetime * lifetime)) * Math.Pow(2, score);
            }
            else
            {
                fitness = Math.Floor((double)lifetime * lifetime);
                fitness *= Math.Pow(2, 10);
                fitness *= (score - 9);
            }
        }

        public void look()
        {  //look in all 8 directions and check for food, body and wall
            vision = new double[24];
            double[] temp = lookInDirection(new PointF(-SnakeAI.SIZE, 0));
            vision[0] = temp[0];
            vision[1] = temp[1];
            vision[2] = temp[2];
            temp = lookInDirection(new PointF(-SnakeAI.SIZE, -SnakeAI.SIZE));
            vision[3] = temp[0];
            vision[4] = temp[1];
            vision[5] = temp[2];
            temp = lookInDirection(new PointF(0, -SnakeAI.SIZE));
            vision[6] = temp[0];
            vision[7] = temp[1];
            vision[8] = temp[2];
            temp = lookInDirection(new PointF(SnakeAI.SIZE, -SnakeAI.SIZE));
            vision[9] = temp[0];
            vision[10] = temp[1];
            vision[11] = temp[2];
            temp = lookInDirection(new PointF(SnakeAI.SIZE, 0));
            vision[12] = temp[0];
            vision[13] = temp[1];
            vision[14] = temp[2];
            temp = lookInDirection(new PointF(SnakeAI.SIZE, SnakeAI.SIZE));
            vision[15] = temp[0];
            vision[16] = temp[1];
            vision[17] = temp[2];
            temp = lookInDirection(new PointF(0, SnakeAI.SIZE));
            vision[18] = temp[0];
            vision[19] = temp[1];
            vision[20] = temp[2];
            temp = lookInDirection(new PointF(-SnakeAI.SIZE, SnakeAI.SIZE));
            vision[21] = temp[0];
            vision[22] = temp[1];
            vision[23] = temp[2];
        }

        double[] lookInDirection(PointF direction)
        {  //look in a direction and check for food, body and wall
            double[] look = new double[3];
            PointF pos = new PointF(head.X, head.Y);
            double distance = 0;
            bool foodFound = false;
            bool bodyFound = false;
            pos.X += direction.X;
            pos.Y += direction.Y;
            distance += 1;
            while (!wallCollide(pos.X, pos.Y))
            {
                if (!foodFound && foodCollide(pos.X, pos.Y))
                {
                    foodFound = true;
                    look[0] = 1;
                }
                if (!bodyFound && bodyCollide(pos.X, pos.Y))
                {
                    bodyFound = true;
                    look[1] = 1;
                }
                if (replay && SnakeAI.seeVision)
                {
                    App.stroke(0, 255, 0);
                    App.point(pos.X, pos.Y);
                    if (foodFound)
                    {
                        App.noStroke();
                        App.fill(255, 255, 51);
                        App.ellipseMode(App.CENTER);
                        App.ellipse(pos.X, pos.Y, 5, 5);
                    }
                    if (bodyFound)
                    {
                        App.noStroke();
                        App.fill(102, 0, 102);
                        App.ellipseMode(App.CENTER);
                        App.ellipse(pos.X, pos.Y, 5, 5);
                    }
                }
                pos.X += direction.X;
                pos.Y += direction.Y;
                distance += 1;
            }
            if (replay && SnakeAI.seeVision)
            {
                App.noStroke();
                App.fill(0, 255, 0);
                App.ellipseMode(App.CENTER);
                App.ellipse(pos.X, pos.Y, 5, 5);
            }
            look[2] = 1 / distance;
            return look;
        }

        public void think()
        {  //think about what direction to move
            decision = brain.output(vision);
            int maxIndex = 0;
            double max = 0;
            for (int i = 0; i < decision.Length; i++)
            {
                if (decision[i] > max)
                {
                    max = decision[i];
                    maxIndex = i;
                }
            }

            switch (maxIndex)
            {
                case 0:
                    moveUp();
                    break;
                case 1:
                    moveDown();
                    break;
                case 2:
                    moveLeft();
                    break;
                case 3:
                    moveRight();
                    break;
            }
        }

        public void moveUp()
        {
            if (yVel != SnakeAI.SIZE)
            {
                xVel = 0; yVel = -SnakeAI.SIZE;
            }
        }
        public void moveDown()
        {
            if (yVel != -SnakeAI.SIZE)
            {
                xVel = 0; yVel = SnakeAI.SIZE;
            }
        }
        public void moveLeft()
        {
            if (xVel != SnakeAI.SIZE)
            {
                xVel = -SnakeAI.SIZE; yVel = 0;
            }
        }
        public void moveRight()
        {
            if (xVel != -SnakeAI.SIZE)
            {
                xVel = SnakeAI.SIZE; yVel = 0;
            }
        }
    }

}
