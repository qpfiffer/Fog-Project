using System;
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
        protected float leftRightRot, upDownRot;
        protected MetaModel model;
        protected BasicEffect material;
        protected GraphicsDevice gDevice;
        #endregion

        #region Properties
        public Vector3 Position 
        { 
            get { return position; }
            set
            {
                model.Position = value;
                this.position = value;
            }
        }
        public List<BoundingSphere> BoundingSpheres
        {
            get { return boundingSpheres; }
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

        #region Delegates
        public delegate FogToggle;
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
        }

        public void addNewBounding(ref BoundingSphere toAdd)
        {
            BoundingSpheres.Add(toAdd);
        }

        public virtual void Load(ContentManager gManager)
        {
            throw new NotImplementedException("This probably shouldn't be called directly.");
        }

        public virtual void Update(GameTime gTime)
        {
            foreach (BoundingSphere sphere in BoundingSpheres)
            {
                // Move each bounding sphere to its place around this object's
                // position. You'll probably have to store offsets for each
                // sphere somewhere.
            }
        }
    }
}
