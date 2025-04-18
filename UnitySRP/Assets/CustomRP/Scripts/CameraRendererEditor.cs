using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    partial void DrawUnsupportedShaders();
    partial void DrawGizmos();
    partial void PrepareforSceneWindow();
    partial void PrepareBuffer();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    #region ���Ʋ�֧��Shader��ʾ
    static ShaderTagId[] legacyShaderTagIds = {
         new ShaderTagId("Always"),
         new ShaderTagId("ForwardBase"),
         new ShaderTagId("PrepassBase"),

         new ShaderTagId("Vertex"),
         new ShaderTagId("VertexLMRGBM"),
         new ShaderTagId("VertexLM")
    };

    static Material errorMaterial;

    string SampleName { get; set; }

    partial void DrawUnsupportedShaders()
    {
        if (errorMaterial == null)
        {
            errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
        }

        var drawingSettings = new DrawingSettings(legacyShaderTagIds[0],new SortingSettings(camera))
        {
            overrideMaterial = errorMaterial
        };

        for (int i = 1; i < legacyShaderTagIds.Length; i++)
        {
            drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
        }

        var filteringSettings = FilteringSettings.defaultValue;
        context.DrawRenderers(cullingResults,ref drawingSettings,ref filteringSettings);
    }
    #endregion

    /// <summary>
    /// ����Gismos
    /// </summary>
    partial void DrawGizmos()
    {
        //�ж��Ƿ���Ҫ����Gizmos
        if (Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(camera,GizmoSubset.PreImageEffects);
            context.DrawGizmos(camera,GizmoSubset.PostImageEffects);
        }
    }

    /// <summary>
    /// ����UI
    /// </summary>
    partial void PrepareforSceneWindow()
    {
        //�ж�������Ƿ���Scene������Ⱦ
        if(camera.cameraType == CameraType.SceneView)
        {
            //��UI��������뵽Scene���ڽ�����Ⱦ
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
    }

    //׼�����ݽ�����buffer�����������������
    partial void PrepareBuffer()
    {
        Profiler.BeginSample("Editor Only");
        buffer.name = SampleName = camera.name;
        Profiler.EndSample();
    }
#endif
}
