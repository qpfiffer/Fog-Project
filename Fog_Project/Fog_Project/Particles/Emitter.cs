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

        #region Particle_Vars
        public int minParticles { get; set; }
        public int maxParticles { get; set; }
        public Texture2D particleTexture { get; set; }
        public Vector2 particleSize { get; set; }
        #endregion

        public Emitter(Vector3 position)
        {
            emitterPos = position;
            particles = new List<Particle>();
        }

        public void Update(GameTime gTime)
        {
            List<Particle> toDelete = null;
            foreach (Particle particle in particles)
            {
                particle.Update(gTime);
                if (particle.shouldDie())
                {
                    if (toDelete == null)
                        toDelete = new List<Particle>();

                    toDelete.Add(particle);
                }
            }

            if (toDelete != null)
            {
                foreach (Particle particle in toDelete)
                {
                    particles.Remove(particle);
                }

                // A haiku:
                // Just because I am
                // paranoid about having
                // garbage collected
                toDelete.Clear();
                toDelete = null;
            }
        }

        public void Draw(GraphicsDevice gDevice)
        {
            foreach (Particle particle in particles)
            {
                particle.Draw(gDevice);
            }
        }
    }
}
