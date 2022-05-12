using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projectile_Library
{
    public class Projectile
    {
        //creates projectile properties
        private double xPos, yPos, xVelocity, yVelocity, acceleration, initialXPos, initialYPos;
        private byte r, g, b;
        bool landed;
        double latestFiringCanvasScale = 10;

        //encapsulates projectile properties
        public double XPos { get => xPos; set => xPos = value; }
        public double YPos { get => yPos; set => yPos = value; }
        public double XVelocity { get => xVelocity; set => xVelocity = value; }
        public double YVelocity { get => yVelocity; set => yVelocity = value; }
        public double Acceleration { get => acceleration; set => acceleration = value; }
        public byte R { get => r; set => r = value; }
        public byte G { get => g; set => g = value; }
        public byte B { get => b; set => b = value; }
        public double InitialXPos { get => initialXPos; set => initialXPos = value; }
        public double InitialYPos { get => initialYPos; set => initialYPos = value; }
        public bool Landed { get => landed; set => landed = value; }

        public Projectile(double x, double y, double height, double width, bool polar, double angle, double actualSpeed, double xVector, double yVector, double gravity)
        {
            //assign projectile properties
            XPos = x;
            YPos = y;
            InitialXPos = x;
            InitialYPos = y;
            Acceleration = -gravity;
            Landed = false;

            //program will assign speeds required for calculations differently depending on the format the user has used (polar/vector)
            if (polar == true)
            {
                PolarProjectile(angle, actualSpeed);
            }
            else
            {
                VectorProjectile(xVector, yVector);
            }
        }

        private void PolarProjectile(double angle, double actualSpeed)
        {
            //user used polar so trigonometry is used to find the vertical and horizontal components of speed
            XVelocity = actualSpeed * Math.Cos(angle * (Math.PI / 180));
            YVelocity = actualSpeed * Math.Sin(angle * (Math.PI / 180));

            //projectiles with polar velocity are blue
            R = 0;
            G = 0;
            B = 255;

        }

        private void VectorProjectile(double xVector, double yVector)
        {
            //user used vector (the values they specified ARE the vertical and horizontal components of speed)
            XVelocity = xVector;
            YVelocity = yVector;

            //projectiles with polar velocity are red
            R = 255;
            G = 0;
            B = 0;
        }

        public void Move(Projectile projectile, double time, double firingCanvasScale, double projectileDirectionX1, double canvasBaseY)
        {
            //Finds the next position of the projectile

            if (projectile.Landed == false)
            {
                //uses suvat equations (s = ut + 1/2at^2) to find the projectiles displacement. The new position is the initial position + displacement
                projectile.XPos = projectile.InitialXPos + (firingCanvasScale * (projectile.XVelocity * time));
                projectile.YPos = projectile.InitialYPos - (firingCanvasScale * ((projectile.YVelocity * time) + (0.5 * projectile.Acceleration * Math.Pow(time, 2))));
            }


            //if projectile has moved through the floor
            if (projectile.YPos >= canvasBaseY - (2 * firingCanvasScale))
            {
                if (projectile.Landed)
                {
                    //if scale has been adjusted since latest step through
                    if (firingCanvasScale != latestFiringCanvasScale)
                    {
                        projectile.XPos *= (firingCanvasScale / latestFiringCanvasScale);
                    }
                }

                //projectile put ON base (as opposed to being under)
                projectile.YPos = canvasBaseY - (2 * firingCanvasScale);
                projectile.Landed = true;
                latestFiringCanvasScale = firingCanvasScale;
            }

            //this occurs if the projectile has landed, but then changes Y-Pos according to the zoom being altered
            if (projectile.Landed && (projectile.YPos < canvasBaseY - (2 * firingCanvasScale)))
            {
                //projectile moved to correct position
                projectile.XPos *= (firingCanvasScale / latestFiringCanvasScale);
                projectile.YPos = canvasBaseY - (2 * firingCanvasScale);
                latestFiringCanvasScale = firingCanvasScale;
            }
        }



        static void Main(string[] args)
        {
        }
    }
}