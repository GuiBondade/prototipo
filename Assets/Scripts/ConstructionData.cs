using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileInfo {
    public Vector2Int offset;
    public TileBase tile;
    public int layer = 1; // camada onde o tile será colocado, padrão 1
}

[CreateAssetMenu(fileName = "ConstructionData", menuName = "Game/Construction")]
public class ConstructionData : ScriptableObject {
    [Header("Tiles que aparecem visualmente (preview e colocação)")]
    public List<TileInfo> tilesVisual = new List<TileInfo>();

    [Header("Tiles que ocupam espaço (verificação de ocupação)")]
    public List<TileInfo> tilesOccupied = new List<TileInfo>();

    // Opcional
    public string displayName;
}
