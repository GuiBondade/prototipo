using UnityEngine;
using UnityEngine.Pool; // Necessário para usar o sistema nativo
using System.Collections.Generic;

public class blocksFactory : MonoBehaviour
{
    public ProgrammingPool pools;
    public BlockDatas blockDatas;

    private void Awake() {
        if (blockDatas != null) blockDatas.Initialize();
    }

    public GameObject InitializeBlock(BlockData blockData, Transform parent) {
        /* var blockData = blockDatas.GetData(//string// blockDataId); */ // so usa aqui, sepa que nem precisa
        if (blockData == null) return null;

        GameObject block = null;

        switch (blockData.prefab) {
            case PrefabsId.next:
                block = InitializeNext();
                break;
            case PrefabsId.body:
                block = InitializeBody();
                break;
        } 

        if (block.TryGetComponent<BlockUI>(out BlockUI ui)) { //tem nem pq verificar
            ui.SetupUI(blockData);
        }

        block.transform.SetParent(parent, false);

        return block;
    }

    public void CleanUpBlock (GameObject block) {
        var ui = block.GetComponent<BlockUI>();

        if (ui.slotBody != null) CleanUpBlockBody(block); // bloco body
        else CleanUpBlockNext(block); // bloco next

        if (ui.parameterInitialList.Count > 0) { // tem parametro inicial
            foreach (var param in ui.parameterInitialList) {
                CleanUpParameter(param.gameObject);
            }
        }
    }

    // Functions Block Next
    public GameObject InitializeNext() {
        var blockNext = pools.GetBlockNext();

        return blockNext;
    }


    public void CleanUpBlockNext(GameObject block) {
        var ui = block.GetComponent<BlockUI>();
        var blockNext = ui.GetNext();
        if (blockNext != null) CleanUpBlock(blockNext.gameObject);        

        pools.ReleaseBlockNext(block);
    }

    // Functions Block Body
    public GameObject InitializeBody() {
        var blockBody = pools.GetBlockBody();
        
        return blockBody;
    }

    public void CleanUpBlockBody(GameObject block) {
        var ui = block.GetComponent<BlockUI>();
        var blockNext = ui.GetNext();
        if (blockNext != null) CleanUpBlock(blockNext.gameObject); 
        var blockBody = ui.GetBody();
        if (blockBody != null) CleanUpBlock(blockBody.gameObject); 

        pools.ReleaseBlockBody(block);
    }

    // Functions Parameter
    public ReferenceHolder InitializeParameter(string paramName, ParameterType type, Transform parent) { // passar data de parametro? pra dar setup ja (setup do parameter setup?, parameternode?)
        var parameter = pools.GetParameter();
        
        var paramRefs = parameter.GetComponent<ReferenceHolder>();

        GameObject rootParameter = null; 
        if (parent.GetComponent<ReferenceHolder>() == null) {
            paramRefs.rootParameter = parameter;
        } else {
            paramRefs.rootParameter = blockParent.GetComponent<ReferenceHolder>().rootParameter;
        }

        paramSetup = parameter.AddComponent<Type.GetType(type)>() as ParameterSetup;
        paramRefs.type = type;

        paramSetup.Initialize(paramRefs);
        paramSetup.Setup(paramName);
        paramInstance.GetComponent<AdjustWidthByText>().AdjustWidth();
        
        parameter.transform.SetParent(parent, false);

        return paramRefs;
    }

    public void CleanUpParameter(GameObject parameter) {
        var paramRefs = parameter.GetComponent<ReferenceHolder>();
        if (paramRefs.inputText != null) paramRefs.inputText.SetActive(false);
        paramRefs.parentesesInstance.SetActive(false);

        if (paramRefs.leftOperand != null) {
            CleanUpParameter(paramRefs.leftOperand);
        }
        if (paramRefs.rightOperand != null) {
            CleanUpParameter(paramRefs.rightOperand);
        }

        pools.ReleaseParameter(parameter);
    }

    // fazer initialize e cleanup pra value/section tambem (dai ja ve se une section e value) 
    // passar o sectonInfo/valueInfo como parametro pro initializeOption?

    public SectionInfo InitializeSection(ParameterSections sectionName, List<string> values/*nao sei se é lista de string mesmo*/ , Transform parent) {
        var section = pools.GetSection();

        var sectionOptions = section.GetComponent<SectionInfo>();
        sectionOptions.label.text = sectionName.ToString();
        sectionOptions.label.ForceMeshUpdate();
        sectionOptions.sectionCurrent = sectionName;

        section.transform.SetParent(parent, false);

        return sectionOptions;
    }

    public void CleanUpSection(GameObject section) {
        // pegar os values
        // dar release nos values
        pools.ReleaseSection(section);
    }

    public void InitializeValue(string valor, Transform parent) {
        var value = pools.GetValue();

        var valueInfo = value.GetComponent<ValueInfo>(); 
        valueInfo.label.text = valor;
        valueInfo.label.ForceMeshUpdate();
        valueInfo.sectionCurrent = sectionInfo.sectionCurrent;
        valueInfo.value = valor;

        value.transform.SetParent(parent, false);

        return value;
    }

    public void CleanUpValue(GameObject value) {

        pools.ReleaseValue(value);
    }
}