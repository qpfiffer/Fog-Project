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
    public class Emitter
    {
        private Vector3 emitterPos;
        private List<Particle> particles;

        public Emitter(Vector3 position)
        {
            emitterPos = position;
            particles = new List<Particle>();
        }

        public void Update(GameTime gTime)
        {
            foreach (Particle particle in particles)
            {
                particle.Update(gTime);
            }
        }

        public void Draw(GraphicsDevice gDevice)
        {
            foreach (Particle particle in particles)
            {
                particle.Draw(GraphicsDevice gDevice);
            }
        }
    }
}
