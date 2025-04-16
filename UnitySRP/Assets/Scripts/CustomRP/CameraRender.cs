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

    static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit"); //SRPDefaultUnlit��Unity�е�һ�����õ�Shader��������Ⱦ���ܹ���Ӱ�������

    public void Render(ScriptableRenderContext context ,Camera camera)
    {
        this.context = context;
        this.camera = camera;

        PrepareBuffer();
        //����UI
        this.PrepareforSceneWindow();
        if (!this.Cull())
        {
            return;
        }

        this.Setup();
        //���Ƽ�����
        this.DrawVisibleGeometry();
        //���Ʋ�֧��shader��ʾ
        this.DrawUnsupportedShaders();
        //����Gizmos
        this.DrawGizmos();
        this.Submit();
    }

    void Setup()
    {
        //��camera���������õ�context
        context.SetupCameraProperties(camera);
        CameraClearFlags flags = camera.clearFlags;
        //��������Culling Mask���������Ļ���
        buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth,flags == CameraClearFlags.Color,flags == CameraClearFlags.Color?camera.backgroundColor.linear:Color.clear);
        
        //���ȫ������
        //buffer.ClearRenderTarget(true, true, Color.clear);
        buffer.BeginSample(SampleName);
        this.ExecuteBuffer();
    }

    void DrawVisibleGeometry()
    {
       //��Ⱦ��͸������
        var sortingSetting = new SortingSettings(camera)
        {
            criteria = SortingCriteria.CommonOpaque
        };

        //ָ�������shader pass name����Ⱦ˳��
        var drawingSettings = new DrawingSettings(unlitShaderTagId,sortingSetting);
        //ָ��������Щ��Ⱦ����
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

        //��Ⱦ�ɼ��ļ�����
        context.DrawRenderers(cullingResults,ref drawingSettings,ref filteringSettings);

        //��Ⱦ��պ�
        context.DrawSkybox(camera);

        //��Ⱦ͸������
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
    /// �޳�������
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
