using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Fog_Project.Utilities;

namespace Fog_Project
{
    class Player
    {
        #region Fields
        private BoundingSphere BBoxFloor;
        private BoundingSphere BBoxChest;
        private Vector3 position;
        private MatrixDescriptor matrices;
        private float leftRightRot;
        private float upDownRot;
        #endregion
        #region Properties
        /// <summary>
        /// The current position of the player, and therefor the camera.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return position;
            }
        }

        /// <summary>
        /// Controls whether noclip is enabled or not.
        /// </summary>
        public bool NoClip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current matrices that this instance of player is using.
        /// </summary>
        public MatrixDescriptor Matrices
        {
            get { return matrices; }
        }
        #endregion
        #region Constants
        public const float rotationSpeed = 0.3f;
        public const float moveSpeed = 5.0f;
        public const float chestHeight = 2.75f;
        public const float floorBoxHeight = chestHeight - 0.5f;
        public const float gravity = 0.35f;
        public const float rightAngleRadians = 1.57079633f;
        #endregion

        public Player()
        {
            position = Vector3.Zero;
            matrices = new MatrixDescriptor();
        }

        public void rotateCamera(ref Point mouseDifference, float amount)
        {
            leftRightRot -= rotationSpeed * mouseDifference.X * amount;
            if (leftRightRot > (2 * Math.PI))
            {
                leftRightRot -= (float)(2 * Math.PI);
            }
            else if (leftRightRot < (-2 * Math.PI))
            {
                leftRightRot += (float)(2 * Math.PI);
            }

            float upDownTemp = upDownRot - (rotationSpeed * mouseDifference.Y * amount);
            if (upDownTemp < rightAngleRadians &&
                upDownTemp > -rightAngleRadians)
            {
                upDownRot = upDownTemp;
            }
        }

        public void addToCameraPosition(ref Vector3 toAdd)
        {
            Vector3 oldPosition = position;
            Matrix cameraRotation = Matrix.Identity;
            if (NoClip)
            {
                cameraRotation = Matrix.CreateRotationX(upDownRot) * Matrix.CreateRotationY(leftRightRot);
            }
            else
            {
                // We don't move in the direction we're looking if we are looking up. Discard the upDown
                // rotation.
                cameraRotation = Matrix.CreateRotationX(0.0f) * Matrix.CreateRotationY(leftRightRot);
            }

            Vector3 rotatedVector = Vector3.Transform(toAdd, cameraRotation);
            oldPosition += moveSpeed * rotatedVector;

            // DO COLLISION HERE

            position = oldPosition;
            ModelUtil.UpdateViewMatrix(upDownRot, leftRightRot, ref position, ref matrices);
        }
    }
}
