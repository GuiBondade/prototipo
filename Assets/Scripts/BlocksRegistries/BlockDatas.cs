using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockDatas", menuName = "ScriptableObjects/BlockDatas")]
public class BlockDatas : ScriptableObject
{
    [SerializeField] public List<BlockData> listDatas;

    private Dictionary<string, BlockData> dictionaryDatas;

    public void Initialize() {
        dictionaryDatas = new Dictionary<string, BlockData>();//
        foreach (var entry in listDatas) {
            if (entry == null) continue;
            var keyId = entry.id;
            if (!dictionaryDatas.ContainsKey(keyId)) {
                dictionaryDatas.Add(keyId, entry);
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