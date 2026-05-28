using UnityEngine;
using UnityEngine.Pool; // Necessário para usar o sistema nativo
using System.Collections.Generic;

public class ProgrammingPool : MonoBehaviour
{
    [System.Serializable]
    public struct PoolConfig {
        public GameObject prefab;
        public int defaultCapacity;
        public int maxSize;   
    }

    [SerializeField] public PoolConfig blockNextConfig;
    [SerializeField] public PoolConfig blockBodyConfig;
    [SerializeField] public PoolConfig parameterConfig;

    private ObjectPool<GameObject> blockNextPool;
    private ObjectPool<GameObject> blockBodyPool;
    private ObjectPool<GameObject> parameterPool;

    void Awake()
    {
        
        blockNextPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(blockNextConfig.prefab, this.transform),
            actionOnGet: go => go.SetActive(true), 
            actionOnRelease: go => go.SetActive(false), 
            actionOnDestroy: go => Destroy(go), 
            collectionCheck: true,
            blockNextConfig.defaultCapacity, 
            blockNextConfig.maxSize
        );

        blockBodyPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(blockBodyConfig.prefab, this.transform), 
            actionOnGet: go => go.SetActive(true), 
            actionOnRelease: go => go.SetActive(false), 
            actionOnDestroy: go => Destroy(go), 
            collectionCheck: true,
            blockBodyConfig.defaultCapacity, 
            blockBodyConfig.maxSize
        );

        parameterPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(parameterConfig.prefab, this.transform), 
            actionOnGet: go => go.SetActive(true), 
            actionOnRelease: go => go.SetActive(false), 
            actionOnDestroy: go => Destroy(go), 
            collectionCheck: true,
            parameterConfig.defaultCapacity, 
            parameterConfig.maxSize
        );
        
    }

    public GameObject GetBlockNext() => blockNextPool.Get();
    public void ReleaseBlockNext(GameObject obj) => blockNextPool.Release(obj);

    public GameObject GetBlockBody() => blockBodyPool.Get();
    public void ReleaseBlockBody(GameObject obj) => blockBodyPool.Release(obj);

    public GameObject GetParameter() => parameterPool.Get();
    public void ReleaseParameter(GameObject obj) => parameterPool.Release(obj);
}