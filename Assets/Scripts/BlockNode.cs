using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BlockNode
{
   public int id;
   public string blockDataId;
   public List<int> paramIds;
   public int rootId;
   public int next; // id do próximo bloco
   public int body; // id do bloco dentro (loop/if)
}