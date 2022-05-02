/*
 * Author: Amy Hua
 * File Name: Game1.cs
 * Project Name: Minesweeper
 * Creation Date: April 19, 2022
 * Modified Date: May 1, 2022
 * Description: Allows user to play minesweeper
 */
using Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;

namespace Minesweeper
{
    public class Game1 : Game
    {
        const int TILELENGTHEASY = 45;
        const int TILELENGTHMED = 30;
        const int TILELENGTHHARD = 25;

        const int GAMEPLAY = 1;
        const int GAMEOVER = 2;

        const int EASY = 1;        
        const int MED = 2;
        const int HARD = 3;

        const int EASYMINES = 10;
        const int MEDMINES = 40;
        const int HARDMINES = 99;

        const int HUDBUTTONHEIGHT = 35;
        const int HUDSPACING = 10;

        const int MAXTIME = 999;

        const int WIN = 1;
        const int LOSS = 2;

        const float volume = 0.8f;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        MouseState prevMouse;
        MouseState mouse;

        StreamReader inFile;
        StreamWriter outFile;

        int gameState;

        int difficulty;
        int newDifficulty;

        int tileLength;

        int screenWidth, screenHeight;

        SpriteFont generalFont;

        Song winSong, loseSong;
        
        SoundEffect smallClear;
        SoundEffect largeClear;
        SoundEffect clearFlag;
        SoundEffect placeFlag;
        SoundEffect mine;

        Timer timer;
        int time;
        int bestTimeEasy, bestTimeMed, bestTimeHard;

        bool showInstr;
        Texture2D instruction;
        Rectangle intructionRec;

        Texture2D hud;
        Rectangle hudRec;

        Texture2D easyButton, medButton, hardButton;
        Texture2D diffButton;
        Rectangle diffRec;

        Texture2D dropDown;
        Rectangle dropDownRec;
        bool showDropDown = false;

        Texture2D volOn, volOff;
        Rectangle volRec;
        Texture2D exitButton;
        Rectangle exitRec;

        Texture2D watch, trophy;
        Rectangle watchRec;
        Vector2 watchCountLoc;

        Texture2D flag;
        Rectangle flagRec;
        Vector2 flagCountLoc;

        Texture2D boardEasy, boardMed, boardHard;
        Rectangle boardEasyRec, boardMedRec, boardHardRec;

        Texture2D clearDark, clearLight;

        Texture2D gameOverWin, gameOverLoss;
        Vector2 gameOverLoc;

        Texture2D tryAgainButton;
        Texture2D playAgainButton;
        Rectangle retryButtonRec;

        int gameOverTimeY;

        int gameResult;

        Texture2D colourOverlay;
        Rectangle colourOverlayRec;

        TilesManager tiles;

        Texture2D[] minesImgs = new Texture2D[8];
        Texture2D[] numsImgs = new Texture2D[8];

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        
        protected override void Initialize()
        {         
            gameState = GAMEPLAY;
            showInstr = true;

            try
            {
                inFile = File.OpenText("Info.txt");
                
                string[] scores = inFile.ReadLine().Split();
                bestTimeEasy = Convert.ToInt32(scores[0]);
                bestTimeMed = Convert.ToInt32(scores[1]);
                bestTimeHard = Convert.ToInt32(scores[2]);

                difficulty = Convert.ToInt32(inFile.ReadLine());

                inFile.Close();
            }
            catch
            {
                bestTimeEasy = MAXTIME + 1;
                bestTimeMed = MAXTIME + 1;
                bestTimeHard = MAXTIME + 1;

                difficulty = EASY;

                SaveInfo();
            }

            IsMouseVisible = true;

            base.Initialize();
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);       

            MediaPlayer.Volume = volume;
            SoundEffect.MasterVolume = volume;

            generalFont = Content.Load<SpriteFont>("Fonts/GeneralFont");

            winSong = Content.Load<Song>("Audio/Win");
            loseSong = Content.Load<Song>("Audio/Lose");

