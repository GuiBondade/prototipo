using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRegistry : MonoBehaviour
{
    [SerializeField] private BlockDatas blockDatas; 
    [SerializeField] private ParamDatas paramDatas; 

    void Awake() {
        // Inicializa o dicionário uma única vez no início do jogo
        if (paramDatas != null)
        {
            paramDatas.Initialize();
        }
        if (blockDatas != null)
        {
            blockDatas.Initialize();
        }
    }

    public ParamData GetParamData(ParameterType type)
    {
        if (paramDatas == null)
        {
            Debug.LogError("ParamDatas não configurado no BlockRegistry!");
            return null;
        }
        return paramDatas.GetData(type);
    }

    public BlockData GetBlockData(string id)
    {
        if (blockDatas == null)
        {
            Debug.LogError("BlockDatas não configurado no BlockRegistry!");
            return null;
        }
        return blockDatas.GetData(id);
    }
}
