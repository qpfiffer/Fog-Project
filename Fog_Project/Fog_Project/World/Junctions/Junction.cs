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
        private Vector3 rotation;
        private MetaModel model;
        private BasicEffect material;
        #endregion

        #region Properties
        public Vector3 Position
        {
            get { return position; }
        }

        public Vector3 Rotation
        {
            get { return rotation; }
        }
        #endregion

        public Junction(Vector3 position, Vector3 rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }

        public void Load(ContentManager gManager, GraphicsDevice gDevice, string modelName)
        {
            material = ModelUtil.CreateGlobalEffect(gDevice);
            model = new MetaModel();
            model.Rotation = rotation;
            model.Position = position;
            model.model = gManager.Load<Model>("Models/Junctions/" + modelName);
            model.Texture = gManager.Load<Texture2D>("Textures/Junctions/" + modelName);
        }

        public void Update(GameTime gTime)
        {
        }

        public void Draw(GraphicsDevice gDevice)
        {        
            ModelUtil.DrawModel(model, material);
        }
    }
}
