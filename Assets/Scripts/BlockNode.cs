using UnityEngine;

[System.Serializable]
public class BlockNode
{
   public int id;
   public string blockName;
   public BlockType type;

   public int next; // id do próximo bloco
   public int body; // id do bloco dentro (loop/if)
}
