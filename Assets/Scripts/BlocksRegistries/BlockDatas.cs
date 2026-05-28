using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockDatas", menuName = "ScriptableObjects/BlockDatas")]
public class BlockDatas : ScriptableObject
{
    [Serializable]
    public struct DataEntry {
        public BlockData data;
    }

    [SerializeField] public List<DataEntry> listDatas;

    private Dictionary<string, BlockData> dictionaryDatas;

    public void Initialize() {
        dictionaryDatas = new Dictionary<string, BlockData>();//
        foreach (var entry in listDatas) {
            if (entry.data == null) continue;
            var keyId = entry.data.id;
            if (!dictionaryDatas.ContainsKey(keyId)) {
                dictionaryDatas.Add(keyId, entry.data);
            }
        }
    }

    public BlockData GetData(string id) {
        if (dictionaryDatas == null) Initialize();

        if (dictionaryDatas.TryGetValue(id, out BlockData data))
        {
            return data;
        }

        Debug.Log($"Erro no GetData, id: '{id}' não settado");
        return null;
    }
}