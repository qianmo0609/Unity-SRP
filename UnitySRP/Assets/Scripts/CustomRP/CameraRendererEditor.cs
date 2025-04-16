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
    #region 绘制不支持Shader显示
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
    /// 绘制Gismos
    /// </summary>
    partial void DrawGizmos()
    {
        //判断是否需要绘制Gizmos
        if (Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(camera,GizmoSubset.PreImageEffects);
            context.DrawGizmos(camera,GizmoSubset.PostImageEffects);
        }
    }

    /// <summary>
    /// 绘制UI
    /// </summary>
    partial void PrepareforSceneWindow()
    {
        //判断摄像机是否在Scene窗口渲染
        if(camera.cameraType == CameraType.SceneView)
        {
            //把UI几何体放入到Scene窗口进行渲染
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
    }

    //准备数据将绘制buffer用摄像机的名字命名
    partial void PrepareBuffer()
    {
        Profiler.BeginSample("Editor Only");
        buffer.name = SampleName = camera.name;
        Profiler.EndSample();
    }
#endif
}
