using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ParameterInfo {
    public ParameterType type;
    public string name;

}

[CreateAssetMenu(menuName = "Block Programming/Block Data")]
public class BlockData : ScriptableObject
{
    public string blockName;
    public BlockType type;
    public Color color = Color.white;

    public List<ParameterInfo> parametros = new List<ParameterInfo>();
}

public enum BlockType {
    Action,
    Condition,
    Loop,
    Event
}

public enum ParameterType {
    Boolean,
    Number,
    Direction
}
