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
        private Texture2D waterTexture;
        private List<TexturedPlane> waterTiles;
        private BasicEffect waterEffect;
        #endregion

        #region Properties
        public JunctionType Type { get; set; }
        #endregion
        public Junction(ref Vector3 position, ref Vector3 rotation, GraphicsDevice gDevice)
            : base(ref position, ref rotation, gDevice)
        {
        }

        public void Load(ContentManager gManager, string modelName)
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

            // This is the texture used for all of the water tiles around this junction:
            waterTexture = gManager.Load<Texture2D>("Textures/Ocean/ocean");
            waterTiles = new List<TexturedPlane>();
            waterEffect = (BasicEffect)material.Clone();
            waterEffect.Texture = waterTexture;         
            createWaterTiles();
        }

        private void createWaterTiles()
        {
            if (waterTiles == null)
                throw new Exception("Why is waterTiles null?");

            const float oceanTileSize = 5.0f;
            const int numTiles = 3;
            for (int x = -(numTiles * (int)position.X); x < (numTiles * (int)position.X); x++)
            {
                for (int y = -(numTiles * (int)position.Y); y < (numTiles * (int)position.Y); y++)
                {
                    TexturedPlane test = ModelUtil.CreateTexturedPlane(
                        new Vector3(x * oceanTileSize, -0.5f, y * oceanTileSize),
                        new Vector2(oceanTileSize),
                        waterTexture,
                        gDevice);
                    waterTiles.Add(test);
                }
            }

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

            foreach (TexturedPlane tile in waterTiles)
            {
                ModelUtil.DrawTexturedPlane(tile, waterEffect);
            }
        }
    }
}
