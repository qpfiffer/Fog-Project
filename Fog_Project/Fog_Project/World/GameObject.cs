﻿using System;
using System.Collections.Generic;
using System.Text;
using Fog_Project.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Fog_Project.World
{
    public class GameObject
    {
        #region Fields
        protected Vector3 position;
        protected List<BoundingSphere> boundingSpheres;
        protected List<BoundingBox> boundingBoxes;
        protected float leftRightRot, upDownRot;
        protected MetaModel model;
        protected BasicEffect material;
        protected GraphicsDevice gDevice;
        protected List<Vector3> boundingOffsets;
        #endregion

        #region Properties
        public Vector3 Position 
        { 
            get { return position; }
            set
            {
                model.Position = value;
                
                if (model.model != null)
                    ModelUtil.UpdateBoundingBoxes(ref model);
                
                this.position = value;
            }
        }
        public List<BoundingSphere> BoundingSpheres
        {
            get { return boundingSpheres; }
        }
        public List<BoundingBox> BoundingBoxes
        {
            get { return boundingBoxes; }
        }
        public float LeftRightRot
        {
            get { return leftRightRot; }
        }
        public float UpDownRot 
        {
            get { return upDownRot; }
        }
        public MetaModel Model
        {
            get { return model; }
        }
        public BasicEffect Material
        {
            get { return material; }
        }
        #endregion

        // Commenting this out becuase it is confusing. You should be using one of the other
        // constructors.
        /// <summary>
        /// Creates a blank game object.
        /// </summary>
        //public GameObject(GraphicsDevice gDevice)
        //{
        //    position = Vector3.Zero;
        //    boundingSpheres = new List<BoundingSphere>();
        //    leftRightRot = 0.0f;
        //    upDownRot = 0.0f;
        //}

        /// <summary>
        /// Creates a GameObject that is useful for cameras.
        /// </summary>
        /// <param name="position">Where it should go.</param>
        /// <param name="rotation">How it should be rotated.</param>
        public GameObject(ref Vector3 position, ref Vector2 rotation, GraphicsDevice gDevice)
        {
            this.position = position;
            this.gDevice = gDevice;
            leftRightRot = rotation.X;
            upDownRot = rotation.Y;
            boundingSpheres = new List<BoundingSphere>();
            boundingBoxes = new List<BoundingBox>();
            boundingOffsets = new List<Vector3>();
        }

        /// <summary>
        /// Creates a GameObject that will eventually have a model mesh.
        /// </summary>
        /// <param name="position">Where it should go.</param>
        /// <param name="rotation">How it will be rotated when displayed.</param>
        public GameObject(ref Vector3 position, ref Vector3 rotation, GraphicsDevice gDevice)
        {
            this.position = position;
            this.gDevice = gDevice;
            leftRightRot = 0.0f;
            upDownRot = 0.0f;
            model = new MetaModel();
            model.Position = position;
            model.Rotation = rotation;            
            boundingSpheres = new List<BoundingSphere>();
            boundingBoxes = new List<BoundingBox>();
            boundingOffsets = new List<Vector3>();
        }

        public void addNewBounding(BoundingSphere toAdd, Vector3 offset)
        {
            BoundingSpheres.Add(toAdd);
            boundingOffsets.Add(offset);
        }

        public void addNewBounding(BoundingBox toAdd, Vector3 offset)
        {
            boundingBoxes.Add(toAdd);
            boundingOffsets.Add(offset);
            
        }

        public virtual void Load(ContentManager gManager)
        {
            throw new NotImplementedException("This probably shouldn't be called directly.");
        }

        public void ToggleFog(bool fogEnabled)
        {
            material.FogEnabled = fogEnabled;
        }

        public void ToggleFog()
        {
            material.FogEnabled = !material.FogEnabled;
        }

        public virtual void Update(GameTime gTime)
        {
            for (int i = 0; i < boundingSpheres.Count; i++)
            {
                boundingSpheres[i] = new BoundingSphere(position - boundingOffsets[i],
                    boundingSpheres[i].Radius);
            }

            for (int i = 0; i < boundingBoxes.Count; i++)
            {
                Vector3 curMin = boundingBoxes[i].Min;
                Vector3 curMax = boundingBoxes[i].Max;

                Vector3 minActual = position - curMin;
                Vector3 maxActual = position - curMax;
                boundingBoxes[i] = new BoundingBox(minActual - boundingOffsets[i], maxActual - boundingOffsets[i]);
            }
        }
    }
}
