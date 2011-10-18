using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fog_Project.Utilities
{
    public struct MetaModel
    {
        public Model model { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Texture2D Texture { get; set; }
        public BoundingBox BBox { get; set; }
    }

    public struct MatrixDescriptor
    {
        public Matrix view  { get; set; }
        public Matrix proj  { get; set; }
        public Matrix world { get; set; }
    }

    public static class ModelUtil
    {
        public static void DrawModel(MetaModel m, MatrixDescriptor matrices)
        {
            Matrix[] transforms = new Matrix[m.model.Bones.Count];
            m.model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in m.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.FogEnabled = true;
                    effect.FogColor = Color.LightCyan.ToVector3();
                    effect.FogStart = 1.0f;
                    effect.FogEnd = 5.0f;

                    effect.EnableDefaultLighting();
                    effect.Texture = m.Texture;
                    effect.TextureEnabled = true;

                    effect.View = matrices.view;
                    effect.Projection = matrices.proj;
                    effect.World = transforms[mesh.ParentBone.Index];
                    effect.World *= Matrix.CreateRotationX(m.Rotation.X);
                    effect.World *= Matrix.CreateRotationY(m.Rotation.Y);
                    effect.World *= Matrix.CreateRotationY(m.Rotation.Z);
                    effect.World *= Matrix.CreateTranslation(m.Position);
                }
                mesh.Draw();
            }
        }
    }
}
