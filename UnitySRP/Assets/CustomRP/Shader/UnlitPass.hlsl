#ifndef CUSTOM_UNLIT_PASS_INCLUDED
#define CUSTOM_UNLIT_PASS_INCLUDED

#include "../ShaderLib/Common.hlsl"

//SRP Batcher
/*CBUFFER_START(UnityPerMaterial)
//float4 _BaseColor;
CBUFFER_END*/

//GPU Instancing
//支持每个实例的材质数据，即添加GPU Instancing的数组支持
UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
    UNITY_DEFINE_INSTANCED_PROP(float4,_BaseMap_ST)
    UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
    UNITY_DEFINE_INSTANCED_PROP(float,_Cutoff)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

struct Attributes{
    float3 positionOS:POSITION;
    float2 baseUV:TEXCOORD0;
    //需要知道当前被渲染的对象索引，来访问实例化的数据数组
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings{
    float4 positionCS:SV_POSITION;
    float2 baseUV:VAR_BASE_UV;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

TEXTURE2D(_BaseMap);   //纹理必须上传到GPU内存中
SAMPLER(sampler_BaseMap); //声明采样器

Varyings UnlitPassVertex(Attributes input){
    Varyings output;
    UNITY_SETUP_INSTANCE_ID(input);  //从input中提取索引，并将其存储到一个全局静态变量中，其他的GPU Instancing相关的宏都可以访问这个变量
    UNITY_TRANSFER_INSTANCE_ID(input,output);//将索引从input传递到output中
    float3 positionOS = input.positionOS;
    float3 positionWS = TransformObjectToWorld(positionOS.xyz);
    output.positionCS = TransformWorldToHClip(positionWS);

    float4 baseST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_BaseMap_ST);
    output.baseUV = input.baseUV * baseST.xy + baseST.zw;

	return output;
}

float4 UnlitPassFragment(Varyings input):SV_TARGET{
    UNITY_SETUP_INSTANCE_ID(input);
           //从实例化的数据数组中获取当前被渲染的对象的材质数据
    float4 base = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_BaseColor)* SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,input.baseUV);
    //Alpha裁剪
   #if defined(_CLIPPING)
        clip(base.a - UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_Cutoff));
    #endif
    return base;
}
#endif
