using System.Reflection;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
    [System.Serializable]
    public class FastBloomSettings
    {
        [SerializeField]
        public Material blitMaterial = null;
        [SerializeField]
        public bool SetBloomIterations = false;
        [SerializeField]
        [Range(2, 9)]
        public int BloomIterations = 6;
        [SerializeField]
        [Range(0, 1)]
        public float BloomDiffusion = 1f;
        [SerializeField]
        public Color BloomColor = Color.white;
        [SerializeField]
        [Min(0f)]
        public float BloomAmount = 1f;
        [SerializeField]
        [Min(0f)]
        public float BloomThreshold = 0.0f;
        [SerializeField]
        [Range(0, 1)]
        public float BloomSoftness = 0.0f;
    }

    public class FastBloomUrp : ScriptableRendererFeature
    {
        public FastBloomSettings settings = new FastBloomSettings();

        FastBloomUrpPass fastBloomUrpPass;
        ScriptableRendererData srd;

        public override void Create()
        {
            fastBloomUrpPass = new FastBloomUrpPass(settings);
            if (srd == null)
            {
                var pipeline = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
                var propertyInfo = pipeline?.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
                srd = ((ScriptableRendererData[])propertyInfo?.GetValue(pipeline))?[0];
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (srd != null && srd.rendererFeatures[srd.rendererFeatures.Count - 1].GetType().Name == GetType().Name && !renderingData.postProcessingEnabled)
                renderingData.cameraData.resolveFinalTarget = false;
            fastBloomUrpPass.Setup(renderer.cameraColorTarget, renderingData.cameraData.resolveFinalTarget, name);
            renderer.EnqueuePass(fastBloomUrpPass);
        }

        public class FastBloomUrpPass : ScriptableRenderPass
        {
            private RenderTargetIdentifier source;
            private RenderTargetIdentifier temp = new RenderTargetIdentifier(tempString, 0, CubemapFace.Unknown, -1);

            private RenderTargetIdentifier[] bloomTemps =
            {
                 new RenderTargetIdentifier(bloomTempStrings[0], 0, CubemapFace.Unknown, -1),
                 new RenderTargetIdentifier(bloomTempStrings[1], 0, CubemapFace.Unknown, -1),
                 new RenderTargetIdentifier(bloomTempStrings[2], 0, CubemapFace.Unknown, -1),
                 new RenderTargetIdentifier(bloomTempStrings[3], 0, CubemapFace.Unknown, -1),
                 new RenderTargetIdentifier(bloomTempStrings[4], 0, CubemapFace.Unknown, -1),
                 new RenderTargetIdentifier(bloomTempStrings[5], 0, CubemapFace.Unknown, -1),
                 new RenderTargetIdentifier(bloomTempStrings[6], 0, CubemapFace.Unknown, -1),
                 new RenderTargetIdentifier(bloomTempStrings[7], 0, CubemapFace.Unknown, -1),
                 new RenderTargetIdentifier(bloomTempStrings[8], 0, CubemapFace.Unknown, -1)
            };

            private RenderTargetIdentifier[] bloomTemps1 =
            {
                 new RenderTargetIdentifier(bloomTempStrings1[0], 0, CubemapFace.Unknown, -1),
                 new RenderTargetIdentifier(bloomTempStrings1[1], 0, CubemapFace.Unknown, -1),
                 new RenderTargetIdentifier(bloomTempStrings1[2], 0, CubemapFace.Unknown, -1),
                 new RenderTargetIdentifier(bloomTempStrings1[3], 0, CubemapFace.Unknown, -1),
                 new RenderTargetIdentifier(bloomTempStrings1[4], 0, CubemapFace.Unknown, -1),
                 new RenderTargetIdentifier(bloomTempStrings1[5], 0, CubemapFace.Unknown, -1),
                 new RenderTargetIdentifier(bloomTempStrings1[6], 0, CubemapFace.Unknown, -1),
                 new RenderTargetIdentifier(bloomTempStrings1[7], 0, CubemapFace.Unknown, -1),
                 new RenderTargetIdentifier(bloomTempStrings1[8], 0, CubemapFace.Unknown, -1)
            };

            private readonly float bloomDiffusion;
            private readonly bool setBloomIterations;
            private readonly int bloomIterations;
            private readonly Color bloomColor;
            private readonly float bloomThreshold;
            private readonly float bloomSoftness;
            private readonly Material material;
            readonly GraphicsFormat format;
            readonly bool rgbm;
            private float knee;
            private Vector4 bloomData;
            private Vector2[] offset;
            private bool isFinal;
            private string name;
            private ProfilingSampler sampler  = new ProfilingSampler("FastBloom");

            static readonly int tempString = Shader.PropertyToID("_Temp");
            static readonly int blurAmountString = Shader.PropertyToID("_BlurAmount");
            static readonly int offsetString = Shader.PropertyToID("_Offset");
            static readonly int bloomColorString = Shader.PropertyToID("_BloomColor");
            static readonly int blDataString = Shader.PropertyToID("_BloomData");

            static readonly string rgbmKeyword = "_USE_RGBM";

            static readonly int[] bloomTempStrings =
            {
                Shader.PropertyToID("_BloomTemp"),
                Shader.PropertyToID("_BloomTemp4"),
                Shader.PropertyToID("_BloomTemp8"),
                Shader.PropertyToID("_BloomTemp16"),
                Shader.PropertyToID("_BloomTemp32"),
                Shader.PropertyToID("_BloomTemp64"),
                Shader.PropertyToID("_BloomTemp128"),
                Shader.PropertyToID("_BloomTemp256"),
                Shader.PropertyToID("_BloomTemp512")
            };
            static readonly int[] bloomTempStrings1 =
            {
                Shader.PropertyToID("_BloomTemp1"),
                Shader.PropertyToID("_BloomTemp41"),
                Shader.PropertyToID("_BloomTemp81"),
                Shader.PropertyToID("_BloomTemp161"),
                Shader.PropertyToID("_BloomTemp321"),
                Shader.PropertyToID("_BloomTemp641"),
                Shader.PropertyToID("_BloomTemp1281"),
                Shader.PropertyToID("_BloomTemp2561"),
                Shader.PropertyToID("_BloomTemp5121")
            };

            RenderTextureDescriptor opaqueDesc;

            public FastBloomUrpPass(FastBloomSettings settings)
            {
                material = settings.blitMaterial;
                bloomDiffusion = Mathf.Lerp(0.05f, 0.95f, settings.BloomDiffusion);
                var c = settings.BloomColor;
                var l = ColorUtils.Luminance(c);
                bloomColor = l > 0f ? c * (1f / l) : Color.white;
                bloomColor *= settings.BloomAmount;
                bloomThreshold = Mathf.GammaToLinearSpace(settings.BloomThreshold);
                bloomIterations = settings.BloomIterations;
                setBloomIterations = settings.SetBloomIterations;
                bloomSoftness = settings.BloomSoftness;
                knee = bloomThreshold * bloomSoftness;
                bloomData = new Vector4(bloomThreshold, bloomThreshold - knee, 2f * knee, 1f / (4f * knee + 0.0001f));
                rgbm = !SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, FormatUsage.Linear | FormatUsage.Render);
                format = !rgbm ? GraphicsFormat.B10G11R11_UFloatPack32 : QualitySettings.activeColorSpace == ColorSpace.Linear ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_UNorm;
            }

            public void Setup(RenderTargetIdentifier source, bool isFinal, string tag)
            {
                this.source = source;
                this.isFinal = isFinal;
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
                name = tag;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {

                CommandBuffer cmd = CommandBufferPool.Get(name);
                using (new ProfilingScope(cmd, sampler))
                {
                    if (bloomDiffusion == 0 && bloomColor.r == 0)
                        return;

                    var cameraData = renderingData.cameraData;
                    material.SetFloat(blurAmountString, bloomDiffusion);
                    material.SetColor(bloomColorString, bloomColor);
                    material.SetVector(blDataString, bloomData);

                    if (rgbm && !material.IsKeywordEnabled(rgbmKeyword))
                        material.EnableKeyword(rgbmKeyword);
                    else if (!rgbm && material.IsKeywordEnabled(rgbmKeyword))
                        material.DisableKeyword(rgbmKeyword);

                    opaqueDesc = cameraData.cameraTargetDescriptor;
                    opaqueDesc.autoGenerateMips = false;
                    opaqueDesc.useMipMap = false;
                    opaqueDesc.msaaSamples = 1;
                    opaqueDesc.depthBufferBits = 0;

                    var bloomIteration = setBloomIterations ? bloomIterations : Mathf.Clamp(Mathf.FloorToInt(Mathf.Log(Mathf.Max(opaqueDesc.width >> 1, opaqueDesc.height >> 1), 2f) - 1), 1, 9);
                    offset = new Vector2[bloomIteration];

                    if (isFinal)
                        cmd.GetTemporaryRT(tempString, opaqueDesc, FilterMode.Bilinear);

                    opaqueDesc.graphicsFormat = format;

                    for (int i = 0; i < bloomIteration; i++)
                    {
                        opaqueDesc.height = Mathf.Max(1, opaqueDesc.height >> 1);
                        opaqueDesc.width = Mathf.Max(1, opaqueDesc.width >> 1);
                        cmd.GetTemporaryRT(bloomTempStrings[i], opaqueDesc, FilterMode.Bilinear);
                        cmd.GetTemporaryRT(bloomTempStrings1[i], opaqueDesc, FilterMode.Bilinear);
                        offset[i] = new Vector2(1f / opaqueDesc.width, 1f / opaqueDesc.height);
                    }

                    if (isFinal)
                        Render(cmd, cameraData, source, temp, material, 4);
                    //cmd.Blit(source, temp);

                    Render(cmd, cameraData, source, bloomTemps[0], material, 0);
                    //cmd.Blit(source, bloomTemps[0], material, 0);
                    for (int i = 0; i < bloomIteration - 1; i++)
                    {
                        cmd.SetGlobalVector(offsetString, offset[i]);
                        Render(cmd, cameraData, bloomTemps[i], bloomTemps[i + 1], material, 1);
                        //cmd.Blit(bloomTemps[i], bloomTemps[i + 1], material, 1);
                    }

                    for (int i = bloomIteration - 2; i >= 0; i--)
                    {
                        cmd.SetGlobalTexture(bloomTempStrings[0], i == bloomIteration - 2 ? bloomTemps[i + 1] : bloomTemps1[i + 1]);
                        Render(cmd, cameraData, bloomTemps[i], bloomTemps1[i], material, 2);
                        //cmd.Blit(bloomTemps[i], bloomTemps1[i], material, 2);
                    }

                    cmd.SetGlobalTexture(bloomTempStrings[0], bloomTemps1[0]);
                    Render(cmd, cameraData, isFinal ? temp : source, new RenderTargetIdentifier(isFinal ? source : cameraData.targetTexture != null ? cameraData.targetTexture : RenderTargetHandle.CameraTarget.Identifier(), 0, CubemapFace.Unknown, -1), material, 3);
                    //cmd.Blit(isFinal ? temp : source, isFinal ? source : RenderTargetHandle.CameraTarget.Identifier(), material, 3);

                    for (int i = 0; i < bloomIteration; i++)
                    {
                        cmd.ReleaseTemporaryRT(bloomTempStrings[i]);
                        cmd.ReleaseTemporaryRT(bloomTempStrings1[i]);
                    }

                    if (isFinal)
                        cmd.ReleaseTemporaryRT(tempString);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            private void Render(CommandBuffer cmd, CameraData cameraData, RenderTargetIdentifier baseMap, RenderTargetIdentifier target, Material m, int pass = -1)
            {
                cmd.SetGlobalTexture("_BloomTex", baseMap);
                cmd.SetRenderTarget(target, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
                cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
                cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, m, 0, pass);
                cmd.SetViewProjectionMatrices(cameraData.camera.worldToCameraMatrix, cameraData.camera.projectionMatrix);
            }
        }
    }
}