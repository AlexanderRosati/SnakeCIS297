﻿using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Microsoft.Graphics.Canvas.Text;
using Windows.Media.Playback;
using Windows.Media.Core;
using Windows.Gaming.Input;
using Microsoft.Graphics.Canvas;
using System.Threading;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
// This comment is pointless

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Game : Page
    {
        private Snake snake;

        private MediaPlayer backgroundMusic;
        private MediaPlayer duckTalesSongPlayer;
        private MediaPlayer GTOSongPlayer;
        private MediaPlayer OneMoreLineSongPlayer;
        private MediaPlayer gameoverSoundEffect;
        private MediaPlayer thankYouSoundEffect;
        private MediaPlayer iGotItSoundEffect;

        

        private Gamepad controller;

        menuSelector menuSelector;
        menuSelector_Settings menuSelector_Settings;

        private int gameOverCounter;
        private int turnCounter;
        private int loadCounter;
        private int thankYouSoundEffectCounter;
        private int delayForDPad;

        private bool cantChangeDirection;
        private bool turningLeft;
        private bool turningRight;
        private bool turningDown;
        private bool turningUp;
        private bool gameOver;
        private bool gameIsRunning;
        private bool canTurn;
        private bool startPageDisplaying;
        private bool changeToDuckTales;
        private bool changeToGTO;
        private bool changeToOneMoreLine;
        private bool settingsPageDisplaying;
        private bool IsColorColumn; // For Color Column
        private bool dpadDownIsPressed;
        private bool dpadUpIsPressed;
        private bool scoreScreen;
        private bool resetTheGame;


        private bool howToPlayDisplaying;
        private bool loading;
        private bool credits;

        private Color currentColor;

        public Game()
        {
            this.InitializeComponent();
            snake = new Snake(Colors.DarkOrange, Colors.Black);
            currentColor = Colors.DarkOrange;
            //Add method to keydown event
            Window.Current.CoreWindow.KeyDown += Canvas_KeyDown;
            cantChangeDirection = false; //prevents key down event from firing off twice
            turningLeft = false;
            turningRight = false;
            turningDown = false;
            turningUp = false;
            gameOver = false;
            gameIsRunning = false;
            canTurn = true; //Serves different purpose than cantChangeDirection
            startPageDisplaying = false;
            settingsPageDisplaying = false;
            IsColorColumn = true;
            howToPlayDisplaying = false;
            loading = true;
            credits = false;
            changeToDuckTales = false;
            changeToOneMoreLine = false;
            changeToGTO = false;
            dpadDownIsPressed = false;
            dpadUpIsPressed = false;
            scoreScreen = false;
            resetTheGame = false;


            /*@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
            gameOverCounter = 0;
            turnCounter = 0;
            thankYouSoundEffectCounter = 0;
            delayForDPad = 0;

            //Set width and height of window
            ApplicationView.PreferredLaunchViewSize = new Size(600, 400);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            //Start background music
            //Source: https://www.youtube.com/watch?v=2UuHesrJROw
            duckTalesSongPlayer = new MediaPlayer();
            duckTalesSongPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/duck_tales_music.wav"));
            duckTalesSongPlayer.Volume = 0.06;
            duckTalesSongPlayer.MediaEnded += resetDuckTalesSong;
            duckTalesSongPlayer.Play();
            backgroundMusic = duckTalesSongPlayer;

            //Source: https://www.youtube.com/watch?v=Uc6vF1PWdFY
            GTOSongPlayer = new MediaPlayer();
            GTOSongPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/gto_theme.wav"));
            GTOSongPlayer.Volume = 0.06;
            GTOSongPlayer.MediaEnded += resetGTOSong;

            //Source: https://www.youtube.com/watch?v=lYldC9K3hrg
            OneMoreLineSongPlayer = new MediaPlayer();
            OneMoreLineSongPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/one_more_line_song.wav"));
            OneMoreLineSongPlayer.Volume = 0.06;
            OneMoreLineSongPlayer.MediaEnded += resetOneMoreLineSong;

            //Gameover sound effect
            //Source: https://themushroomkingdom.net/media/sm64/wav
            gameoverSoundEffect = new MediaPlayer();
            gameoverSoundEffect.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sm64_mario_game_over.wav"));

            //Thank you sound effect
            //Source: https://themushroomkingdom.net/media/sm64/wav
            thankYouSoundEffect = new MediaPlayer();
            thankYouSoundEffect.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sm64_mario_thank_you.wav"));

            //I Got It Sound Effect
            //Source: http://www.realmofdarkness.net/sb/category/mp/mp3/
            iGotItSoundEffect = new MediaPlayer();
            iGotItSoundEffect.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/i_got_it.wav"));

            menuSelector = new menuSelector();
            menuSelector_Settings = new menuSelector_Settings();
        }

        private void resetDuckTalesSong(MediaPlayer sender, object args)
        {
            duckTalesSongPlayer.PlaybackSession.Position = TimeSpan.FromSeconds(0);
            duckTalesSongPlayer.Play();
        }

        private void resetGTOSong(MediaPlayer sender, object args)
        {
            GTOSongPlayer.PlaybackSession.Position = TimeSpan.FromSeconds(0);
            GTOSongPlayer.Play();
        }

        private void resetOneMoreLineSong(MediaPlayer sender, object args)
        {
            OneMoreLineSongPlayer.PlaybackSession.Position = TimeSpan.FromSeconds(0);
            OneMoreLineSongPlayer.Play();
        }

        private void canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            //Draw Black Background
            Rect rect = new Rect();
            rect.X = 0;
            rect.Y = 0;
            rect.Width = 600;
            rect.Height = 400;

            args.DrawingSession.DrawRectangle(rect, snake.foregroundColor);
            args.DrawingSession.FillRectangle(rect, snake.backgroundColor);

            if (gameIsRunning)
            {
                Rect scoreBoard = new Rect();
                scoreBoard.X = 401;
                scoreBoard.Y = 0;
                scoreBoard.Width = 199;
                scoreBoard.Height = 400;
                args.DrawingSession.DrawRectangle(scoreBoard, snake.foregroundColor);
                args.DrawingSession.FillRectangle(scoreBoard, snake.foregroundColor);

                Rect playerScoreRec = new Rect();
                playerScoreRec.X = 441;
                playerScoreRec.Y = 181;
                playerScoreRec.Width = 160;
                playerScoreRec.Height = 40;

                CanvasTextFormat textFormatOfScoreText = new CanvasTextFormat()
                {
                    FontFamily = "Courier New",
                    FontSize = 72
                };

                args.DrawingSession.DrawText($"{snake.playerScore}", playerScoreRec, snake.backgroundColor, textFormatOfScoreText);

                //Draw Game
                snake.drawGame(args.DrawingSession);
            }          

            else if (gameOver)
            {
                Rect locOfGameOverText = new Rect();
                locOfGameOverText.X = 200;
                locOfGameOverText.Y = 125;
                locOfGameOverText.Width = 200;
                locOfGameOverText.Height = 200;

                CanvasTextFormat textFormatOfGameOverText = new CanvasTextFormat()
                {
                    FontFamily = "Courier New",
                    FontSize = 72
                };

                args.DrawingSession.DrawText("GAME OVER", locOfGameOverText, Colors.White, textFormatOfGameOverText);
            }

            else if (startPageDisplaying)
            {
                Rect titleRec = new Rect();
                titleRec.X = 25;
                titleRec.Y = 20;
                titleRec.Width = 400;
                titleRec.Height = 100;

                Rect selectionText = new Rect();
                selectionText.X = 60;
                selectionText.Y = 200;
                selectionText.Width = 400;
                selectionText.Height = 200;

                Rect imageRec = new Rect();
                imageRec.X = 300;
                imageRec.Y = 40;
                imageRec.Width = 216;
                imageRec.Height = 216;

                CanvasTextFormat textFormatOfTitleText = new CanvasTextFormat()
                {
                    FontFamily = "Courier New",
                    FontSize = 72
                };

                CanvasTextFormat textFormatOfSelectionText = new CanvasTextFormat()
                {
                    FontFamily = "Courier New",
                    FontSize = 44
                };

                string selectionString = "PLAY!!!\nSETTINGS!!!\nHOW TO PLAY???\nCREDITS!!!\n";
                args.DrawingSession.DrawText("SNAKE!!!", titleRec, Colors.White, textFormatOfTitleText);
                args.DrawingSession.DrawText(selectionString, selectionText, Colors.White, textFormatOfSelectionText);
                menuSelector.draw(args.DrawingSession);
            }

            else if(settingsPageDisplaying)
            {
                Rect SettingsTitleRec = new Rect();
                SettingsTitleRec.X = 25;
                SettingsTitleRec.Y = 10;
                SettingsTitleRec.Width = 200;
                SettingsTitleRec.Height = 50;

                Rect selectionTextColor = new Rect();
                selectionTextColor.X = 60;
                selectionTextColor.Y = 100;
                selectionTextColor.Width = 400;
                selectionTextColor.Height = 200;

                Rect selectionTextMusic = new Rect();
                selectionTextMusic.X = 380;
                selectionTextMusic.Y = 100;
                selectionTextMusic.Width = 400;
                selectionTextMusic.Height = 200;

                Rect ExitSign = new Rect();
                ExitSign.X = 25;
                ExitSign.Y = 340;
                ExitSign.Width = 550;
                ExitSign.Height = 200;

                Rect instructions = new Rect();
                instructions.X = 25;
                instructions.Y = 55;
                instructions.Width = 550;
                instructions.Height = 60;

                string select_Colors = "DARK ORANGE \n\nGREEN \n\nCYAN \n\nHOT PINK\n\n";
                string select_Music = "SONG 1\n\nSONG 2\n\nSONG 3\n\n";

                CanvasTextFormat SettingsFormatOfTitleText = new CanvasTextFormat()
                {
                    FontFamily = "Courier New",
                    FontSize = 25
                };

                args.DrawingSession.DrawText("SETTINGS!!!", SettingsTitleRec, Colors.White, SettingsFormatOfTitleText);
                args.DrawingSession.DrawText(select_Colors, selectionTextColor, Colors.White, SettingsFormatOfTitleText);
                args.DrawingSession.DrawText(select_Music, selectionTextMusic, Colors.White, SettingsFormatOfTitleText);
                args.DrawingSession.DrawText("PRESS ENTER OR X TO GO TO START MENU", ExitSign, Colors.White, SettingsFormatOfTitleText);
                menuSelector_Settings.draw(args.DrawingSession);
                args.DrawingSession.DrawText("HIT LEFT AND RIGHT TO SWITCH COLUMNS", instructions, Colors.White, SettingsFormatOfTitleText);
            }

            else if (scoreScreen)
            {
                Rect scoreScreenRec = new Rect();
                scoreScreenRec.X = 20;
                scoreScreenRec.Y = 20;
                scoreScreenRec.Width = 570;
                scoreScreenRec.Height = 570;

                CanvasTextFormat scoreScreenFormat = new CanvasTextFormat()
                {
                    FontFamily = "Courier New",
                    FontSize = 30
                };

                string scoreScreenText = "GAMING HEROS\n\nMARIO ....... 59\nYOSHI ....... 49\nLINK ........ 39\n\n" +
                                         "YOU ......... " + snake.playerScore + "\n\n";

                if (snake.playerScore > 59)
                {
                    scoreScreenText += "YOU'RE A TRUE GAMING HERO!!!\n\n";
                }

                else
                {
                    scoreScreenText += "YOU SUCK! GET BETTER, SCRUB!\n\n";
                }

                scoreScreenText += "PRESS ENTER OR X TO GO TO START\n";

                args.DrawingSession.DrawText(scoreScreenText, scoreScreenRec, Colors.White, scoreScreenFormat);
            }

            else if(howToPlayDisplaying)
            {
                Rect howToPlayRect = new Rect();
                    howToPlayRect.X = 25;
                    howToPlayRect.Y = 25;
                    howToPlayRect.Width = 550;
                    howToPlayRect.Height = 300;

                CanvasTextFormat rectFormat = new CanvasTextFormat()
                {
                    FontFamily = "Courier New",
                    FontSize = 24
                };

                args.DrawingSession.DrawText("HOW TO PLAY XBOX/WINDOWS:\n\n" +
                                             "D-PAD UP/UP ARROW = GO UP\n" +
                                             "D-PAD DOWN/DOWN ARROW = GO DOWN\n" +
                                             "D-PAD LEFT/LEFT ARROW = GO LEFT\n" +
                                             "D-PAD RIGHT/RIGHT ARROW = GO RIGHT\n" +
                                             "ENTER = SELECT/BACK/ADVANCE\n" +
                                             "A BUTTON = SELECT\n" +
                                             "X BUTTON = BACK\n\n" +
                                             "PRESS ENTER OR X TO GO TO START MENU", howToPlayRect, Colors.White, rectFormat);
            }

            else if(loading)
            {
                Rect locOfLoadText = new Rect();
                locOfLoadText.X = 125;
                locOfLoadText.Y = 150;
                locOfLoadText.Width = 400;
                locOfLoadText.Height = 200;

                CanvasTextFormat textFormatOfLoadText = new CanvasTextFormat()
                {
                    FontFamily = "Courier New",
                    FontSize = 56,
                    FontStyle = Windows.UI.Text.FontStyle.Italic
                };

                args.DrawingSession.DrawText("LOADING...", locOfLoadText, Colors.White, textFormatOfLoadText);
            }

            else if (credits)
            {
                Rect creditsRec = new Rect();
                creditsRec.Y = 20;
                creditsRec.X = 20;
                creditsRec.Width = 600;
                creditsRec.Height = 400;

                CanvasTextFormat textFormatOfCredits = new CanvasTextFormat()
                {
                    FontFamily = "Courier New",
                    FontSize = 32
                };

                string creditsString = "THE CREW:\n\nALEX ROSATI (SUPER COOL)\nAVIAN CALADO (SLEEPY)\nPETER SCHUBERT (COOL)\nNISARG"
                                        + " PATEL (COOL) \n\nPRESS ENTER OR X TO GO BACK";

                args.DrawingSession.DrawText(creditsString, creditsRec, Colors.White, textFormatOfCredits);
            }
        }

        private void changeMusic()
        {
            if (changeToDuckTales)
            {
                backgroundMusic.Pause();
                backgroundMusic = duckTalesSongPlayer;
                backgroundMusic.Play();
                changeToDuckTales = false;
            }

            else if (changeToGTO)
            {
                backgroundMusic.Pause();
                backgroundMusic = GTOSongPlayer;
                backgroundMusic.Play();
                changeToGTO = false;
            }

            else if (changeToOneMoreLine)
            {
                backgroundMusic.Pause();
                backgroundMusic = OneMoreLineSongPlayer;
                backgroundMusic.Play();
                changeToOneMoreLine = false;
            }
        }

        private void canvas_Update(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
            changeMusic();

           if(gameIsRunning)
           {
               controllerGameLogic();

                if (resetTheGame)
                {
                    snake.resetGame(currentColor);
                    resetTheGame = false;
                }

                if (!canTurn)
               {
                    ++turnCounter;

                    if(turnCounter == 6)
                    {
                        canTurn = true;
                        turnCounter = 0;
                    }
               }

               updateGame();
           }

           else if (gameOver)
           {
                ++gameOverCounter;

                if (gameOverCounter == 1)
                {
                    gameoverSoundEffect.Play();
                }

                if (gameOverCounter == 360)
                {
                    gameOver = false;
                    scoreScreen = true;
                    resetTheGame = true;
                    gameOverCounter = 0;

                    if (snake.playerScore > 59)
                    {
                        iGotItSoundEffect.Play();
                    }
                }
           }

           else if(startPageDisplaying)
           {
                gameControllerLogic_StartMenu_SettingsMenu();
           }

           else if(settingsPageDisplaying)
           {
                gameControllerLogic_StartMenu_SettingsMenu();
           }

           else if(howToPlayDisplaying)
           {
                gameControllerLogic_StartMenu_SettingsMenu();
           }

           else if(loading)
           {
                ++loadCounter;

                if(loadCounter == 80)
                {
                    loading = false;
                    howToPlayDisplaying = true;
                }
           }

           else if (credits)
           {
                gameControllerLogic_StartMenu_SettingsMenu();
           }

           else if (scoreScreen && Gamepad.Gamepads.Count > 0)
           {
                Gamepad firstController = Gamepad.Gamepads.First();
                GamepadReading reading = firstController.GetCurrentReading();

                if (reading.Buttons.HasFlag(GamepadButtons.X) && scoreScreen)
                {
                    scoreScreen = false;
                    startPageDisplaying = true;
                }
            }
        }

        private void updateGame()
        {
            snake.updateGame();

            if (turningLeft)
            {
                //Put a cover over turn
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l, snake.foregroundColor));

                //Change direction
                snake.snakeHead.goingLeft = true;
                snake.snakeHead.goingRight = false;
                snake.snakeHead.goingDown = false;
                snake.snakeHead.goingUp = false;

                int bodySegmentNumber = 1;

                //Number of blocks lined up with snake head when it turns
                int numberOfBlocksInLineWithHead = snake.snakeHead.distanceSinceLastTurn / 20;

                //Calcuate how far each body segment should travel and which direction it should go
                //When it gets there
                for (int i = 0; i < snake.bodySegments.Count; ++i)
                {
                    if (bodySegmentNumber <= numberOfBlocksInLineWithHead)
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(bodySegmentNumber * 20);
                        snake.bodySegments[i].waysToTurn.Enqueue(direction.L);
                        ++bodySegmentNumber;
                    }

                    else
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(snake.snakeHead.distanceSinceLastTurn);
                        snake.bodySegments[i].waysToTurn.Enqueue(direction.L);
                    }
                }

                //Reset distanceSinceLastTurn since we just turned
                snake.snakeHead.distanceSinceLastTurn = 0;


                turningLeft = false;
            }

            else if (turningRight)
            {
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l, snake.foregroundColor));

                snake.snakeHead.goingLeft = false;
                snake.snakeHead.goingRight = true;
                snake.snakeHead.goingDown = false;
                snake.snakeHead.goingUp = false;

                int bodySegmentNumber = 1;
                int numberOfBlocksInLineWithHead = snake.snakeHead.distanceSinceLastTurn / 20;

                for (int i = 0; i < snake.bodySegments.Count; ++i)
                {
                    if (bodySegmentNumber <= numberOfBlocksInLineWithHead)
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(bodySegmentNumber * 20);
                        snake.bodySegments[i].waysToTurn.Enqueue(direction.R);
                        ++bodySegmentNumber;
                    }

                    else
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(snake.snakeHead.distanceSinceLastTurn);
                        snake.bodySegments[i].waysToTurn.Enqueue(direction.R);
                    }
                }

                snake.snakeHead.distanceSinceLastTurn = 0;
                turningRight = false;
            }

            else if (turningDown)
            {
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l, snake.foregroundColor));
         
                snake.snakeHead.goingLeft = false;
                snake.snakeHead.goingRight = false;
                snake.snakeHead.goingDown = true;
                snake.snakeHead.goingUp = false;
         
                int bodySegmentNumber = 1;
                int numberOfBlocksInLineWithHead = snake.snakeHead.distanceSinceLastTurn / 20;
         
                for (int i = 0; i < snake.bodySegments.Count; ++i)
                {
                   if (bodySegmentNumber <= numberOfBlocksInLineWithHead)
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(bodySegmentNumber * 20);
                        snake.bodySegments[i].waysToTurn.Enqueue(direction.D);
                        ++bodySegmentNumber;
                    }

                    else
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(snake.snakeHead.distanceSinceLastTurn);
                        snake.bodySegments[i].waysToTurn.Enqueue(direction.D);
                    }
                }

                snake.snakeHead.distanceSinceLastTurn = 0;
                turningDown = false;
            }

            else if (turningUp)
            {
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l, snake.foregroundColor));

                snake.snakeHead.goingLeft = false;
                snake.snakeHead.goingRight = false;
                snake.snakeHead.goingDown = false;
                snake.snakeHead.goingUp = true;

                int bodySegmentNumber = 1;
                int numberOfBlocksInLineWithHead = snake.snakeHead.distanceSinceLastTurn / 20;

                for (int i = 0; i < snake.bodySegments.Count; ++i)
                {
                    if (bodySegmentNumber <= numberOfBlocksInLineWithHead)
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(bodySegmentNumber * 20);
                        snake.bodySegments[i].waysToTurn.Enqueue(direction.U);
                        ++bodySegmentNumber;
                    }

                    else
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(snake.snakeHead.distanceSinceLastTurn);
                        snake.bodySegments[i].waysToTurn.Enqueue(direction.U);
                    }
                }

                snake.snakeHead.distanceSinceLastTurn = 0;
                turningUp = false;
            }

            //Game ends if player hits themself
            if (snake.playerRanIntoThemself())
            {
                gameOver = true;
                gameIsRunning = false;
            }

            //Game ends if snake hits edge of window
            if (snake.snakeHead.x == 0 || snake.snakeHead.x == 380 ||
                snake.snakeHead.y == 0 || snake.snakeHead.y == 380)
            {
                gameOver = true;
                gameIsRunning = false;
            }
        }

        //Runs when key is pressed down.
        private void Canvas_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            bool startedAtStartPage = startPageDisplaying;

            //When game is running
            //Left key is pressed when the user was not already going left or right
            if (e.VirtualKey == Windows.System.VirtualKey.Left && !snake.snakeHead.goingRight && !snake.snakeHead.goingLeft 
                && !cantChangeDirection&& gameIsRunning && canTurn)
            {
                cantChangeDirection = true;
                turningLeft = true;
                canTurn = false;
                cantChangeDirection = false;
            }

            //Right key is pressed when we are not going right or left
            else if (e.VirtualKey == Windows.System.VirtualKey.Right && !snake.snakeHead.goingLeft && !snake.snakeHead.goingRight 
                && !cantChangeDirection && gameIsRunning && canTurn)
            {
                cantChangeDirection = true;
                turningRight = true;
                canTurn = false;
                cantChangeDirection = false;
            }

            //Down key is pressed when we are not going up or down
            else if (e.VirtualKey == Windows.System.VirtualKey.Down && !snake.snakeHead.goingUp && !snake.snakeHead.goingDown 
                && !cantChangeDirection && gameIsRunning && canTurn)
            {
                cantChangeDirection = true;
                turningDown = true;
                canTurn = false;
                cantChangeDirection = false;
            }

            //Up key is pressed when we are not going up or down
            else if (e.VirtualKey == Windows.System.VirtualKey.Up && !snake.snakeHead.goingDown && !snake.snakeHead.goingUp 
                && !cantChangeDirection && gameIsRunning && canTurn)
            {
                cantChangeDirection = true;
                turningUp = true;
                canTurn = false;
                cantChangeDirection = false;
            }

            if(startPageDisplaying)
            {
                if(e.VirtualKey == Windows.System.VirtualKey.Up)
                {
                    menuSelector.moveUp();
                }

                else if (e.VirtualKey == Windows.System.VirtualKey.Down)
                {
                    menuSelector.moveDown();
                }

                if (e.VirtualKey == Windows.System.VirtualKey.Enter)
                {
                    if (menuSelector.selection == startPageSelection.Play)
                    {
                        startPageDisplaying = false;
                        gameIsRunning = true;
                    }

                    else if (menuSelector.selection == startPageSelection.Settings)
                    {
                        startPageDisplaying = false;
                        settingsPageDisplaying = true;
                    }

                    else if (menuSelector.selection == startPageSelection.HowToPlay)
                    {
                        startPageDisplaying = false;
                        howToPlayDisplaying = true;
                    }

                    else if (menuSelector.selection == startPageSelection.Credits)
                    {
                        startPageDisplaying = false;
                        credits = true;
                        thankYouSoundEffect.Play();
                    }
                }
            }
            // Peter's Part 
            if (settingsPageDisplaying)
            {
                
                if (e.VirtualKey == Windows.System.VirtualKey.Up && IsColorColumn )
                {
                    menuSelector_Settings.moveUp_Color();
                }

                else if (e.VirtualKey == Windows.System.VirtualKey.Down && IsColorColumn)
                {
                    menuSelector_Settings.moveDown_Color();
                }
                else if(e.VirtualKey == Windows.System.VirtualKey.Right && IsColorColumn)
                {
                    IsColorColumn = false;
                }
                else if (e.VirtualKey == Windows.System.VirtualKey.Left && !IsColorColumn)
                {
                    IsColorColumn = true;
                }
                else if(e.VirtualKey==Windows.System.VirtualKey.Up && !IsColorColumn)
                {
                    menuSelector_Settings.moveUp_Music();
                }
                else if (e.VirtualKey == Windows.System.VirtualKey.Down && !IsColorColumn)
                {
                    menuSelector_Settings.moveDown_Music();
                }

            }

            //Go to start menu from how to play menu
            if (!startedAtStartPage && howToPlayDisplaying && e.VirtualKey == Windows.System.VirtualKey.Enter)
            {
                howToPlayDisplaying = false;
                startPageDisplaying = true;
                menuSelector = new menuSelector();
            }

            //Go back to start menu from credits
            if (!startedAtStartPage && credits && e.VirtualKey == Windows.System.VirtualKey.Enter)
            {
                credits = false;
                startPageDisplaying = true;
                menuSelector = new menuSelector();
            }

            //Go back to start menu from settings page
            if (!startedAtStartPage && settingsPageDisplaying && e.VirtualKey == Windows.System.VirtualKey.Enter)
            {
                if(menuSelector_Settings.selection_color == ColorSelection.DarkOrange)
                {
                    currentColor = Colors.DarkOrange;
                }
                else if (menuSelector_Settings.selection_color == ColorSelection.Cyan)
                {
                    currentColor = Colors.Cyan;
                }
                else if (menuSelector_Settings.selection_color == ColorSelection.Green)
                {
                    currentColor = Colors.Green;
                }
                else if (menuSelector_Settings.selection_color == ColorSelection.HotPink)
                {
                    currentColor = Colors.HotPink;
                }
                snake.resetGame(currentColor);


                // Menu

                if(menuSelector_Settings.selection_music == MusicSelection.Song1)
                {
                    changeToDuckTales = true;


                }
                else if (menuSelector_Settings.selection_music == MusicSelection.Song2)
                {
                    // One more line
                    changeToOneMoreLine = true;


                }
                else if (menuSelector_Settings.selection_music== MusicSelection.Song3)
                {
                    // GTO theme
                    // source   https://www.youtube.com/watch?v=Uc6vF1PWdFY 
                    changeToGTO = true;


                }
                settingsPageDisplaying = false;
                startPageDisplaying = true;
                menuSelector = new menuSelector();
            }

            if (e.VirtualKey == Windows.System.VirtualKey.Enter && scoreScreen)
            {
                scoreScreen = false;
                startPageDisplaying = true;
            }
        }

        //Makes Snake Turn when d-pad on controller is pressed
        private void controllerGameLogic()
        {
            if (Gamepad.Gamepads.Count > 0)
            {
                controller = Gamepad.Gamepads.First();
                GamepadReading reading = controller.GetCurrentReading();

                if (reading.Buttons.HasFlag(GamepadButtons.DPadLeft) && !snake.snakeHead.goingRight && !snake.snakeHead.goingLeft
                && canTurn)
                {
                    turningLeft = true;
                    canTurn = false;
                }

                else if (reading.Buttons.HasFlag(GamepadButtons.DPadRight) && !snake.snakeHead.goingLeft && !snake.snakeHead.goingRight
                && canTurn)
                {
                    turningRight = true;
                    canTurn = false;
                }

                else if (reading.Buttons.HasFlag(GamepadButtons.DPadDown) && !snake.snakeHead.goingUp && !snake.snakeHead.goingDown
                && canTurn)
                {
                    turningDown = true;
                    canTurn = false;
                }

                else if (reading.Buttons.HasFlag(GamepadButtons.DPadUp) && !snake.snakeHead.goingDown && !snake.snakeHead.goingUp
                && canTurn)
                {
                    turningUp = true;
                    canTurn = false;
                }
            }
        }

        private void gameControllerLogic_StartMenu_SettingsMenu()
        {
            bool startedAtStartPage = startPageDisplaying;
            if (Gamepad.Gamepads.Count > 0) {
                controller = Gamepad.Gamepads.First();
                GamepadReading reading = controller.GetCurrentReading();

                if (startPageDisplaying)
                {

                    if (reading.Buttons.HasFlag(GamepadButtons.DPadDown) && !dpadDownIsPressed)
                    {
                        menuSelector.moveDown();
                        dpadDownIsPressed = true;
                    }

                    else if (!reading.Buttons.HasFlag(GamepadButtons.DPadDown) && dpadDownIsPressed)
                    {
                        dpadDownIsPressed = false;
                    }

                    if (reading.Buttons.HasFlag(GamepadButtons.DPadUp) && !dpadUpIsPressed)
                    {
                        menuSelector.moveUp();
                        dpadUpIsPressed = true;
                    }

                    else if (!reading.Buttons.HasFlag(GamepadButtons.DPadUp) && dpadUpIsPressed)
                    {
                        dpadUpIsPressed = false;
                    }


                    if (reading.Buttons.HasFlag(GamepadButtons.A))
                    {
                        if (menuSelector.selection == startPageSelection.Play)
                        {
                            startPageDisplaying = false;
                            gameIsRunning = true;
                        }

                        else if (menuSelector.selection == startPageSelection.Settings)
                        {
                            startPageDisplaying = false;
                            settingsPageDisplaying = true;
                        }

                        else if (menuSelector.selection == startPageSelection.HowToPlay)
                        {
                            startPageDisplaying = false;
                            howToPlayDisplaying = true;
                        }

                        else if (menuSelector.selection == startPageSelection.Credits)
                        {
                            startPageDisplaying = false;
                            credits = true;
                            thankYouSoundEffect.Play();
                        }
                    }
                }
                
                if (settingsPageDisplaying)
                {
                    if (reading.Buttons.HasFlag(GamepadButtons.DPadUp) && IsColorColumn && !dpadUpIsPressed)
                    {
                        menuSelector_Settings.moveUp_Color();
                        dpadUpIsPressed = true;
                    }

                    else if (!reading.Buttons.HasFlag(GamepadButtons.DPadUp) && IsColorColumn && dpadUpIsPressed)
                    {
                        dpadUpIsPressed = false;
                    }

                    if (reading.Buttons.HasFlag(GamepadButtons.DPadDown) && IsColorColumn && !dpadDownIsPressed)
                    {
                        menuSelector_Settings.moveDown_Color();
                        dpadDownIsPressed = true;
                    }

                    else if (!reading.Buttons.HasFlag(GamepadButtons.DPadDown) && IsColorColumn && dpadDownIsPressed)
                    {
                        dpadDownIsPressed = false;
                    }

                   if (reading.Buttons.HasFlag(GamepadButtons.DPadRight) && IsColorColumn)
                   {
                        IsColorColumn = false;
                   }

                    if (reading.Buttons.HasFlag(GamepadButtons.DPadLeft) && !IsColorColumn)
                    {
                         IsColorColumn = true;
                    }

                    if (reading.Buttons.HasFlag(GamepadButtons.DPadUp) && !IsColorColumn && !dpadUpIsPressed)
                    {
                        menuSelector_Settings.moveUp_Music();
                        dpadUpIsPressed = true;
                    }

                    else if (!reading.Buttons.HasFlag(GamepadButtons.DPadUp) && !IsColorColumn && dpadUpIsPressed)
                    {
                        dpadUpIsPressed = false;
                    }

                    if (reading.Buttons.HasFlag(GamepadButtons.DPadDown) && !IsColorColumn && !dpadDownIsPressed)
                    {
                        menuSelector_Settings.moveDown_Music();
                        dpadDownIsPressed = true;
                    }

                    else if (!reading.Buttons.HasFlag(GamepadButtons.DPadDown) && !IsColorColumn && dpadDownIsPressed)
                    {
                        dpadDownIsPressed = false;
                    }

                }
                //Go to start menu from how to play menu
                if (!startedAtStartPage && howToPlayDisplaying && reading.Buttons.HasFlag(GamepadButtons.X))
                {
                    howToPlayDisplaying = false;
                    startPageDisplaying = true;
                    menuSelector = new menuSelector();
                }

                //Go back to start menu from credits
                if (!startedAtStartPage && credits && reading.Buttons.HasFlag(GamepadButtons.X))
                {
                    credits = false;
                    startPageDisplaying = true;
                    menuSelector = new menuSelector();
                }

                //Go back to start menu from settings page
                if (!startedAtStartPage && settingsPageDisplaying && reading.Buttons.HasFlag(GamepadButtons.X))
                {
                    if (menuSelector_Settings.selection_color == ColorSelection.DarkOrange)
                    {
                        currentColor = Colors.DarkOrange;
                    }
                    else if (menuSelector_Settings.selection_color == ColorSelection.Cyan)
                    {
                        currentColor = Colors.Cyan;
                    }
                    else if (menuSelector_Settings.selection_color == ColorSelection.Green)
                    {
                        currentColor = Colors.Green;
                    }
                    else if (menuSelector_Settings.selection_color == ColorSelection.HotPink)
                    {
                        currentColor = Colors.HotPink;
                    }
                    snake.resetGame(currentColor);


                    // Menu

                    if (menuSelector_Settings.selection_music == MusicSelection.Song1)
                    {
                        changeToDuckTales = true;


                    }
                    else if (menuSelector_Settings.selection_music == MusicSelection.Song2)
                    {
                        // One more line
                        changeToOneMoreLine = true;


                    }
                    else if (menuSelector_Settings.selection_music == MusicSelection.Song3)
                    {
                        // GTO theme
                        // source   https://www.youtube.com/watch?v=Uc6vF1PWdFY 
                        changeToGTO = true;


                    }
                    settingsPageDisplaying = false;
                    startPageDisplaying = true;
                    menuSelector = new menuSelector();
                }
            }
        }
    }
}