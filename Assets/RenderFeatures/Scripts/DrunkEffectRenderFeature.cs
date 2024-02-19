using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RenderFeatures.Scripts
{
    public interface IMaterialIntensity
    {
        void SetIntensity(float newIntensity);
    }
    
    public class DrunkEffectRenderFeature : ScriptableRendererFeature, IMaterialIntensity
    {
        public Material _passMaterial;
        private RenderPass _renderPass;
        private static readonly int Intensity = Shader.PropertyToID("_Intensity");

        class RenderPass : ScriptableRenderPass
        {
            private readonly Material _material;
            private RTHandle _source;
            private readonly RTHandle _tempTexture;

            public RenderPass(Material material) : base()
            {
                _material = material;
                _tempTexture = RTHandles.Alloc("_TempTexture", "_TempTexture");
            }
            
            public void SetTarget(RTHandle colorHandle)
            {
                _source = colorHandle;
            }

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                ConfigureTarget(_source);
            }
            
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var cameraData = renderingData.cameraData;
                if (cameraData.camera.cameraType != CameraType.Game)
                    return;

                if (_material == null)
                    return;
                
                CommandBuffer cmd = CommandBufferPool.Get("DrunkEffectFeature");
                
                Blitter.BlitCameraTexture(cmd, _source, _source, _material, 0);

                context.ExecuteCommandBuffer(cmd);
                
                CommandBufferPool.Release(cmd);
            }
            
            public override void FrameCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(Shader.PropertyToID(_tempTexture.name));
            }
        }
        
        public override void Create()
        {
            _renderPass = new RenderPass(_passMaterial)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
            };
            SetActive(false);
            SetIntensity(0);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_renderPass);
        }
        
        public override void SetupRenderPasses(ScriptableRenderer renderer,
            in RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                _renderPass.ConfigureInput(ScriptableRenderPassInput.Color);
                _renderPass.SetTarget(renderer.cameraColorTargetHandle);
            }
        }

        public void SetIntensity(float newIntensity)
        {
            _passMaterial.SetFloat(Intensity, newIntensity);
        }
    }
}