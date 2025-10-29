using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementManager : MonoBehaviour {
    [Header("Referências")]
    public Tilemap collisionTilemap; // camada 0, usada para verificação de ocupação
    public Tilemap visualLayer1; // camada 1, onde as construções são pintadas
    public Tilemap visualLayer2; // camada 2, onde as construções são pintadas
    public Tilemap visualLayer3; // camada 3, onde as construções são pintadas
    public Tilemap visualLayer4; // camada 4, onde as construções são pintadas
    public Tilemap previewTilemap; // Tilemap separado para preview

    public ConstructionData constructionToPlace;

    // Classes auxiliares para armazenar informações sobre tiles e construções colocadas
    private class PlacedTile {
        public Vector3Int pos;
        public TileBase tile;
        public int layer;

        public PlacedTile(Vector3Int pos, TileBase tile, int layer) {
            this.pos = pos;
            this.tile = tile;
            this.layer = layer;
        }
    }

    private class PlacedConstruction {
        public ConstructionData data; // tipo de construção
        public List<PlacedTile> visualTiles = new List<PlacedTile>(); // tiles visuais
        public HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int>(); // posições ocupadas

        public PlacedConstruction(ConstructionData data) {
            this.data = data;
        }
    }

    // Dicionário que mapeia cada célula ocupada para a construção que a ocupa
    private Dictionary<Vector3Int, PlacedConstruction> occupiedCells = new Dictionary<Vector3Int, PlacedConstruction>();

    // Lista de todas as construções colocadas no mapa
    private List<PlacedConstruction> placedInstances = new List<PlacedConstruction>();

    // controle de posicionamento
    bool isPlacing = false;
    Vector3Int lastPreviewBaseCell; // posição inicial do último preview
    bool hasPreview = false; // se já tem preview desenhado

    // controle de destruição
    bool isDestroying = false;
    PlacedConstruction previewDestructionTarget = null;

    // Chamado pelo Botão UI
    public void StartPlacing() {
        if (constructionToPlace == null) {
            Debug.LogWarning("Nenhuma construção atribuída ao PlacementManager");
            return;
        }
        isPlacing = true;
        isDestroying = false; // garantir que não está em modo de destruição
        ClearPreview();
        Debug.Log("Modo de posicionamento ativado: " + constructionToPlace.displayName);
    }

    public void StartDestroying() {
        isDestroying = true;
        isPlacing = false; // garantir que não está em modo de posicionamento
        ClearPreview();
        Debug.Log("Modo de destruição ativado");
    }

    // Cancela o modo de posicionamento
    public void CancelPlacing() {
        ClearPreview();
        isPlacing = false;
        Debug.Log("Posicionamento cancelado");
    }

    public void CancelDestroying() {
        ClearPreview();
        isDestroying = false;
        previewDestructionTarget = null;
        Debug.Log("Destruição cancelada");
    }

    void Update() {
        if (isPlacing) {
            HandlePlacing();
            return;
        }

        if (isDestroying) {
            HandleDestroying();
            return;
        }
    }

    void HandlePlacing() {
        // Célula do mouse
        Vector3Int cellPos = GetMouseCell(collisionTilemap);

        // Atualizar preview
        if (!hasPreview || cellPos != lastPreviewBaseCell) {
            ClearPreview(); // Limpar preview antigo
            DrawPreview(cellPos); // Desenhar novo preview
            lastPreviewBaseCell = cellPos;
            hasPreview = true;
        }

        // ao clicar com botão esquerdo, tenta colocar a construção
        if (Input.GetMouseButtonDown(0)) {
            PlaceConstructionAt(cellPos);
            // Mudar linhas abaixo para dentro de PlaceConstructionAt se quiser continuar colocando mesmo apos errar a colocação
            isPlacing = false; // desativa o modo de posicionamento após colocar, pro jogador colocar 1 por vez que clica no botão UI
            ClearPreview(); // Limpar preview após colocar
        }          

        // botão direito cancela
        if (Input.GetMouseButtonDown(1)) {
            CancelPlacing();
        }
    }

    void HandleDestroying() {
        // Célula do mouse
        Vector3Int cellPos = GetMouseCell(collisionTilemap);

        // Verifica se há uma construção nessa célula
        PlacedConstruction target = null;
        occupiedCells.TryGetValue(cellPos, out target);

        // Se o alvo mudou, atualiza o preview
        if (target != previewDestructionTarget) {
            ClearPreview();
            previewDestructionTarget = target;
            
            if (previewDestructionTarget != null) {
                DrawDestructionPreview(previewDestructionTarget);
            }
        }
        // ao clicar com botão esquerdo, destrói a construção
        if (Input.GetMouseButtonDown(0)) {
            if (previewDestructionTarget == null) return; // nada para destruir
            DestroyPlacedConstruction(previewDestructionTarget);
            previewDestructionTarget = null; // resetar alvo
        }
        // ao clicar com o botão direito, cancela o modo de destruição
        if (Input.GetMouseButtonDown(1)) {
            CancelDestroying(); 
        }

    }

    // Verifica se todas as células da construção estão livres
    private bool CanPlaceAt(Vector3Int baseCell) {
        if (constructionToPlace == null) return false;

        foreach (var t in constructionToPlace.tilesOccupied) {
            if (occupiedCells.ContainsKey(GetTilePosition(baseCell, t))) return false; // célula já ocupada
        }
        return true; // todas as células livres
    }

    // Desenha preview fantasma
    void DrawPreview(Vector3Int baseCell) {
        if (constructionToPlace == null) return;

        Color previewColor = CanPlaceAt(baseCell) ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);

        foreach (var t in constructionToPlace.tilesVisual) {
            Vector3Int pos = GetTilePosition(baseCell, t);
            SetTileWithColor(previewTilemap, pos, t.tile, previewColor);
    }}

    void DrawDestructionPreview(PlacedConstruction target) {
        Color previewColor = new Color(1f, 0f, 0f, 0.5f); // vermelho transparente 

        foreach (var v in target.visualTiles) {
            SetTileWithColor(previewTilemap, v.pos, v.tile, previewColor);
        }
    }

    // limpa o preview
    void ClearPreview() {
        previewTilemap.ClearAllTiles();
        hasPreview = false;
    }

    // Pinta a construção no Tilemap a partir da célula base
    void PlaceConstructionAt(Vector3Int baseCell) {
        if (constructionToPlace == null) return;

        if (!CanPlaceAt(baseCell)) {
            Debug.Log("Não é possível colocar a construção aqui.");
            return;
        }

        PlacedConstruction pc = new PlacedConstruction(constructionToPlace);

        // Coloca a construção
        foreach (var t in constructionToPlace.tilesVisual) {
            Vector3Int pos = GetTilePosition(baseCell, t);
            Tilemap layerMap = GetTilemapForLayer(t.layer);
            SetTileWithColor(layerMap, pos, t.tile, Color.white); // cor normal

            // registra no objeto da instancia
            pc.visualTiles.Add(new PlacedTile(pos, t.tile, t.layer));
        }

        // marca a célula como ocupada
        foreach (var t in constructionToPlace.tilesOccupied) {
            Vector3Int pos = GetTilePosition(baseCell, t);
            occupiedCells[pos] = pc; // marca como ocupada

            // registra no objeto da instancia
            pc.occupiedPositions.Add(pos);
        }

        placedInstances.Add(pc); // adiciona à lista de construções colocadas

        Debug.Log($"Construção '{constructionToPlace.displayName}' colocada em {baseCell}.");
    }

    void DestroyPlacedConstruction(PlacedConstruction pc) {
        if (pc == null) return;

        // Remove os tiles visuais
        foreach (var pt in pc.visualTiles) {
            Tilemap layerMap = GetTilemapForLayer(pt.layer);
            SetTileWithColor(layerMap, pt.pos, null, Color.white); // remove o tile
        }

        // Libera as células ocupadas
        foreach (var pos in pc.occupiedPositions) {
            if (occupiedCells.ContainsKey(pos)) occupiedCells.Remove(pos);
        }

        placedInstances.Remove(pc); // remove da lista de construções colocadas

        ClearPreview();
        Debug.Log($"Construção '{previewDestructionTarget.data.displayName}' destruída.");
    }

    // FUNÇÕES AUXILIARES

    Tilemap GetTilemapForLayer(int layer) {
        switch (layer) {
            case 1: return visualLayer1;
            case 2: return visualLayer2;
            case 3: return visualLayer3;
            case 4: return visualLayer4;
            default: return visualLayer1; // padrão
        }
    }

    private Vector3Int GetTilePosition(Vector3Int baseCell, TileInfo tile) {
        return baseCell + new Vector3Int(tile.offset.x, tile.offset.y, 0);
    }

    private void SetTileWithColor(Tilemap map, Vector3Int pos, TileBase tile, Color color) {
        map.SetTile(pos, tile);
        map.SetColor(pos, color);
    }

    private Vector3Int GetMouseCell(Tilemap referenceTilemap) {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Alinha z ao do tilemap (evitar problemas com Z da Camera diferente)
        mouseWorld.z = referenceTilemap.transform.position.z;
        return referenceTilemap.WorldToCell(mouseWorld);
    }
}
