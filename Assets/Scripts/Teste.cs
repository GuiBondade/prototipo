using UnityEngine;
using UnityEngine.UI;

public class Teste : MonoBehaviour
{
    public WorkspaceGraphBuilder builder;

    public void TestGraph()
    {
        var nodes = builder.WorkspaceBuildGraph();
        foreach (var n in nodes)
        {
            Debug.Log($"Block {n.id} - {n.blockName} | Next: {n.next} | Body: {n.body}");
        }
    }
}
