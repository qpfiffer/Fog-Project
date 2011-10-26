using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fog_Project.Utilities
{
    public struct InputInfo
    {
        public KeyboardState oldKBDState;
        public KeyboardState curKBDState;

        public MouseState oldMouseState;
        public MouseState curMouseState;

        public float timeDifference;
    }

    public struct InputInfoXbox
    {
        public GamePadState oldState;
        public GamePadState curState;
    }
}
