using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BoundingBoxImporter
{
    public struct GeneratedBoundingData
    {
        List<BoundingBox> bBoxes = new List<BoundingBox>();
        List<BoundingSphere> bSphere = new List<BoundingSphere>();
    }

    [ContentProcessor(DisplayName="Bounding Box Importer")]
    public class Importer: ModelProcessor
    {        
        ContentProcessorContext context;

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            this.context = context;
            GeneratedBoundingData mData = new GeneratedBoundingData();
            ModelContent baseModel = base.Process(input, context);
            generateBoundingVolumes(input, mData);
            baseModel.Tag = mData;

            return baseModel;
        }

        private void generateBoundingVolumes(NodeContent input, GeneratedBoundingData mData)
        {
            // Started from here:
            // http://xna-uk.net/blogs/randomchaos/archive/2010/08/09/boundingbox-s-in-xna-4-0.aspx
        }
    }
}
