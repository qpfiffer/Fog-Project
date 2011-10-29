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
        #endregion

        #region World
        List<Junction> junctions;
        #endregion
        public World()
        {
            Vector3 playerPos = new Vector3(0,0,1.0f);
            Vector2 playerRot = new Vector2(0.0f, 0.0f);
            mainPlayer = new Player(ref playerPos, ref playerRot);
            junctions = new List<Junction>();
        }

        public void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            this.gDevice = gDevice;
            this.gManager = gManager;

            Junction test = new Junction();
            test.Load(gManager, gDevice, "junctionT");
            test.Position = new Vector3(0, 0, 2.0f);
            junctions.Add(test);
        }

        private void collideMove(float amount, Vector3 moveVector)
        {
            // Collisions will go here eventually.
            Vector3 finalVector = moveVector * amount;
            mainPlayer.addToCameraPosition(ref finalVector);
        }

        public void Update(GameTime gTime)
        {
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
                ModelUtil.UpdateViewMatrix(mainPlayer.UpDownRot, mainPlayer.LeftRightRot, mainPlayer.Position, ref cMatrices);
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
                moveVector.X += 1;
            }
            else if (info.curKBDState.IsKeyDown(Keys.D))
            {
                moveVector.X -= 1;
            }

            if (moveVector != Vector3.Zero)
            {
                collideMove(info.timeDifference, moveVector);
            }
        }

        public void Draw()
        {
            foreach (Junction junction in junctions)
            {
                junction.updateMatrices(mainPlayer.Matrices);
                junction.Draw(gDevice);
            }
        }
    }
}
