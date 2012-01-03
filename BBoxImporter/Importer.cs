using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace BBoxImporter
{
    [ContentProcessor(DisplayName = "Bounding Box Generator")]
    public class Importer : ModelProcessor
    {
        double minX = double.MaxValue;
        double minY = double.MaxValue;
        double minZ = double.MaxValue;
        double maxX = double.MinValue;
        double maxY = double.MinValue;
        double maxZ = double.MinValue;

        List<BoundingBox> boxes = new List<BoundingBox>();
        List<List<Vector3>> MeshVerts = new List<List<Vector3>>();

        object[] ModelData = new object[2];

        // Bounding Box's
        private void CheckNode(NodeContent content)
        {
            foreach (NodeContent o in content.Children)
            {
                if (o is MeshContent && o.Name.Contains("bounding"))
                {
                    // Get VertData
                    GetAllVerticies((MeshContent)o);
                    BoundingBox bb = new BoundingBox();

                    minX = double.MaxValue;
                    minY = double.MaxValue;
                    minZ = double.MaxValue;
                    maxX = double.MinValue;
                    maxY = double.MinValue;
                    maxZ = double.MinValue;

                    MeshContent mesh = (MeshContent)o;
                    foreach (Vector3 basev in mesh.Positions)
                    {
                        Vector3 v = basev;

                        if (v.X < minX)
                            minX = v.X;

                        if (v.Y < minY)
                            minY = v.Y;

                        if (v.Z < minZ)
                            minZ = v.Z;

                        if (v.X > maxX)
                            maxX = v.X;

                        if (v.Y > maxY)
                            maxY = v.Y;

                        if (v.Z > maxZ)
                            maxZ = v.Z;

                    }

                    double lenX = maxX - minX;
                    double lenZ = maxZ - minZ;
                    double lenY = maxY - minY;

                    bb.Min = new Vector3((float)minX, (float)minY, (float)minZ);
                    bb.Max = new Vector3((float)maxX, (float)maxY, (float)maxZ);
                    boxes.Add(bb);
                }
                else
                    CheckNode(o);
            }
        }

        // Vertex positions
        private void GetAllVerticies(MeshContent mesh)
        {
            for (int g = 0; g < mesh.Geometry.Count; g++)
            {
                GeometryContent geometry = mesh.Geometry[g];

                List<Vector3> temp = new List<Vector3>();

                for (int ind = 0; ind < geometry.Indices.Count; ind++)
                {
                    // Transforms all of my verticies to local space.
                    Vector3 position = Vector3.Transform(geometry.Vertices.Positions[geometry.Indices[ind]], mesh.AbsoluteTransform);
                    temp.Add(position);
                }
                MeshVerts.Add(temp);
            }
        }

        // Tangents.
        private void GenerateTangents(NodeContent input, ContentProcessorContext context)
        {
            MeshContent mesh = input as MeshContent;

            if (mesh != null)
            {
                MeshHelper.CalculateTangentFrames(mesh,
                    VertexChannelNames.TextureCoordinate(0),
                    VertexChannelNames.Tangent(0),
                    VertexChannelNames.Binormal(0));
            }

            foreach (NodeContent child in input.Children)
            {
                GenerateTangents(child, context);
            }
        }
        // Normals
        private void GenerateNormals(NodeContent input, ContentProcessorContext context)
        {
            MeshContent mesh = input as MeshContent;

            if (mesh != null)
            {
                MeshHelper.CalculateNormals(mesh, true);
            }

            foreach (NodeContent child in input.Children)
            {
                GenerateNormals(child, context);
            }
        }

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            //GenerateNormals(input, context);
            GenerateTangents(input, context);

            // Setup bounding box data.
            CheckNode(input);

            ModelData[0] = boxes;
            ModelData[1] = MeshVerts;

            ModelContent basemodel = base.Process(input, context);
            basemodel.Tag = ModelData;
            return basemodel;
        }
    }
}