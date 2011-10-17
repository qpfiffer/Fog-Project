using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fog_Project
{
    class MenuItem
    {
        string text;

        public string Text
        {
            get { return text; }
        }

        public delegate void WorkFunc(object o, EventArgs e);
        
        public MenuItem(string text)
        {
            this.text = text;
        }

        public event WorkFunc doWork;
    }
}
