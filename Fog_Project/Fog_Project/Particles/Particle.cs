using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Fog_Project.Utilities;

namespace Fog_Project.Particles
{
    public class Particle
    {
        #region Constants
        const float particleThickness = 0.2f;
        const float rotationPerFrame = 0.1f;
        #endregion

        #region Fields
        protected Vector3 position;
        protected Vector3 velocity;
        protected Vector2 size;
        protected int lifetimeSeconds;
        protected Texture2D texture;
        protected GraphicsDevice gDevice;
        protected TimeSpan startLife;
        protected int deathTimeSeconds;
        protected bool timeToDie;
        protected float currentRotation = 0.0f;
        protected TexturedPlane particlePlane;
        #endregion

        public Particle(Vector3 startPos, Vector3 velocity, int lifetimeSeconds, Texture2D texture, Vector2 size, GraphicsDevice gDevice)
        {
            this.position = startPos;
            this.velocity = velocity;
            this.lifetimeSeconds = lifetimeSeconds;
            this.texture = texture;
            this.gDevice = gDevice;
            this.size = size;

            this.startLife = TimeSpan.MaxValue;
            this.deathTimeSeconds = startLife.Seconds + lifetimeSeconds;
            particlePlane = ModelUtil.CreateTexturedPlane(startPos, size, texture, gDevice);
            timeToDie = false;
        }

        public virtual void Update(GameTime gTime)
        {
            if (startLife == TimeSpan.MaxValue)
            {
                startLife = gTime.TotalGameTime;
            }

            position += velocity;
            currentRotation += rotationPerFrame;
            particlePlane = ModelUtil.CreateTexturedPlane(position, size, texture, gDevice);

            if (gTime.TotalGameTime.Seconds > deathTimeSeconds)
            {
                timeToDie = true;
            }
        }

        public bool shouldDie()
        {
            return timeToDie;
        }

        public virtual void Draw(GraphicsDevice gDevice)
        {
        }
    }
}
