/* Author:          Steven Ma
 * Filename:        Gorillaz.cs
 * Project Name:    Gorillaz
 * Creation Date:   April 26, 2018
 * Modified Date:   May 14, 2018
 * Description:     Recreation of the classic game, Gorillas, with a military themed
 *                  using turrets and missiles. Control of the game is mouse based, 
 *                  using the mouse to measure both angle and velocity.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Gorillaz
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Gorillaz : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Creates a random object 
        Random rng = new Random();

        //Creates and stores gamestates in enum
        enum GameState { InGame, GameOver, MainMenu, ScoreBoard, LoadingScreen }
        GameState currentGameState = GameState.MainMenu;

        //Stores the game's dimensions
        int screenWidth;
        int screenHeight;

        //Stores title font
        SpriteFont defaultFont;

        //Stores background information 
        BackDrop mainMenu;
        BackDrop scoreBackDrop;

        //Stores the keyboard tips
        BackDrop escapeMainMenu;
        BackDrop tabKeyScores;
        BackDrop enterKeyCont;

        //Stores the moving sky background's information
        Texture2D skiesBg;
        Rectangle bgLocl;
        Rectangle bgLoc2;

        //Stores the mouse's state 
        MouseState mouse;
        MouseState prevMouse;

        //Stores keyboard's state
        KeyboardState kb;
        KeyboardState prevKb;

        //Creates and stores the buildings' information in game
        Building[] buildings = new Building[10];
        Texture2D[] buildingPics = new Texture2D[4];

        //Creates and stores the players/turrets information
        Player[] players = new Player[2];
        Point stopTurretAnim = new Point(2, 2);

        //Stores whether turret was fired
        bool isFiring;
        bool isClicked;

        //Creates and stores the buttons in game
        Button startButton;
        Button scoreBoardButton;
        Button quitButton;
        Button mainMenuButton;

        //Creates and stores missile and it's projectile properties
        Projectile missile;
        Vector2 missileTracj;
        bool isMissileFired;

        //Stores the information about gravity of the game
        string gravityNum = "";
        const float DEFAULT_GRAVITY = 9.81f / 60f;
        Vector2 gravity;

        //Stores information on the explosion of missile
        Explosion explode;
        Point stopExplodeAnim = new Point(3, 3);
        bool isExplode;

        //Stores the current turn of the players
        bool isP1Turn = true;
        
        //Stores the settings during loading screen state
        const int MAX_CHAR_NAME = 3;
        const int MAX_GRAV_CHAR = 4;
        bool playerEnter = true;
        bool hasP1Enter;
        bool hasP2Enter;

        //Stores the scores of both players
        const int NUM_PLAYERS = 2;
        const int MAX_ROUND_NUM = 3;       

        //Stores the number of rounds passed
        int roundNum;

        //Stores the scoreboard info
        const int MAX_NUM_BOARD = 10;
        Text[] scoreNamesTxt = new Text[MAX_NUM_BOARD];
        Text[] scoreNumsTxt = new Text[MAX_NUM_BOARD];
        const int MAX_SCORE = 10000;
        bool isScoreCalc = true;
        int winnerScore;

        //Stores the text regarding the title screen
        Text titleScreen;

        //Stores text information of the gameplay
        Text angleHUD;
        Text velocityHUD;

        //Stores all the sound effect instances of the game
        SoundEffectInstance turretFireInstance;
        SoundEffectInstance explosionSoundInstance;
        SoundEffectInstance turretHitInstance;
        SoundEffectInstance scorePointInstance;
        SoundEffectInstance victoryInstance;

        //Stores the all music in the game
        Song mainMenuMusic;
        Song loadingScreenMusic;
        Song inGameMusic;

        public Gorillaz()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //Makes mouse visible in game
            IsMouseVisible = true;

            //Repeats the in game music when song ends
            MediaPlayer.IsRepeating = true;

            //Sets the resolution of the game
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            //Stores the resolution of the game into variables
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //Loads the default font
            defaultFont = Content.Load<SpriteFont>("Fonts/DefaultFont");

            //Loads the game's music
            mainMenuMusic = Content.Load<Song>("Sounds/Music/mainMenuMusic");
            loadingScreenMusic = Content.Load<Song>("Sounds/Music/Tekken 3 OST PlayStation - Character select");
            inGameMusic = Content.Load<Song>("Sounds/Music/inGameMusic");

            //Loads and stores the moving background's information
            skiesBg = Content.Load<Texture2D>("Images/Backgrounds/skies");
            bgLocl = new Rectangle(0, 0, screenWidth, screenHeight);
            bgLoc2 = new Rectangle(screenWidth, 0, screenWidth, screenHeight);

            //Loads the main menu background into game
            Texture2D mainMenuPic = Content.Load<Texture2D>("Images/Backgrounds/bgImg");
            Rectangle bgLoc = new Rectangle(0, 0, screenWidth, screenHeight);
            mainMenu = new BackDrop(mainMenuPic, bgLoc, 1f);

            //Loads and stores the player scores' backdrop
            Texture2D scoreBackDroPic = Content.Load<Texture2D>("Images/Backgrounds/scoreBackDrop");
            Rectangle scoreLoc = new Rectangle(375, 215, scoreBackDroPic.Width, scoreBackDroPic.Height);
            scoreBackDrop = new BackDrop(scoreBackDroPic, scoreLoc, 0.7f);

            //Loads and stores the Tab key's info text
            Texture2D tabKeyPic = Content.Load<Texture2D>("Images/Backgrounds/tabKey");
            Rectangle tabKeyLoc = new Rectangle(700, 0, (int)(tabKeyPic.Width * 0.4), (int)(tabKeyPic.Height * 0.4));
            tabKeyScores = new BackDrop(tabKeyPic, tabKeyLoc, 1.0f);

            //Loads and stores the Escape key's info text
            Texture2D escapeKeyPic = Content.Load<Texture2D>("Images/Backgrounds/escapeKey");
            Rectangle escapeKeyLoc = new Rectangle(720, 512, (int)(escapeKeyPic.Width * 0.3), (int)(escapeKeyPic.Height * 0.3));
            escapeMainMenu = new BackDrop(escapeKeyPic, escapeKeyLoc, 1.0f);

            //Loads and stores Enter key tips
            Texture2D enterKeyPic = Content.Load<Texture2D>("Images/Backgrounds/enterKey");
            Rectangle enterKeyLoc = new Rectangle(350, 420, (int)(escapeKeyPic.Width * 0.4), (int)(escapeKeyPic.Height * 0.4));
            enterKeyCont = new BackDrop(enterKeyPic, enterKeyLoc,1.0f);

            //Loads the tall building into game
            buildingPics[0] = Content.Load<Texture2D>("Images/Buildings/building1");
            buildingPics[1] = Content.Load<Texture2D>("Images/Buildings/building2");
            buildingPics[2] = Content.Load<Texture2D>("Images/Buildings/building3");
            buildingPics[3] = Content.Load<Texture2D>("Images/Buildings/building4");
            MakeBuildingScape();

            //Loads the turret's frame and sheet size into the game
            Point[] turretFrameDimensions = new Point[2];
            turretFrameDimensions[0] = new Point(38, 38);
            turretFrameDimensions[1] = new Point(3, 3);

            //Loads the right turret location (Player 2)
            Texture2D rightTurretSprite = Content.Load<Texture2D>("Images/Sprites/rightTurret");
            Point rightTurretCurrentFrame = new Point(0, 0);
            Vector2 rightTurretPos = new Vector2(screenWidth - turretFrameDimensions[0].X, screenHeight - turretFrameDimensions[0].Y);
            players[1] = new Player(rightTurretSprite, rightTurretPos, turretFrameDimensions[0], rightTurretCurrentFrame, turretFrameDimensions[1]);

            //Loads the left turret location (Player 1)
            Texture2D leftTurretSprite = Content.Load<Texture2D>("Images/Sprites/leftTurret");
            Point leftTurretCurrentFrame = new Point(0, 0);
            Vector2 leftTurretPos = new Vector2(0, screenHeight - turretFrameDimensions[0].Y);
            players[0] = new Player(leftTurretSprite, leftTurretPos, turretFrameDimensions[0], leftTurretCurrentFrame, turretFrameDimensions[1]);

            //Loads the sounds effeccts of the game
            SoundEffect turretFire = Content.Load<SoundEffect>("Sounds/Sound Effects/Artillery+Burst");
            turretFireInstance = turretFire.CreateInstance();
            SoundEffect explosionSound = Content.Load<SoundEffect>("Sounds/Sound Effects/Explosion+1");
            explosionSoundInstance = explosionSound.CreateInstance();
            SoundEffect turretHit = Content.Load<SoundEffect>("Sounds/Sound Effects/sfx_hit");
            turretHitInstance = turretHit.CreateInstance();
            SoundEffect scorePoint = Content.Load<SoundEffect>("Sounds/Sound Effects/sfx_point");
            scorePointInstance = scorePoint.CreateInstance();
            SoundEffect victory = Content.Load<SoundEffect>("Sounds/Sound Effects/Ta Da-SoundBible.com-1884170640");
            victoryInstance = victory.CreateInstance();

            //Loads the missile location
            Texture2D missileImg = Content.Load<Texture2D>("Images/Sprites/Bomb_2");
            const double MISSILE_SCALE = 0.07;
            Rectangle missileLoc = new Rectangle((int)leftTurretPos.X + turretFrameDimensions[0].X, (int)leftTurretPos.Y, (int)(missileImg.Width * MISSILE_SCALE), (int)(missileImg.Height * MISSILE_SCALE));
            missile = new Projectile(missileImg, missileLoc, DeterminePlayerTurn());

            //Loads the explosion sprite and its locations
            Texture2D explodeSprite = Content.Load<Texture2D>("Images/Sprites/explode");
            Point explodeCurrentFrame = new Point(0, 0);
            Point[] explodeFrameDimensions = new Point[2];
            explodeFrameDimensions[0] = new Point(64, 64);
            explodeFrameDimensions[1] = new Point(4, 4);
            Vector2 explodePos = new Vector2(missileLoc.X, missileLoc.Y);
            explode = new Explosion(explodeSprite, explodePos, explodeFrameDimensions[0], explodeCurrentFrame, explodeFrameDimensions[1]);

            //Creates and stores the main menu buttons
            Texture2D startBtnPic = Content.Load<Texture2D>("Images/Buttons/startBtn");
            Rectangle startBtnLoc = new Rectangle(243, 361, (int)(startBtnPic.Width * 0.5), (int)(startBtnPic.Height * 0.5));
            startButton = new Button(startBtnPic, startBtnLoc);
            Texture2D quitBtnPic = Content.Load<Texture2D>("Images/Buttons/quitBtn");
            Rectangle quitBtnLoc = new Rectangle(650, 361, (int)(quitBtnPic.Width * 0.27), (int)(quitBtnPic.Height * 0.27));
            quitButton = new Button(quitBtnPic, quitBtnLoc);
            Texture2D scoreBtnPic = Content.Load<Texture2D>("Images/Buttons/scoreBtn");
            Rectangle scoreBtnLoc = new Rectangle(450, 364, (int)(scoreBtnPic.Width * 0.23), (int)(scoreBtnPic.Height * 0.23));
            scoreBoardButton = new Button(scoreBtnPic, scoreBtnLoc);

            //Stores and creates the return to main menu button
            Texture2D menuBtnPic = Content.Load<Texture2D>("Images/Buttons/menuBtn");
            Rectangle menuBtnLoc = new Rectangle(402, 418, (int)(menuBtnPic.Width * 0.5), (int)(menuBtnPic.Height * 0.5));
            mainMenuButton = new Button(menuBtnPic, menuBtnLoc);

            //Positions the turrets based on the buildings that spawned
            PlacePlayers();

            //Creates and stores the temporary values of scoreboard
            string[] scoreNames = new string[MAX_NUM_BOARD];
            int[] scoreNums = new int[MAX_NUM_BOARD];

            //Initializes the default names
            for (int i = 0; i < MAX_NUM_BOARD; i++)
            {
                scoreNames[i] = "AAA";
            }

            //Initializes the default scores
            for (int i = 0; i < MAX_NUM_BOARD; i++)
            {
                scoreNamesTxt[i] = new Text(defaultFont, scoreNames[i], new Vector2(350, 50 + (i * 50)), Color.Bisque);
                scoreNumsTxt[i] = new Text(defaultFont, Convert.ToString(scoreNums[i]), new Vector2(550, 50 + (i * 50)), Color.Blue);
            }

            //Initializes the username input as blank
            for (int i = 0; i < NUM_PLAYERS; i++)
            {
                players[i].playerName = "";
            }

            //Creates and stores the title screen text's information
            titleScreen = new Text(defaultFont, "GORILLAS", new Vector2(400, 174), Color.Bisque);

            //Creates and stores the HUD's in game information 
            string hudText = "";
            Vector2 angleHUDPos = new Vector2(0, 40);
            angleHUD = new Text(defaultFont, hudText, angleHUDPos, Color.Bisque);
            velocityHUD = new Text(defaultFont, hudText, Vector2.Zero, Color.Bisque);

            //Plays the main menu music
            MediaPlayer.Play(mainMenuMusic);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            //Gets keyboard and mouse state
            mouse = Mouse.GetState();
            kb = Keyboard.GetState();

            //Changes gamestate based on gameplay inputs
            switch (currentGameState)
            {
                case GameState.MainMenu:
                    //Checks to see if any buttons was clicked in main menu
                    if (startButton.ButtonClicked(mouse) && prevMouse.LeftButton == ButtonState.Released)
                    {
                        //Starts playing the loading screen music
                        MediaPlayer.Play(loadingScreenMusic);

                        //Changes gamestate to the loading screen
                        currentGameState = GameState.LoadingScreen;
                    }
                    else if (quitButton.ButtonClicked(mouse))
                    {
                        //Exits the game
                        Exit();
                    }
                    else if (scoreBoardButton.ButtonClicked(mouse) && prevMouse.LeftButton == ButtonState.Released)
                    {
                        //Changes gamestate to scoreboard
                        currentGameState = GameState.ScoreBoard;
                    }
                    
                    prevMouse = mouse;
                    break;

                case GameState.LoadingScreen:
                    //Allows for players to input their in game names
                    if (playerEnter)
                    {
                        if (!hasP1Enter)
                        {
                            //Allows player 1 to enter their name via keyboard
                            if (players[0].playerName.Length < MAX_CHAR_NAME)
                            {
                                players[0].playerName += TextInput();
                            }

                            //Allows player 1 to erase text using backspace key
                            if (kb.IsKeyDown(Keys.Back) && prevKb.IsKeyUp(Keys.Back) && players[0].playerName.Length > 0)
                            {
                                players[0].playerName = players[0].playerName.Remove(players[0].playerName.Length - 1, 1);
                            }

                            //Allows player 2 to input their name after player 1 presses enter
                            if (kb.IsKeyDown(Keys.Enter) && prevKb.IsKeyUp(Keys.Enter))
                            {
                                hasP1Enter = true;
                            }
                        }
                        else
                        {
                            //Allows player 2 to eneter their name via keyboard
                            if (players[1].playerName.Length < MAX_CHAR_NAME)
                            {
                                players[1].playerName += TextInput();
                            }

                            //Allows player 2 to erase text using backspace key
                            if (kb.IsKeyDown(Keys.Back) && prevKb.IsKeyUp(Keys.Back) && players[1].playerName.Length > 0)
                            {
                                players[1].playerName = players[1].playerName.Remove(players[1].playerName.Length - 1, 1);
                            }

                            //Allows user to input gravity accleration after player 2 presses enter
                            if (kb.IsKeyDown(Keys.Enter) && prevKb.IsKeyUp(Keys.Enter))
                            {
                                hasP2Enter = true;
                                playerEnter = false;
                            }
                        }

                        prevKb = kb;
                    }

                    //Allows for input of gravity after player 2 enters name
                    if (hasP2Enter)
                    {
                        //Allows's user to input gravity via numpad
                        if (gravityNum.Length < MAX_GRAV_CHAR)
                        {
                            gravityNum += NumInput();
                        }

                        //Allows user to erase gravity text via backspace
                        if (kb.IsKeyDown(Keys.Back) && prevKb.IsKeyUp(Keys.Back) && gravityNum.Length > 0)
                        {
                            gravityNum = gravityNum.Remove(gravityNum.Length - 1, 1);
                        }

                        //Stores user input as the game's gravity
                        if (kb.IsKeyDown(Keys.Enter) && prevKb.IsKeyUp(Keys.Enter))
                        {
                            //Sets the gravity to the default if text input is empty
                            if (gravityNum == "")
                            {
                                gravity = new Vector2(0, DEFAULT_GRAVITY);
                            }
                            else
                            {
                                gravity = new Vector2(0, (float)(Convert.ToDouble(gravityNum)) / 60f);
                            }

                            //Position's the players based on  building's spawn
                            PlacePlayers();

                            //Plays the in game music
                            MediaPlayer.Play(inGameMusic);

                            //Changes gamestate to the in game
                            currentGameState = GameState.InGame;
                        }

                        prevKb = kb;
                    }
                    break;
                case GameState.InGame:
                    //Animates the sky background
                    bgLocl.X--;
                    bgLoc2.X--;
                    if (bgLocl.X <= -screenWidth)
                    {
                        bgLocl.X = screenWidth;
                    }
                    else if (bgLoc2.X <= -screenWidth)
                    {
                        bgLoc2.X = screenWidth;
                    }

                    //Sets the angle and velocity that the player wishes to enter
                    missile.CalcAngle(mouse, isP1Turn);
                    angleHUD.text = "Angle:" + Convert.ToString((int)(missile.angle * (180 / Math.PI)));
                    missile.CalcVelocity(mouse);
                    velocityHUD.text = "Velocity:" + Convert.ToString(Math.Round(missile.objectTotalVelocity,2));

                    //Fires the missile if player hits enter
                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && !isClicked)
                    {
                        //Increments the number launches made by each player
                        if (isP1Turn)
                        {
                            players[0].playerLaunches++;
                        }
                        else
                        {
                            players[1].playerLaunches++;
                        }

                        //Plays the turret fire sound effect
                        turretFireInstance.Play();

                        //Changes the state of bool to prevent inputs during missile fire
                        isClicked = true;

                        //Makes the missile fire and the turret animate
                        isFiring = true;
                        isMissileFired = true;

                        //Converts the velocity inputted into a Vector2 for missile to fire 
                        missileTracj = missile.CalcVelocity(mouse);
                    }

                    prevMouse = mouse;

                    //Animates the firing turret once for either side
                    if (isFiring)
                    {
                        DeterminePlayerTurn().Animate(gameTime);

                        if (DeterminePlayerTurn().currentFrame == stopTurretAnim)
                        {
                            isFiring = false;
                        }
                    }

                    //Calculates the projectile motion of missile based on who fired it
                    if (isMissileFired)
                    {
                        //Changes the positioning of the missile based on projecitle motion and player's turn
                        if (isP1Turn)
                        {
                            missileTracj += gravity;
                            missile.bounds.X += (int)(missileTracj.X);
                            missile.bounds.Y += (int)(missileTracj.Y);
                        }
                        else
                        {
                            missileTracj += gravity;
                            missile.bounds.X -= (int)(missileTracj.X);
                            missile.bounds.Y += (int)(missileTracj.Y);
                        }

                        //Angle of the missile is updated, causing missile to rotate in game
                        missile.angle = (float)Math.Atan2(-missileTracj.Y, missileTracj.X);

                        //Called to check for any collisions while missile is fired
                        CheckCollision();
                    }

                    //Causes explosion animation when the missile collides with target
                    if (isExplode)
                    {

                        //Sets the position of the explosion and animates it
                        if (isP1Turn)
                        {
                            explode.position = new Vector2(missile.bounds.X, missile.bounds.Y);
                        }
                        else
                        {
                            explode.position = new Vector2(missile.bounds.X - missile.bounds.Width, missile.bounds.Y);
                        }
                        explode.Animate(gameTime);

                        //Stops the animaton of the explosion after a certain amount of frames
                        if (explode.currentFrame == stopExplodeAnim)
                        {
                            //Stops animation of the explosion
                            isExplode = false;

                            //Alternates the player's turn and missile's location
                            if (isP1Turn)
                            {
                                isP1Turn = false;
                            }
                            else
                            {
                                isP1Turn = true;
                            }
                            missile.player = DeterminePlayerTurn();

                            //Changes the missile location relative to player's turn
                            missile.ChangePosition(isP1Turn);

                            //Mouse input of angle and velocity enabled again for next player
                            isClicked = false;
                        }
                    }

                    //Ends the game when three rounds have passed
                    if (roundNum >= MAX_ROUND_NUM)
                    {
                        //Stops the background music
                        MediaPlayer.Stop();

                        //Plays victory sounf effect
                        victoryInstance.Play();

                        //Changes gamestate to gameover state
                        currentGameState = GameState.GameOver;
                    }
                    break;

                case GameState.GameOver:
                    //Checks to see which player has won the game
                    if (isScoreCalc)
                    {
                        //Updates the scoreboard based on who won
                        if (players[0].playerScore > players[1].playerScore)
                        {
                            CalcScoreBoard(0);
                        }
                        else
                        {
                            CalcScoreBoard(1);
                        }
                    }

                    //Checks to see if the return to main menu button was clicked
                    if (mainMenuButton.ButtonClicked(mouse) && prevMouse.LeftButton == ButtonState.Released)
                    {
                        //Resets the game values and returns user to main meny
                        Reset();

                        //Plays the main menu music
                        MediaPlayer.Play(mainMenuMusic);

                        //Changes gamestate back to main menu
                        currentGameState = GameState.MainMenu;
                    }
                    
                    prevMouse = mouse;
                    break;
                case GameState.ScoreBoard:
                    //Returns user to main menu after pressing escape key
                    if (kb.IsKeyDown(Keys.Escape))
                    {
                        currentGameState = GameState.MainMenu;
                    }
                    break;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            //Draws the game based on gamestate its in
            switch (currentGameState)
            {
                case GameState.MainMenu:
                    //Draws the background and title of the game
                    mainMenu.Draw(spriteBatch);
                    titleScreen.DrawString(spriteBatch);

                    //Draws the menu buttons
                    startButton.Draw(spriteBatch);
                    quitButton.Draw(spriteBatch);
                    scoreBoardButton.Draw(spriteBatch);
                    break;

                case GameState.LoadingScreen:
                    //Draws the background
                    mainMenu.Draw(spriteBatch);

                    //Draws the user inputs for the players during loading screen
                    spriteBatch.DrawString(defaultFont, "Input P1 Name:", new Vector2(200, 100), Color.BlanchedAlmond);
                    spriteBatch.DrawString(defaultFont, players[0].playerName, new Vector2(500, 100), Color.Wheat);
                    if (hasP1Enter)
                    {
                        spriteBatch.DrawString(defaultFont, "Input P2 Name:", new Vector2(200, 200), Color.Tomato);
                        spriteBatch.DrawString(defaultFont, players[1].playerName, new Vector2(500, 200), Color.Tomato);
                    }
                    if (hasP2Enter)
                    {
                        spriteBatch.DrawString(defaultFont, "Input Gravity:", new Vector2(200, 300), Color.Teal);
                        spriteBatch.DrawString(defaultFont, gravityNum, new Vector2(500, 300), Color.Teal);

                        //Draws the Enter key tips 
                        enterKeyCont.Draw(spriteBatch);
                    }
                    break;

                case GameState.InGame:
                    //Draws the in game background
                    spriteBatch.Draw(skiesBg, bgLocl, Color.White);
                    spriteBatch.Draw(skiesBg, bgLoc2, Color.White);

                    //Draws the building scapes and players
                    for (int i = 0; i < buildings.Length; i++)
                    {
                        buildings[i].Draw(spriteBatch);
                    }
                    players[1].Draw(spriteBatch);
                    players[0].Draw(spriteBatch);

                    //Draws the HUD of the game when missile isn't fired
                    if (!isMissileFired)
                    {
                        //Draws the gameplay's information for the user to see
                        angleHUD.DrawString(spriteBatch);
                        velocityHUD.DrawString(spriteBatch);
                        spriteBatch.DrawString(defaultFont, DeterminePlayerTurn().playerName + " Turn", new Vector2(400, 0), Color.Orange);
                        tabKeyScores.Draw(spriteBatch);

                        //Draws the current scores when user presses tab
                        if (kb.IsKeyDown(Keys.Tab))
                        {
                            scoreBackDrop.Draw(spriteBatch);
                            for (int i = 0; i < NUM_PLAYERS; i++)
                            {
                                spriteBatch.DrawString(defaultFont, players[i].playerName + " Score:" + players[i].playerScore, new Vector2(400, 240 + (i * 50)), Color.Goldenrod);
                            }
                        }
                    }

                    //Draws the state of the projectile
                    if (!isExplode)
                    {
                        missile.Draw(spriteBatch, missile.angle, isP1Turn);
                    }
                    else
                    {
                        explode.Draw(spriteBatch);
                    }
                    break;

                case GameState.GameOver:
                    //Draws the background and main menu button
                    mainMenu.Draw(spriteBatch);
                    mainMenuButton.Draw(spriteBatch);

                    //Draws all the information that is displayed when the game is over                  
                    spriteBatch.DrawString(defaultFont, "Game Over!", new Vector2(370, 90), Color.DarkKhaki);
                    for (int i = 0; i < NUM_PLAYERS; i++)
                    {
                        spriteBatch.DrawString(defaultFont, players[i].playerName + " Score:" + players[i].playerScore, new Vector2(365, 160 + (i * 50)), Color.Orange);
                    }
                    spriteBatch.DrawString(defaultFont, WinnerName() + " Wins!", new Vector2(390, 275), Color.DarkSalmon);
                    spriteBatch.DrawString(defaultFont, "Winner Score:" + Convert.ToString(winnerScore), new Vector2(295, 330), Color.Wheat);
                    break;

                case GameState.ScoreBoard:
                    //Draws the background and escape key text
                    mainMenu.Draw(spriteBatch);
                    escapeMainMenu.Draw(spriteBatch);

                    //Draws the leaderboard
                    for (int i = 0; i < MAX_NUM_BOARD; i++)
                    {
                        scoreNamesTxt[i].DrawString(spriteBatch);
                        scoreNumsTxt[i].DrawString(spriteBatch);
                    }
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
        /// <summary>
        /// Detects for Collision between rectangles
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public bool RecCollision(Rectangle r1, Rectangle r2)
        {

            if (r1.Bottom < r2.Top || r1.Top > r2.Bottom || r1.Right < r2.Left || r1.Left > r2.Right)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        /// <summary>
        /// Determines who's turn is it currently
        /// </summary>
        /// <returns></returns>
        public Player DeterminePlayerTurn()
        {

            //Checks and changes the player's turn
            if (isP1Turn)
            {
                return players[0];
            }
            else
            {
                return players[1];
            }

        }
        /// <summary>
        /// Checks for collisions between missiles and obstacles and changes gameplay based on that
        /// </summary>
        public void CheckCollision()
        {

            //Creates the collision rectangle for the missile based on player's turn
            Rectangle missileCollide;
            if (!isP1Turn)
            {
                missileCollide = new Rectangle(missile.bounds.X - missile.bounds.Width, 
                                               missile.bounds.Y, 
                                               missile.bounds.Width, 
                                               missile.bounds.Height);
            }
            else
            {
                missileCollide = missile.bounds;
            }

            //Creates the collision rectangles for all targets in game
            Rectangle[] obstacles = new Rectangle[players.Length + buildings.Length];
            obstacles[0] = new Rectangle((int)players[0].position.X,
                                         (int)players[0].position.Y,
                                         players[0].frameSize.X,
                                         players[0].frameSize.Y);
            obstacles[1] = new Rectangle((int)players[1].position.X,
                                         (int)players[1].position.Y,
                                         players[1].frameSize.X,
                                         players[1].frameSize.Y);
            for (int i = players.Length; i < buildings.Length + NUM_PLAYERS; i++)
            {
                obstacles[i] = buildings[i - NUM_PLAYERS].bounds;
            }

            //Detects for missile collision with either obtacles or boundaries
            for (int i = 0; i < obstacles.Length; i++)
            {
                if (RecCollision(missileCollide, obstacles[i]) || missile.bounds.Y >= screenHeight || missileCollide.X <= -missile.bounds.Width || missileCollide.X >= screenWidth)
                {
                    //Makes the missile explode
                    explosionSoundInstance.Play();
                    isMissileFired = false;
                    isExplode = true;

                    //Resets the missile trajectory
                    missileTracj = Vector2.Zero;

                    //Checks to see if missile hits the turrets and changes the round
                    if (RecCollision(missileCollide, obstacles[0]) || RecCollision(missileCollide, obstacles[1]))
                    {
                        //Inceeases the round number played
                        roundNum++;

                        //Updates player's score based on the player hit and who done it
                        if (isP1Turn)
                        {
                            if (RecCollision(missileCollide, obstacles[1]))
                            {
                                scorePointInstance.Play();
                                players[0].playerScore++;
                            }
                            else if (RecCollision(missileCollide, obstacles[0]))
                            {
                                turretHitInstance.Play();
                                players[1].playerScore++;
                            }
                        }
                        else
                        {
                            if (RecCollision(missileCollide, obstacles[0]))
                            {
                                scorePointInstance.Play();
                                players[1].playerScore++;
                            }
                            else if (RecCollision(missileCollide, obstacles[1]))
                            {
                                turretHitInstance.Play();
                                players[0].playerScore++;
                            }
                        }

                        //Rebuilds terrain and player's position after round has passed
                        MakeBuildingScape();
                        PlacePlayers();
                    }
                }
            }

        }
        /// <summary>
        /// Randomizes the turret's location after winning round
        /// </summary>
        public void PlacePlayers()
        {

            //Loops twice for both players to have their new positions
            for (int i = 0; i < NUM_PLAYERS; i++)
            {
                //Produces a random number for the spawning of players
                int rdmPlacement = rng.Next(1, 4);

                //Changes the both player's position randomly on the buildings of up to three from the screen width
                switch (rdmPlacement)
                {
                    case 1:
                        if (i == 0)
                        {
                            players[i].position = new Vector2(buildings[0].bounds.X,
                                                              screenHeight - buildings[0].bounds.Height - players[i].frameSize.Y);
                        }
                        else
                        {
                            players[i].position = new Vector2(buildings[buildings.Length - 1].bounds.Right - players[i].frameSize.X,
                                                              screenHeight - buildings[buildings.Length - 1].bounds.Height - players[i].frameSize.Y);
                        }
                        break;
                    case 2:
                        if (i == 0)
                        {
                            players[i].position = new Vector2(buildings[1].bounds.X,
                                                              screenHeight - buildings[1].bounds.Height - players[i].frameSize.Y);
                        }
                        else
                        {
                            players[i].position = new Vector2(buildings[buildings.Length - 2].bounds.Right - players[i].frameSize.X,
                                                              screenHeight - buildings[buildings.Length - 2].bounds.Height - players[i].frameSize.Y);
                        }
                        break;
                    case 3:
                        if (i == 0)
                        {
                            players[i].position = new Vector2(buildings[2].bounds.X,
                                                              screenHeight - buildings[2].bounds.Height - players[i].frameSize.Y);
                        }
                        else
                        {
                            players[i].position = new Vector2(buildings[buildings.Length - 3].bounds.Right - players[i].frameSize.X,
                                                              screenHeight - buildings[buildings.Length - 3].bounds.Height - players[i].frameSize.Y);
                        }
                        break;
                }

                //Changes missile's position based on player's position
                missile.ChangePosition(isP1Turn);
            }

        }
        /// <summary>
        /// Randomizes building location 
        /// </summary>
        public void MakeBuildingScape()
        {

            //Stores the sum of widths of buildings as each is added
            int buildingWidthSum = 0;

            for (int i = 0; i < buildings.Length; i++)
            {
                //Randomizes which building type is to be spawned
                int rdmBuilding = rng.Next(0, buildingPics.Length);

                Rectangle buildingRec;

                //Creates the new building scape from left to right
                buildingRec = new Rectangle(buildingWidthSum, screenHeight - buildingPics[rdmBuilding].Height, buildingPics[rdmBuilding].Width, buildingPics[rdmBuilding].Height);
                buildings[i] = new Building(buildingPics[rdmBuilding], buildingRec);

                //Calculates the sum of the building's width as buildigns are added
                buildingWidthSum += buildings[i].bounds.Width;
            }

        }
        /// <summary>
        /// Gets text input from keyboard
        /// </summary>
        /// <returns></returns>
        public string TextInput()
        {

            //Checks for keyboard presses and returns a letter based on keyboard input
            if (kb.IsKeyDown(Keys.A) && prevKb.IsKeyUp(Keys.A))
            {
                prevKb = kb;
                return "A";
            }
            else if (kb.IsKeyDown(Keys.B) && prevKb.IsKeyUp(Keys.B))
            {
                prevKb = kb;
                return "B";
            }
            else if (kb.IsKeyDown(Keys.C) && prevKb.IsKeyUp(Keys.C))
            {
                prevKb = kb;
                return "C";
            }
            else if (kb.IsKeyDown(Keys.D) && prevKb.IsKeyUp(Keys.D))
            {
                prevKb = kb;
                return "D";
            }
            else if (kb.IsKeyDown(Keys.E) && prevKb.IsKeyUp(Keys.E))
            {
                prevKb = kb;
                return "E";
            }
            else if (kb.IsKeyDown(Keys.F) && prevKb.IsKeyUp(Keys.F))
            {
                prevKb = kb;
                return "F";
            }
            else if (kb.IsKeyDown(Keys.G) && prevKb.IsKeyUp(Keys.G))
            {
                prevKb = kb;
                return "G";
            }
            else if (kb.IsKeyDown(Keys.H) && prevKb.IsKeyUp(Keys.H))
            {
                prevKb = kb;
                return "H";
            }
            else if (kb.IsKeyDown(Keys.I) && prevKb.IsKeyUp(Keys.I))
            {
                prevKb = kb;
                return "I";
            }
            else if (kb.IsKeyDown(Keys.J) && prevKb.IsKeyUp(Keys.J))
            {
                prevKb = kb;
                return "J";
            }
            else if (kb.IsKeyDown(Keys.K) && prevKb.IsKeyUp(Keys.K))
            {
                prevKb = kb;
                return "K";
            }
            else if (kb.IsKeyDown(Keys.L) && prevKb.IsKeyUp(Keys.L))
            {
                prevKb = kb;
                return "L";
            }
            else if (kb.IsKeyDown(Keys.M) && prevKb.IsKeyUp(Keys.M))
            {
                prevKb = kb;
                return "M";
            }
            else if (kb.IsKeyDown(Keys.N) && prevKb.IsKeyUp(Keys.N))
            {
                prevKb = kb;
                return "N";
            }
            else if (kb.IsKeyDown(Keys.O) && prevKb.IsKeyUp(Keys.O))
            {
                prevKb = kb;
                return "O";
            }
            else if (kb.IsKeyDown(Keys.P) && prevKb.IsKeyUp(Keys.P))
            {
                prevKb = kb;
                return "P";
            }
            else if (kb.IsKeyDown(Keys.Q) && prevKb.IsKeyUp(Keys.Q))
            {
                prevKb = kb;
                return "Q";
            }
            else if (kb.IsKeyDown(Keys.R) && prevKb.IsKeyUp(Keys.R))
            {
                prevKb = kb;
                return "R";
            }
            else if (kb.IsKeyDown(Keys.S) && prevKb.IsKeyUp(Keys.S))
            {
                prevKb = kb;
                return "S";
            }
            else if (kb.IsKeyDown(Keys.T) && prevKb.IsKeyUp(Keys.T))
            {
                prevKb = kb;
                return "T";
            }
            else if (kb.IsKeyDown(Keys.U) && prevKb.IsKeyUp(Keys.U))
            {
                prevKb = kb;
                return "U";
            }
            else if (kb.IsKeyDown(Keys.V) && prevKb.IsKeyUp(Keys.V))
            {
                prevKb = kb;
                return "V";
            }
            else if (kb.IsKeyDown(Keys.W) && prevKb.IsKeyUp(Keys.W))
            {
                prevKb = kb;
                return "W";
            }
            else if (kb.IsKeyDown(Keys.X) && prevKb.IsKeyUp(Keys.X))
            {
                prevKb = kb;
                return "X";
            }
            else if (kb.IsKeyDown(Keys.Y) && prevKb.IsKeyUp(Keys.Y))
            {
                prevKb = kb;
                return "Y";
            }
            else if (kb.IsKeyDown(Keys.Z) && prevKb.IsKeyUp(Keys.Z))
            {
                prevKb = kb;
                return "Z";
            }
            else
            {
                return "";
            }

        }
        /// <summary>
        /// Gets number input from keypad
        /// </summary>
        /// <returns></returns>
        public string NumInput()
        {

            //Chacks for key presses and returns a number based on numpad pressed
            if (kb.IsKeyDown(Keys.NumPad0) && prevKb.IsKeyUp(Keys.NumPad0))
            {
                prevKb = kb;
                return "0";
            }
            else if (kb.IsKeyDown(Keys.NumPad1) && prevKb.IsKeyUp(Keys.NumPad1))
            {
                prevKb = kb;
                return "1";
            }
            else if (kb.IsKeyDown(Keys.NumPad2) && prevKb.IsKeyUp(Keys.NumPad2))
            {
                prevKb = kb;
                return "2";
            }
            else if (kb.IsKeyDown(Keys.NumPad3) && prevKb.IsKeyUp(Keys.NumPad3))
            {
                prevKb = kb;
                return "3";
            }
            else if (kb.IsKeyDown(Keys.NumPad4) && prevKb.IsKeyUp(Keys.NumPad4))
            {
                prevKb = kb;
                return "4";
            }
            else if (kb.IsKeyDown(Keys.NumPad5) && prevKb.IsKeyUp(Keys.NumPad5))
            {
                prevKb = kb;
                return "5";
            }
            else if (kb.IsKeyDown(Keys.NumPad6) && prevKb.IsKeyUp(Keys.NumPad6))
            {
                prevKb = kb;
                return "6";
            }
            else if (kb.IsKeyDown(Keys.NumPad7) && prevKb.IsKeyUp(Keys.NumPad7))
            {
                prevKb = kb;
                return "7";
            }
            else if (kb.IsKeyDown(Keys.NumPad8) && prevKb.IsKeyUp(Keys.NumPad8))
            {
                prevKb = kb;
                return "8";
            }
            else if (kb.IsKeyDown(Keys.NumPad9) && prevKb.IsKeyUp(Keys.NumPad9))
            {
                prevKb = kb;
                return "9";
            }
            else
            {
                return "";
            }

        }
        /// <summary>
        /// Determines the winner's name after game is over
        /// </summary>
        /// <returns></returns>
        public string WinnerName()
        {

            //Returns the either player's name based on who won the game
            if (players[0].playerScore > players[1].playerScore)
            {
                return players[0].playerName;
            }
            else
            {
                return players[1].playerName;
            }

        }
        /// <summary>
        /// Updates the scoreboard based on winner's score
        /// </summary>
        /// <param name="winnerNum"></param>
        public void CalcScoreBoard(int winnerNum)
        {

            //Calculates the winner's score to a minimum of zero
            winnerScore = Math.Max(0, MAX_SCORE - players[winnerNum].playerLaunches);

            //Stores the current index of the scoreboard's rank being investigated
            int player = 0;

            //Loops check for winner's score on scoreboard 
            while (player < MAX_NUM_BOARD)
            {
                //Checks to see if the winner's score can fit on the scoreboard
                if (winnerScore < Convert.ToInt32(scoreNumsTxt[player].text))
                {
                    //Increments rank's index if winner's score is less than current index
                    player++;
                }
                else
                {
                    //Moves the latest ranks down to make room for the new winner's rank
                    for (int i = MAX_NUM_BOARD - 1; i > player; i--)
                    {
                        scoreNamesTxt[i].text = scoreNamesTxt[i - 1].text;
                        scoreNumsTxt[i].text = scoreNumsTxt[i - 1].text;
                    }

                    //Sets the winner's rank into the scoreboard based based on winner's score
                    scoreNamesTxt[player].text = players[winnerNum].playerName;
                    scoreNumsTxt[player].text = Convert.ToString(winnerScore);

                    //Sets the scoreboard check to false to prevent repeating checks
                    isScoreCalc = false;
                    break;
                }
            }

        }
        /// <summary>
        /// Resets all values regarding the gameplay 
        /// </summary>
        public void Reset()
        {

            //Resets the projectile values to default
            gravityNum = "";
            gravity = new Vector2(0, DEFAULT_GRAVITY);
            missileTracj = Vector2.Zero;

            //Sets the missile's turn back to player 1
            missile.player = players[0];

            //Resets the states of the game's setting to default;
            isScoreCalc = true;
            isP1Turn = true;
            isFiring = false;
            isClicked = false;
            isExplode = false;

            //Resets the settings for the loading screen
            playerEnter = true;
            hasP1Enter = false;
            hasP2Enter = false;

            //Resets the stored winner's score and and thw number of rounds passed          
            winnerScore = 0;
            roundNum = 0;

            //Resets the values stored for the current players
            for (int i = 0; i < NUM_PLAYERS; i++)
            {
                players[i].playerName = "";
                players[i].playerScore = 0;
                players[i].playerLaunches = 0;
            }

        }
    }
}