            smallClear = Content.Load<SoundEffect>("Audio/SmallClear");
            largeClear = Content.Load<SoundEffect>("Audio/LargeClear");
            clearFlag = Content.Load<SoundEffect>("Audio/ClearFlag");
            placeFlag = Content.Load<SoundEffect>("Audio/PlaceFlag");
            mine = Content.Load<SoundEffect>("Audio/Mine");

            ResetTimer();

            hud = Content.Load<Texture2D>("Images/HUDBar");

            instruction = Content.Load<Texture2D>("Images/Instructions");

            boardEasy = Content.Load<Texture2D>("Images/board_easy");
            boardEasyRec = new Rectangle(0, hud.Height, boardEasy.Width, boardEasy.Height);
            boardMed = Content.Load<Texture2D>("Images/board_med");
            boardMedRec = new Rectangle(0, hud.Height, boardMed.Width, boardMed.Height);
            boardHard = Content.Load<Texture2D>("Images/board_hard");
            boardHardRec = new Rectangle(0, hud.Height, boardHard.Width, boardHard.Height);

            easyButton = Content.Load<Texture2D>("Images/EasyButton");
            medButton = Content.Load<Texture2D>("Images/MedButton");
            hardButton = Content.Load<Texture2D>("Images/HardButton");

            dropDown = Content.Load<Texture2D>("Images/DropDown");

            flag = Content.Load<Texture2D>("Images/flag");

            watch = Content.Load<Texture2D>("Images/Watch");
            trophy = Content.Load<Texture2D>("Images/Trophy");

            volOn = Content.Load<Texture2D>("Images/SoundOn");
            volOff = Content.Load<Texture2D>("Images/SoundOff");

            exitButton = Content.Load<Texture2D>("Images/Exit");

            clearDark = Content.Load<Texture2D>("Images/Clear_Dark");
            clearLight = Content.Load<Texture2D>("Images/Clear_Light");

            for (int i = 0; i < minesImgs.Length; i++)
            {
                minesImgs[i] = Content.Load<Texture2D>("Images/Mine" + (i + 1));
            }

            for (int i = 0; i < numsImgs.Length; i++)
            {
                numsImgs[i] = Content.Load<Texture2D>("Images/" + (i + 1));
            }

            gameOverWin = Content.Load<Texture2D>("Images/GameOver_WinResults");
            gameOverLoss = Content.Load<Texture2D>("Images/GameOver_Results");

            playAgainButton = Content.Load<Texture2D>("Images/GameOver_PlayAgain");
            tryAgainButton = Content.Load<Texture2D>("Images/GameOver_TryAgain");

            colourOverlay = Content.Load<Texture2D>("Images/colourOverlay");            
            
            SetDifficulty();
            CalcRecs();
        }

        protected override void UnloadContent()
        {}

