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
    [SerializeField] public PoolConfig sectionConfig;
    [SerializeField] public PoolConfig valueConfig;

    private ObjectPool<GameObject> blockNextPool;
    private ObjectPool<GameObject> blockBodyPool;
    private ObjectPool<GameObject> parameterPool;
    private ObjectPool<GameObject> sectionPool;
    private ObjectPool<GameObject> valuePool;

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

        sectionPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(sectionConfig.prefab, this.transform), 
            actionOnGet: go => go.SetActive(true), 
            actionOnRelease: go => go.SetActive(false), 
            actionOnDestroy: go => Destroy(go), 
            collectionCheck: true,
            sectionConfig.defaultCapacity, 
            sectionConfig.maxSize
        );

        valuePool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(valueConfig.prefab, this.transform), 
            actionOnGet: go => go.SetActive(true), 
            actionOnRelease: go => go.SetActive(false), 
            actionOnDestroy: go => Destroy(go), 
            collectionCheck: true,
            valueConfig.defaultCapacity, 
            valueConfig.maxSize
        );
    }

    public GameObject GetBlockNext() => blockNextPool.Get();
    public void ReleaseBlockNext(GameObject obj) {
        obj.SetParent(blockNextPool, false);
        blockNextPool.Release(obj);
    }

    public GameObject GetBlockBody() => blockBodyPool.Get();
    public void ReleaseBlockBody(GameObject obj) {
        obj.SetParent(blockBodyPool, false);
        blockBodyPool.Release(obj);
    } 

    public GameObject GetParameter() => parameterPool.Get();
    public void ReleaseParameter(GameObject obj) {
        obj.SetParent(parameterPool, false);
        parameterPool.Release(obj);
    } 

    public GameObject GetSection() => sectionPool.Get();
    public void ReleaseSection(GameObject obj) {
        obj.SetParent(sectionPool, false);
        sectionPool.Release(obj);
    } 

    public GameObject GetValue() => valuePool.Get();
    public void ReleaseValue(GameObject obj) {
        obj.SetParent(valuePool, false);
        valuePool.Release(obj);
    } 
}