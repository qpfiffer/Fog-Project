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
    class Junction: GameObject
    {
        #region Fields
        private Dictionary<BoundingBox, Junction> exits;
        #endregion

        #region Properties
        #endregion
        public void Load(ContentManager gManager, GraphicsDevice gDevice, string modelName)
        {
            // If we created this junction using the wrong constructor our rotations will be
            // all sorts of fucked up.
            if (this.leftRightRot != 0.0f && this.upDownRot != 0.0f)
            {
                throw new Exception("This junction created using the wrong constructor!");
            }

            exits = new Dictionary<BoundingBox, Junction>();

            material = ModelUtil.CreateGlobalEffect(gDevice);
            model = new MetaModel();
            // Rotation and Position of a model are set in the constructor. Hopefully.
            model.model = gManager.Load<Model>("Models/Junctions/" + modelName);
            model.Texture = gManager.Load<Texture2D>("Textures/Junctions/" + modelName);
        }

        public override void Update(GameTime gTime)
        {
            base.Update(gTime);
        }

        public void Draw(GraphicsDevice gDevice)
        {        
            ModelUtil.DrawModel(model, material);
        }
    }
}
