using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveNodesData
{
   public List<ParameterNode> parametrosData = new();
   public List<BlockNode> blocosData = new();

   public void Clear()
   {
      parametrosData.Clear();
      blocosData.Clear();
   }
}