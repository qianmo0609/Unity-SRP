using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    ScriptableRenderContext context;

    Camera camera;

    const string bufferName = "Render Camera";

    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName,
    };

    CullingResults cullingResults;

    static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit"); //SRPDefaultUnlit是Unity中的一个内置的Shader，用于渲染不受光照影响的物体

    public void Render(ScriptableRenderContext context ,Camera camera)
    {
        this.context = context;
        this.camera = camera;

        PrepareBuffer();
        //绘制UI
        this.PrepareforSceneWindow();
        if (!this.Cull())
        {
            return;
        }

        this.Setup();
        //绘制几何体
        this.DrawVisibleGeometry();
        //绘制不支持shader显示
        this.DrawUnsupportedShaders();
        //绘制Gizmos
        this.DrawGizmos();
        this.Submit();
    }

    void Setup()
    {
        //将camera的属性设置到context
        context.SetupCameraProperties(camera);
        CameraClearFlags flags = camera.clearFlags;
        //清除摄像机Culling Mask符合条件的绘制
        buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth,flags == CameraClearFlags.Color,flags == CameraClearFlags.Color?camera.backgroundColor.linear:Color.clear);
        
        //清除全部绘制
        //buffer.ClearRenderTarget(true, true, Color.clear);
        buffer.BeginSample(SampleName);
        this.ExecuteBuffer();
    }

    void DrawVisibleGeometry()
    {
       //渲染不透明物体
        var sortingSetting = new SortingSettings(camera)
        {
            criteria = SortingCriteria.CommonOpaque
        };

        //指定允许的shader pass name和渲染顺序
        var drawingSettings = new DrawingSettings(unlitShaderTagId,sortingSetting);
        //指定允许哪些渲染队列
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

        //渲染可见的几何体
        context.DrawRenderers(cullingResults,ref drawingSettings,ref filteringSettings);

        //渲染天空盒
        context.DrawSkybox(camera);

        //渲染透明物体
        sortingSetting.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSetting;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;

        context.DrawRenderers(cullingResults,ref drawingSettings, ref filteringSettings);
    }

    void Submit()
    {
        buffer.EndSample(SampleName);
        this.ExecuteBuffer();
        context.Submit();
    }

    void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    /// <summary>
    /// 剔除几何体
    /// </summary>
    /// <returns></returns>
    bool Cull()
    {
        if(camera.TryGetCullingParameters(out ScriptableCullingParameters p))
        {
            cullingResults = context.Cull(ref p);
            return true;
        }
        return false;
    }
}
