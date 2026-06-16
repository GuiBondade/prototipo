using UnityEngine;
using UnityEngine.Pool;
using System;
using System.Collections.Generic;

public class BlocksFactory : MonoBehaviour
{
    public static BlocksFactory instance {get; private set;}

    public ProgrammingPool pools;
    public BlockRegistry blockRegistry;
    public VariableManager variableManager;

    private void Awake()
    {
        // Padrão de segurança para Singletons em Unity
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public BlockUI InitializeBlock(BlockData blockData, bool isRoot, Transform parent) {
        if (blockData == null) return null;

        BlockUI blockUI = null;

        switch (blockData.prefab) {
            case PrefabsId.next:
                blockUI = pools.GetBlockNext();
                break;
            case PrefabsId.body:
                blockUI = pools.GetBlockBody();
                blockUI.ResetSpacer();
                break;
            case PrefabsId.variavel:
                blockUI = pools.GetBlockVar();
                break;
        } 

        if (blockUI == null) return null;

        blockUI.SetupUI(blockData);
        ConnectToSlot(blockUI, isRoot, parent);
        

        return blockUI;
    }

    public void CleanUpBlock(BlockUI blockUI) {
        if (blockUI == null) return;

        if (blockUI.parameterInitialList.Count > 0) { // tem parametro inicial
            foreach (var param in blockUI.parameterInitialList) {
                CleanUpParameter(param);
            }
        }

        CleanUpBlock(blockUI.GetNext());
        blockUI.slotNext.currentBlock = null;  

        blockUI.bodyAncestors.Clear();
        blockUI.parameterInitialList.Clear();
        blockUI.parentBodyOwner = null;
        
        if (blockUI.slotBody != null) {
            CleanUpBlock(blockUI.GetBody());
            blockUI.slotBody.currentBlock = null;

            pools.ReleaseBlockBody(blockUI);
        } else {
            if (blockUI is VariableBlockUI varBlockUI) {
                Debug.Log("deletar variavel: " + varBlockUI.variableName);
                var variableName = varBlockUI.variableName;
                // precisa? VVVV
                variableManager.UpdateVariableData(variableName, variableManager.GetVarTypeByName(variableName), variableManager.GetVarSectionByName(variableName), null);
                //variableManager.UpdateVariableName(variableName, defaultParamName); //acho que nada a ver
                variableManager.DeleteVariable(variableName);
                pools.ReleaseBlockVar(blockUI);
            } else {
                pools.ReleaseBlockNext(blockUI);
            }
        } 
    }

    public void ConnectToSlot(BlockUI blockUI, bool preserveWorldSpace, Transform slot) { //slot vai ser pego na hora de chamar o initializeblock, por isso dessa forma também aceita workspace
        var rect = blockUI.GetComponent<RectTransform>();
        var blockTransform = blockUI.transform;
        BlockUI tail = blockUI.GetTail();
        BlockSlot blockSlot = null;
        SlotType slotType = SlotType.Null; // so seta Null por script, nunca vai ter slot de verdade com type Null, (aqui é so se for fora de bloco)
        if (slot.TryGetComponent<BlockSlot>(out blockSlot)) { // por de parametro extra BlockSlot, que fica null quando da na workspace
            slotType = blockSlot.slotType;
            blockSlot.currentBlock = blockUI;
        }
        blockTransform.SetParent(slot, preserveWorldSpace);

        rect.localScale = blockTransform.localScale; //PRECISA??
        Vector2 anchors;
        if (preserveWorldSpace) {
            rect.pivot = new Vector2(0.5f, 0.5f);
            anchors = new Vector2(0.5f, 0.5f);
        } else {
            rect.pivot = new Vector2(0f, 1f);
            anchors = new Vector2(0f, 0f);
            rect.anchoredPosition = Vector2.zero;
        }
        
        blockTransform.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y, 0f); // força z = 0

        rect.anchorMin = anchors;
        rect.anchorMax = anchors;
        rect.localScale = new Vector2(1f, 1f);

        float dragHeight = blockUI.GetTailHeight();

        BlockUI parentBodyOwner = null;
        BlockUI existingBlock = null;
        if (blockSlot != null) {
            var parentUI = blockSlot.parentBlock;
            List<BlockUI> parentAncestorList = new List<BlockUI>(parentUI.bodyAncestors);
            if (slotType == SlotType.Body) {
                parentBodyOwner = parentUI; 
                rect.anchoredPosition = new Vector2(blockUI.defaultSpacerWidth, 0); //pq usa a altura padrão como largura padrão (tem como não ser quadrado?)
            }
            blockUI.AssignBodyAncestorsRecursive(parentAncestorList, parentBodyOwner);
        
            if (slot.childCount > 1 && tail != null) // se tem + de 1 filho (se tinha um bloco antes do que acabou de setar como filho)
            {
                existingBlock = slot.GetChild(0).GetComponent<BlockUI>();
                RectTransform nextSlotTransform = tail.slotNext.GetComponent<RectTransform>();
                existingBlock.transform.SetParent(nextSlotTransform, false);
                existingBlock.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                tail.slotNext.currentBlock = existingBlock;
                existingBlock.AssignBodyAncestorsRecursive(blockUI.bodyAncestors, null);
            }
        }/*  else {
            blockUI.AssignBodyAncestorsRecursive(null, null);
        } */
        
        if (blockUI.bodyAncestors != null && blockUI.bodyAncestors.Count > 0)
        {
            Debug.Log("dragHeight: " + dragHeight);
            blockUI.AdjustBodySpacers(dragHeight);
            if (blockSlot != null && slotType == SlotType.Body && existingBlock == null) {
                //dropou direto no body sendo o 1 primeiro e unico filho
                Debug.Log(-(blockUI.defaultSpacerHeight));
                blockUI.AdjustBodySpacers(-(blockUI.defaultSpacerHeight));
            }
        }
    }

