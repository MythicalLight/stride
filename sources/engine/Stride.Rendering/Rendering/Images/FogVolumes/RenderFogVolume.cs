using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Rendering.Lights;

namespace Stride.Rendering.Images
{
    public struct RenderFogVolume
    {
        //public RenderLight Light;
        //public IDirectLight Light2;
        public int SampleCount;
        public float DensityValue;
        //public IReadOnlyList<RenderLightShaftBoundingVolume> BoundingVolumes;
        public bool VolumetricCalculations;
        public Matrix World;
        public Model Model;
    }
}