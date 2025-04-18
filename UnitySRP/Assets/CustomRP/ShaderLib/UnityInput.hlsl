#ifndef CUSTOM_UNLIT_PASS_Input_INCLUDED
#define CUSTOM_UNLIT_PASS_Input_INCLUDED

//SRP Batcher 使用
CBUFFER_START(UnityPerDraw)
float4x4 unity_ObjectToWorld;
float4x4 unity_WorldToObject;
float4 unity_LODFade; //cbuffer中包含了lod的信息，lod是lod的索引，unity_LODFade是lod的权重,必须加上，否则SRP Batcher不兼容
real4 unity_WorldTransformParams; //real4根据目标平台而选择的 float4 或者 half4的别名
CBUFFER_END

float4x4 unity_MatrixVP;
float4x4 unity_MatrixV;
float4x4 unity_MatrixInvV;
float4x4 unity_prev_MatrixM;
float4x4 unity_prev_MatrixIM;
float4x4 glstate_matrix_projection;

#endif