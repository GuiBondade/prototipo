using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BlockNode
{
   public int id;
   public string blockName;
   public BlockType type;
   public BlockFunction function; //blockdata pra instanciar 
   //(nao faz sentido ter um enum a mais ao inves de somente propria ação do bloco(funcao pro tradutor), seria um aninhamento de switchs no melhor caso, oque nao é recomendado)
   // prefab do bloco, ja poe no block data né
   public List<int> paramIds;
   public int next; // id do próximo bloco
   public int body; // id do bloco dentro (loop/if)
}