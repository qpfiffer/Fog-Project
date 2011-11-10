using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Fog_Project.Utilities;
using Fog_Project.World;

namespace Fog_Project
{
    public enum GameState
    {
        menu, loading, game
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region CONSTANTS      
        const int DEFAULT_WINDOW_WIDTH = 1280;
        const int DEFAULT_WINDOW_HEIGHT = 720;
        const bool AA_DEFAULT_ON = true;
        const bool FULLSCREEN_DEFAULT_ON = true;
        #endregion

        #region INPUT
        InputInfo inputInfo;
        #endregion

        #region GAMESTATE_MANAGEMENT
        GameState gameState;
        #endregion

        #region MENU
        Menu mainMenu;
        #endregion

        #region WORLD
        World.World mWorld;
        #endregion

        #region FONTS
        SpriteFont mainFont;
        #endregion

        #region MISC
        Random WOLOLO;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            if (FULLSCREEN_DEFAULT_ON)
            {
                graphics.IsFullScreen = FULLSCREEN_DEFAULT_ON;
                graphics.PreferredBackBufferWidth = 
                    GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight =
                    GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                graphics.PreferredBackBufferWidth = DEFAULT_WINDOW_WIDTH;
                graphics.PreferredBackBufferHeight = DEFAULT_WINDOW_HEIGHT;
            }

            graphics.PreferMultiSampling = AA_DEFAULT_ON;            
            graphics.ApplyChanges();

            gameState = GameState.menu;

            Window.Title = "Fog Project";

            WOLOLO = new Random();
        }

        protected override void Initialize()
        {
            // YOUR MOTHER! HA HA HA!
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            mainFont = Content.Load<SpriteFont>("Fonts/mainFont");

            mainMenu = new Menu(GraphicsDevice, "FOG PROJECT");
            mainMenu.Load(Content);

            inputInfo = new InputInfo();
            inputInfo.oldKBDState = Keyboard.GetState();
            inputInfo.oldMouseState = Mouse.GetState();
        }

        protected override void UnloadContent()
        {
        }

        private void loadWorld()
        {
            gameState = GameState.loading;
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2,
                GraphicsDevice.Viewport.Height / 2);
            inputInfo.oldMouseState = Mouse.GetState();
            this.mWorld = new World.World();
            mWorld.Load(Content, GraphicsDevice);
            gameState = GameState.game;
        }

        protected override void Update(GameTime gameTime)
        {
            #region INPUT_UPDATE
            inputInfo.curKBDState = Keyboard.GetState();
            inputInfo.curMouseState = Mouse.GetState();
            inputInfo.timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            #endregion

            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            switch (gameState)
            {
                case GameState.menu:
                    mainMenu.handleInput(ref inputInfo);
                    mainMenu.Update(gameTime);

                    if (mainMenu.Flag == MenuFlags.quit)
                        this.Exit();
                    else if (mainMenu.Flag == MenuFlags.startGame)
                        loadWorld();

                    break;
                case GameState.loading:
                    break;
                case GameState.game:
                    mWorld.handleInput(ref inputInfo);
                    mWorld.Update(gameTime);
                    break;
            }

            #region INPUT_UPDATE
            // Do not update the mouse state. The one we have here is the center of the screen.
            //inputInfo.oldMouseState = inputInfo.curMouseState;
            inputInfo.oldKBDState = inputInfo.curKBDState;
            #endregion

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightCyan);

            switch (gameState)
            {
                case GameState.menu:
                    mainMenu.Draw(spriteBatch);
                    break;
                case GameState.loading:                   
                    spriteBatch.Begin();
                    Vector2 stringSize = mainFont.MeasureString("Loading...");
                    spriteBatch.DrawString(mainFont, "Loading...", new Vector2(GraphicsDevice.Viewport.Width / 2 - stringSize.X,
                        GraphicsDevice.Viewport.Height / 2 - stringSize.Y), Color.Black);
                    spriteBatch.End();
                    break;
                case GameState.game:
                    mWorld.Draw();
#if DEBUG
                    spriteBatch.Begin();
                    spriteBatch.DrawString(mainFont, "LR Rot: " + mWorld.MPlayer.LeftRightRot, new Vector2(1, 0), Color.Black);
                    spriteBatch.DrawString(mainFont, "UD Rot: " + mWorld.MPlayer.UpDownRot, new Vector2(1, 20), Color.Black);
                    spriteBatch.DrawString(mainFont, " X Pos: " + mWorld.MPlayer.Position.X, new Vector2(1, 40), Color.Black);
                    spriteBatch.DrawString(mainFont, " Z Pos: " + mWorld.MPlayer.Position.Z, new Vector2(1, 60), Color.Black);
                    spriteBatch.End();
#endif
                    break;
            }

            base.Draw(gameTime);
        }
    }
}
