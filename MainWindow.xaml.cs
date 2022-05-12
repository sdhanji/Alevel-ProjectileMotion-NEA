using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using Projectile_Library;
using System.Diagnostics;

namespace Projectile_World
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<Projectile> projectileList; //projectiles added to this

        List<Line> verticalLines = new List<Line>();
        List<Line> horizontalLines = new List<Line>(); //creates grid so zoom can be visualised easily
        int index;

        Stopwatch stopWatch; //keeps track of flight time


        DispatcherTimer targetChangedTimer;



        DispatcherTimer theTimer;
        double seconds; //time for SUVAT equations in Projectile.cs
        double angleForVector; //angle between line on simulation and horizontal base
        bool polar = true; //polar or vector?




        /// <summary>
        /// for switching between pages
        /// </summary>
        bool visual = true;
        bool revision = false;
        bool calculator = false;
        List<UIElement> objectsToMove;
        List<UIElement> ballsToRemove;
        bool suvatInProjectile = true;
        bool suvatInRevision = false;
        bool suvatInCalculator = false;


        /// <summary>
        /// checking tickboxes
        /// </summary>
        bool sChecked = false;
        bool uChecked = false;
        bool vChecked = false;
        bool aChecked = false;
        bool tChecked = false;

        double s, u, v, a, t, t1, t2; //actual variables

        /// <summary>
        /// checking textboxes
        /// </summary>
        bool sEmpty = true;
        bool uEmpty = true;
        bool vEmpty = true;
        bool aEmpty = true;
        bool tEmpty = true;


        


        public MainWindow()
        {
            InitializeComponent();
            //these have content so i can see them when editing the xaml, but aren't always needed so text removed at start and can be changed later
            warningBlock.Text = "";
            or.Content = "";
            answer2.Content = "";
            
            //draws gridlines for zoom
            for (int i = 0; i < 273; i++)
            {
                Line newVerticalLine = new Line();
                verticalLines.Add(newVerticalLine);
            }
            for (int i = 0; i < 120; i++)
            {
                Line newHorizontalLine = new Line();
                horizontalLines.Add(newHorizontalLine);
            }
            RenderGridLines();

            AddRevisionContent();

            //constanly refreshes distance from projectile start position to game target
            targetChangedTimer = new DispatcherTimer();
            targetChangedTimer.Interval = new TimeSpan(0, 0, 0, 0, 0);
            targetChangedTimer.Tick += new EventHandler(TargetChangedEvent);
            targetChangedTimer.Start();





        }
        


        /// <summary>
        /// calculates distance from projectile to target and displays it
        /// </summary>
        private void TargetChangedEvent(object sender, EventArgs e)
        {

            double targetPos = ((((2 * (double)target1.GetValue(LeftProperty)) + target1.Width) / 2) / firingCanvas.Width);
            double lines = -1;

            //calculates number of vertical lines on the screen
            for (int i = 0; i < 273; i++)
            {
                if (verticalLines[i].X1 <= firingCanvas.Width)
                {
                    lines += 1;
                    if (i < 272)
                    {
                        if (verticalLines[i + 1].X1 > firingCanvas.Width)
                        {
                            lines += (firingCanvas.Width - verticalLines[i].X1) / (5 * zoom);
                        }
                    }
                }
            }
            targetPos = ((targetPos * lines) - 1) * 5;
            targetPos = Math.Round(targetPos, 4);
            targetDistanceLabel.Content = "Distance from target = " + targetPos;
        }


        /// <summary>
        /// writes out entire revision page
        /// </summary>
        private void AddRevisionContent()
        {
            mainFormulas.Text = "(1) v = u + at";
            mainFormulas.Text += Environment.NewLine + Environment.NewLine + Environment.NewLine + "(2) s = 1/2(u + v)t";
            mainFormulas.Text += Environment.NewLine + Environment.NewLine + Environment.NewLine + "(3) v^2 = u^2 + 2as";
            mainFormulas.Text += Environment.NewLine + Environment.NewLine + Environment.NewLine + "(4) s = ut + 1/2at^2";
            mainFormulas.Text += Environment.NewLine + Environment.NewLine + Environment.NewLine + "(5) s = vt - 1/2at^2";
            mainFormulas.Text += Environment.NewLine + Environment.NewLine + Environment.NewLine + "(6) x = ut (distance = speed x time)";






            conversions.Text = "If you have a velocity in vector form, ai + bj:";
            conversions.Text += Environment.NewLine + Environment.NewLine + "- Horizontal Velocity = a";
            conversions.Text += Environment.NewLine + Environment.NewLine + "- Vertical Velocity = b";
            conversions.Text += Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + "If you have a velocity in polar form,";
            conversions.Text += Environment.NewLine + Environment.NewLine + "Speed = U & Angle from horizontal = θ:";
            conversions.Text += Environment.NewLine + Environment.NewLine + "- Horizontal Velocity = UCos(θ)";
            conversions.Text += Environment.NewLine + Environment.NewLine + "- Vertical Velocity = USin(θ)";





            keyFacts.Text = "- The value of 'a' on Earth = g = 9.8ms^-2, this is not always stated in the question, so you need to remember it";
            keyFacts.Text += Environment.NewLine + Environment.NewLine + Environment.NewLine + "- Before you start, you should pick a direction to be positive and make sure you're consistent throughout the question";
            keyFacts.Text += Environment.NewLine + Environment.NewLine + Environment.NewLine + "- A projectile will always have a velocity (v) of 0 at it's greatest height";
            keyFacts.Text += Environment.NewLine + Environment.NewLine + Environment.NewLine + "- Time of flight = time until the projectile hits the horizonatal plane (vertical displacement is normally, but not always, 0)";
            keyFacts.Text += Environment.NewLine + Environment.NewLine + Environment.NewLine + "- Range = horizontal distance from point of projection to point of landing";
            keyFacts.Text += Environment.NewLine + Environment.NewLine + Environment.NewLine + "- Time is the ONLY variable for which the vertical and horizontal components of a question can be equated";
            keyFacts.Text += Environment.NewLine + Environment.NewLine + Environment.NewLine + "- Equations (4) and (5) are quadratic, so they give 2 values, the only solutions are positive";






            extraFormulaeContext.Text = Environment.NewLine + "The following derivations assume a projectile is projected at speed U and angle θ:";

            extraFormulae.Text = "- To work out the time of greatest height, T, we use equation(1), with v=0, a=g, and u = -USin(θ) (downwards is positive)";
            extraFormulae.Text += Environment.NewLine + "This tells us that T = USin(θ)/g";

            extraFormulae.Text += Environment.NewLine + Environment.NewLine + "- The shape of a projectiles trajection is parabolic (symmetrical) so the time of flight, T, assuming that final vertical displacement = 0";
            extraFormulae.Text += Environment.NewLine + "Therfore, T = 2 x time to reach greatest height = 2USin(θ)/g";

            extraFormulae.Text += Environment.NewLine + Environment.NewLine + "- The range = total horizontal displacement, and since horizontal speed is constant, it equals UTCos(θ)";
            extraFormulae.Text += Environment.NewLine + "In this scennario, T = time of flight because it can be equated in horizontal and vertical components";
            extraFormulae.Text += Environment.NewLine + "Therfore Range = UTCos(θ) = UCos(θ) x T = UCos(θ) x 2USin(θ)/g = (U^2)(2Sin(θ)Cos(θ))/g = (U^2)Sin(2θ)/g";
            extraFormulae.Text += Environment.NewLine + "Range = (U^2)Sin(2θ)/g";
            extraFormulae.Text += Environment.NewLine + "(double angle formula: 2Sin(θ)Cos(θ) = Sin(2θ))";

            extraFormulae.Text += Environment.NewLine + Environment.NewLine + "- There is also a formula for the trajectory of projectile motion (equation of parabolic path)";
            extraFormulae.Text += Environment.NewLine + "The horizontal component has the formula x = ut  =>  x = UtCos(θ)  =>  t = x/UCos(θ)";
            extraFormulae.Text += Environment.NewLine + "The horizontal component has the formula s = ut + 1/2at^2  => y = [UtSin(θ)] - [1/2gt^2]";
            extraFormulae.Text += Environment.NewLine + "The t from the horizontal formula can be substituted into the vertical formula  => y = [USin(θ)(x/UCos(θ))] - [1/2g(x/UCos(θ))^2] ";
            extraFormulae.Text += Environment.NewLine + "=>  y = [xSin(θ)/Cos(θ)] - [gx^2 / 2U^2Cos^2(θ)]";
            extraFormulae.Text += Environment.NewLine + "=>  y = [xTan(θ)] - [gx^2Sec^2(θ) / 2U^2]                            because 1/Cos^2(θ) = Sec^2(θ)";
            extraFormulae.Text += Environment.NewLine + "=>  y = [xTan(θ)] - [gx^2(1 + Tan^2(θ)) / 2U^2]                   because Sec^2(θ) = 1 + Tan^2(θ)";








            dualTimeExplanation.Text = "Equations (4) and (5) are quadratic equations, so they have 2 solutions";
            dualTimeExplanation.Text += Environment.NewLine + "The easiest way to understand this is to look at the projectile paths graphically";
            dualTimeExplanation.Text += Environment.NewLine + Environment.NewLine + "If your projectile had an initial speed of u and an acceleration of -a, the graph represneting it's path would look like the curve to the right and it's equation would be s = ut - 1/2at^2";
            extraDualTimeExplanation.Text = "If I wanted to find the time at which the displacement was 40, I would substitute that into the equation, and using the values of u and a, find the values of t";
            extraDualTimeExplanation.Text += Environment.NewLine + "Graphically however, I would draw the line 's = 40' and find the points of intersection. As you can see there are 2 points at which the lines intersect, which indicates 2 solutions, t1 & t2";
            extraDualTimeExplanation.Text += Environment.NewLine + Environment.NewLine + "You can also have negative solutions of time if the parabola intersects the given value(s) of s above/below the negative t-axis, as well as only a single solution if the line and the parabola intersect tangentially";
        }





        /// <summary>
        /// actually draws grid lines by:
        ///     making new line
        ///     adding line to canvas.children
        /// </summary>
        private void RenderGridLines()
        {
            if (verticalLines != null)
            {
                foreach (Line line in verticalLines) //making vertical lines
                {
                    line.Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    index = verticalLines.IndexOf(line);
                    line.X1 = index * 5 * zoom;
                    line.Y1 = 0;
                    line.X2 = index * 5 * zoom;
                    line.Y2 = firingCanvas.Height;
                    if (firingCanvas.Children.Contains(line) == false)
                    {
                        firingCanvas.Children.Add(line); //adds vertical lines to canvas.children
                    }
                }
            }


            if (horizontalLines != null)
            {
                foreach (Line line in horizontalLines) //making horizontal lines
                {
                    line.Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    index = horizontalLines.IndexOf(line);
                    line.X1 = 0;
                    line.Y1 = firingCanvas.Height - (index * 5 * zoom);
                    line.X2 = firingCanvas.Width;
                    line.Y2 = firingCanvas.Height - (index * 5 * zoom);
                    if (firingCanvas.Children.Contains(line) == false)
                    {
                        firingCanvas.Children.Add(line); //adds horizontal lines to canvas.children
                    }
                }
            }
        }




        /// <summary>
        /// constanly changes projectile position as it moves (accoring to value of 'seconds')
        /// </summary>
        private void TimerEvent(object sender, EventArgs e)
        {
            seconds = stopWatch.Elapsed.TotalSeconds;
            time.Content = Math.Round(seconds, 3);
            foreach (Projectile projectile in projectileList)
            {
                projectile.Move(projectile, seconds, zoom, projectileDirection.X1, firingCanvas.Height);
                if (projectile.Landed)
                {
                    theTimer.Stop();
                    stopWatch.Stop();
                    PlayGame();
                }
            }
            Render();
        }




        /// <summary>
        /// creates new projectile
        /// starts new timer & stopwatch
        /// </summary>
        private void FireProjectile()
        {
            pausePlayButton.Content = "pause";
            projectileList = new List<Projectile>();
            Projectile projectile = new Projectile(projectileDirection.X1, projectileDirection.Y1 - (2 * zoom), 2 * zoom, 2 * zoom, polar, angleSlider.Value, actualSpeedSlider.Value, XVectorSlider.Value, YVectorSlider.Value, GravitySlider.Value);
            projectileList.Add(projectile);



            if (theTimer != null)
            {
                theTimer.Stop();
            }
            if (stopWatch != null)
            {
                stopWatch.Reset();
            }
            theTimer = new DispatcherTimer();
            stopWatch = new Stopwatch();
            seconds = 0;
            theTimer.Interval = new TimeSpan(0, 0, 0, 0, 0);
            theTimer.Tick += new EventHandler(TimerEvent);
            theTimer.Start();
            stopWatch.Start();




            Render();
        }






        /// <summary>
        /// moves projectile to new position (gets rid of it and makes new one in new position)
        /// </summary>
        private void Render()
        {
            //gets rid of old projectile
            if (firingCanvas.Children != null)
            {
                ballsToRemove = new List<UIElement>();
                foreach (UIElement item in firingCanvas.Children)
                {
                    if (item.GetType() == typeof(Ellipse))
                    {
                        int position = firingCanvas.Children.IndexOf(item);
                        var obj = firingCanvas.Children[position];
                        ballsToRemove.Add(obj);
                    }
                }
                foreach (UIElement item in ballsToRemove)
                {
                    firingCanvas.Children.Remove(item);
                }
            }


            //makes new projectile in new position
            foreach (Projectile projectile in projectileList)
            {
                Ellipse drawProjectile = new Ellipse();
                drawProjectile.Height = zoom * 2;
                drawProjectile.Width = zoom * 2;
                drawProjectile.Fill = new SolidColorBrush(Color.FromRgb(projectile.R, projectile.G, projectile.B));
                Canvas.SetLeft(drawProjectile, projectile.XPos);
                Canvas.SetTop(drawProjectile, projectile.YPos);
                firingCanvas.Children.Add(drawProjectile);
            }
        }





        private void fireProjectile_Click(object sender, RoutedEventArgs e)
        {
            FireProjectile();
        }



        /// <summary>
        /// stops/starts timer & stopwatch
        /// </summary>
        private void pausePlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (projectileList != null)
            {
                foreach (Projectile projectile in projectileList)
                {
                    if (projectile.Landed != true)
                    {
                        if ((string)pausePlayButton.Content == "pause")
                        {
                            pausePlayButton.Content = "unpause";
                            theTimer.Stop();
                            stopWatch.Stop();
                        }
                        else
                        {
                            pausePlayButton.Content = "pause";
                            theTimer.Start();
                            stopWatch.Start();
                        }
                    }
                }
            }
        }



        private void heightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {            
            projectileDirection.Y1 -= e.NewValue - e.OldValue; //projectile direction line moves up/down
            projectileDirection.Y2 -= e.NewValue - e.OldValue;
            heightLabel.Content = "Height = " + (heightSlider.Value / 10); //displays new height


            //if height > 0, direction line could be pointed down so angle slider range must be altered to allow this to occur
            if (heightSlider.Value > 0)
            {
                angleSlider.Minimum = -90;
                YVectorSlider.Minimum = -30;
            }
            else
            {
                angleSlider.Minimum = 0;
                YVectorSlider.Minimum = 0;
            }
        }








        /// <summary>
        /// switches between polar and vector form of velocity after button click
        /// </summary>

        private void polarVectorConvert_Click(object sender, RoutedEventArgs e)
        {
            if (polar == true)
            {
                polar = false;
                polarVectorConvert.Content = "Polar";

                angleForVector = Math.Atan2(YVectorSlider.Value, XVectorSlider.Value);

                projectileDirection.X2 = projectileDirection.X1 + (hypotenuse * Math.Cos(angleForVector));
                projectileDirection.Y2 = projectileDirection.Y1 - (hypotenuse * Math.Sin(angleForVector));

                angleSlider.Visibility = Visibility.Hidden; //hides polar functionality
                angleLabel.Visibility = Visibility.Hidden;
                incrementAngle.Visibility = Visibility.Hidden;
                decrementAngle.Visibility = Visibility.Hidden;
                actualSpeedSlider.Visibility = Visibility.Hidden;
                actualSpeedLabel.Visibility = Visibility.Hidden;
                incrementActualSpeed.Visibility = Visibility.Hidden;
                decrementActualSpeed.Visibility = Visibility.Hidden;

                XVectorSlider.Visibility = Visibility.Visible; //vector functionality visible
                XVectorLabel.Visibility = Visibility.Visible;
                incrementXVector.Visibility = Visibility.Visible;
                decrementXVector.Visibility = Visibility.Visible;
                YVectorSlider.Visibility = Visibility.Visible;
                YVectorLabel.Visibility = Visibility.Visible;
                incrementYVector.Visibility = Visibility.Visible;
                decrementYVector.Visibility = Visibility.Visible;
            }
            else
            {
                polar = true;
                polarVectorConvert.Content = "Vector";

                angleLabel.Content = "Angle from Horizontal = " + angleSlider.Value;

                projectileDirection.X2 = projectileDirection.X1 + (hypotenuse * Math.Cos(angleSlider.Value * (Math.PI / 180)));
                projectileDirection.Y2 = projectileDirection.Y1 - (hypotenuse * Math.Sin(angleSlider.Value * (Math.PI / 180)));

                angleSlider.Visibility = Visibility.Visible; //polar functionality visible
                angleLabel.Visibility = Visibility.Visible;
                incrementAngle.Visibility = Visibility.Visible;
                decrementAngle.Visibility = Visibility.Visible;
                actualSpeedSlider.Visibility = Visibility.Visible;
                actualSpeedLabel.Visibility = Visibility.Visible;
                incrementActualSpeed.Visibility = Visibility.Visible;
                decrementActualSpeed.Visibility = Visibility.Visible;

                XVectorSlider.Visibility = Visibility.Hidden; //hides vector functionality
                XVectorLabel.Visibility = Visibility.Hidden;
                incrementXVector.Visibility = Visibility.Hidden;
                decrementXVector.Visibility = Visibility.Hidden;
                YVectorSlider.Visibility = Visibility.Hidden;
                YVectorLabel.Visibility = Visibility.Hidden;
                incrementYVector.Visibility = Visibility.Hidden;
                decrementYVector.Visibility = Visibility.Hidden;
            }
        }








        private void angleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (angleLabel != null)
            {
                angleLabel.Content = "Angle from Horizontal = " + angleSlider.Value;
                projectileDirection.X2 = projectileDirection.X1 + (hypotenuse * Math.Cos(angleSlider.Value * (Math.PI / 180))); //moves projectile direction line when user changes angle
                projectileDirection.Y2 = projectileDirection.Y1 - (hypotenuse * Math.Sin(angleSlider.Value * (Math.PI / 180)));
            }
        }



        //changes angle by +/-1
        private void incrementAngle_Click(object sender, RoutedEventArgs e)
        {
            angleSlider.Value += 1;
        }

        private void decrementAngle_Click(object sender, RoutedEventArgs e)
        {
            angleSlider.Value -= 1;
        }







        
        private void actualSpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            actualSpeedLabel.Content = "Actual Speed = " + actualSpeedSlider.Value; //displays new actual speed
        }



        //changes actual speed by +/-1
        private void incrementActualSpeed_Click(object sender, RoutedEventArgs e)
        {
            actualSpeedSlider.Value += 1;
        }

        private void decrementActualSpeed_Click(object sender, RoutedEventArgs e)
        {
            actualSpeedSlider.Value -= 1;
        }






        private void XVectorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (XVectorSlider != null && XVectorLabel != null)
            {
                XVectorLabel.Content = "X Vector = " + XVectorSlider.Value; //displays new x vector
                angleForVector = Math.Atan2(YVectorSlider.Value, XVectorSlider.Value);
                projectileDirection.X2 = projectileDirection.X1 + (hypotenuse * Math.Cos(angleForVector));
                projectileDirection.Y2 = projectileDirection.Y1 - (hypotenuse * Math.Sin(angleForVector)); //moves projectile direction line
            }
        }

        //changes x vector by +/-1
        private void incrementXVector_Click(object sender, RoutedEventArgs e)
        {
            XVectorSlider.Value += 1;
        }

        private void decrementXVector_Click(object sender, RoutedEventArgs e)
        {
            XVectorSlider.Value -= 1;
        }




        private void YVectorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (YVectorSlider != null && YVectorLabel != null)
            {
                YVectorLabel.Content = "Y Vector = " + YVectorSlider.Value; //displays new y vector
                angleForVector = Math.Atan2(YVectorSlider.Value, XVectorSlider.Value);
                projectileDirection.X2 = projectileDirection.X1 + (hypotenuse * Math.Cos(angleForVector));
                projectileDirection.Y2 = projectileDirection.Y1 - (hypotenuse * Math.Sin(angleForVector)); //moves projectile direction line
            }
        }


        //changes y vector by +/-1
        private void incrementYVector_Click(object sender, RoutedEventArgs e)
        {
            YVectorSlider.Value += 1;
        }

        private void decrementYVector_Click(object sender, RoutedEventArgs e)
        {
            YVectorSlider.Value -= 1;
        }




        private void GravitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GravitySlider.Value = Math.Round(GravitySlider.Value, 2);
            if (GravityLabel != null)
            {
                GravityLabel.Content = "Gravity = " + GravitySlider.Value; //displays new gravity
            }
        }


        //gravity changes by +/-1
        private void incrementGravity_Click(object sender, RoutedEventArgs e)
        {
            GravitySlider.Value += 0.01;
        }

        private void decrementGravity_Click(object sender, RoutedEventArgs e)
        {
            GravitySlider.Value -= 0.01;
        }

        private void resetGravity_Click(object sender, RoutedEventArgs e)
        {
            GravitySlider.Value = 9.8; //Earth gravity value
        }



        /// <summary>
        /// switches canvas
        /// </summary>
        private void changeCanvas1_Click(object sender, RoutedEventArgs e)
        {
            if (visual)
            {
                if (projectileList != null)
                {
                    foreach (Projectile projectile in projectileList)
                    {
                        if (projectile.Landed != true)
                        {
                            if ((string)pausePlayButton.Content == "pause")
                            {
                                pausePlayButton.Content = "unpause"; //pauses projectile if there is one mid-flight
                                theTimer.Stop();
                            }
                        }
                    }
                }
                visual = false;
                projectileCanvas.Visibility = Visibility.Hidden;
                firingCanvas.Visibility = Visibility.Hidden;

                GetSuvat(projectileCanvas); //adds suvat calculator objects to list as they must be moved into correct position depending on which page will be displayed next
                if ((string)changeCanvas1.Content == "Revision")
                {
                    revision = true;
                    revisionCanvas.Visibility = Visibility.Visible;
                    SuvatToRevision(projectileCanvas); //move calculator to correct position for revision page
                }
                else
                {
                    calculator = true;
                    calculatorCanvas.Visibility = Visibility.Visible;
                    SuvatToCalculator(projectileCanvas);//move calculator to correct position for calculator page
                }

                changeCanvas1.Content = "Projectile";
            }


            //same as above but in this case we are moving from revision page to another page
            else if (revision)
            {
                revision = false;
                revisionCanvas.Visibility = Visibility.Hidden;

                GetSuvat(revisionCanvas);
                if ((string)changeCanvas1.Content == "Projectile")
                {
                    visual = true;
                    projectileCanvas.Visibility = Visibility.Visible;
                    firingCanvas.Visibility = Visibility.Visible;
                    SuvatToProjectile(revisionCanvas); //move calculator to correct position for simulation page
                }
                else
                {
                    calculator = true;
                    calculatorCanvas.Visibility = Visibility.Visible;
                    SuvatToCalculator(revisionCanvas); //move calculator to correct position for calculator page
                }

                changeCanvas1.Content = "Revision";
            }



            //same as above but in this case we are moving from calcualtor page to another page
            else
            {
                calculator = false;
                calculatorCanvas.Visibility = Visibility.Hidden;

                GetSuvat(calculatorCanvas);
                if ((string)changeCanvas1.Content == "Projectile")
                {
                    visual = true;
                    projectileCanvas.Visibility = Visibility.Visible;
                    firingCanvas.Visibility = Visibility.Visible;
                    SuvatToProjectile(calculatorCanvas); //move calculator to correct position for simulation page
                }
                else
                {
                    revision = true;
                    revisionCanvas.Visibility = Visibility.Visible;
                    SuvatToRevision(calculatorCanvas); //move calculator to correct position for revision page
                }

                changeCanvas1.Content = "Calc + W/O";
            }
        }



        /// <summary>
        /// identical to above button click
        /// needed because if there are 3 different available pages, there need to be atleast 2 buttons
        /// </summary>
        private void changeCanvas2_Click(object sender, RoutedEventArgs e)
        {
            if (visual)
            {
                if (projectileList != null)
                {
                    foreach (Projectile projectile in projectileList)
                    {
                        if (projectile.Landed != true)
                        {
                            if ((string)pausePlayButton.Content == "pause")
                            {
                                pausePlayButton.Content = "unpause";
                                theTimer.Stop();
                            }
                        }
                    }
                }
                visual = false;
                projectileCanvas.Visibility = Visibility.Hidden;
                firingCanvas.Visibility = Visibility.Hidden;

                GetSuvat(projectileCanvas);

                if ((string)changeCanvas2.Content == "Revision")
                {
                    revision = true;
                    revisionCanvas.Visibility = Visibility.Visible;
                    SuvatToRevision(projectileCanvas);
                }
                else
                {
                    calculator = true;
                    calculatorCanvas.Visibility = Visibility.Visible;
                    SuvatToCalculator(projectileCanvas);
                }

                changeCanvas2.Content = "Projectile";
            }


            else if (revision)
            {
                revision = false;
                revisionCanvas.Visibility = Visibility.Hidden;
                GetSuvat(revisionCanvas);

                if ((string)changeCanvas2.Content == "Projectile")
                {
                    visual = true;
                    projectileCanvas.Visibility = Visibility.Visible;
                    firingCanvas.Visibility = Visibility.Visible;
                    SuvatToProjectile(revisionCanvas);
                }
                else
                {
                    calculator = true;
                    calculatorCanvas.Visibility = Visibility.Visible;
                    SuvatToCalculator(revisionCanvas);
                }

                changeCanvas2.Content = "Revision";
            }



            else
            {
                calculator = false;
                calculatorCanvas.Visibility = Visibility.Hidden;
                GetSuvat(calculatorCanvas);

                if ((string)changeCanvas2.Content == "Projectile")
                {
                    visual = true;
                    projectileCanvas.Visibility = Visibility.Visible;
                    firingCanvas.Visibility = Visibility.Visible;
                    SuvatToProjectile(calculatorCanvas);

                }
                else
                {
                    revision = true;
                    revisionCanvas.Visibility = Visibility.Visible;
                    SuvatToRevision(calculatorCanvas);
                }

                changeCanvas2.Content = "Calc + W/O";
            }
        }







        /// <summary>
        /// collects all objects that make up the calculator 
        /// </summary>
        private void GetSuvat(Canvas canvas)
        {
            objectsToMove = new List<UIElement>();
            foreach (UIElement item in canvas.Children)
            {
                if ((item.GetType() == typeof(TextBox)) || (item.GetType() == typeof(CheckBox)) || item.Equals(answer1) || item.Equals(answer2) || item.Equals(or) || item.Equals(calculateButton)) //all calculator objects
                {
                    if (!(item.Equals(player1CheckBox) || item.Equals(player2CheckBox) || item.Equals(player3CheckBox))) //don't want to move game functionality (also uses checkboxes)
                    {
                        int position = canvas.Children.IndexOf(item);
                        var obj = canvas.Children[position];
                        objectsToMove.Add(obj); //adds objects to list
                    }
                }
            }
        }



        /// <summary>
        /// moves calculator to correct position in simulation page:
        ///     removes object from current canvas
        ///     moves it to simulation canvas
        ///     repositions it
        /// </summary>
        private void SuvatToProjectile(Canvas canvas)
        {
            foreach (UIElement obj in objectsToMove)
            {
                canvas.Children.Remove(obj);
                projectileCanvas.Children.Add(obj);
                if (suvatInCalculator)
                {
                    Canvas.SetLeft(obj, (Canvas.GetLeft(obj) + 563));
                    Canvas.SetTop(obj, (Canvas.GetTop(obj) + 560));
                }
                else if (suvatInRevision)
                {
                    Canvas.SetLeft(obj, (Canvas.GetLeft(obj) - 400));
                    Canvas.SetTop(obj, (Canvas.GetTop(obj) + 590));
                }
            }
            suvatInProjectile = true;
            suvatInCalculator = false;
            suvatInRevision = false;
        }


        /// <summary>
        /// moves calculator to correct position in revision page:
        ///     removes object from current canvas
        ///     moves it to revision canvas
        ///     repositions it
        /// </summary>
        private void SuvatToRevision(Canvas canvas)
        {
            foreach (UIElement obj in objectsToMove)
            {
                canvas.Children.Remove(obj);
                revisionCanvas.Children.Add(obj);
                if (suvatInProjectile)
                {
                    Canvas.SetLeft(obj, (Canvas.GetLeft(obj) + 400));
                    Canvas.SetTop(obj, (Canvas.GetTop(obj) - 590));
                }
                else if (suvatInCalculator)
                {
                    Canvas.SetLeft(obj, (Canvas.GetLeft(obj) + 963));
                    Canvas.SetTop(obj, (Canvas.GetTop(obj) - 30));
                }
            }
            suvatInProjectile = false;
            suvatInCalculator = false;
            suvatInRevision = true;
        }



        /// <summary>
        /// moves calculator to correct position in calculator page:
        ///     removes object from current canvas
        ///     moves it to calculator canvas
        ///     repositions it
        /// </summary>
        private void SuvatToCalculator(Canvas canvas)
        {
            foreach (UIElement obj in objectsToMove)
            {
                canvas.Children.Remove(obj);
                calculatorCanvas.Children.Add(obj);
                if (suvatInProjectile)
                {
                    Canvas.SetLeft(obj, (Canvas.GetLeft(obj) - 563));
                    Canvas.SetTop(obj, (Canvas.GetTop(obj) - 560));
                }
                else if (suvatInRevision)
                {
                    Canvas.SetLeft(obj, (Canvas.GetLeft(obj) - 963));
                    Canvas.SetTop(obj, (Canvas.GetTop(obj) + 30));
                }
            }
            suvatInProjectile = false;
            suvatInCalculator = true;
            suvatInRevision = false;
        }









        /// <summary>
        /// functionality for zooming in/out
        /// </summary>


        double zoom = 10;
        double hypotenuse = 100 * Math.Sqrt(2);
        double theta;

        private void firingCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (zoom >= 1 && zoom <= 40) // 1 <= zoom <= 40
            {
                zoom = Convert.ToDouble(zoomLabel.Content);
                hypotenuse = Math.Sqrt((Math.Pow((projectileDirection.X2 - projectileDirection.X1), 2)) + (Math.Pow((projectileDirection.Y2 - projectileDirection.Y1), 2))); //current length of projectile direction line
                if (e.Delta > 0) //zoom went up
                {
                    if (zoom != 40) //can't go up to 41
                    {
                        //zoom += 1;
                        hypotenuse *= (zoom + 1) / zoom; //new length of projectile direction line

                        //game target moved & size adjusted
                        target1.Width *= (zoom + 1) / zoom;
                        target1.SetValue(LeftProperty, (double)target1.GetValue(LeftProperty) * ((zoom + 1) / zoom));//#
                        target2.Width *= (zoom + 1) / zoom;
                        target2.SetValue(LeftProperty, (double)target1.GetValue(LeftProperty) + (target1.Width / 5));
                        target3.Width *= (zoom + 1) / zoom;
                        target3.SetValue(LeftProperty, (double)target1.GetValue(LeftProperty) + (3 * target1.Width / 5));///
                        /*target1.Width *= zoom / (zoom - 1);
                        target1.SetValue(LeftProperty, (double)target1.GetValue(LeftProperty) * (zoom / (zoom - 1)));//#
                        target2.Width *= zoom / (zoom - 1);
                        target2.SetValue(LeftProperty, (double)target1.GetValue(LeftProperty) + (target1.Width / 5));
                        target3.Width *= zoom / (zoom - 1);
                        target3.SetValue(LeftProperty, (double)target1.GetValue(LeftProperty) + (3 * target1.Width / 5));*/
                        zoom += 1;
                    }
                }
                else //zoom went down
                {
                    if (zoom != 1) //can't go down to 0
                    {
                        //zoom -= 1;
                        hypotenuse *= (zoom - 1) / zoom; //new length of projectile direction line

                        //game target moved and size adjusted
                        target1.Width *= (zoom - 1) / zoom;
                        target1.SetValue(LeftProperty, (double)target1.GetValue(LeftProperty) * ((zoom - 1) / zoom));
                        target2.Width *= (zoom - 1) / zoom;
                        target2.SetValue(LeftProperty, (double)target1.GetValue(LeftProperty) + (target1.Width / 5));
                        target3.Width *= (zoom - 1) / zoom;
                        target3.SetValue(LeftProperty, (double)target1.GetValue(LeftProperty) + (3 * target1.Width / 5));///
                        /*target1.Width *= zoom / (zoom + 1);
                        target1.SetValue(LeftProperty, (double)target1.GetValue(LeftProperty) * (zoom / (zoom + 1)));
                        target2.Width *= zoom / (zoom + 1);
                        target2.SetValue(LeftProperty, (double)target1.GetValue(LeftProperty) + (target1.Width / 5));
                        target3.Width *= zoom / (zoom + 1);
                        target3.SetValue(LeftProperty, (double)target1.GetValue(LeftProperty) + (3 * target1.Width / 5));*/
                        zoom -= 1;
                    }
                }
                projectileDirection.X1 = 4 * zoom; //projectile direction line start position moves horizontally, but not vertically
                RenderGridLines(); //grid lines change according to new zoom
                if (polar)
                {
                    //changes end position of projectile direction according to new length (hypotenuse) when in polar form
                    projectileDirection.Y2 = projectileDirection.Y1 - (hypotenuse * Math.Sin(angleSlider.Value * (Math.PI / 180)));
                    projectileDirection.X2 = projectileDirection.X1 + (hypotenuse * Math.Cos(angleSlider.Value * (Math.PI / 180)));
                }
                else
                {
                    //changes end position of projectile direction according to new length (hypotenuse) when in vector form
                    theta = Math.Atan2(YVectorSlider.Value, XVectorSlider.Value); //angle between projectile direction and base
                    projectileDirection.Y2 = projectileDirection.Y1 - (hypotenuse * Math.Sin(theta));
                    projectileDirection.X2 = projectileDirection.X1 + (hypotenuse * Math.Cos(theta));

                }
                zoomLabel.Content = zoom;




                //changes projectile position according to new zoom
                if (projectileList != null)
                {
                    foreach (Projectile projectile in projectileList)
                    {
                        if (projectile.Landed) //if landed = true, projectile just has to move horizontally
                        {
                            projectile.Move(projectile, seconds, zoom, projectileDirection.X1, firingCanvas.Height);
                        }
                        //if ((string)pausePlayButton.Content == "unpause") //if projectile mid-flight
                        else
                        {
                            //adjusts x and y positions of projectile
                            if ((e.Delta > 0) && (zoom != 40))
                            {
                                projectile.XPos *= zoom / (zoom - 1);
                                projectile.YPos = firingCanvas.Height - ((firingCanvas.Height - projectile.YPos) * (zoom / (zoom - 1)));
                            }
                            else if ((e.Delta < 0) && (zoom != 1))
                            {
                                projectile.XPos *= zoom / (zoom + 1);
                                projectile.YPos = firingCanvas.Height - ((firingCanvas.Height - projectile.YPos) * (zoom / (zoom + 1)));
                            }
                        }
                        Render();
                    }
                }
            }
        }


        //right mouse click pauses projectile
        private void firingCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            pausePlayButton_Click(sender, e);
        }




        /// <summary>
        /// for moving game target with mouse
        /// </summary>
        Point mousePos;
        double startPos, endPos, distanceMoved;
        private void firingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            mousePos = Mouse.GetPosition(this);
            startPos = (double)target1.GetValue(LeftProperty);
            if (Math.Abs(mousePos.Y - (firingCanvas.Height - target1.Height)) < 50) //if mouse is vertically less than 50 from target
            {
                if (Math.Abs(mousePos.X - ((double)target1.GetValue(LeftProperty) + (target1.Width / 2))) < 100) //if mouse is horizontally less than 100 from target centre
                {
                    if ((mousePos.X >= target1.Width / 2) && (mousePos.X + (target1.Width / 2) <= firingCanvas.Width)) //stops target moving out of simulation range
                    {
                        target1.SetValue(LeftProperty, mousePos.X - (target1.Width / 2)); //middle of target moves to mouse x position (y position unchanged)
                    }
                }
            }

            //moves rest of target (made of 3 rectangles, middle one moved above, other 2 moved below)
            endPos = (double)target1.GetValue(LeftProperty);
            distanceMoved = endPos - startPos;
            target2.SetValue(LeftProperty, (double)target2.GetValue(LeftProperty) + distanceMoved);
            target3.SetValue(LeftProperty, (double)target3.GetValue(LeftProperty) + distanceMoved);
        }







        /// <summary>
        /// for playing game
        /// </summary>
        bool player1Checked = false;
        bool player2Checked = false;
        bool player3Checked = false;
        private void player1CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //only player 1 box ticked & only player 1 bool true
            player1Checked = true;

            player2Checked = false;
            player2CheckBox.IsChecked = false;
            player3Checked = false;
            player3CheckBox.IsChecked = false;
        }

        private void player2CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //only player 2 box ticked & only player 2 bool true
            player2Checked = true;


            player1Checked = false;
            player1CheckBox.IsChecked = false;
            player3Checked = false;
            player3CheckBox.IsChecked = false;
        }

        private void player3CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //only player 3 box ticked & only player 3 bool true
            player3Checked = true;

            player1Checked = false;
            player1CheckBox.IsChecked = false;
            player2Checked = false;
            player2CheckBox.IsChecked = false;
        }

        private void resetGameButton_Click(object sender, RoutedEventArgs e)
        {
            //resets point, unticks boxes, all bools false
            player1Points.Content = 0;
            player2Points.Content = 0;
            player3Points.Content = 0;
            player1CheckBox.IsChecked = false;
            player2CheckBox.IsChecked = false;
            player3CheckBox.IsChecked = false;
            player1Checked = false;
            player2Checked = false;
            player3Checked = false;
        }



        /// <summary>
        /// adds points depending on how close projectile lands to target
        /// </summary>
        private void PlayGame()
        {
            double pointsToAdd = 0;
            foreach (Projectile projectile in projectileList)
            {
                if ((projectile.XPos + zoom >= (double)target1.GetValue(LeftProperty)) && (projectile.XPos + zoom < (double)target2.GetValue(LeftProperty)))
                {
                    pointsToAdd = 1; //projectile lands on 1st fifth of target (left edge)
                }
                else if ((projectile.XPos + zoom >= (double)target2.GetValue(LeftProperty)) && (projectile.XPos + zoom < (double)target2.GetValue(LeftProperty) + target2.Width))
                {
                    pointsToAdd = 2; //projectile lands on 2nd fifth of target
                }
                else if ((projectile.XPos + zoom >= (double)target2.GetValue(LeftProperty) + target2.Width) && (projectile.XPos + zoom <= (double)target3.GetValue(LeftProperty)))
                {
                    pointsToAdd = 3; //projectile lands on 3rd fifth of target (middle)
                }
                else if ((projectile.XPos + zoom > (double)target3.GetValue(LeftProperty)) && (projectile.XPos + zoom <= (double)target3.GetValue(LeftProperty) + target3.Width))
                {
                    pointsToAdd = 2; //projectile lands on 4th fifth of target
                }
                else if ((projectile.XPos + zoom > (double)target3.GetValue(LeftProperty) + target3.Width) && (projectile.XPos + zoom <= (double)target1.GetValue(LeftProperty) + target1.Width))
                {
                    pointsToAdd = 1; //projectile lands on 5th fifth of target (right edge)
                }
            }

            //adds point to player whose box is ticked
            if (player1Checked)
            {
                player1Points.Content = Convert.ToInt32(player1Points.Content) + pointsToAdd;
            }
            else if (player2Checked)
            {
                player2Points.Content = Convert.ToInt32(player2Points.Content) + pointsToAdd;
            }
            else if (player3Checked)
            {
                player3Points.Content = Convert.ToInt32(player3Points.Content) + pointsToAdd;
            }
        }











        //*************************************************************











        //               SUVAT CALCULATION CODE BEGINS











        //*************************************************************




        private void sTickBox_Checked(object sender, RoutedEventArgs e)
        {
            if (uChecked | vChecked | aChecked | tChecked)
            {
                //only s is ticked
                uTickBox.IsChecked = false;
                vTickBox.IsChecked = false;
                aTickBox.IsChecked = false;
                tTickBox.IsChecked = false;
            }
            //only s bool is true
            sChecked = true;
            uChecked = false;
            vChecked = false;
            aChecked = false;
            tChecked = false;
        }

        private void uTickBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sChecked | vChecked | aChecked | tChecked)
            {
                //only u is ticked
                sTickBox.IsChecked = false;
                vTickBox.IsChecked = false;
                aTickBox.IsChecked = false;
                tTickBox.IsChecked = false;
            }
            //only u bool is true
            sChecked = false;
            uChecked = true;
            vChecked = false;
            aChecked = false;
            tChecked = false;
        }

        private void vTickBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sChecked | uChecked | aChecked | tChecked)
            {
                //only v is ticked
                sTickBox.IsChecked = false;
                uTickBox.IsChecked = false;
                aTickBox.IsChecked = false;
                tTickBox.IsChecked = false;
            }
            //only v bool is true
            sChecked = false;
            uChecked = false;
            vChecked = true;
            aChecked = false;
            tChecked = false;
        }

        private void aTickBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sChecked | uChecked | vChecked | tChecked)
            {
                //only a is ticked
                sTickBox.IsChecked = false;
                uTickBox.IsChecked = false;
                vTickBox.IsChecked = false;
                tTickBox.IsChecked = false;
            }
            //only a bool is true
            sChecked = false;
            uChecked = false;
            vChecked = false;
            aChecked = true;
            tChecked = false;
        }

        private void tTickBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sChecked | uChecked | vChecked | aChecked)
            {
                //only t is ticked
                sTickBox.IsChecked = false;
                uTickBox.IsChecked = false;
                vTickBox.IsChecked = false;
                aTickBox.IsChecked = false;
            }
            //only t bool is true
            sChecked = false;
            uChecked = false;
            vChecked = false;
            aChecked = false;
            tChecked = true;
        }



        private void sTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //bool to see if s has content
            if (sTextBox.Text != "")
            {
                sEmpty = false;
            }
            else
            {
                sEmpty = true;
            }
        }

        private void uTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //bool to see if u has content
            if (uTextBox.Text != "")
            {
                uEmpty = false;
            }
            else
            {
                uEmpty = true;
            }
        }

        private void vTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //bool to see if v has content
            if (vTextBox.Text != "")
            {
                vEmpty = false;
            }
            else
            {
                vEmpty = true;
            }
        }



        private void aTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //bool to see if a has content
            if (aTextBox.Text != "")
            {
                aEmpty = false;
            }
            else
            {
                aEmpty = true;
            }
        }

        private void tTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //bool to see if t has content
            if (tTextBox.Text != "")
            {
                tEmpty = false;
            }
            else
            {
                tEmpty = true;
            }
        }






        /// <summary>
        /// makes sure textboxes actually contain numbers (not letters/symbols)
        /// if they contain invalid characters, content is cleared
        /// </summary>
        private void calculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (sChecked | uChecked | vChecked | aChecked | tChecked) //if a box is ticked
            {
                double result;
                if (!sEmpty)
                {
                    if (double.TryParse(sTextBox.Text, out result)) //tries to parse it as a 'double'
                    {
                        s = Convert.ToDouble(sTextBox.Text); //if successful 's' takes the value of the textbox
                    }
                    else
                    {
                        sTextBox.Text = ""; //else the textbox is cleared
                    }
                }
                if (!uEmpty)
                {
                    if (double.TryParse(uTextBox.Text, out result)) //tries to parse it as a 'double'
                    {
                        u = Convert.ToDouble(uTextBox.Text); //if successful 'u' takes the value of the textbox
                    }
                    else
                    {
                        uTextBox.Text = ""; //else the textbox is cleared
                    }
                }
                if (!vEmpty)
                {
                    if (double.TryParse(vTextBox.Text, out result)) //tries to parse it as a 'double'
                    {
                        v = Convert.ToDouble(vTextBox.Text); //if successful 'v' takes the value of the textbox
                    }
                    else
                    {
                        vTextBox.Text = ""; //else the textbox is cleared
                    }
                }
                if (!aEmpty)
                {
                    if (double.TryParse(aTextBox.Text, out result)) //tries to parse it as a 'double'
                    {
                        a = Convert.ToDouble(aTextBox.Text); //if successful 'a' takes the value of the textbox
                    }
                    else
                    {
                        aTextBox.Text = ""; //else the textbox is cleared
                    }
                }
                if (!tEmpty)
                {
                    if (double.TryParse(tTextBox.Text, out result)) //tries to parse it as a 'double'
                    {
                        t = Convert.ToDouble(tTextBox.Text); //if successful 't' takes the value of the textbox
                    }
                    else
                    {
                        tTextBox.Text = ""; //else the textbox is cleared
                    }
                }
                Calculate();
            }
            else
            {
                warningBlock.Text = "Warning: please tick a box"; //tells user a box must be ticked
            }
        }

        private void displayAnswer(double value)
        {
            if (warningBlock.Text == null)
            {
                warningBlock.Text = "Warning: None"; //answer has been calculated therfore no warnings to be displayed about user error
            }
            value = Math.Round(value, 5);
            answer1.Content = "Answer = " + value; //displays answer rounded to 5 decimal places
        }



        private void displayAnswer2(double value)
        {
            //same as above function, used when there are 2 answers (time can sometimes have 2 solutions)
            if (warningBlock.Text == null)
            {
                warningBlock.Text = "Warning: None";
            }
            value = Math.Round(value, 5);
            or.Content = "OR";
            answer2.Content = "Answer = " + value;
        }





        /// <summary>
        /// 1st line of working out tells user what SUVAT equation will be used
        /// </summary>

        private void suvaEquation()
        {
            workingOut.Text += Environment.NewLine + Environment.NewLine + "In this scenario we will use " + "'" + "v^2 = u^2 + 2as" + "'" + " because t is the only variable that is not known/desired";
        }

        private void suvtEquation()
        {
            workingOut.Text += Environment.NewLine + Environment.NewLine + "In this scenario we will use " + "'" + "s = 0.5(u + v)t" + "'" + " because a is the only variable that is not known/desired";
        }

        private void suatEquation()
        {
            workingOut.Text += Environment.NewLine + Environment.NewLine + "In this scenario we will use " + "'" + "s = ut + 0.5at^2" + "'" + " because v is the only variable that is not known/desired";
        }

        private void svatEquation()
        {
            workingOut.Text += Environment.NewLine + Environment.NewLine + "In this scenario we will use " + "'" + "s = vt - 0.5at^2" + "'" + " because u is the only variable that is not known/desired";
        }

        private void uvatEquation()
        {
            workingOut.Text += Environment.NewLine + Environment.NewLine + "In this scenario we will use " + "'" + "v = u + at" + "'" + " because s is the only variable that is not known/desired";
        }



        /// <summary>
        /// will write out final line of working at the end, once all working out has been written
        /// </summary>
        private void lastLineOfWorking(double value)
        {

            workingOut.Text += Environment.NewLine + Environment.NewLine + "This calculation gives us the answer of: ";
            if (sChecked)
            {
                workingOut.Text += "s";
            }
            else if (uChecked)
            {
                workingOut.Text += "u";
            }
            else if (vChecked)
            {
                workingOut.Text += "v";
            }
            else if (aChecked)
            {
                workingOut.Text += "a";
            }
            else if (tChecked)
            {
                workingOut.Text += "t";
            }
            workingOut.Text += " = " + value;
        }











        /// <summary>
        /// actually calculates answer depending on what box was ticked and what other 3 variables were entered
        /// warnings are text blocks that explain impossible SUVAT scenarios
        /// 
        /// working out is a text block that
        ///     takes the appropriate SUVAT equation
        ///     rearranges it
        ///     substitutes in the 3 inputs given by the user
        ///     calculates and displays the answer
        /// </summary>
        private void Calculate()
        {
            //empties working out, warnings & answers from previous calculations
            warningBlock.Text = "";
            answer1.Content = "Answer = ";
            answer2.Content = "";
            or.Content = "";
            workingOut.Text = "Working Out:";

            if (sChecked) // calculate displacement
            {
                if (!uEmpty && !vEmpty && !aEmpty && tEmpty) //equation without t
                {
                    s = (Math.Pow(v, 2) - Math.Pow(u, 2)) / (2 * a); //s = (v^2 - u^2) / 2a
                    displayAnswer(s);


                    //warnings
                    if (a == 0)
                    {
                        if (u == v)
                        {
                            warningBlock.Text = "Warning: if acceleration = 0, u will always equal v (if they start as the same value), so any value of displacement could be the answer, hence there is no answer";
                        }
                        else
                        {
                            warningBlock.Text = "Warning: acceleration can't be equal to 0, otherwise velocity would always be " + u + " and never reach " + v + ", hence the answer is NaN/infinity";
                        }
                    }

                    //working out
                    suvaEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating displacement so the equation is rearranged to be: s = (v^2 - u^2) / 2a";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of u, v & a to give: s = (" + v + "^2 + " + u + "^2) / (2*"  + a + ")";
                    lastLineOfWorking(s);
                }
                else if (!uEmpty && !vEmpty && aEmpty && !tEmpty) //equation without a
                {
                    s = 0.5 * (u + v) * t; //s = 1/2(u+v)t
                    displayAnswer(s);

                    //working out
                    suvtEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating displacement so the equation is in the correct form";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substiture values of u, v & t to give s = 0.5 * (" + u + " + " + v + ") * " + t;
                    lastLineOfWorking(s);
                }
                else if (!uEmpty && vEmpty && !aEmpty && !tEmpty) //equation without v
                {
                    s = (u * t) + (0.5 * a * Math.Pow(t, 2)); //s = ut + 1/2at^2
                    displayAnswer(s);

                    //working out
                    suatEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating displacement so the equation is in the correct form";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of u, a & t to give s = (" + u + "*" + t + ") + (0.5*" + a + "*" + t + "^2)";
                    lastLineOfWorking(s);
                }
                else if (uEmpty && !vEmpty && !aEmpty && !tEmpty) //equation without u
                {
                    s = (v * t) - (0.5 * a * Math.Pow(t, 2));//s = vt - 1/2at^2
                    displayAnswer(s);

                    //working out
                    svatEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating displacement so the equation is in the correct form";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of v, a & t to give s = (" + v + "*" + t + ") - (0.5*" + a + "*" + t + "^2)";
                    lastLineOfWorking(s);
                }
                else // more/less than 3 other variables have been entered
                {
                    warningBlock.Text = "Warning: please enter exactly 3 variables";
                    answer1.Content = "Answer = ";
                }
            }






            else if (uChecked) // calculate initial speed
            {
                if (!sEmpty && !vEmpty && !aEmpty && tEmpty) //equation without t
                {
                    u = Math.Sqrt((Math.Pow(v, 2)) - (2 * a * s)); //u = sqrt(v^2 - 2as)
                    displayAnswer(u);

                    if (((Math.Pow(v, 2)) - (2 * a * s)) < 0)
                    {
                        //warning (inside of sqare root must be >= 0)
                        warningBlock.Text = "Warning: this is an impossible scenario so there is no answer, make sure that v^2 - 2as >= 0, otherwise it can't be square rooted in the formula";
                    }

                    //working out
                    suvaEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating initial speed so the equation is rearranged to be: u = sqrt(v^2 - 2as)";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of s, v & a to give u = sqrt((" + v + "^2) - (2*" + a + "*" + s + ")) = sqrt(" + (Math.Pow(v, 2) - (2 * a * s)) + ")";
                    lastLineOfWorking(u);
                }
                else if (!sEmpty && !vEmpty && aEmpty && !tEmpty) //equation without a
                {
                    u = ((2 * s) / t) - v; //u = (2s/t) - v
                    displayAnswer(u);


                    //warnings
                    if (t == 0)
                    {
                        if (s == 0)
                        {
                            warningBlock.Text = "Warning: there is no answer using suvat equations, but using intuition, if after 0 movement through time and space, the velocity is " + v + " it must have started at that same velocity, hence the answer is " + v;
                        }
                        else
                        {
                            warningBlock.Text = "Warning: there is no answer because it's impossible for an object to have been displaced by a value of " + s + " in 0 time";
                        }
                    }


                    //working out
                    suvtEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating initial speed so the equation is rearranged to be: u = (2s / t) - v";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of s, v & t to give u = ((2*" + s + ")/" + t + ")" + " - " + v;
                    lastLineOfWorking(u);

                }
                else if (!sEmpty && vEmpty && !aEmpty && !tEmpty) //equation without v
                {
                    u = (s / t) - (0.5 * a * t); //simpler way to write u = (s - 1/2at^2) / t
                    displayAnswer(u);


                    //warnings
                    if (t == 0)
                    {
                        if (s == (0.5 * a * Math.Pow(t, 2)))
                        {
                            warningBlock.Text = "Warning: if s = 1/2 at^2, ut = 0 (this is obvious from suvat equation 4) but because t = 0, 'u' could have any value and still satisfy 'ut = 0', hence there is no specific value (infinite solutions)";
                            warningBlock.Text += Environment.NewLine + Environment.NewLine + "Another explanation is that, when t=0 if s = 1/2 at^2, s = 0, so given no changes in time or space, only a value for acceleration, 'u' could have any value";
                        }
                        else
                        {
                            warningBlock.Text = "Warning: there is no answer because it's impossible for an object to have been displaced by a value of " + s + " in 0 time";
                        }
                    }

                    //working out
                    suatEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating initial speed so the equation is rearranged to be: u = (s - 0.5at^2) / t";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of s, a & t to give u = (" + s + " - (0.5*" + a + "*" + t + "^2)) / " + t;
                    lastLineOfWorking(u);
                }
                else if (sEmpty && !vEmpty && !aEmpty && !tEmpty) //equation without s
                {
                    u = v - (a * t); // u = v - at
                    displayAnswer(u);

                    //working out
                    uvatEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating initial speed so the equation is rearranged to be: u = v - at";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of v, a & t to give u = " + v + " - " + "(" + a + "*" + t + ")";
                    lastLineOfWorking(u);
                }
                else // more/less than 3 other variables have been entered
                {
                    warningBlock.Text = "Warning: please enter exactly 3 variables";
                    answer1.Content = "Answer = ";
                }
            }




            else if (vChecked) // calculate final speed
            {
                if (!sEmpty && !uEmpty && !aEmpty && tEmpty) //equation without t
                {
                    v = Math.Sqrt((Math.Pow(u, 2)) + (2 * a * s)); //v = sqrt(u^2 + 2as)
                    displayAnswer(v);
                    if (((Math.Pow(u, 2)) + (2 * a * s)) < 0)
                    {
                        //warning (inside of sqare root must be >= 0)
                        warningBlock.Text = "Warning: this is an impossible scenario so there is no answer, make sure that u^2 + 2as >= 0, otherwise it can't be square rooted in the formula";
                    }

                    //working out
                    suvaEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating final speed so the equation is rearranged to be: v = sqrt(u^2 + 2as)";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of s, u & a to give v = sqrt((" + u + "^2) + (2*" + a + "*" + s + ")) = sqrt(" + (Math.Pow(u, 2) + (2 * a * s)) + ")";
                    lastLineOfWorking(v);
                }
                else if (!sEmpty && !uEmpty && aEmpty && !tEmpty) //equation without a
                {
                    v = ((2 * s) / t) - u; //v = (2s/t) - u
                    displayAnswer(v);


                    //warnings
                    if (t == 0)
                    {
                        if (s == 0)
                        {
                            warningBlock.Text = "Warning: there is no answer using suvat equations, but using intuition, if the velocity starts as " + u + ", after 0 movement through space or time, the velocity will remain the same, hence the answer is " + u;
                        }
                        else
                        {
                            warningBlock.Text = "Warning: there is no answer because it's impossible for an object to have been displaced by a value of " + s + " in 0 time";
                        }
                    }

                    //working out
                    suvtEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating final speed so the equation is rearranged to be: v = (2s / t) - u";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of s, u & t to give v = ((2*" + s + ")/" + t + ")" + " - " + u;
                    lastLineOfWorking(v);
                }
                else if (!sEmpty && uEmpty && !aEmpty && !tEmpty) //equation without u
                {
                    v = (s / t) + (0.5 * a * t); //simpler way to write v = (s + 1/2at^2) / t
                    displayAnswer(v);


                    //warnings
                    if (t == 0)
                    {
                        if (-s == (0.5 * a * Math.Pow(t, 2)))
                        {
                            warningBlock.Text = "Warning: if s = -1/2 at^2, vt = 0 (this is obvious from suvat equation 5) but because t = 0, v could have any value and still satisfy 'vt = 0', hence there is no specific value (v could be anything)";
                            warningBlock.Text += Environment.NewLine + Environment.NewLine + "Another explanation is that, when t=0 if s = -1/2 at^2, s = 0, so given no changes in time or space, only a value for acceleration, v could have any value";
                        }
                        else
                        {
                            warningBlock.Text = "Warning: there is no answer because it's impossible for an object to have been displaced by a value of " + s + " in 0 time";
                        }
                    }

                    //working out
                    svatEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating final speed so the equation is rearranged to be: v = (s + 0.5at^2) / t";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of s, a & t to give v = (" + s + " + (0.5*" + a + "*" + t + "^2)) / " + t;
                    lastLineOfWorking(v);
                }
                else if (sEmpty && !uEmpty && !aEmpty && !tEmpty) //equation without s
                {
                    v = u + (a * t); //v = u + at
                    displayAnswer(v);

                    //working out
                    uvatEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating final speed so the equation is rearranged to be: v = u + at";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of u, a & t to give v = " + u + " + " + "(" + a + "*" + t + ")";
                    lastLineOfWorking(v);
                }
                else // more/less than 3 other variables have been entered
                {
                    warningBlock.Text = "Warning: please enter exactly 3 variables";
                    answer1.Content = "Answer = ";
                }
            }

            else if (aChecked) // calculate acceleration
            {
                if (!sEmpty && !uEmpty && !vEmpty && tEmpty) //equation without t
                {
                    a = (Math.Pow(v, 2) - Math.Pow(u, 2)) / (2 * s); //a = (v^2 - u^2) / 2s
                    displayAnswer(a);

                    //warnings
                    if (s == 0)
                    {
                        if (u == v)
                        {
                            warningBlock.Text = "Warning: If displacement = 0, u will always equal v (if they start as the same value) because the object never moves so it's velocity can't change, hence acceleration could take any value so there are infinite solutions";
                        }
                        else if (u == -v)
                        {
                            warningBlock.Text = "Warning: Acceleration is constant in projectile motion, so if u = -v, displacement is always 0";
                            warningBlock.Text += Environment.NewLine + "This means that there are infinite solutions because acceleration could be any number and still satisfy your inputs";

                        }
                        else
                        {
                            warningBlock.Text = "Warning: if displacement 0, the velocity would always be " + u + " and never reach " + v + " because the object never moves, hence the answer is NaN/infinity";
                        }
                    }
                    else if (u == -v)
                    {
                        warningBlock.Text = Environment.NewLine + Environment.NewLine + "Warning: Acceleration is constant in projectile motion, so if u = -v, displacement is always 0";
                        warningBlock.Text += Environment.NewLine + "You have said that s = " + s + ". This is an impossible scenario so there is no solution";
                    }

                    //working out
                    suvaEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating acceleration so the equation is rearranged to be: a = (v^2 - u^2) / 2s";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of s, u & v to give a = ((" + v + "^2)" + " + (" + u + "^2)) / (2*" + s + ")";
                    lastLineOfWorking(a);
                }
                else if (!sEmpty && !uEmpty && vEmpty && !tEmpty) //equation without v
                {
                    a = (2 * (s - (u * t))) / Math.Pow(t, 2); // a = (2(s - ut)) / t^2
                    displayAnswer(a);

                    //warnings
                    if (t == 0)
                    {
                        if (s == 0)
                        {
                            warningBlock.Text = "Warning: if s and t are both 0, the object doesn't travel through time or space, hence it's acceleration could have any value (there are infinite solutions)";
                        }
                        else
                        {
                            warningBlock.Text = "Warning: If the time of movement = 0, it's impossible for the object to be displaced by a value of " + s + ", hence acceleration requires a theoretical value of infinity";
                            if (s < 0)
                            {
                                warningBlock.Text += Environment.NewLine + Environment.NewLine + "Why NEGATIVE infinity? displacement = speed x time, so if the initial velocity x time > total displacement (as in this case) the object's final velocity < it's initial velocity. The velocity has thus decreased, hence the acceleration is negative";
                            }
                        }
                    }


                    //working out
                    suatEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating acceleration so the equation is rearranged to be a: = 2(s - ut) / t^2";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of s, u & t to give a = (2*(" + s + " - " + "(" + u + "*" + t + "))) / (" + t + "^2)";
                    lastLineOfWorking(a);
                }
                else if (!sEmpty && uEmpty && !vEmpty && !tEmpty) //equation without u
                {
                    a = (2 * ((v * t) - s)) / Math.Pow(t, 2); // a = (2(vt - s)) / t^2
                    displayAnswer(a);

                    //warnings
                    if (t == 0)
                    {
                        if (s == 0)
                        {
                            warningBlock.Text = "Warning: if s and t are both 0, the object doesn't travel through time or space, hence it's acceleration could have any value";
                        }
                        else
                        {
                            warningBlock.Text = "Warning: If the time of movement = 0, it's impossible for the object to have been displaced by a value of " + s + ", hence acceleration requires a theoretical value of infinity";
                            if (s > 0)
                            {
                                warningBlock.Text += Environment.NewLine + Environment.NewLine + "Why NEGATIVE infinity? displacement = speed x time, so if the final velocity x time < total displacement (as in this case) the object's initial velocity > it's final velocity. The velocity has thus decreased, hence the acceleration is negative";
                            }
                        }
                    }

                    //working out
                    svatEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating acceleration so the equation is rearranged to be: a = 2(vt - s) / t^2";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of s, u & t to give a = (2*(" + "(" + v + "*" + t + ")" + " - " + s + ")) / (" + t + "^2)";
                    lastLineOfWorking(a);
                }
                else if (sEmpty && !uEmpty && !vEmpty && !tEmpty) //equation without s
                {
                    a = (v - u) / t;
                    displayAnswer(a);

                    //warnings
                    if (t == 0)
                    {
                        if (v == u)
                        {
                            warningBlock.Text = "Warning: The initial and final velocities are equal which makes sense because t=0, but it also means that we don't have enough information to determine the acceleration, because we're only looking at a single point in time, hence there are infinite solutions";
                        }
                        else
                        {
                            warningBlock.Text = "Warning: If t=0, it's impossible for the velocity to change from " + u + " to " + v + " hence the acceration is theoretically infinity";
                            if (v > u)
                            {
                                warningBlock.Text += Environment.NewLine + Environment.NewLine + "The acceleration is +infinity because the velocity has increased";
                            }
                            else
                            {
                                warningBlock.Text += Environment.NewLine + Environment.NewLine + "The acceleration is -infinity because the velocity has decreased";
                            }
                        }
                    }

                    //working out
                    uvatEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating acceleration so the equation is rearranged to be: a = (v - u) / t";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of u, v & t to give a = (" + v + " - " + u + ") / " + t;
                    lastLineOfWorking(a);
                }
                else // more/less than 3 other variables have been entered
                {
                    warningBlock.Text = "Warning: please enter exactly 3 variables";
                    answer1.Content = "Answer = ";
                }
            }

            else if (tChecked) // calculate time
            {
                if (!sEmpty && !uEmpty && !vEmpty && aEmpty) //equation without a
                {
                    t = (2 * s) / (u + v); //t = 2s / (u + v),
                    displayAnswer(t);


                    //working out
                    suvtEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating time so the equation is rearranged to be: t = 2s / (u + v)";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of s, u & v to give t = (2*" + s + ") / (" + u + " + " + v + ")";
                    lastLineOfWorking(t);
                    if (u == -v)
                    {
                        warningBlock.Text = "Warning: Acceleration is constant in projectile motion, so if u = -v, displacement is always 0";
                        if (s == 0)
                        {
                            warningBlock.Text += Environment.NewLine + "This means that there are infinite solutions for time because acceleration could be any number and still satisfy your conditions";
                        }
                        else
                        {
                            warningBlock.Text += Environment.NewLine + "You have said that s = " + s + ". This is an impossible scenario so there is no solution";
                        }
                    }
                    else if (s == 0 && u == v)
                    {
                        warningBlock.Text = "Warning: The working out below doesn't produce any mathematical errors, but if s = 0, the magnitude of u = the magnitude of v";
                        warningBlock.Text += Environment.NewLine + "However |" + u + "| != |" + v + "| so this is an impossible scenario";
                        warningBlock.Text += Environment.NewLine + "For the velocity to change from " + u + " to " + v + ", there must be an infinite acceleration";
                        warningBlock.Text += Environment.NewLine + "The working out below therefore implies that t = 0 because if the acceleration was infinite, the initial speed would change to the final speed instantly";
                    }


                    //warning
                    if (t < 0)
                    {
                        warningBlock.Text = "Warning: You're negative answer indicates that you would have to go backwards in time to reach this scenario, which isn't possible, therefore there is no solution";
                    }



                }
                else if (!sEmpty && !uEmpty && vEmpty && !aEmpty) //equation without v
                {
                    double discriminant = Math.Pow(u, 2) + (2 * a * s);
                    //warning (inside of sqare root must be >= 0)
                    if (discriminant < 0)
                    {
                        warningBlock.Text = "Warning: this is an impossible scenario so there is no answer, make sure that u^2 + 2as >= 0, otherwise it can't be square rooted in the formula";
                    }
                    else 
                    {
                        if (a != 0)
                        {
                            t1 = (-u + Math.Sqrt(discriminant)) / a; //t1 = (-u + sqrt(u^2 + 2as)) / a
                            t2 = (-u - Math.Sqrt(discriminant)) / a; //t2 = (-u - sqrt(u^2 + 2as)) / a
                            displayAnswer(t1);
                            displayAnswer2(t2);
                        }

                        //warnings
                        if (t1 < 0 || t2 < 0)
                        {
                            warningBlock.Text = "Warning: You're negative answer(s) indicate that you would have to go backwards in time to reach this scenario, since this isn't possible, this isn't an answer, the only solutions are positive values of time";
                        }
                    }

                    //working out
                    suatEquation();
                    if (a != 0) //quadratic equation as coefficient of t^2 != 0
                    {
                        workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating time so the equation is rearranged to be: 0.5at^2 + ut - s = 0, (in the form ax^2 + bx + c = 0) because there's a t^2 and a t, so there are 2 solutions and this must be solved with the quadratic equation";
                        workingOut.Text += Environment.NewLine + Environment.NewLine + "The quadratic formula tells us that x = (-b +/- Sqrt(b^2 - 4ac)) / 2a which translates to t = (-u +/- Sqrt(u^2 + 2as)) / a where the '+/-' indicates 2 answers";
                        workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of s, u & a to give t = (" + -u + " + Sqrt(" + Math.Pow(u, 2) + " + 2*" + a + "*" + s + ")) / " + a + "        or        t = (" + -u + " - Sqrt(" + Math.Pow(u, 2) + " + 2*" + a + "*" + s + ")) / " + a;
                        if (discriminant < 0)
                        {
                            workingOut.Text += Environment.NewLine + Environment.NewLine + "The calcualtion requires you to use sqrt(" + discriminant + ") which doesn't produce a real number therefore there is no answer";
                        }
                        else
                        {
                            workingOut.Text += Environment.NewLine + Environment.NewLine + "The first calculation tells us that t = " + t1 + " and the second calculation tells us that t = " + t2;
                        }
                    }
                    else //linear equation as coefficient of t^2 = 0
                    {
                        workingOut.Text += Environment.NewLine + Environment.NewLine + "a = 0 so s = ut + 0.5at^2 becomes s = ut (distance = speed x time)";
                        workingOut.Text += Environment.NewLine + Environment.NewLine + "t = s / u  = " + s + " / " + u + " = " + (s / u);
                        t = s / u;
                        displayAnswer(t);
                    }
                }
                else if (!sEmpty && uEmpty && !vEmpty && !aEmpty) //equation without u
                {
                    double discriminant = Math.Pow(v, 2) - (2 * a * s);
                    //warning (inside of sqare root must be >= 0)
                    if (discriminant < 0)
                    {
                        warningBlock.Text = "Warning: this is an impossible scenario so there is no answer, make sure that v^2 - 2as >= 0, otherwise it can't be square rooted in the formula";
                    }
                    else
                    {
                        if (a != 0)
                        {
                            t1 = (v + Math.Sqrt(discriminant)) / a; //t1 = (v + sqrt(v^2 - 2as)) / a
                            t2 = (v - Math.Sqrt(discriminant)) / a; //t2 = (v + sqrt(v^2 - 2as)) / a
                            displayAnswer(t1);
                            displayAnswer2(t2);
                        }

                        //warnings
                        if (t1 < 0 || t2 < 0)
                        {
                            warningBlock.Text = "Warning: You're negative answer(s) indicate that you would have to go backwards in time to reach this scenario, since this isn't possible, this isn't an answer, the only solutions are positive values of time";
                        }
                    }

                    //working out
                    svatEquation();
                    if (a != 0) //quadratic equation as coefficient of t^2 != 0
                    {

                        workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating time so the equation is rearranged to be: 0.5at^2 - vt + s = 0, (in the form ax^2 + bx + c = 0) because there's a t^2 and a t, so there are 2 solutions and this must be solved with the quadratic equation";
                        workingOut.Text += Environment.NewLine + Environment.NewLine + "The quadratic formula tells us that x = (-b +/- Sqrt(b^2 - 4ac)) / 2a which translates to t = (v +/- Sqrt(v^2 - 2as)) / a where the '+/-' indicates 2 answers";
                        workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of s, v & a to give t = (" + v + " + Sqrt(" + Math.Pow(v, 2) + " - 2*" + a + "*" + s + ")) / " + a + "        or        t = (" + v + " - Sqrt(" + Math.Pow(v, 2) + " - 2*" + a + "*" + s + ")) / " + a;
                        if (discriminant < 0)
                        {
                            workingOut.Text += Environment.NewLine + Environment.NewLine + "The calcualtion requires you to use sqrt(" + discriminant + ") which doesn't produce a real number therefore there is no answer";
                        }
                        else
                        {
                            workingOut.Text += Environment.NewLine + Environment.NewLine + "The first calculation tells us that t = " + t1 + " and the second calculation tells us that t = " + t2;
                        }
                    }
                    else //linear eqation as coefficient of t^2 = 0
                    {
                        workingOut.Text += Environment.NewLine + Environment.NewLine + "a = 0 so s = vt - 0.5at^2 becomes s = vt (distance = speed x time)";
                        workingOut.Text += Environment.NewLine + Environment.NewLine + "t = s / v  = " + s + " / " + v + " = " + (s / v);
                        t = s / v;
                        displayAnswer(t);
                    }
                }
                else if (sEmpty && !uEmpty && !vEmpty && !aEmpty) //equation without s
                {
                    t = (v - u) / a;
                    displayAnswer(t);
                    
                    //warnings
                    if (a == 0)
                    {
                        if (v == u)
                        {
                            warningBlock.Text = "Warning: if a = 0, u will always = v, so there are infinite solutions (this scenario occurs for all values of t)";
                        }
                        else
                        {
                            warningBlock.Text = "Warning: If a = 0, u should always = v, because u will never change. The scenario you have inputted doesn't follow this because " + u + " != " + v + " hence there is no solution because this is an impossible scenario";
                        }
                    }
                    if (t < 0)
                    {
                        warningBlock.Text += "\n\nWarning: You're negative answer indicates that you would have to go backwards in time to reach this scenario, which isn't possible, therefore there is no solution";
                    }

                    //working out
                    uvatEquation();
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "We are calculating time so the equation is rearranged to be: t = (v - u) / a";
                    workingOut.Text += Environment.NewLine + Environment.NewLine + "Next, we substitute values of u, v & a to give t = (" + v + " - " + u + ") / " + a;
                    lastLineOfWorking(t);
                }
                else // more/less than 3 other variables have been entered
                {
                    warningBlock.Text = "Warning: please enter exactly 3 variables";
                    answer1.Content = "Answer = ";
                }
            }

            else
            {
                //no box has been ticked
                warningBlock.Text = "Warning: Please tick a box";
            }
        }
    }
}