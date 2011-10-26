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
        GraphicsDevice gDevice;
        #endregion
        public World()
        {
            mainPlayer = new Player();
        }

        public void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            this.gDevice = gDevice;
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
                float xDelta = info.curMouseState.X - info.oldMouseState.X;
                float yDelta = info.curMouseState.Y - info.oldMouseState.Y;

                Mouse.SetPosition(gDevice.Viewport.Width / 2, gDevice.Viewport.Height / 2);
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
            else if (info.curKBDState.IsKeyUp(Keys.D))
            {
                moveVector.X -= 1;
            }

            if (moveVector != Vector3.Zero)
            {
                collideMove(info.timeDifference, moveVector);
            }
        }
    }
}
