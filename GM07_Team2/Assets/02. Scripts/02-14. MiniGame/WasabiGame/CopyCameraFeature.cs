using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

//현재 카메라의 activeColorTexture를 외부 RenderTexture에 복사하는 기능
//외부 텍스처는 Render Graph에 가져와 사용
public class CopyCameraFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderTexture destination;
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    [SerializeField] private Settings _settings = new();

    private CopyCameraColorPass _pass;
    private RTHandle _destinationHandle;

    public override void Create()
    {
        ReleaseHandle();
        if (_settings.destination == null)
        {
            _pass = null;
            return;
        }
        _destinationHandle = RTHandles.Alloc(_settings.destination);
        _pass = new CopyCameraColorPass(_destinationHandle)
        {
            renderPassEvent = _settings.passEvent
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_pass == null) return;
        if (renderingData.cameraData.cameraType != CameraType.Game) return;
        if (renderingData.cameraData.renderType != CameraRenderType.Base) return;

        renderer.EnqueuePass(_pass);
    }

    protected override void Dispose(bool disposing)
    {
        ReleaseHandle();
    }

    private void ReleaseHandle()
    {
        _destinationHandle?.Release();
        _destinationHandle = null;
    }

    private class CopyCameraColorPass : ScriptableRenderPass
    {
        private class PassData
        {
            public TextureHandle source;
        }

        private readonly RTHandle _destination;

        public CopyCameraColorPass(RTHandle destination)
        {
            _destination = destination;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (_destination == null) return;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

            if (resourceData.isActiveTargetBackBuffer)
            {
                Debug.LogWarning(
                    "CopyCameraColorFeature: 카메라 컬러가 Back Buffer입니다. " +
                    "Renderer Asset의 Intermediate Texture를 Always로 변경하세요.");
                return;
            }

            TextureHandle source = resourceData.activeColorTexture;
            TextureHandle destination = renderGraph.ImportTexture(_destination);

            using IRasterRenderGraphBuilder builder =
                renderGraph.AddRasterRenderPass<PassData>("Copy Camera Color", out PassData passData);

            passData.source = source;

            builder.UseTexture(source, AccessFlags.Read);
            builder.SetRenderAttachment(destination, 0, AccessFlags.WriteAll);
            builder.AllowPassCulling(false);

            builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
            {
                Blitter.BlitTexture(
                    context.cmd,
                    data.source,
                    new Vector4(1f, 1f, 0f, 0f),
                    0,
                    false);
            });
        }
    }
}
