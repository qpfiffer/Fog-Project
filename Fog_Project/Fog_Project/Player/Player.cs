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
        public const float moveSpeed = 2.5f;
        public const float chestHeight = 1.0f;
        public const float floorBoxHeight = chestHeight - 0.5f;
        public const float gravity = 0.35f;
        public const float rightAngleRadians = 1.57079633f;
        #endregion

        public Player(ref Vector3 position, ref Vector2 rotation, GraphicsDevice gDevice):
            base(ref position, ref rotation, gDevice)
        {
            matrices = new MatrixDescriptor();
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

        public void setCameraPosition(ref Vector3 newPosition)
        {
            Matrix cameraRotation = Matrix.Identity;
            cameraRotation = Matrix.CreateRotationX(0.0f) * Matrix.CreateRotationY(leftRightRot);
            position = Vector3.Transform(newPosition, cameraRotation);
            ModelUtil.UpdateViewMatrix(upDownRot, leftRightRot, position, ref matrices);
        }
    }
}
