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
    public enum JunctionType { single, corner, triple, quad };
    /// <summary>
    /// The class all junction pieces inherit from.
    /// </summary>
    class Junction: GameObject
    {
        #region Fields
        private Dictionary<BoundingBox, Junction> exits;
        private List<MetaModel> giblies;
        #endregion

        #region Properties
        public JunctionType Type { get; set; }
        #endregion
        public void Load(ContentManager gManager, GraphicsDevice gDevice, string modelName)
        {
            // If we created this junction using the wrong constructor our rotations will be
            // all sorts of fucked up.
            if (this.leftRightRot != 0.0f && this.upDownRot != 0.0f)
            {
                throw new Exception("This junction created using the wrong constructor!");
            }

            // Someone should set this later, but if they don't just default to single:
            this.Type = JunctionType.single;

            exits = new Dictionary<BoundingBox, Junction>();
            // Set up any models we might have on this juction:
            giblies = new List<MetaModel>();
            // Add some random models:
            addRandomModels(gManager);

            material = ModelUtil.CreateGlobalEffect(gDevice);
            // Rotation and Position of a model are set in the constructor. Hopefully.
            model.model = gManager.Load<Model>("Models/Junctions/" + modelName);
            //model.Texture = gManager.Load<Texture2D>("Textures/Junctions/" + modelName);
            model.Texture = gManager.Load<Texture2D>("Textures/Junctions/junctionAll");
        }

        private void addRandomModels(ContentManager gManager)
        {
            Random tRandom = new Random();
            #region Bench
            // Check to see if we have a random bench here:
            if (tRandom.NextDouble() > 0.85)
            {
                MetaModel bench = new MetaModel();
                bench.model = gManager.Load<Model>("Models/Giblies/bench");
                bench.Position = this.position;
                bench.Rotation = this.model.Rotation;
                bench.Texture = gManager.Load<Texture2D>("Textures/Giblies/bench");

                giblies.Add(bench);
            }
            #endregion
        }

        public override void Update(GameTime gTime)
        {
            base.Update(gTime);
        }      

        public void updateMatrices(MatrixDescriptor currentMatrices)
        {
            this.material.View = currentMatrices.view;
            this.material.World = currentMatrices.world;
            this.material.Projection = currentMatrices.proj;
        }

        public void Draw(GraphicsDevice gDevice)
        {        
            ModelUtil.DrawModel(model, material);

            foreach (MetaModel gibly in giblies)
            {
                ModelUtil.DrawModel(gibly, material);
            }
        }
    }
}
