using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

// chama o WorkspaceJSON (LOAD)
public class WorkspaceBuilder : MonoBehaviour
{
    private Dictionary<BlockUI, int> blockIdMap = new();
    private Dictionary<ReferenceHolder, int> paramIdMap = new();
    private List<BlockNode> blocos = new(); // nao da pra trocar isso por nodesData.blocosData??
    private List<ParameterNode> parametros = new(); // nao da pra trocar isso por nodesData.parametrosData??
    private SaveNodesData nodesData = new();
    private BlockNode[] blocoDadosArray;
    private ParameterNode[] paramDadosArray;
    private int blockIdCounter = 0;
    private int paramIdCounter = 0;

    private int blockRootId;

    public BlockDatas blockDatas;
    public blocksFactory blocksFactory;

    public RectTransform workspace;

    void Start() {
        workspace = CanvasAreasManager.instancia.contentWorkspace;
    }

    public SaveNodesData WorkspaceBuildGraph() {
        paramIdMap.Clear();
        blockIdMap.Clear();
        parametros.Clear();
        blocos.Clear();
        nodesData.Clear();
        paramIdCounter = 0;
        blockIdCounter = 0;

        // pega todos blocos no workspace (nível raiz)
        foreach (Transform child in workspace) {
            var block = child.GetComponent<BlockUI>();
            if (block != null) {
                Traverse(block, true);
            }
        }
        nodesData = new SaveNodesData {
            blocosData = blocos,
            parametrosData = parametros
        };
        foreach (var parametro in nodesData.parametrosData) {
            Debug.Log(parametro.id);
        }
        
        return nodesData;
    }

    private bool TryGetParameterNode(ReferenceHolder parameter, out int id) {
        if (paramIdMap.ContainsKey(parameter)) {
            id = paramIdMap[parameter];
            return true;
        }

        GameObject selectedValue = parameter.selectedValue;
        if (selectedValue == null) {
            Debug.Log("Erro no Salvamento - Parametro não settado no parametro: " + parameter);
            //resetgeral? (so dar log de erro, e deixar inexecutavel, dai na hora de salvar de novo ele pula as que ja tiver salvo exceto pelas marcadas como erro)
                /* paramIdMap.Clear();
                blockIdMap.Clear();
                parametros.Clear();
                blocos.Clear();
                nodesData.Clear();
                paramIdCounter = 0; 
                blockIdCounter = 0; */
            id = -1;
            return false; // reset geral a ser feito (so dar log de erro, e deixar inexecutavel, dai na hora de salvar de novo ele pula as que ja tiver salvo exceto pelas marcadas como erro)
        }
        int myId = paramIdCounter++;
        paramIdMap[parameter] = myId;

        string valor = selectedValue.GetComponent<ValueInfo>().value;
        if (valor == "Digitar Valor") valor = parameter.GetComponent<ParameterConfig>().inputText.GetComponent<TMP_InputField>().text;
        // if valor == *variavel*, como faz????
        ParameterType typeParameter = parameter.type;
        /*
            string typeParameter = "";
            ParameterSetup parameterSetup; // pelo jeito precisa
            if (parameter.TryGetComponent<ParameterSetup>(out parameterSetup)) { // parameter é refHolder, da certo?
                typeParameter = parameterSetup.GetType().AssemblyQualifiedName; //antes tava: parameterSetup.type; 
            }
        */

        var paramNode = new ParameterNode {
            id = myId,
            type = typeParameter,
            value = valor,
            leftOperandId = -1,
            rightOperandId = -1
        };

        if (parameter.leftOperand != null && TryGetParameterNode(parameter.leftOperand.GetComponent<ReferenceHolder>(), out int leftId)) {
            paramNode.leftOperandId = leftId;
        }
        if (parameter.rightOperand != null && TryGetParameterNode(parameter.rightOperand.GetComponent<ReferenceHolder>(), out int rightId)) {
            paramNode.rightOperandId = rightId;
        }

        parametros.Add(paramNode);
        id = myId;
        return true;
    }

    private int Traverse(BlockUI block, bool isRoot) {
        if (blockIdMap.ContainsKey(block)) return blockIdMap[block];

        int myId = blockIdCounter++;
        blockIdMap[block] = myId;

        if (isRoot) blockRootId = myId;

        var bloco = new BlockNode {
            id = myId,
            blockDataId = block.data.id,
            /* blockName = block.data.blockName,
            function = block.data.function, // referencia pra switch? de todas as possiveis funções de ação (chama direto)
            prefab = block.data.prefab,  */
            paramIds = new List<int> (),
            rootId = blockRootId,
            next = -1,
            body = -1
        };

        // adiciona so os param inicial no paramIds
        foreach (var param in block.parameterInitialList) { // reference holders dos blockUIs???
            if (TryGetParameterNode(param, out int id)) {
                bloco.paramIds.Add(id);
            }
        }

        // Seguir conexões
        var next = block.GetNext();
        if (next != null) bloco.next = Traverse(next, false);

        var body = block.GetBody();
        if (body != null) bloco.body = Traverse(body, false);

        blocos.Add(bloco);
        return myId;
    }

