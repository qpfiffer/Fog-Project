using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Fog_Project.Utilities;
using Fog_Project.World;

namespace Fog_Project
{
    class Player: GameObject
    {
        #region Fields
        private MatrixDescriptor matrices;
        #endregion
        #region Properties
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
            set { matrices = value; }
        }
        #endregion
        #region Constants
        public const float rotationSpeed = 0.3f;
        public const float moveSpeed = 3.5f;
        public const float chestHeight = 1.0f;
        public const float floorBoxHeight = chestHeight - 0.5f;
        public const float gravity = 0.35f;
        public const float rightAngleRadians = 1.57079633f;
        #endregion

        public Player(ref Vector3 position, ref Vector2 rotation, GraphicsDevice gDevice):
            base(ref position, ref rotation, gDevice)
        {
            matrices = new MatrixDescriptor();
            BoundingSphere chestSphere = new BoundingSphere(position, 0.25f);
            base.addNewBounding(chestSphere, Vector3.Zero);
        }

        public void rotateCamera(ref Point mouseDifference, float timeDifference)
        {
            leftRightRot -= rotationSpeed * mouseDifference.X * timeDifference;
            if (leftRightRot > (2 * Math.PI))
            {
                leftRightRot -= (float)(2 * Math.PI);
            }
            else if (leftRightRot < (-2 * Math.PI))
            {
                leftRightRot += (float)(2 * Math.PI);
            }

            float upDownTemp = upDownRot - (rotationSpeed * mouseDifference.Y * timeDifference);
            if (upDownTemp < rightAngleRadians &&
                upDownTemp > -rightAngleRadians)
            {
                upDownRot = upDownTemp;
            }
        }

        /// <summary>
        /// Rotates the camera.
        /// </summary>
        /// <param name="point">The point about which to rotate.</param>
        /// <param name="degrees">How far to rotate.</param>
        public void rotateCameraAboutYAxisPoint(Vector2 point, float degrees)
        {
            if (degrees == 0.0f)
                return;

            Vector3 newPosition = position;
            float radians = MathHelper.ToRadians(degrees);
            newPosition.X = point.X + 
                (float)(Math.Cos(radians) * (newPosition.X - point.X) -
                Math.Sin(radians) * (newPosition.Z - point.Y));
            newPosition.Z = point.Y +
                (float)(Math.Sin(radians) * (newPosition.X - point.X) +
                Math.Cos(radians) * (newPosition.Z - point.Y));
            
            position = newPosition;
            
            leftRightRot -= radians;

            ModelUtil.UpdateViewMatrix(upDownRot, leftRightRot, position, ref matrices);
        }

        public void addToCameraPosition(ref Vector3 toAdd)
        {
            float localMoveSpeed = moveSpeed;
            Vector3 oldPosition = position;
            Matrix cameraRotation = Matrix.Identity;
            if (NoClip)
            {
                cameraRotation = Matrix.CreateRotationX(upDownRot) * Matrix.CreateRotationY(leftRightRot);
                localMoveSpeed *= 2;
            }
            else
            {
                // We don't move in the direction we're looking if we are looking up. Discard the upDown
                // rotation.
                cameraRotation = Matrix.CreateRotationX(0.0f) * Matrix.CreateRotationY(leftRightRot);
            }

            Vector3 rotatedVector = Vector3.Transform(toAdd, cameraRotation);
            oldPosition += localMoveSpeed * rotatedVector;

            position = oldPosition;
            ModelUtil.UpdateViewMatrix(upDownRot, leftRightRot, position, ref matrices);
        }

        public void setCameraPosition(Vector3 newPosition, Vector3 offset)
        {
            Matrix cameraRotation = Matrix.Identity;
            cameraRotation = Matrix.CreateRotationX(0.0f) * Matrix.CreateRotationY(leftRightRot);
            Vector3 afterOffset = newPosition - offset;
            position = afterOffset;
            ModelUtil.UpdateViewMatrix(upDownRot, leftRightRot, position, ref matrices);
        }

        public override void Update(GameTime gTime)
        {
            base.Update(gTime);
        }
    }
}
