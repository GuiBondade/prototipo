using System.Collections.Generic;
using UnityEngine;

public class WorkspaceGraphBuilder : MonoBehaviour
{
    public RectTransform workspace;

    private Dictionary<BlockUI, int> idMap = new();
    private List<BlockNode> nodes = new();
    private int idCounter = 0;

    public List<BlockNode> WorkspaceBuildGraph() {
        idMap.Clear();
        nodes.Clear();
        idCounter = 0;

        // pega todos blocos no workspace (nível raiz)
        foreach (Transform child in workspace) {
            var block = child.GetComponent<BlockUI>();
            if (block != null) {
                Traverse(block);
            }
        }
        return nodes;
    }

    private int Traverse(BlockUI block) {
        if (idMap.ContainsKey(block)) return idMap[block];

        int myId = idCounter++;
        idMap[block] = myId;

        var node = new BlockNode {
            id = myId,
            blockName = block.data.blockName,
            type = block.data.type,
            next = -1,
            body = -1
        };

        // Seguir conexões
        var next = block.GetNext();
        if (next != null) node.next = Traverse(next);

        var body = block.GetBody();
        if (body != null) node.body = Traverse(body);

        nodes.Add(node);
        return myId;
    }
}
