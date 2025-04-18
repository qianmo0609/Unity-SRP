using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipline : RenderPipeline
{
    CameraRenderer cameraRender = new CameraRenderer();

    bool useDynamicBatching, useGPUInstancing;

    public CustomRenderPipline(bool useDynamicBatching,bool useGPUInstancing,bool useSRPBatcher)
    {
        this.useDynamicBatching = useDynamicBatching;
        this.useGPUInstancing = useGPUInstancing;
        //SRP Batcher�Ƿ���
        GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameraRender.Render(context, cameras[i],useDynamicBatching,useGPUInstancing);
        }  
    }
}
