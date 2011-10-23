using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fog_Project.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Fog_Project.World
{
    /// <summary>
    /// The class all junction pieces inherit from.
    /// </summary>
    class Junction
    {
        #region Fields
        private Vector3 position;
        private MetaModel model;
        private BasicEffect material;
        #endregion

        #region Properties
        public Vector3 Position
        {
            get { return position; }
        }
        #endregion

        public Junction(Vector3 position)
        {
            this.position = position;
        }

        /// <summary>
        /// This function will be overridden in child classes because they need
        /// to load their own models and textures and whatnot.
        /// </summary>
        /// <param name="gManager"></param>
        public virtual void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            material = ModelUtil.CreateGlobalEffect(gDevice);
        }

        public virtual void Update(GameTime gTime)
        {
        }

        public virtual void Draw(GraphicsDevice gDevice)
        {        
            ModelUtil.DrawModel(model, material);
        }
    }
}
