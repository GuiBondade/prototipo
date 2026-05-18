using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Teste : MonoBehaviour // arquivo set/delete json
{
    public WorkspaceBuilder builder;

    public void TestGraph() // save (set)
    {
        // salvo em: C:\Users\<seu_usuario>\AppData\LocalLow\<CompanyName>[DefaultCompany*]\<ProductName>\workspace.json
        var nodes = builder.WorkspaceBuildGraph();
        var json = JsonUtility.ToJson(nodes, true); // true = readability (tira quando salvar de verdade)
        var folder = Application.persistentDataPath;
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, "workspace.json");
        File.WriteAllText(path, json);

        foreach (var p in nodes.parametrosData)
        {
            Debug.Log($"Parameter {p.id} - {p.type} | Value: {p.value} | LeftOperand: {p.leftOperandId} | RightOperand: {p.rightOperandId}");
        }
        foreach (var b in nodes.blocosData) // le de tras pra frente, pois salva de tras pra frente
        {
            Debug.Log($"Block {b.id} - {b.blockName} - {b.type} | Function: {b.function} | ParamIds: {string.Join(", ", b.paramIds)} | Next: {b.next} | Body: {b.body}");
        }
    }
}
