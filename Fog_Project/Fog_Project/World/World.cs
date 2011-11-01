using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Fog_Project.Interfaces;
using Fog_Project.Utilities;

namespace Fog_Project.World
{
    class World: IInputHandler
    {
        #region Player
        Player mainPlayer;
        public Player MPlayer { get { return mainPlayer; } }
        #endregion

        #region GameStuff
        ContentManager gManager;
        GraphicsDevice gDevice;
        BasicEffect globalEffect;
        RasterizerState rState;
        #endregion

        #region World
        List<Junction> junctions;
        List<MetaModel> modelsToDraw;
        #endregion
        public World()
        {
            Vector3 playerPos = new Vector3(0,1.0f,3.0f);
            Vector2 playerRot = new Vector2(0.0f, 0.0f);

            rState = new RasterizerState();
            rState.FillMode = FillMode.Solid;
            rState.CullMode = CullMode.CullCounterClockwiseFace;
            rState.ScissorTestEnable = true;

            mainPlayer = new Player(ref playerPos, ref playerRot);
            junctions = new List<Junction>();
            modelsToDraw = new List<MetaModel>();
        }

        public void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            this.gDevice = gDevice;
            this.gManager = gManager;
            globalEffect = ModelUtil.CreateGlobalEffect(gDevice);

            Junction test = new Junction();
            test.Load(gManager, gDevice, "junctionT");
            test.Position = new Vector3(0, 0, 2.0f);
            junctions.Add(test);

            // We create a matrixDescriptor here becuase we can't set properties of properties:
            MatrixDescriptor cMatrices = mainPlayer.Matrices;
            ModelUtil.UpdateViewMatrix(mainPlayer.UpDownRot, mainPlayer.LeftRightRot, mainPlayer.Position,
                ref cMatrices);
            // Set the pieces of out matrix descriptor:
            cMatrices.proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(75.0f),
                gDevice.Viewport.AspectRatio, 0.3f, 1000.0f);
            cMatrices.world = Matrix.CreateTranslation(Vector3.Zero);

            // Update the global effect:
            globalEffect.View = cMatrices.view;
            globalEffect.World= cMatrices.world;
            globalEffect.Projection= cMatrices.proj;

            // Finally place it where it needs to be:
            mainPlayer.Matrices = cMatrices;
        }

        private void collideMove(float amount, Vector3 moveVector)
        {
            // Collisions will go here eventually.
            Vector3 finalVector = moveVector * amount;
            mainPlayer.addToCameraPosition(ref finalVector);
        }

        public void Update(GameTime gTime)
        {
            globalEffect.View = mainPlayer.Matrices.view;
            globalEffect.World = mainPlayer.Matrices.world;
            globalEffect.Projection = mainPlayer.Matrices.proj;
        }

        public void handleInput(ref InputInfo info)
        {
            if (info.curKBDState.IsKeyDown(Keys.E) &&
                info.oldKBDState.IsKeyUp(Keys.E))
            {
                // Reserved for interacting with objects
            }

            if (info.curMouseState != info.oldMouseState)
            {
                int xDelta = info.curMouseState.X - info.oldMouseState.X;
                int yDelta = info.curMouseState.Y - info.oldMouseState.Y;

                Point deltas = new Point(xDelta, yDelta);
                mainPlayer.rotateCamera(ref deltas, info.timeDifference);

                Mouse.SetPosition(gDevice.Viewport.Width / 2, gDevice.Viewport.Height / 2);
                MatrixDescriptor cMatrices = mainPlayer.Matrices;
                ModelUtil.UpdateViewMatrix(mainPlayer.UpDownRot, mainPlayer.LeftRightRot,
                    mainPlayer.Position, ref cMatrices);
                mainPlayer.Matrices = cMatrices;
            }

            Vector3 moveVector = Vector3.Zero;
            if (info.curKBDState.IsKeyDown(Keys.W))
            {
                moveVector.Z -= 1;
            } 
            else if (info.curKBDState.IsKeyDown(Keys.S)) 
            {
                moveVector.Z += 1;
            }

            if (info.curKBDState.IsKeyDown(Keys.A))
            {
                moveVector.X -= 1;
            }
            else if (info.curKBDState.IsKeyDown(Keys.D))
            {
                moveVector.X += 1;
            }

            if (moveVector != Vector3.Zero)
            {
                collideMove(info.timeDifference, moveVector);
            }
        }

        public void Draw()
        {
            gDevice.DepthStencilState = DepthStencilState.Default;
            gDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.LightCyan, 1.0f, 0);
            gDevice.RasterizerState = rState;

            foreach (Junction junction in junctions)
            {
                junction.updateMatrices(mainPlayer.Matrices);
                junction.Draw(gDevice);
            }

            //foreach (MetaModel model in modelsToDraw)
            //{
            //    ModelUtil.DrawModel(model, globalEffect);
            //}
        }
    }
}
