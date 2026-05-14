using UnityEngine;
using System.Collections.Generic;
//using TMPro; precisa? sepa se usar OptionData do dropdown diretamente sim

[CreateAssetMenu(menuName = "Block Programming/Block Data")]
public class BlockData : ScriptableObject
{
    public string blockName;
    public BlockType type;
    public Color color = Color.white;

    public List<ParameterInfo> listaParametros = new List<ParameterInfo>();
}

public enum BlockType {
    Action,
    Condition,
    Loop,
    Event
}

[System.Serializable]
public class ParameterInfo {
    public string name;
    [SerializeField] public UnityEditor.MonoScript parameterScriptData;

}




