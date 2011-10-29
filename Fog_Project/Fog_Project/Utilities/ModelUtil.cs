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

    public struct TexturedPlane
    {
        public Texture2D texture;
        public GraphicsDevice gDevice;
        public VertexPositionNormalTexture[] vertices;
        public short[] indices;
        public VertexBuffer vBuffer;
        public IndexBuffer iBuffer;
    }

    public struct MatrixDescriptor
    {
        public Matrix view  { get; set; }
        public Matrix proj  { get; set; }
        public Matrix world { get; set; }
    }

    public static class ModelUtil
    {
        /// <summary>
        /// Updates the view matrix given a camera's rotation and position.
        /// </summary>
        /// <param name="upDownRot">Up down rotation.</param>
        /// <param name="leftRightRot">Left and right rotation</param>
        /// <param name="cameraPos">Position of the camera</param>
        /// <param name="currentMatrices">The matrix descriptor containing the Workd, View and Projection matrices you want to use.</param>
        public static void UpdateViewMatrix(float upDownRot, float leftRightRot, Vector3 cameraPos, ref MatrixDescriptor currentMatrices)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(upDownRot) * Matrix.CreateRotationY(leftRightRot);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = cameraPos + cameraRotatedTarget;

            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

            currentMatrices.view = Matrix.CreateLookAt(cameraPos, cameraFinalTarget, cameraRotatedUpVector);
        }

        public static BasicEffect CreateGlobalEffect(GraphicsDevice gDevice)
        {
            BasicEffect globalEffect = new BasicEffect(gDevice);
            globalEffect.FogEnabled = true;
            globalEffect.FogColor = Color.LightCyan.ToVector3();
            globalEffect.FogStart = 1.0f;
            globalEffect.FogEnd = 7.0f;

            globalEffect.EnableDefaultLighting();
            globalEffect.TextureEnabled = true;
            return globalEffect;
        }

        public static void DrawModel(MetaModel m, BasicEffect globalEffect)
        {
            Matrix[] transforms = new Matrix[m.model.Bones.Count];
            m.model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in m.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.FogEnabled = globalEffect.FogEnabled;
                    effect.FogColor = globalEffect.FogColor;
                    effect.FogStart = globalEffect.FogStart;
                    effect.FogEnd = globalEffect.FogEnd;

                    effect.EnableDefaultLighting();
                    effect.Texture = m.Texture;
                    effect.TextureEnabled = globalEffect.TextureEnabled;

                    effect.View = globalEffect.View;
                    effect.Projection = globalEffect.Projection;
                    effect.World = transforms[mesh.ParentBone.Index];
                    effect.World *= Matrix.CreateRotationX(m.Rotation.X);
                    effect.World *= Matrix.CreateRotationY(m.Rotation.Y);
                    effect.World *= Matrix.CreateRotationY(m.Rotation.Z);
                    effect.World *= Matrix.CreateTranslation(m.Position);
                }
                mesh.Draw();
            }
        }
        public static void DrawTexturedPlane(TexturedPlane plane, BasicEffect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                effect.TextureEnabled = true;
                effect.Texture = plane.texture;
                effect.EnableDefaultLighting();

                plane.gDevice.Indices = plane.iBuffer;
                plane.gDevice.SetVertexBuffer(plane.vBuffer);
                pass.Apply();
                plane.gDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, plane.vertices.Length, 0, 2);
            }

        }
        public static TexturedPlane CreateTexturedPlane(Vector3 position, Vector2 size, Texture2D texture, GraphicsDevice gDevice)
        {
            TexturedPlane toReturn = new TexturedPlane();
            toReturn.gDevice = gDevice;
            toReturn.indices = new short[6] { 1, 2, 0, 1, 3, 2 };
            toReturn.vertices = new VertexPositionNormalTexture[4];
            toReturn.texture = texture;
            #region Vertex_Creation
            // Planes are drawn like this:
            // 0__1   Z
            // |__|   |__ X
            // 2  3
            // With counter clockwise culling on, we only get the top. -QWP
            toReturn.vertices[0].Position = new Vector3(position.X - (size.X / 2), position.Y, position.Z - (size.Y / 2));
            toReturn.vertices[0].Normal = Vector3.Up;
            toReturn.vertices[0].TextureCoordinate = new Vector2(1, 1);
            toReturn.vertices[1].Position = new Vector3(position.X + (size.X / 2), position.Y, position.Z - (size.Y / 2));
            toReturn.vertices[1].Normal = Vector3.Up;
            toReturn.vertices[1].TextureCoordinate = new Vector2(0, 1);
            toReturn.vertices[2].Position = new Vector3(position.X - (size.X / 2), position.Y, position.Z + (size.Y / 2));
            toReturn.vertices[2].Normal = Vector3.Up;
            toReturn.vertices[2].TextureCoordinate = new Vector2(1, 0);
            toReturn.vertices[3].Position = new Vector3(position.X + (size.X / 2), position.Y, position.Z + (size.Y / 2));
            toReturn.vertices[3].Normal = Vector3.Up;
            toReturn.vertices[3].TextureCoordinate = new Vector2(0, 0);
            #endregion
            toReturn.iBuffer = new IndexBuffer(gDevice, typeof(short), toReturn.indices.Length, BufferUsage.WriteOnly);
            toReturn.iBuffer.SetData(toReturn.indices);
            toReturn.vBuffer = new VertexBuffer(gDevice, VertexPositionNormalTexture.VertexDeclaration, toReturn.vertices.Length, BufferUsage.WriteOnly);
            toReturn.vBuffer.SetData<VertexPositionNormalTexture>(toReturn.vertices);
            return toReturn;
        }
    }
}
