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
        // All junctions are kept here:
        List<Junction> junctions;
        // Spawned junctions:
        List<Junction> spawnedJunctions;
        // Any random models that are needed:
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

            mainPlayer = new Player(ref playerPos, ref playerRot, gDevice);
            junctions = new List<Junction>();
            spawnedJunctions = new List<Junction>();
            modelsToDraw = new List<MetaModel>();
        }

        public void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            this.gDevice = gDevice;
            this.gManager = gManager;                    

            SetupJunctions(gManager, gDevice);
            Setup3D(gDevice);            
        }

        private void Setup3D(GraphicsDevice gDevice)
        {
            // Get a unified global effect from the model util thing:
            globalEffect = ModelUtil.CreateGlobalEffect(gDevice);
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
            globalEffect.World = cMatrices.world;
            globalEffect.Projection = cMatrices.proj;

            // Finally place it where it needs to be:
            mainPlayer.Matrices = cMatrices;
        }

        private void SetupJunctions(ContentManager gManager, GraphicsDevice gDevice)
        {
            Random tRandom = new Random();
            int randomJunctionNum = tRandom.Next(5, 10);
            for (int i = 0; i < randomJunctionNum; i++)
            {
                const int X_JUNCTION_RANGE = 3;
                const int Y_JUNCTION_RANGE = 3;
                const int JUNCTION_RANGE_MULTIPLIER = 10;

                // This little gem here gets the number of different types in an 
                // enum:
                int enumCount = Enum.GetValues(typeof(JunctionType)).Length;
                // Pick a random one:
                int newJunctionType = tRandom.Next(enumCount);
                // Create a new junction, the info if which we will fill out
                // in a second:
                Vector3 positionVector = new Vector3(
                    tRandom.Next(-X_JUNCTION_RANGE, X_JUNCTION_RANGE) * JUNCTION_RANGE_MULTIPLIER,
                    0,
                    tRandom.Next(-Y_JUNCTION_RANGE, Y_JUNCTION_RANGE) * JUNCTION_RANGE_MULTIPLIER);
                Vector3 rotationVector = Vector3.Zero;
                Junction newJunction = new Junction(ref positionVector,
                    ref rotationVector,
                    gDevice);

                switch (newJunctionType)
                {
                    // Single:
                    case 0:
                        newJunction.Type = JunctionType.single;
                        newJunction.Load(gManager, "junctionSingle");
                        junctions.Add(newJunction);
                        break;
                    // Corner:
                    case 1:
                        newJunction.Type = JunctionType.corner;
                        newJunction.Load(gManager, "junctionCorner");
                        junctions.Add(newJunction);
                        break;
                    // Triple:
                    case 2:
                        newJunction.Type = JunctionType.triple;
                        newJunction.Load(gManager, "junctionT");
                        junctions.Add(newJunction);
                        break;
                    // Quad:
                    case 3:
                        newJunction.Type = JunctionType.quad;
                        newJunction.Load(gManager, "junctionQuad");
                        junctions.Add(newJunction);
                        break;
                    default:
                        newJunction.Type = JunctionType.triple;
                        newJunction.Load(gManager, "junctionT");
                        junctions.Add(newJunction);
                        break;
                }
            }
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

#if DEBUG
            if (info.curKBDState.IsKeyDown(Keys.N) &&
                info.oldKBDState.IsKeyUp(Keys.N))
            {
                mainPlayer.NoClip = !mainPlayer.NoClip;
            }
#endif

            if (info.curKBDState.IsKeyDown(Keys.F) &&
                info.oldKBDState.IsKeyUp(Keys.F))
            {
                foreach (Junction junction in junctions)
                {
                    junction.ToggleFog();
                }
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