    public CreateVariableUI InitializeCreateVariable(Transform parent) {
        var createVarUI = pools.GetCreateVar();
        var rect = createVarUI.GetComponent<RectTransform>();
        var createVarTransform = createVarUI.transform; 
        
        createVarUI.Setup();
        createVarTransform.SetParent(parent, true);

        //rect.localScale = createVarTransform.localScale; // PRECISA??
        createVarTransform.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y, 0f); // força z = 0

        rect.pivot = new Vector2(0.5f, 0.5f);
        Vector2 anchors = new Vector2(0.5f, 0.5f);
        rect.anchorMin = anchors;
        rect.anchorMax = anchors;
        rect.localScale = new Vector2(1f, 1f);

        return createVarUI;
    }

    public void CleanUpCreateVariable(CreateVariableUI createVarUI) {
        Debug.Log(createVarUI.variableName);
        variableManager.DeleteVariable(createVarUI.variableName);

        pools.ReleaseCreateVar(createVarUI);
    }

    // Functions Parameter
    public ReferenceHolder InitializeParameter(string paramName, ParameterType type, Transform parent) { // passar data de parametro? pra dar setup ja (setup do parameter setup?, parameternode?)
        var paramRefs = pools.GetParameter();
        var parameter = paramRefs.gameObject;
                    // TANTO GET COMPONENT, DAVA PRA REFERENCIAR NO REFERENCE HOLDER NÃO? !!!!!!!!!!!!!
        var parentRefs = parent.GetComponent<ReferenceHolder>();
        if (parentRefs == null) {
            paramRefs.rootParameter = parameter;
        } else {
            paramRefs.rootParameter = parentRefs.rootParameter;
            parentRefs.parentesesInstance.SetActive(true);
        }
        
        var paramSetup = parameter.GetComponent<ParameterSetup>();
        
        paramRefs.type = type;
        paramSetup.paramData = blockRegistry.GetParamData(type);
        paramSetup.Setup(paramName);
        parameter.GetComponent<AdjustWidthByText>().AdjustWidth();
        
        parameter.transform.SetParent(parent, false);

        return paramRefs;
    }

    public void CleanUpParameter(ReferenceHolder paramRefs) {
        if (paramRefs == null) return;

        if (paramRefs.inputTextInstance != null) Destroy(paramRefs.inputTextInstance);
        paramRefs.parentesesInstance.SetActive(false);

        // reiterar os values e sections pra dar cleanup
        // ACHAR JEITO MELHOR DO QUE FICAR DANDO GET COMPONENT
        // poe referencia pra lista de optioninfo no refs sei la, descobre um jeito melhor
        for (var i = paramRefs.sectionContent.transform.childCount - 1; i >= 0; i--) {
            var section = paramRefs.sectionContent.transform.GetChild(i);
            CleanUpSection(section.GetComponent<SectionInfo>());
        }
        for (var j = paramRefs.valueContent.transform.childCount - 1; j >= 0; j--) {
            var value = paramRefs.valueContent.transform.GetChild(j);
            CleanUpValue(value.GetComponent<ValueInfo>());
        }

        paramRefs.selectedSection = null;
        paramRefs.selectedValueVisual = null;
        paramRefs.selectedValueKey = null;
        paramRefs.selectedVariableName = null;

        if (paramRefs.leftOperand is ReferenceHolder leftOperand) {
            CleanUpParameter(leftOperand);
            paramRefs.leftOperand = null;
        }
        if (paramRefs.rightOperand is ReferenceHolder rightOperand) {
            CleanUpParameter(rightOperand);
            paramRefs.rightOperand = null;
        }

        pools.ReleaseParameter(paramRefs);
    }

    // fazer initialize e cleanup pra value/section tambem (dai ja ve se une section e value) 
    // passar o sectonInfo/valueInfo como parametro pro initializeOption?

    public SectionInfo InitializeSection(ParameterSections sectionName, Transform parent) { // pode ser private se quiser eu acho
        var sectionOptions = pools.GetSection();
        var section = sectionOptions.gameObject;
        sectionOptions.label.text = sectionName.ToString();
        sectionOptions.label.ForceMeshUpdate();
        sectionOptions.sectionCurrent = sectionName;

        section.transform.SetParent(parent, false);
        sectionOptions.Setup();

        return sectionOptions;
    }

    public void CleanUpSection(SectionInfo sectionInfo) {
        sectionInfo.btn.onClick.RemoveAllListeners();
        sectionInfo.valueList.Clear();
        pools.ReleaseSection(sectionInfo);
    }

    public ValueInfo InitializeValue(string valor, ParameterSections sectionCurrent, Transform parent, bool isVar = false) {
        var valueInfo = pools.GetValue();
        var value = valueInfo.gameObject;
        valueInfo.label.text = valor;
        valueInfo.label.ForceMeshUpdate();
        valueInfo.sectionCurrent = sectionCurrent;
        valueInfo.value = valor;
        if (isVar) valueInfo.value = valor;

        value.transform.SetParent(parent, false);
        valueInfo.Setup();

        return valueInfo;
    }

    public void CleanUpValue(ValueInfo valueInfo) {
        valueInfo.checkmark.enabled = false;
        valueInfo.btn.onClick.RemoveAllListeners();
        pools.ReleaseValue(valueInfo);
    }
}