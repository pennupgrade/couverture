// SPDX-FileCopyrightText: 2023 Logan Cho
// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineMaskFeature : ScriptableRendererFeature
{
    public LayerMask outlineLayerMask;
    public RenderTexture OutlineMaskTexture;

    public RenderPassEvent _OutlineMaskEvent = RenderPassEvent.AfterRenderingOpaques;

    OutlineMaskPass m_OutlineMaskPass;
    public Material outlineMaskMaterial;


    /// <inheritdoc/>
    public override void Create()
    {
        m_OutlineMaskPass = new OutlineMaskPass(OutlineMaskTexture, outlineLayerMask, outlineMaskMaterial);
        m_OutlineMaskPass.renderPassEvent = _OutlineMaskEvent;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
            renderer.EnqueuePass(m_OutlineMaskPass);
    }
}

class OutlineMaskPass : ScriptableRenderPass
{
    private ProfilingSampler m_ProfilingSampler;
    private FilteringSettings m_FilteringSettings;
    private List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
    private RenderTexture target;
    private Material outlineMaskMaterial;

    public OutlineMaskPass(RenderTexture targetTexture, LayerMask layerMask, Material mat)
    {
        m_ProfilingSampler = new ProfilingSampler("RenderOutlineMask");
        m_FilteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);

        target = targetTexture;

        m_ShaderTagIdList.Add(new ShaderTagId("DepthOnly")); // Only render DepthOnly pass
        outlineMaskMaterial = mat;
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        ConfigureTarget(target);
        ConfigureClear(ClearFlag.All, Color.black);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType != CameraType.Game)
            return;
        SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
        DrawingSettings drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortingCriteria);
        drawingSettings.overrideMaterial = outlineMaskMaterial;

        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, m_ProfilingSampler))
        {
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    // Cleanup any allocated resources that were created during the execution of this render pass.
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
    }
}