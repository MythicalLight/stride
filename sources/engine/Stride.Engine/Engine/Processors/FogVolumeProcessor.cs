
using System;
using System.Collections.Generic;
using System.Linq;
using Stride.Core.Mathematics;
using Stride.Games;
using Stride.Rendering;
using Stride.Rendering.Images;
using Stride.Rendering.Lights;

namespace Stride.Engine.Processors
{
    public class FogVolumeProcessor : EntityProcessor<VolumeFogComponent, FogVolumeProcessor.AssociatedData>, IEntityComponentRenderProcessor
    {
        private readonly List<RenderFogVolume> activeFogVolumes = new List<RenderFogVolume>();


        private Dictionary<VolumeFogComponent, List<RenderFogVolume>> volumesPerFogVolume = new Dictionary<VolumeFogComponent, List<RenderFogVolume>>();
        private bool isDirty;


        




        public IReadOnlyList<RenderFogVolume> GetBoundingVolumesForComponent(VolumeFogComponent component)
        {
            if (!volumesPerFogVolume.TryGetValue(component, out var data))
                return null;
            return data;
        }




        /// <inheritdoc/>
        public VisibilityGroup VisibilityGroup { get; set; }

        protected internal override void OnSystemAdd()
        {
            base.OnSystemAdd();

            VisibilityGroup.Tags.Set(FogVolumes.CurrentFogVolumes, activeFogVolumes);
        }

        protected internal override void OnSystemRemove()
        {
            VisibilityGroup.Tags.Set(FogVolumes.CurrentFogVolumes, null);

            base.OnSystemRemove();
        }

        /// <inheritdoc />
        protected override AssociatedData GenerateComponentData(Entity entity, VolumeFogComponent component)
        {
            return new AssociatedData
            {
                Component = component,
                //LightComponent = entity.Get<LightComponent>(),
            };
        }

        /// <inheritdoc />
        protected override bool IsAssociatedDataValid(Entity entity, VolumeFogComponent component, AssociatedData associatedData)
        {
            return component == associatedData.Component; //&&
                                                          //entity.Get<LightComponent>() == associatedData.LightComponent;
        }






        

        //private void ComponentOnEnabledChanged(object sender, EventArgs eventArgs)
        //{
        //    isDirty = true;
        //}

        //private void ComponentOnModelChanged(object sender, EventArgs eventArgs)
        //{
        //    isDirty = true;
        //}







        //private void ComponentOnLightShaftChanged(object sender, EventArgs eventArgs)
        //{
        //    isDirty = true;
        //}









        

        /// <inheritdoc />
        public override void Update(GameTime time)
        {
            RegenerateVolumesPerFogVolume();


            
            activeFogVolumes.Clear();

            // Get processors
            var lightProcessor = EntityManager.GetProcessor<LightProcessor>();
            if (lightProcessor == null)
                return;


            foreach (var pair in ComponentDatas)
            {
                if (!pair.Key.Enabled)
                    continue;

                var fogVolume = pair.Value;
                //if (lightShaft.LightComponent == null)
                //    continue;

                

                //var light = lightProcessor.GetRenderLight(lightShaft.LightComponent); // HERE WE ACCESS THE LIGHT
                //if (light == null)
                //    continue;

                //var directLight = light.Type as IDirectLight;
                //if (directLight == null)
                //    continue;

                var boundingVolumes = GetBoundingVolumesForComponent(fogVolume.Component);
                if (boundingVolumes == null)
                    continue;

                activeFogVolumes.Add(new RenderFogVolume
                {
                    //Light = light,
                    //Light2 = directLight,
                    SampleCount = fogVolume.Component.SampleCount,
                    DensityValue = fogVolume.Component.DensityValue,
                    BoundingVolumes = boundingVolumes,
                    //SeparateBoundingVolumes = lightShaft.Component.SeparateBoundingVolumes,
                });
            }
        }

        public class AssociatedData
        {
            public VolumeFogComponent Component;
            public LightComponent LightComponent;
        }







        private void RegenerateVolumesPerFogVolume()
        {
            // Clear
            if (isDirty)
            {
                volumesPerFogVolume.Clear();
            }
            // Keep existing collections
            else
            {
                foreach (var fogVolume in volumesPerFogVolume)
                {
                    fogVolume.Value.Clear();
                }
            }

            foreach (var pair in ComponentDatas)
            {
                if (!pair.Key.Enabled)
                    continue;

                var fogVolume = pair.Key.FogVolume;
                if (fogVolume == null)
                    continue;

                List<RenderFogVolume> data;
                if (!volumesPerFogVolume.TryGetValue(fogVolume, out data))
                    volumesPerFogVolume.Add(fogVolume, data = new List<RenderFogVolume>());

                data.Add(new RenderFogVolume
                {
                    World = pair.Key.Entity.Transform.WorldMatrix,
                    Model = pair.Key.Model,
                });
            }

            isDirty = false;
        }






    }
}
