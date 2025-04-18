using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]//��������ض��
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
    /// ����������ֵ����ʱ���ô˺���
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
