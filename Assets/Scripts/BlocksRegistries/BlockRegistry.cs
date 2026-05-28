using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRegistry : MonoBehaviour
{
    // Singleton instanciado de antemão que referencia os prefab assets
    // POR NA CENA INICIAL, É PRA ESTAR SEMPRE CARREGADO (da pra alterar isso futuramente se quiser)
    public static BlockRegistry instancia { get; private set; }

    /* [SerializeField] private BlockPrefabs prefabs;  */
    [SerializeField] private BlockDatas datas; 

    void Awake() {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);

        // Inicializa o dicionário uma única vez no início do jogo
        /* if (prefabs != null)
        {
            prefabs.Initialize();
        } */
        if (datas != null)
        {
            datas.Initialize();
        }
    }

    /* public GameObject GetBlockPrefab(string id)
    {
        if (prefabs == null)
        {
            Debug.LogError("BlockPrefabs não configurado no BlockRegistry!");
            return null;
        }
        return prefabs.GetPrefab(id);
    } */

    public BlockData GetBlockData(string id)
    {
        if (datas == null)
        {
            Debug.LogError("BlockDatas não configurado no BlockRegistry!");
            return null;
        }
        return datas.GetData(id);
    }
}
