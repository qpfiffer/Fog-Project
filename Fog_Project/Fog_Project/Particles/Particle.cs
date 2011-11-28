using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fog_Project.Particles
{
    public class Particle
    {
        Vector3 position;
        Vector3 velocity;
        int lifetimeSeconds;
        Texture2D texture;

        TimeSpan startLife;
        bool timeToDie;

        public Particle(Vector3 startPos, Vector3 velocity, int lifetimeSeconds, Texture2D texture)
        {
            this.position = startPos;
            this.velocity = velocity;
            this.lifetimeSeconds = lifetimeSeconds;
            this.texture = texture;

            this.startLife = TimeSpan.MaxValue;
            timeToDie = false;
        }

        public void Update(GameTime gTime)
        {
            if (startLife == TimeSpan.MaxValue)
            {
                startLife = gTime.TotalGameTime;
            }

            position += velocity;

            if (gTime.TotalGameTime.Seconds > (startLife.Seconds + lifetimeSeconds))
            {
                timeToDie = true;
            }
        }

        private bool shouldDie()
        {
            return timeToDie;
        }

        public void Draw(GraphicsDevice gDevice)
        {
        }
    }
}
