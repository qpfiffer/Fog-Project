using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Fog_Project.Utilities;
using Fog_Project.Interfaces;

namespace Fog_Project
{
    public enum MenuFlags { normal, startGame, quit, options };
    class Menu: IInputHandler
    {
        #region MENU_STUFF
        string title;
        int selectedEntry;
        List<MenuItem> menuItems;
        SpriteFont mFont;

        public MenuFlags Flag { get; set; }
        #endregion

        #region SCENE_TO_DRAW
        MatrixDescriptor currentMatrices;
        SoundEffect bgSound;
        BasicEffect globalEffect;
        Vector3 cameraPos;
        float leftRightRot, upDownRot;
        List<MetaModel> modelsToDraw;
        List<TexturedPlane> oceanTiles;
        RasterizerState rState;
        GraphicsDevice gDevice;
        #endregion

        public Menu(GraphicsDevice gDevice, string title)
        {
            #region Menu_Stuff
            this.selectedEntry = 0;
            this.gDevice = gDevice;
            this.title = title;
            this.menuItems = new List<MenuItem>();
            this.Flag = MenuFlags.normal;
            #endregion
            #region Scenery
            modelsToDraw = new List<MetaModel>();
            oceanTiles = new List<TexturedPlane>();
            cameraPos = new Vector3(0.0f, 1.0f, 3.0f);

            rState = new RasterizerState();
            rState.FillMode = FillMode.Solid;
            rState.CullMode = CullMode.CullCounterClockwiseFace;
            rState.ScissorTestEnable = true;

            leftRightRot = 0.0f;
            upDownRot = MathHelper.ToRadians(-10.0f);
            #endregion
        }

        public void Load(ContentManager gManager)
        {
            #region Basic_Effect
            globalEffect = ModelUtil.CreateGlobalEffect(gDevice);
            #endregion
            #region Menu_Stuff
            mFont = gManager.Load<SpriteFont>("Fonts/mainFont");

            // Set up entries:
            MenuItem begin = new MenuItem("Begin");
            MenuItem quit = new MenuItem("Quit");

            begin.doWork += beginFunc;
            quit.doWork += quitFunc;

            menuItems.Add(begin);
            menuItems.Add(quit);
            #endregion
            #region Scenery
            MetaModel junction = new MetaModel();
            junction.model = gManager.Load<Model>("Models/Junctions/junctionT");
            junction.Position = Vector3.Zero;
            junction.Rotation = Vector3.Zero;
            junction.Texture = gManager.Load<Texture2D>("Textures/Junctions/junction_T");

            MetaModel singleLeft = new MetaModel();
            singleLeft.model = gManager.Load<Model>("Models/Junctions/junctionSingle");
            singleLeft.Position = new Vector3(-5.0f, 0, 0);
            singleLeft.Rotation = Vector3.Zero;
            singleLeft.Texture = gManager.Load<Texture2D>("Textures/Junctions/junctionSingle");

            MetaModel singleRight = new MetaModel();
            singleRight.model = gManager.Load<Model>("Models/Junctions/junctionSingle");
            singleRight.Position = new Vector3(5.0f, 0, 0);
            singleRight.Rotation = Vector3.Zero;
            singleRight.Texture = gManager.Load<Texture2D>("Textures/Junctions/junctionSingle");

            MetaModel bench = new MetaModel();
            bench.model = gManager.Load<Model>("Models/Giblies/bench");
            bench.Position = new Vector3(-1.5f, 0, -0.5f);
            bench.Rotation = Vector3.Zero;
            bench.Texture = gManager.Load<Texture2D>("Textures/Giblies/bench");

            MetaModel benchTwo = new MetaModel();
            benchTwo.model = gManager.Load<Model>("Models/Giblies/bench");
            benchTwo.Position = bench.Position + new Vector3(1.75f, 0, 0);
            benchTwo.Rotation = Vector3.Zero;
            benchTwo.Texture = gManager.Load<Texture2D>("Textures/Giblies/bench");

            const float oceanTileSize = 5.0f;
            for (int x = -2; x < 3; x++)
            {
                for (int y = -2; y < 3; y++)
                {
                    TexturedPlane test = ModelUtil.CreateTexturedPlane(new Vector3(x * oceanTileSize, -0.5f, y * oceanTileSize),
                        new Vector2(oceanTileSize),
                        gManager.Load<Texture2D>("Textures/Ocean/ocean"),
                        gDevice);
                    oceanTiles.Add(test);
                }
            }

            modelsToDraw.Add(junction);
            modelsToDraw.Add(singleLeft);
            modelsToDraw.Add(singleRight);
            modelsToDraw.Add(bench);
            modelsToDraw.Add(benchTwo);

            ModelUtil.UpdateViewMatrix(upDownRot, leftRightRot, ref cameraPos, ref currentMatrices);
            currentMatrices.proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(75.0f), gDevice.Viewport.AspectRatio, 0.3f, 1000.0f);
            currentMatrices.world = Matrix.CreateTranslation(Vector3.Zero);
            globalEffect.View = currentMatrices.view;
            globalEffect.Projection = currentMatrices.proj;
            globalEffect.World = currentMatrices.world;
            #endregion
        }

