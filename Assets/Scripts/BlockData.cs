using UnityEngine;

[CreateAssetMenu(menuName = "Block Programming/Block Data")]
public class BlockData : ScriptableObject
{
    public string blockName;
    public BlockType type;
    public Color color = Color.white;
}

public enum BlockType {
    Action,
    Condition,
    Loop,
    Event
}
