Shader "Custom RP/UnlitShader" {
   Properties{
        _BaseMap("Texture",2D) = "white"{}
        _BaseColor("Color",Color) = (1.0,1.0,1.0,1.0)
        //目前Shader绘制图像颜色值的权重
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("Src Blend",Float)= 1
        //上一帧绘制图像颜色值的权重
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("Dst Blend",Float)= 0
        //深度写入
        [Enum(Off,0,On,1)]_ZWrite("Z Write",Float) = 1

        //Alpha 裁剪
        _Cutoff("Alpha Cutoff",Range(0.0,1.0)) = 0.5
        //Alpha 裁剪开关
        [Toggle(_CLIPPING)]_Clipping("Alpha Clipping",Float) = 0
    }

    SubShader{

        Pass{
            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            HLSLPROGRAM

            //开启GPU Instancing
            #pragma multi_compile_instancing
            //instancing_options 是一个预定义的宏，用于控制GPU Instancing的行为。
            //#pragma instancing_options nolodfade

            #pragma shader_feature _CLIPPING
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment

            #include "UnlitPass.hlsl"
            

            ENDHLSL
        }
    }
}