        protected override void Update(GameTime gameTime)
        {
            prevMouse = mouse;
            mouse = Mouse.GetState();

            switch (gameState)
            {
                case GAMEPLAY:
                    if (mouse.RightButton == ButtonState.Pressed && prevMouse.RightButton != ButtonState.Pressed)
                    {                        
                        tiles.FlagTile(CalcRow(mouse.Y), CalcCol(mouse.X), placeFlag, clearFlag);             
                    }

                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        if (exitRec.Contains(mouse.X, mouse.Y))
                        {
                            SaveInfo();
                            Exit();
                        }

                        UpdateVolume(mouse.X, mouse.Y);

                        if (showDropDown == true && dropDownRec.Contains(mouse.X, mouse.Y))
                        {
                            if (mouse.Y < dropDownRec.Y + dropDownRec.Height / 3)
                            {
                                newDifficulty = EASY;
                                diffButton = easyButton;
                            }

                            else if (mouse.Y < dropDownRec.Y + dropDownRec.Height * 2 / 3)
                            {
                                newDifficulty = MED;
                                diffButton = medButton;
                            }

                            else
                            {
                                newDifficulty = HARD;
                                diffButton = hardButton;
                            }

                            if (newDifficulty != difficulty)
                            {
                                difficulty = newDifficulty;
                                ResetTimer();
                                SetDifficulty();
                                CalcRecs();
                            }
                            showDropDown = false;
                            break;
                        }

                        else if (diffRec.Contains(mouse.X, mouse.Y))
                        {                            
                            showDropDown = !showDropDown;
                        }

                        else if (hudRec.Contains(mouse.X, mouse.Y) == false && tiles.IsFlagged(CalcRow(mouse.Y), CalcCol(mouse.X)) == false)
                        {
                            showInstr = false;
                            showDropDown = false;

                            tiles.ShowTile(CalcRow(mouse.Y), CalcCol(mouse.X), hud.Height, tileLength, smallClear, largeClear);

                            if (tiles.CheckLoss(CalcRow(mouse.Y), CalcCol(mouse.X)) == true)
                            {
                                mine.CreateInstance().Play();
                                tiles.RevealMines();
                                gameResult = LOSS;
                                gameState = GAMEOVER;
                            }

                            else if (tiles.CheckWin() == true)
                            {
                                gameResult = WIN;
                                UpdateBestTime();
                                gameState = GAMEOVER;
                            }

                            timer.Activate();
                        }   
                    }

                    timer.Update(gameTime.ElapsedGameTime.TotalSeconds);
                    CalcTime();
                    break;

                case GAMEOVER:
                    if (MediaPlayer.State != MediaState.Playing)
                    {
                        if (gameResult == WIN)
                        {
                            MediaPlayer.Play(winSong);
                        }

                        else
                        {
                            MediaPlayer.Play(loseSong);
                        }
                    }                    

                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        UpdateVolume(mouse.X, mouse.Y);

                        if (retryButtonRec.Contains(mouse.X, mouse.Y))
                        {
                            gameState = GAMEPLAY;
                            ResetTimer();
                            MediaPlayer.Stop();
                            SetDifficulty();
                        }

                        else if (exitRec.Contains(mouse.X, mouse.Y))
                        {
                            SaveInfo();
                            Exit();
                        }
                    }           
                    
