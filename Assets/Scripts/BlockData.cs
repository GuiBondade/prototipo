using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Block Programming/Block Data")]
public class BlockData : ScriptableObject
{
    public string blockName;
    public BlockFunction function;
    public Color color = Color.white;
    public GameObject blockPrefab; // enum entre next e body, que vai ser usado em switch pra definir qual prefab vai ser instanciado?
    public BlockType type;
    public List<ParameterInfo> listaParametros = new List<ParameterInfo>();
}

public enum BlockFunction { // alterar depois, nao sei como (talvez enum de chamadas de funções do dicionario de traduções(case pro swicth ou chamada direta))
    Action,
    Condition,
    Loop,
    Event
}

public enum BlockType {
    Next,
    Body
}

[System.Serializable]
public class ParameterInfo {
    public string name;
    public UnityEditor.MonoScript ScriptTypeParameter;
}




