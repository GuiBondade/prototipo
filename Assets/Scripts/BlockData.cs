using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBlockData", menuName = "Block Programming/Block Data")]
public class BlockData : ScriptableObject
{
    public string id;
    public string blockName;
    public BlockFunction function;
    public Color color = Color.white;
    //[HideInInspector] public GameObject blockPrefab; // faz a partir do prefab abaixo
    public PrefabsId prefab; 
    public List<ParameterType> listaParametros = new List<ParameterType>();
}

public enum BlockFunction { // alterar depois, nao sei como (talvez enum de chamadas de funções do dicionario de traduções(case pro swicth ou chamada direta))
    Action,
    Condition,
    Loop,
    Event
}

/* [System.Serializable]
public class ParameterInfo {
    public string name;
    public ParameterType type;
} */

public enum PrefabsId {
    next,
    body,
    variavel,
    criarVariavel
}
