using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ParameterNode 
{
   public int id;
   public ParameterType type; // acho que paramSetup funfa, por ser generic deve servir pra inherit dele
   public string value;
   public int leftOperandId;
   public int rightOperandId;
}