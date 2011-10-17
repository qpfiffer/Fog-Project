using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Fog_Project
{
    public enum MenuFlags { normal, startGame, quit, options };
    class Menu
    {
        #region MENU_STUFF
        string title;
        int selectedEntry;
        List<MenuItem> items;
        SpriteFont mFont;

        public MenuFlags Flag { get; set; }
        #endregion

        #region SCENE_TO_DRAW
        MatrixDescriptor currentMatrices;
        SoundEffect bgSound;
        Vector3 cameraPos;
        float leftRightRot, upDownRot;
        List<MetaModel> modelsToDraw;
        RasterizerState rState;
        GraphicsDevice gDevice;
        #endregion

        public Menu(GraphicsDevice gDevice, string title)
        {
            #region Menu_Stuff
            this.selectedEntry = 0;
            this.gDevice = gDevice;
            this.title = title;
            this.items = new List<MenuItem>();
            this.Flag = MenuFlags.normal;
            #endregion
            #region Scenery
            modelsToDraw = new List<MetaModel>();
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
            #region Menu_Stuff
            mFont = gManager.Load<SpriteFont>("Fonts/mainFont");

            // Set up entries:
            MenuItem begin = new MenuItem("Begin");
            MenuItem quit = new MenuItem("Quit");

            begin.doWork += beginFunc;
            quit.doWork += quitFunc;

            items.Add(begin);
            items.Add(quit);
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

            modelsToDraw.Add(junction);
            modelsToDraw.Add(singleLeft);

            UpdateViewMatrix();
            currentMatrices.proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(75.0f), gDevice.Viewport.AspectRatio, 0.3f, 1000.0f);
            currentMatrices.world = Matrix.CreateTranslation(Vector3.Zero);
            #endregion
        }

        private void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(upDownRot) * Matrix.CreateRotationY(leftRightRot);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = cameraPos + cameraRotatedTarget;

            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

            currentMatrices.view = Matrix.CreateLookAt(cameraPos, cameraFinalTarget, cameraRotatedUpVector);
        }

        public void Update(GameTime gTime)
        {
        }

        public void Draw(SpriteBatch sBatch)
        {
            gDevice.DepthStencilState = DepthStencilState.Default;
            gDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.LightCyan, 1.0f, 0);
            gDevice.RasterizerState = rState;

            // Draw the scenery:
            foreach (MetaModel mModel in modelsToDraw)
            {
                ModelUtil.DrawModel(mModel, currentMatrices);
            }

            // Draw the title of the menu:
            sBatch.Begin();
            Vector2 stringSize = mFont.MeasureString(this.title);
            Vector2 menuTitleCenter = new Vector2((gDevice.Viewport.Width / 2) - (stringSize.X / 2), (gDevice.Viewport.Height / 2) - (stringSize.Y / 2));
            sBatch.DrawString(mFont, this.title, menuTitleCenter, Color.DarkBlue);

            // Draw the menu items:
            for (int i = 0; i < items.Count; i++)
            {
                if (i == selectedEntry)
                    sBatch.DrawString(mFont, items[i].Text, new Vector2(menuTitleCenter.X, menuTitleCenter.Y + 35.0f + (i * 16.0f)), Color.Blue);
                else
                    sBatch.DrawString(mFont, items[i].Text, new Vector2(menuTitleCenter.X, menuTitleCenter.Y + 35.0f + (i * 16.0f)), Color.DarkBlue);
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