    public void BuildFromJson(SaveNodesData data) {
        /* if (string.IsNullOrEmpty(jsonText)) return;

        // Transforma o texto JSON em dados brutos na memória RAM
        SaveNodesData data = JsonUtility.FromJson<SaveNodesData>(jsonText); */
        
        if (data == null || data.blocosData == null) return;

        // Descobre o teto máximo de IDs de forma independente
        int maxBlockId = data.blocosData.Count > 0 ? data.blocosData.Max(b => b.id) : -1;
        int maxParamId = data.parametrosData.Count > 0 ? data.parametrosData.Max(p => p.id) : -1;

        // Cria os arrays com os tamanhos exatos necessários (Sem slots nulos extras!)
        blocoDadosArray = new BlockNode[maxBlockId + 1];
        paramDadosArray = new ParameterNode[maxParamId + 1];

        // Distribui cada dado em seu respectivo array indexado
        foreach (var b in data.blocosData) blocoDadosArray[b.id] = b;
        foreach (var p in data.parametrosData) paramDadosArray[p.id] = p;
        
        BuildRoot(data.blocosData);
    }

    // vai ser algo geral a instanciação de blocos e seus parametros, ate porque vai ser feito com pool
    public void BuildRoot(List<BlockNode> blocosData) { // DA PRA FAZER SER ACESSIVEL PELO PALLETE ITEM E DAI LA SER SO CHAMADA DE FUNÇÃO, AQUI TAMBÉM SERIA
        List<int> rootInstancedList = new List<int> ();
        
        workspace.gameObject.SetActive(false);
        foreach (var bloco in blocosData) {
            int rootBlockId = bloco.rootId; // pegar id bloco instanciado na workspace
            if (rootBlockId >= 0) {
                if (!rootInstancedList.Contains(rootBlockId)) {// ver se esse id é de bloco ja instanciado
                    rootInstancedList.Add(rootBlockId); // se nao, instancia e adiciona na lista
                    InstantiateBlock(rootBlockId, workspace); // workspace é ref
                }
            } else {
                Debug.Log("rootId não settado");
            }
        }
        workspace.gameObject.SetActive(true);
        StartCoroutine(RebuildDepois());
    }

    private void InstantiateBlock(int blockId, RectTransform parent) {
        Debug.Log(blockId);
        Debug.Log(blocoDadosArray);
        Debug.Log(blocoDadosArray[blockId]);
        
        var blockNode = blocoDadosArray[blockId];
        var blockData = blockDatas.GetData(blockNode.blockDataId);

        var blockInstance = blocksFactory.InitializeBlock(blockData, parent);

        // olha oq faz no paletteItem e poe aqui sepa

        foreach (var rootParamId in blockNode.paramIds) { // todos os parametros primordiais
            var paramReference = InstantiateParameter(rootParamId, ui.TopSlot).GetComponent<ReferenceHolder>(); // tem que dar .transform no topslot?   
            ui.parameterInitialList.Add(paramReference);
            //layout rebuilder no paramroot?
        }

        if (blockNode.next >= 0) { // faz sentido usar essa operação?
            var slotNextRect = ui.slotNext.GetComponent<RectTransform>();
            InstantiateBlock(blockNode.next, slotNextRect);
        }
        if (blockNode.body >= 0) {
            var slotBodyRect = ui.slotBody.GetComponent<RectTransform>();
            InstantiateBlock(blockNode.body, slotBodyRect);
        }
    }

    private GameObject InstantiateParameter(int paramId, RectTransform blockParent) { // DA PRA FAZER SER ACESSIVEL PELO PALLETE ITEM E DAI LA SER SO CHAMADA DE FUNÇÃO
        var paramNode = paramDadosArray[paramId]; 

        var paramReference = blocksFactory.InitializeParameter(paramNode.value, paramNode.type, blockParent);

        var paramRect = paramReference.GetComponent<RectTransform>();
        
        //tem que settar data do param aqui? (dava pra ter tipo um blockData, pra cada, sei la)
        // tirar os operand do refholder, e criar um ParamData meio que pra isso (nao sei se precisa mesmo, ta de boa por enquanto)
        
        if (paramNode.leftOperandId >= 0) paramReference.leftOperand = InstantiateParameter(paramNode.leftOperandId, paramRect);//arrumar algo?
        if (paramNode.rightOperandId >= 0) paramReference.rightOperand = InstantiateParameter(paramNode.rightOperandId, paramRect);//arrumar algo?

        return paramInstance;
    }

    IEnumerator RebuildDepois()
    {
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(workspace);
    }
}
