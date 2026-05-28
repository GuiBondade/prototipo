using UnityEngine;
using UnityEngine.Pool; // Necessário para usar o sistema nativo
using System.Collections.Generic;

public class blocksFactory : MonoBehaviour
{
    // vai ser chamado pelo workspacebuild e palette item, etc
    // e vai ser o intermedio entre eles e o pool

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
            /* case PrefabsId.parameter:
                block = InitializeParameter(blockData);
                break; */ // na verdade eu vou chamar isso no proprio workspace builder, nao precisa ta aqui
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

        //block.SetParent(pools.blockNextPool); // tem que fazer no script de pools direto

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
    public GameObject InitializeParameter(Transform parent) { // passar data de parametro? pra dar setup ja (setup do parameter setup?, parameternode?)
        var parameter = pools.GetParameter();
        // addcomponent(parameterSetup)
        // .setup()
        
        parameter.transform.SetParent(parent, false);

        return parameter;
    }

    public void CleanUpParameter(GameObject parameter) {
        var paramRefs = parameter.GetComponent<ReferenceHolder>();
        // precisa mais algo?? (anular valor escolhido?)
        if (paramRefs.leftOperand != null) {
            CleanUpParameter(paramRefs.leftOperand);
        }
        if (paramRefs.rightOperand != null) {
            CleanUpParameter(paramRefs.rightOperand);
        }

        pools.ReleaseParameter(parameter);
    }
}