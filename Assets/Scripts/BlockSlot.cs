using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotType { Next, Body, Null }

public class BlockSlot : MonoBehaviour
{
    public BlockUI parentBlock;
    
    public SlotType slotType;

    public BlockUI currentBlock;
}
