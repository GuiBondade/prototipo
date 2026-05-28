//INUTIL

/* using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockPrefabs", menuName = "ScriptableObjects/BlockPrefabs")]
public class BlockPrefabs : ScriptableObject
{
    [Serializable]
    public struct PrefabEntry {
        public PrefabsId id;
        public GameObject prefab;
    }

    [SerializeField] public List<PrefabEntry> listPrefabs;

    private Dictionary<string, GameObject> dictionaryPrefabs;

    public void Initialize() {
        dictionaryPrefabs = new Dictionary<string, GameObject>();
        foreach (var entry in listPrefabs) {
            string idString = entry.id.ToString();
            if (!dictionaryPrefabs.ContainsKey(idString)) {
                dictionaryPrefabs.Add(idString, entry.prefab);
            }
        }
    }

    public GameObject GetPrefab(string id) {
        if (dictionaryPrefabs == null) Initialize();

        if (dictionaryPrefabs.TryGetValue(id, out GameObject prefab))
        {
            return prefab;
        }

        Debug.Log($"Erro no GetPrefab, id: '{id}' não settado");
        return null;
    }
}


 */