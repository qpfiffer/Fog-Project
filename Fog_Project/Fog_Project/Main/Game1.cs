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
        const int DEFAULT_WINDOW_WIDTH = 1440;
        const int DEFAULT_WINDOW_HEIGHT = 900;
        const bool AA_DEFAULT_ON = true;
        const bool FULLSCREEN_DEFAULT_ON = false;
        #endregion

        #region INPUT
        KeyboardState oldKBDState;
        KeyboardState curKBDState;

        MouseState oldMouseState;
        MouseState curMouseState;
        #endregion

        #region GAMESTATE_MANAGEMENT
        GameState gameState;
        #endregion

        #region MENU
        Menu mainMenu;
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

            graphics.PreferredBackBufferWidth = DEFAULT_WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = DEFAULT_WINDOW_HEIGHT;
            graphics.PreferMultiSampling = AA_DEFAULT_ON;
            graphics.IsFullScreen = FULLSCREEN_DEFAULT_ON;
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
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            switch (gameState)
            {
                case GameState.menu:
                    mainMenu.Update(gameTime);
                    break;
                case GameState.loading:
                    break;
                case GameState.game:
                    break;
            }

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
                    break;
                case GameState.game:
                    break;
            }

            base.Draw(gameTime);
        }
    }
}
