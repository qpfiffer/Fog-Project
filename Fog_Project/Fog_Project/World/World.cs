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
        bool justTeleported;
        bool releaseMouseToggle;
        #endregion

        #region World
        Junction currentJunction;
        // All junctions are kept here:
        List<Junction> junctions;
        // Any random models that are needed:
        List<MetaModel> modelsToDraw;
        #endregion

        #region Debug
        public int JunctionID { get { return currentJunction.RandID; } }
        #endregion
        public World()
        {
            Vector3 playerPos = new Vector3(0,Player.chestHeight,3.0f);
            Vector2 playerRot = new Vector2(0.0f, 0.0f);

            rState = new RasterizerState();
            rState.FillMode = FillMode.Solid;
            rState.CullMode = CullMode.CullCounterClockwiseFace;
            rState.ScissorTestEnable = true;

            mainPlayer = new Player(ref playerPos, ref playerRot, gDevice);
            junctions = new List<Junction>();
            modelsToDraw = new List<MetaModel>();
            justTeleported = false;
            releaseMouseToggle = false;
        }

        public void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            this.gDevice = gDevice;
            this.gManager = gManager;                    

            SetupJunctions(gManager, gDevice);
            // Move the player to the first junction:
            mainPlayer.Position = new Vector3(junctions[0].Position.X,
                Player.chestHeight, junctions[0].Position.Z);
            // Make sure the game knows which junction we are
            // standing on:
            currentJunction = junctions[0];

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
            #region Build_Junctions
            for (int i = 0; i < randomJunctionNum; i++)
            {
                const int X_JUNCTION_RANGE = 7;
                const int Y_JUNCTION_RANGE = 7;
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
                        break;
                    // Corner:
                    case 1:
                        newJunction.Type = JunctionType.corner;
                        newJunction.Load(gManager, "junctionCorner");
                        break;
                    // Triple:
                    case 2:
                        newJunction.Type = JunctionType.triple;
                        newJunction.Load(gManager, "junctionT");
                        break;
                    // Quad:
                    case 3:
                        newJunction.Type = JunctionType.quad;
                        newJunction.Load(gManager, "junctionQuad");
                        break;
                    default:
                        newJunction.Type = JunctionType.triple;
                        newJunction.Load(gManager, "junctionT");
                        break;
                }
                // Add it to the list.
                junctions.Add(newJunction);
            }
            #endregion

            #region Build_Portals
            foreach (Junction junction in junctions)
            {
                List<Junction> portalsToAdd = new List<Junction>();
                // Every junction except single has the same number of sides
                // per its enum type.
                int compare = (int)junction.Type + 1;
                
                // The enum is a little fucked up so we do it this way:
                if (junction.Type == JunctionType.single)
                    compare = 2;

                for (int i = 0; i < compare; i++)
                {
                    // Pick a random junction to the list and add it
                    int index = tRandom.Next(junctions.Count);
                    Junction toAdd = junctions[index];
                    while (toAdd == junction)
                    {
                        // Dirty little thing to do but I've got the processing power.
                        toAdd = junctions[tRandom.Next(junctions.Count)];
                    }
                    portalsToAdd.Add(toAdd);
                }
                junction.addPortalJunctions(portalsToAdd);
            }
            #endregion
        }

        private void collideMove(float amount, Vector3 moveVector)
        {
            // Collisions will go here eventually.
            bool hitPortal = false;
            BoundingBox portalWeHit = new BoundingBox();
            foreach (BoundingBox portal in currentJunction.Portals)
            {
                // So what we do here is to teleport them only if they have
                // hit the portal, and are not still in the portal from a 
                // previous teleport.
                foreach (BoundingSphere playerSphere in mainPlayer.BoundingSpheres)
                {
                    if (playerSphere.Intersects(portal))
                    {
                        // If we are still inside a portal from a previous teleport,
                        // don't go back:
                        if (justTeleported == false)
                        {
                            hitPortal = true;
                            justTeleported = true;
                            portalWeHit = portal;
                            break;
                        }
                    }             
                }

                // No point in continuing if we already hit one:
                if (hitPortal)
                    break;
            }

            if (hitPortal == false)
            {
                // If we got here, we didn't collide with anything and we 
                // should make sure justTeleported it false;
                justTeleported = false;
                Vector3 finalVector = moveVector * amount;
                mainPlayer.addToCameraPosition(ref finalVector);
            }
            else
            {
                // We hit a portal, probably. We should do something about it.
                // Get the destination junction of the portal we hit (where it goes):
                Junction destinationJunction = currentJunction.Exits[portalWeHit];
                // Make sure we know that we are on the new junction:
                currentJunction = destinationJunction;
                // Move the player there:
                BoundingBox destinationPortal = destinationJunction.getRandomPortal();
                // Compute the center of the new destination portal:
                Vector3 portalWallSizes = new Vector3(destinationPortal.Max.X - destinationPortal.Min.X, 
                    destinationPortal.Max.Y - destinationPortal.Min.Y,
                    destinationPortal.Max.Z - destinationPortal.Min.Z);
                Vector3 newPortalCenter = new Vector3(destinationPortal.Min.X + (portalWallSizes.X / 2.0f),
                    destinationPortal.Min.Y + (portalWallSizes.Y / 2.0f),
                    destinationPortal.Min.Z + (portalWallSizes.Z / 2.0f));
                // Compute the center of the old portal (so we can get out offset from it:
                Vector3 oldPortalWallSizes = new Vector3(portalWeHit.Max.X - portalWeHit.Min.X,
                    portalWeHit.Max.Y - portalWeHit.Min.Y,
                    portalWeHit.Max.Z - portalWeHit.Min.Z);
                Vector3 oldPortalCenter = new Vector3(portalWeHit.Min.X + (oldPortalWallSizes.X / 2.0f),
                    portalWeHit.Min.Y + (oldPortalWallSizes.Y / 2.0f),
                    portalWeHit.Min.Z + (oldPortalWallSizes.Z / 2.0f));
                Vector3 offset = oldPortalCenter - mainPlayer.Position;
                // Set the player there. Hopefully everything worked.
                mainPlayer.setCameraPosition(newPortalCenter, offset);
            }
        }

        public void Update(GameTime gTime)
        {
            globalEffect.View = mainPlayer.Matrices.view;
            globalEffect.World = mainPlayer.Matrices.world;
            globalEffect.Projection = mainPlayer.Matrices.proj;

            // Does a lot of sphere creation. Might want to thin it out if it gets slow.
            mainPlayer.Update(gTime);
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

            if (info.curKBDState.IsKeyDown(Keys.M) &&
                info.oldKBDState.IsKeyUp(Keys.M))
            {
                releaseMouseToggle = !releaseMouseToggle;
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


            if (info.curMouseState != info.oldMouseState && !releaseMouseToggle)
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

            // If you want to draw all of the junctions at once, use 
            // this:
            //foreach (Junction junction in junctions)
            //{
            //    junction.updateMatrices(mainPlayer.Matrices);
            //    junction.Draw(gDevice);
            //}

            currentJunction.updateMatrices(mainPlayer.Matrices);
            currentJunction.Draw(gDevice);

            //foreach (MetaModel model in modelsToDraw)
            //{
            //    ModelUtil.DrawModel(model, globalEffect);
            //}
        }
    }
}
