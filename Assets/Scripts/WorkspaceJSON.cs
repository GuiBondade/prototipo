using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // SAVE e LOAD (JSON)
    // chama o WorkspaceBuilder (SAVE)

    // salvo em: C:\Users\<seu_usuario>\AppData\LocalLow\<CompanyName>[DefaultCompany*]\<ProductName>\workspace.json
    /* var nodes = builder.WorkspaceBuildGraph();
    var json = JsonUtility.ToJson(nodes, true); // true = readability (tira quando salvar de verdade)
    var folder = Application.persistentDataPath;
    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
    var path = Path.Combine(folder, "workspace.json");
    File.WriteAllText(path, json); */
}
