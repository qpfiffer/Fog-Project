﻿using System;
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

        public void handleInput(ref InputInfo info)
        {
            if (info.curKBDState.IsKeyDown(Keys.E) &&
                info.oldKBDState.IsKeyUp(Keys.E))
            {
                // Reserved for interacting with objects
            }

            if (info.curKBDState.IsKeyDown(Keys.W) &&
                info.oldKBDState.IsKeyUp(Keys.W))
            {
            }

            if (info.curMouseState != info.oldMouseState)
            {
                float xDelta = info.curMouseState.X - info.oldMouseState.X;
                float yDelta = info.curMouseState.Y - info.oldMouseState.Y;

                Mouse.SetPosition(gDevice.Viewport.Width / 2, gDevice.Viewport.Height / 2);
            }
        }
    }
}
