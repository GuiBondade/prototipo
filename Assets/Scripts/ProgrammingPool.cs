using UnityEngine;
using UnityEngine.Pool; // Necessário para usar o sistema nativo
using System.Collections.Generic;

public class ProgrammingPool : MonoBehaviour
{
    [System.Serializable]
    public struct BlockPoolConfig {
        public BlockUI prefab;
        public int defaultCapacity;
        public int maxSize;   
    }

    [System.Serializable]
    public struct CreateVarPoolConfig {
        public CreateVariableUI prefab;
        public int defaultCapacity;
        public int maxSize;   
    }

    [System.Serializable]
    public struct ParamPoolConfig {
        public ReferenceHolder prefab;
        public int defaultCapacity;
        public int maxSize;   
    }

    [System.Serializable]
    public struct SectionPoolConfig {
        public SectionInfo prefab;
        public int defaultCapacity;
        public int maxSize;   
    }

    [System.Serializable]
    public struct ValuePoolConfig {
        public ValueInfo prefab;
        public int defaultCapacity;
        public int maxSize;   
    }

    public BlocksFactory blocksFactory;

    [SerializeField] public BlockPoolConfig blockNextConfig;
    [SerializeField] public BlockPoolConfig blockBodyConfig;
    [SerializeField] public BlockPoolConfig blockVarConfig;
    [SerializeField] public CreateVarPoolConfig createVarConfig;
    [SerializeField] public ParamPoolConfig parameterConfig;
    [SerializeField] public SectionPoolConfig sectionConfig;
    [SerializeField] public ValuePoolConfig valueConfig;

    private ObjectPool<BlockUI> blockNextPool;
    private ObjectPool<BlockUI> blockBodyPool;
    private ObjectPool<BlockUI> blockVarPool;
    private ObjectPool<CreateVariableUI> createVarPool;
    private ObjectPool<ReferenceHolder> parameterPool;
    private ObjectPool<SectionInfo> sectionPool;
    private ObjectPool<ValueInfo> valuePool;

    void Awake()
    {
        
        blockNextPool = new ObjectPool<BlockUI>(
            createFunc: () => Instantiate(blockNextConfig.prefab, this.transform),
            actionOnGet: go => go.gameObject.SetActive(true), 
            actionOnRelease: go => go.gameObject.SetActive(false), 
            actionOnDestroy: go => Destroy(go), 
            collectionCheck: true,
            blockNextConfig.defaultCapacity, 
            blockNextConfig.maxSize
        );

        blockBodyPool = new ObjectPool<BlockUI>(
            createFunc: () => Instantiate(blockBodyConfig.prefab, this.transform),
            actionOnGet: go => go.gameObject.SetActive(true), 
            actionOnRelease: go => go.gameObject.SetActive(false), 
            actionOnDestroy: go => Destroy(go), 
            collectionCheck: true,
            blockBodyConfig.defaultCapacity, 
            blockBodyConfig.maxSize
        );

        blockVarPool = new ObjectPool<BlockUI>(
            createFunc: () => Instantiate(blockVarConfig.prefab, this.transform),
            actionOnGet: go => go.gameObject.SetActive(true), 
            actionOnRelease: go => go.gameObject.SetActive(false), 
            actionOnDestroy: go => Destroy(go), 
            collectionCheck: true,
            blockVarConfig.defaultCapacity, 
            blockVarConfig.maxSize
        );

        createVarPool = new ObjectPool<CreateVariableUI>(
            createFunc: () => Instantiate(createVarConfig.prefab, this.transform),
            actionOnGet: go => go.gameObject.SetActive(true), 
            actionOnRelease: go => go.gameObject.SetActive(false), 
            actionOnDestroy: go => Destroy(go), 
            collectionCheck: true,
            createVarConfig.defaultCapacity, 
            createVarConfig.maxSize
        );

        parameterPool = new ObjectPool<ReferenceHolder>(
            createFunc: () => Instantiate(parameterConfig.prefab, this.transform),
            actionOnGet: go => go.gameObject.SetActive(true), 
            actionOnRelease: go => go.gameObject.SetActive(false), 
            actionOnDestroy: go => Destroy(go), 
            collectionCheck: true,
            parameterConfig.defaultCapacity, 
            parameterConfig.maxSize
        );

        sectionPool = new ObjectPool<SectionInfo>(
            createFunc: () => Instantiate(sectionConfig.prefab, this.transform), 
            actionOnGet: go => go.gameObject.SetActive(true), 
            actionOnRelease: go => go.gameObject.SetActive(false), 
            actionOnDestroy: go => Destroy(go), 
            collectionCheck: true,
            sectionConfig.defaultCapacity, 
            sectionConfig.maxSize
        );

        valuePool = new ObjectPool<ValueInfo>(
            createFunc: () => Instantiate(valueConfig.prefab, this.transform), 
            actionOnGet: go => go.gameObject.SetActive(true), 
            actionOnRelease: go => go.gameObject.SetActive(false), 
            actionOnDestroy: go => Destroy(go), 
            collectionCheck: true,
            valueConfig.defaultCapacity, 
            valueConfig.maxSize
        );
    }

    public BlockUI GetBlockNext() => blockNextPool.Get();
    public void ReleaseBlockNext(BlockUI obj) {
        obj.transform.SetParent(this.transform, false);
        blockNextPool.Release(obj);
    }

    public BlockUI GetBlockBody() => blockBodyPool.Get();
    public void ReleaseBlockBody(BlockUI obj) {
        obj.transform.SetParent(this.transform, false);
        blockBodyPool.Release(obj);
    } 

    public BlockUI GetBlockVar() => blockVarPool.Get();
    public void ReleaseBlockVar(BlockUI obj) {
        obj.transform.SetParent(this.transform, false);
        blockVarPool.Release(obj);
    }

    public CreateVariableUI GetCreateVar() => createVarPool.Get();
    public void ReleaseCreateVar(CreateVariableUI obj) {
        obj.transform.SetParent(this.transform, false);
        createVarPool.Release(obj);
    }

    public ReferenceHolder GetParameter() => parameterPool.Get();
    public void ReleaseParameter(ReferenceHolder obj) {
        obj.transform.SetParent(this.transform, false);
        parameterPool.Release(obj);
    } 

    public SectionInfo GetSection() => sectionPool.Get();
    public void ReleaseSection(SectionInfo obj) {
        obj.transform.SetParent(this.transform, false);
        sectionPool.Release(obj);
    } 

    public ValueInfo GetValue() => valuePool.Get();
    public void ReleaseValue(ValueInfo obj) {
        obj.transform.SetParent(this.transform, false);
        valuePool.Release(obj);
    } 
}