                    break;
            }            
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            switch (gameState)
            {
                case GAMEPLAY:
                    DrawBoard();
                    tiles.DrawTiles(spriteBatch, clearDark, clearLight, minesImgs, numsImgs, flag, hudRec.Height, tileLength);
                    DrawDropDown();
                    DrawSettings();

                    if (showInstr == true)
                    {
                        spriteBatch.Draw(instruction, intructionRec, Color.White * 0.8f);
                    }

                    break;

                case GAMEOVER:
                    DrawBoard();
                    tiles.DrawTiles(spriteBatch, clearDark, clearLight, minesImgs, numsImgs, flag, hudRec.Height, tileLength);
                    DrawResults();
                    DrawSettings();

                    break;

            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawBoard()
        {
            spriteBatch.Draw(hud, hudRec, Color.White);

            switch (difficulty)
            {
                case EASY:
                    spriteBatch.Draw(boardEasy, boardEasyRec, Color.White);
                    spriteBatch.Draw(easyButton, diffRec, Color.White);
                    break;

                case MED:
                    spriteBatch.Draw(boardMed, boardMedRec, Color.White);
                    spriteBatch.Draw(medButton, diffRec, Color.White);
                    break;

                case HARD:
                    spriteBatch.Draw(boardHard, boardHardRec, Color.White);
                    spriteBatch.Draw(hardButton, diffRec, Color.White);
                    break;
            }

            spriteBatch.Draw(flag, flagRec, Color.White);

            spriteBatch.DrawString(generalFont, tiles.GetFlagCount(), flagCountLoc, Color.White);
            spriteBatch.DrawString(generalFont, Convert.ToString(time), watchCountLoc, Color.White);            

            spriteBatch.Draw(watch, watchRec, Color.White);            
        }

        private void DrawSettings()
        {
            if (MediaPlayer.Volume == 0)
            {
                spriteBatch.Draw(volOff, volRec, Color.White);
                spriteBatch.Draw(volOn, volRec, Color.White);
            }

            else
            {
                spriteBatch.Draw(volOn, volRec, Color.White);
            }

            spriteBatch.Draw(exitButton, exitRec, Color.White);
        }

        private void DrawResults()
        {
            spriteBatch.Draw(colourOverlay, colourOverlayRec, Color.Black * 0.7f);

            if (gameResult == WIN)
            {
                spriteBatch.Draw(gameOverWin, gameOverLoc, Color.White);
                spriteBatch.Draw(playAgainButton, retryButtonRec, Color.White);
                spriteBatch.DrawString(generalFont, Convert.ToString(time), new Vector2(CenterTextX(Convert.ToString(time), generalFont, (int)gameOverLoc.X + 85), gameOverTimeY), Color.White);
            }

            else
            {
                spriteBatch.Draw(gameOverLoss, gameOverLoc, Color.White);
                spriteBatch.Draw(tryAgainButton, retryButtonRec, Color.White);
            }

            switch (difficulty)
            {
                case EASY:
                    if (bestTimeEasy <= MAXTIME)
                    {
                        spriteBatch.DrawString(generalFont, Convert.ToString(bestTimeEasy), new Vector2(CenterTextX(Convert.ToString(bestTimeEasy), generalFont, (int)gameOverLoc.X + 215), gameOverTimeY), Color.White);
                    }
                    break;

                case MED:
                    if (bestTimeMed <= MAXTIME)
                    {
                        spriteBatch.DrawString(generalFont, Convert.ToString(bestTimeMed), new Vector2(CenterTextX(Convert.ToString(bestTimeMed), generalFont, (int)gameOverLoc.X + 215), gameOverTimeY), Color.White);
                    }                        
                    break;

                case HARD:
                    if (bestTimeHard <= MAXTIME)
                    {
                        spriteBatch.DrawString(generalFont, Convert.ToString(bestTimeHard), new Vector2(CenterTextX(Convert.ToString(bestTimeHard), generalFont, (int)gameOverLoc.X + 215), gameOverTimeY), Color.White);
                    }                        
                    break;
            }
        }

        private void DrawDropDown()
        {
            if (showDropDown == true)
            {
                spriteBatch.Draw(dropDown, dropDownRec, Color.White);
            }
        }

        private void CalcRecs()
        {
            hudRec = new Rectangle(0, 0, screenWidth, hud.Height);

            int buttonHeight = hudRec.Height / 2 - HUDBUTTONHEIGHT / 2;

            diffRec = new Rectangle(hudRec.Height / 2 - easyButton.Height / 2, hudRec.Height / 2 - easyButton.Height / 2, diffButton.Width, diffButton.Height);

            intructionRec = new Rectangle(screenWidth / 2 - instruction.Width / 4, screenHeight / 2 - instruction.Height / 4, instruction.Width/2, instruction.Height/2);

            dropDownRec = new Rectangle((int)diffRec.X, (int)diffRec.Y + easyButton.Height, dropDown.Width, dropDown.Height);
            
            exitRec = new Rectangle(screenWidth - HUDBUTTONHEIGHT - HUDSPACING, buttonHeight + 8, HUDBUTTONHEIGHT - 15, HUDBUTTONHEIGHT - 15);
            
            volRec = new Rectangle(exitRec.X - HUDBUTTONHEIGHT - HUDSPACING, buttonHeight + 5, HUDBUTTONHEIGHT - 10, HUDBUTTONHEIGHT - 10);

            flagRec = new Rectangle(screenWidth / 2 - flag.Width - 50, buttonHeight, HUDBUTTONHEIGHT, HUDBUTTONHEIGHT);
            flagCountLoc = new Vector2(flagRec.X + flagRec.Width + 5, flagRec.Y + 2);

            watchRec = new Rectangle(screenWidth / 2, buttonHeight, HUDBUTTONHEIGHT, HUDBUTTONHEIGHT);
            watchCountLoc = new Vector2(watchRec.X + watchRec.Width + HUDSPACING, watchRec.Y + 2);

            gameOverLoc = new Vector2(screenWidth / 2 - gameOverWin.Width / 2, screenHeight / 2 - gameOverWin.Height / 2 - tryAgainButton.Height / 2);
            retryButtonRec = new Rectangle((int)gameOverLoc.X, (int)gameOverLoc.Y + gameOverWin.Height + HUDSPACING, tryAgainButton.Width, tryAgainButton.Height);

            gameOverTimeY = (int)gameOverLoc.Y + 90;

            colourOverlayRec = new Rectangle(0, 0, screenWidth, screenHeight);
        }

        private void SetDifficulty()
        {
            switch (difficulty)
            {
                case EASY:
                    tiles = new TilesManager(8, 10, EASYMINES);
                    tileLength = TILELENGTHEASY;
                    diffButton = easyButton;
                    graphics.PreferredBackBufferWidth = boardEasyRec.Width;
                    graphics.PreferredBackBufferHeight = boardEasyRec.Height + hud.Height;
                    break;

                case MED:
                    tiles = new TilesManager(14, 18, MEDMINES);
                    tileLength = TILELENGTHMED;
                    diffButton = medButton;
                    graphics.PreferredBackBufferWidth = boardMedRec.Width;
                    graphics.PreferredBackBufferHeight = boardMedRec.Height + hud.Height;
                    break;

                case HARD:
                    tiles = new TilesManager(20, 24, HARDMINES);
                    tileLength = TILELENGTHHARD;
                    diffButton = hardButton;
                    graphics.PreferredBackBufferWidth = boardHardRec.Width;
                    graphics.PreferredBackBufferHeight = boardHardRec.Height + hud.Height;
                    break;
            }
            graphics.ApplyChanges();

            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;
        }

        private void ResetTimer()
        {
            timer = new Timer(MAXTIME, false);
        }

        private void UpdateBestTime()
        {
            switch (difficulty)
            {
                case EASY:
                    if (time < bestTimeEasy)
                    {
                        bestTimeEasy = time;
                    }
                    break;

                case MED:
                    if (time < bestTimeMed)
                    {
                        bestTimeMed = time;
                    }
                    break;

                case HARD:
                    if (time < bestTimeHard)
                    {
                        bestTimeHard = time;
                    }
                    break;
            }
            SaveInfo();            
        }

        private void UpdateVolume(int mouseX, int mouseY)
        {
            if (volRec.Contains(mouseX, mouseY))
            {
                if (MediaPlayer.Volume == 0)
                {
                    MediaPlayer.Volume = volume;
                    SoundEffect.MasterVolume = volume;
                }

                else
                {
                    MediaPlayer.Volume = 0;
                    SoundEffect.MasterVolume = 0;
                }
            }
        }

        private void SaveInfo()
        {
            outFile = File.CreateText("Info.txt");
            outFile.WriteLine(bestTimeEasy + " " + bestTimeMed + " " + bestTimeHard);
            outFile.WriteLine(difficulty);
            outFile.Close();
        }

        private int CenterTextX(string text, SpriteFont font, int centre)
        {
            int locX = (int)(centre - font.MeasureString(text).X / 2);
            return locX;
        }

        private int CalcRow(int mouseY)
        {
            return (int)((mouseY - hudRec.Height) / tileLength);
        }
        private int CalcCol(int mouseX)
        {
            int col = (int)(mouseX / tileLength);
            return col;
        }

        private void CalcTime()
        {
            time = (int)(timer.GetTimePassed());
        }
    }
}
