using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

using System.Windows.Threading;   // for timer

namespace Runner_4J
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();

        // hit boxes
        Rect playerHitBox;
        Rect groundHitBox;
        Rect obstacleHitBox;

        bool jumping;
        int force = 20;
        int speed = 5;
        int score = 0;

        Random rand = new Random();

        bool gameover = false;

        double spriteInt = 0;

        // Sprites
        ImageBrush playerSprite = new ImageBrush();
        ImageBrush backgroundSprite = new ImageBrush();
        ImageBrush obstacleSprite = new ImageBrush();

        // array positions
        int[] obstaclePosition = {320, 310, 300, 305, 315 };



        public MainWindow()
        {
            InitializeComponent();

            myCanvas.Focus();

            gameTimer.Tick += GameEngine;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);

            backgroundSprite.ImageSource = new BitmapImage(
                new Uri("pack://application:,,,/images/background.gif"));

            background.Fill = backgroundSprite;
            background2.Fill = backgroundSprite;

            StartGame();
        }

        private void myCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && gameover)
            {
                // reset all
                score = 0;
                Canvas.SetLeft(obstacle, 1000);
                scoreText.Content = "Score: " + score.ToString();
                gameover = false;


                StartGame();
            }
        }

        private void myCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && !jumping && Canvas.GetTop(player) > 250)
            {
                jumping = true;
                force = 15;
                speed = -10;

                playerSprite.ImageSource = new BitmapImage(
                new Uri("pack://application:,,,/images/pic2.gif"));
            }
        }

        private void StartGame()
        {
            RunSprite(1);

            obstacleSprite.ImageSource = new BitmapImage(
                new Uri("pack://application:,,,/images/obstacle.png"));

            obstacle.Fill = obstacleSprite;

            gameTimer.Start();
        }

        private void RunSprite(double index)
        {
            if ((int)index > 0 && (int)index <= 8)
            {
                playerSprite.ImageSource = new BitmapImage(
                new Uri("pack://application:,,,/images/pic" + (int)index + ".gif"));

                player.Fill = playerSprite;
            }
        }

        private void GameEngine(object sender, EventArgs e)
        {
            Canvas.SetTop(player, Canvas.GetTop(player) + speed);

            Canvas.SetLeft(background, Canvas.GetLeft(background) - 3);
            Canvas.SetLeft(background2, Canvas.GetLeft(background2) - 3);
            Canvas.SetLeft(obstacle, Canvas.GetLeft(obstacle) - 6);

            scoreText.Content = "Score: " + score;

            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player),
                player.Width, player.Height);

            groundHitBox = new Rect(Canvas.GetLeft(ground), Canvas.GetTop(ground),
                ground.Width, ground.Height);

            obstacleHitBox = new Rect(Canvas.GetLeft(obstacle), Canvas.GetTop(obstacle),
                obstacle.Width, obstacle.Height);

            if (playerHitBox.IntersectsWith(groundHitBox))
            {
                speed = 0;

                Canvas.SetTop(player, Canvas.GetTop(ground) - player.Height);

                jumping = false;

                spriteInt += 0.5;

                if (spriteInt > 8)
                {
                    spriteInt = 1;
                }

                RunSprite(spriteInt);
            }

            
            if (playerHitBox.IntersectsWith(obstacleHitBox))
            {
                gameover = true;
                gameTimer.Stop();
            }

            // jumping
            if (jumping)
            {
                force--;
                speed = -9;
            }
            else
            {
                speed = 12;
            }

            if (force < 0)
            {
                jumping = false;
            }

            // background
            if (Canvas.GetLeft(background) < -1262)
            {
                Canvas.SetLeft(background, Canvas.GetLeft(background2) + background2.Width);
            }

            if (Canvas.GetLeft(background2) < -1262)
            {
                Canvas.SetLeft(background2, Canvas.GetLeft(background) + background.Width);
            }

            // obtacles
            if (Canvas.GetLeft(obstacle) < -50)
            {
                Canvas.SetLeft(obstacle, 1000);   // random
                Canvas.SetTop(obstacle, obstaclePosition[rand.Next(0, obstaclePosition.Length)]);

                score++;
            }

            // gameover
            if (gameover)
            {
                scoreText.Content = "Press Enter to Start game again";

                obstacle.Stroke = Brushes.Black;
                obstacle.StrokeThickness = 2;

                player.Stroke = Brushes.Red;
                player.StrokeThickness = 2;
            }
            else
            {
                obstacle.StrokeThickness = 0;
                player.StrokeThickness = 0;
            }
        }
    }
}