        public void Update(GameTime gTime)
        {
        }

        public void handleInput(ref InputInfo info)
        {
            #region ENTRY_SELECTION
            if (info.curKBDState.IsKeyDown(Keys.Down) &&
                info.oldKBDState.IsKeyUp(Keys.Down))
            {
                selectedEntry++;
            }

            if (info.curKBDState.IsKeyDown(Keys.Up) &&
                info.oldKBDState.IsKeyUp(Keys.Up))
            {
                selectedEntry--;
            }

            if (selectedEntry < 0)
                selectedEntry = menuItems.Count - 1;
            else if (selectedEntry >= menuItems.Count)
                selectedEntry = 0;
            #endregion

            if (info.curKBDState.IsKeyDown(Keys.Enter) &&
                info.oldKBDState.IsKeyUp(Keys.Enter))
            {
                menuItems[selectedEntry].GetItDone(this);
            }
        }

        public void Draw(SpriteBatch sBatch)
        {
            gDevice.DepthStencilState = DepthStencilState.Default;
            gDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.LightCyan, 1.0f, 0);
            gDevice.RasterizerState = rState;

            // Draw the scenery:
            foreach (MetaModel mModel in modelsToDraw)
            {
                ModelUtil.DrawModel(mModel, globalEffect);
            }

            foreach (TexturedPlane tPlane in oceanTiles)
            {
                ModelUtil.DrawTexturedPlane(tPlane, globalEffect);
            }

            // Draw the title of the menu:
            sBatch.Begin();
            Vector2 stringSize = mFont.MeasureString(this.title);
            Vector2 menuTitleCenter = new Vector2((gDevice.Viewport.Width / 2) - (stringSize.X / 2), (gDevice.Viewport.Height / 4) - (stringSize.Y / 2));
            sBatch.DrawString(mFont, this.title, menuTitleCenter, Color.DarkBlue);

            // Draw the menu items:
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (i == selectedEntry)
                    sBatch.DrawString(mFont, menuItems[i].Text, new Vector2(menuTitleCenter.X, menuTitleCenter.Y + 35.0f + (i * 16.0f)), Color.Blue);
                else
                    sBatch.DrawString(mFont, menuItems[i].Text, new Vector2(menuTitleCenter.X, menuTitleCenter.Y + 35.0f + (i * 16.0f)), Color.DarkBlue);
            }
            sBatch.End();
        }

        void beginFunc(object o, EventArgs e)
        {
            this.Flag = MenuFlags.startGame;
        }

        void quitFunc(object o, EventArgs e)
        {
            this.Flag = MenuFlags.quit;
        }
    }
}
