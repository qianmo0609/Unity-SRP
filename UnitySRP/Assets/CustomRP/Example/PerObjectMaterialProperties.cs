using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]//不允许挂载多次
public class PerObjectMaterialProperties : MonoBehaviour
{
    static int baseColorId = Shader.PropertyToID("_BaseColor");
    static int cutoffId = Shader.PropertyToID("_Cutoff");

    [SerializeField]
    Color baseColor = Color.white;

    [SerializeField]
    float cutoff = .5f;

    static MaterialPropertyBlock block;

    private void Awake()
    {
        OnValidate();
    }

    /// <summary>
    /// 当检视面板的值更改时调用此函数
    /// </summary>
    private void OnValidate()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }
        block.SetColor(baseColorId, baseColor); 
        block.SetFloat(cutoffId, cutoff);
        GetComponent<Renderer>().SetPropertyBlock(block);
    }
}
