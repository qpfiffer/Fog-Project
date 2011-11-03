﻿using System;
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
        private List<MetaModel> junctionConnections;
        private Texture2D waterTexture;
        private List<TexturedPlane> waterTiles;
        private int numberOfRotations;
        #endregion

        #region Properties
        public JunctionType Type { get; set; }
        public int Rotations { get; set; }
        #endregion
        public Junction(ref Vector3 position, ref Vector3 rotation, GraphicsDevice gDevice)
            : base(ref position, ref rotation, gDevice)
        {
            // Helps to figure out where we place out junctionConnections.
            numberOfRotations = 0;
            exits = new Dictionary<BoundingBox, Junction>();
            Type = JunctionType.single; // Gets set later.
            giblies = new List<MetaModel>();
            waterTiles = new List<TexturedPlane>();
            junctionConnections = new List<MetaModel>();
        }

        public void Load(ContentManager gManager, string modelName)
        {
            // If we created this junction using the wrong constructor our rotations will be
            // all sorts of fucked up.
            if (this.leftRightRot != 0.0f && this.upDownRot != 0.0f)
            {
                throw new Exception("This junction created using the wrong constructor!");
            }
            
            // Add some random models:
            addRandomModels(gManager);

            material = ModelUtil.CreateGlobalEffect(gDevice);
            // Rotation and Position of a model are set in the constructor. Hopefully.
            model.model = gManager.Load<Model>("Models/Junctions/" + modelName);
            //model.Texture = gManager.Load<Texture2D>("Textures/Junctions/" + modelName);
            model.Texture = gManager.Load<Texture2D>("Textures/Junctions/junctionAll");

            CreateJunctionConnections(gManager);

            // This is the texture used for all of the water tiles around this junction:
            waterTexture = gManager.Load<Texture2D>("Textures/Ocean/ocean");             
            createWaterTiles();
        }

        private void CreateJunctionConnections(ContentManager gManager)
        {
            if (this.Type == JunctionType.single)
            {
                for (int i = 0; i < 2; i++)
                {
                    MetaModel temp = new MetaModel();
                    temp.model = gManager.Load<Model>("Models/Junctions/junctionConnection");
                    //temp.Texture = model.Texture; // Should be junctionAll
                    temp.Texture = gManager.Load<Texture2D>("Textures/Ocean/ocean");
                    temp.Rotation = new Vector3(0, MathHelper.ToRadians(90.0f), 0);
                    if (i == 0)
                    {
                        temp.Position = new Vector3(position.X, position.Y, position.Z + 5.0f);
                    }
                    else
                    {
                        temp.Position = new Vector3(position.X, position.Y, position.Z - 5.0f); 
                    }
                    junctionConnections.Add(temp);
                }
            }
            else
            {
                for (int i = 0; i < (int)this.Type; i++)
                {
                    MetaModel temp = new MetaModel();
                    temp.model = gManager.Load<Model>("Models/Junctions/junctionConnection");
                    //temp.Texture = model.Texture; // Should be junctionAll
                    temp.Texture = gManager.Load<Texture2D>("Textures/Ocean/ocean"); 
                    // Rotate every other junction by 90 degrees:
                    if ((i % 2) == 0)
                    {
                        temp.Rotation = new Vector3(0, MathHelper.ToRadians(90.0f), 0);
                    }
                    else
                    {
                        temp.Rotation = Vector3.Zero;
                    }

                    // Now we do the positioning:
                    switch (i % 4)
                    {
                        case 0:
                            temp.Position = new Vector3(position.X, position.Y, position.Z - 7.5f); 
                            break;
                        case 1:
                            temp.Position = new Vector3(position.X + 7.5f, position.Y, position.Z); 
                            break;
                        case 2:
                            temp.Position = new Vector3(position.X, position.Y, position.Z + 7.5f); 
                            break;
                        case 3:
                            temp.Position = new Vector3(position.X - 7.5f, position.Y, position.Z); 
                            break;
                    }

                    junctionConnections.Add(temp);
                }
            }
        }

        private void createWaterTiles()
        {
            if (waterTiles == null)
                throw new Exception("Why is waterTiles null?");

            const float oceanTileSize = 2.5f;
            const int numTiles = 4;
            for (int x = -numTiles; x < numTiles; x++)
            {
                for (int y = -numTiles; y < numTiles; y++)
                {
                    TexturedPlane test = ModelUtil.CreateTexturedPlane(
                        new Vector3(position.X - (x * oceanTileSize), -0.15f, position.Z - (y * oceanTileSize)),
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

            foreach (TexturedPlane tile in waterTiles)
            {
                ModelUtil.DrawTexturedPlane(tile, material);
            }

            foreach (MetaModel gibly in giblies)
            {
                ModelUtil.DrawModel(gibly, material);
            }

            foreach (MetaModel junctionC in junctionConnections)
            {
                ModelUtil.DrawModel(junctionC, material);
            }
        }
    }
}
