using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Fog_Project.Utilities;

namespace Fog_Project
{
    class Player
    {
        #region Properties
        private Vector3 position;

        /// <summary>
        /// The current position of the player, and therefor the camera.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return position;
            }
        }
        #endregion
        public Player()
        {
            position = Vector3.Zero;
        }
    }
}
