using System.Collections.Generic;
using UnityEngine;
using TMPro;

// chama o WorkspaceJSON (LOAD)
public class WorkspaceBuilder : MonoBehaviour
{
    public RectTransform workspace;

    private Dictionary<BlockUI, int> idMap = new();
    private Dictionary<ReferenceHolder, int> paramIdMap = new();
    private List<BlockNode> blocos = new();
    private List<ParameterNode> parametros = new();
    private SaveNodesData nodesData = new();
    private int idCounter = 0;

    public SaveNodesData WorkspaceBuildGraph() {
        paramIdMap.Clear();
        idMap.Clear();
        parametros.Clear();
        blocos.Clear();
        nodesData.Clear();
        idCounter = 0;

        // pega todos blocos no workspace (nível raiz)
        foreach (Transform child in workspace) {
            var block = child.GetComponent<BlockUI>();
            if (block != null) {
                Traverse(block);
            }
        }
        nodesData = new SaveNodesData {
            blocosData = blocos,
            parametrosData = parametros
        };
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
                idMap.Clear();
                parametros.Clear();
                blocos.Clear();
                nodesData.Clear();
                idCounter = 0; */
            id = -1;
            return false; // reset geral a ser feito (so dar log de erro, e deixar inexecutavel, dai na hora de salvar de novo ele pula as que ja tiver salvo exceto pelas marcadas como erro)
        }
        int myId = idCounter++;
        paramIdMap[parameter] = myId;

        string valor = selectedValue.GetComponent<ValueInfo>().value;
        if (valor == "Digitar Valor") valor = parameter.GetComponent<ParameterConfig>().inputText.GetComponent<TMP_InputField>().text;
        // if valor == *variavel*, como faz????
        ParameterType typeParameter = ParameterType.AllTypeParameter; // default
        ParameterSetup parameterSetup;
        if (parameter.TryGetComponent<ParameterSetup>(out parameterSetup)) {
            typeParameter = parameterSetup.type;
        }

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

    private int Traverse(BlockUI block) {
        if (idMap.ContainsKey(block)) return idMap[block];

        int myId = idCounter++;
        idMap[block] = myId;

        var bloco = new BlockNode {
            id = myId,
            blockName = block.data.blockName,
            function = block.data.function, // referencia pra switch? de todas as possiveis funções de ação (chama direto)
            type = block.data.type, // referencia pra switch pro prefab a ser instanciado (next ou body, ou outro sei la)
            paramIds = new List<int> (),
            next = -1,
            body = -1
        };

        foreach (var param in block.parameterInitialList) {
            if (TryGetParameterNode(param, out int id)) {
                bloco.paramIds.Add(id);
            }
        }

        // Seguir conexões
        var next = block.GetNext();
        if (next != null) bloco.next = Traverse(next);

        var body = block.GetBody();
        if (body != null) bloco.body = Traverse(body);

        blocos.Add(bloco);
        return myId;
    }

    public void WorkspaceBuildUI(SaveNodesData data) 
    {
        return;
        // oq precisa pra recosntruir a partir do script salvo
        /*
        instanciar prefab certo, do block data, na workspace, ou no slot do bloco pai referenciado por id no script,
            ja ta feito as referencias pros blocos seguintes e tals
            e dai segue as funções do blockUI ou outro/novo script, que settam certinho o funcionamento e visualização dos blocos instanciados
        instanciar os parametros certos, do block data, settando o valor referenciado em algum lugar, 
            (talvez adicionar um Dictionary (classe sepa) no block ui, onde é adicionado chave nova pra cada parametro instanciado
            inicialmente, e dai vai recursivamente olhando os value 1 e 2 e as operações, que são os param com value settado,
            e dai tem que ter tambem uma relação dizendo o id do value relativo ao parametro que o referencia, pra poder instanciar
            recursivamente depois (pra ir atras de outra ideia, sepa que IA ajuda))
        
        */

        // na recriação basicamente vai: (iniciando a partir do nodes[-1])
        // pegar block data
        // instanciar 
        // chamar setup de parametros
            // esse é o foda, vai instanciar a partir do blockdata e dai ir recursivo pros values 1 e 2
            // sepa que chama o parameter config no fim da função pra dar Instantiate parameter while ele tiver value 1 ou value 2 sei la
            // da pra so criar uma função especifica pra isso dentro do rebuild e fodase tambem
            //DECIDIDO: a parte da recursão sera feita fora de scripts proprios dos parametros, o setup vai ser o mesmo dos parametros, pra
            // padronizar, e a instanciação de filhos, caso fique igual e sempre va ficar tanto na escolha de operação
            // quanto na reconstrução dos parametros, entao so chama a função, se for trocar algo ou fazer algo diferente, cria uma nova, ou
            // se tiver algo igual nas duas separa a origem em duas, uma reutilizavel nos dois casos, e a outra chama essa e faz o especifico desse caso
        // chamar setup de UI
        // pegar proximo bloco e bloco body (do node)
        // ...repetir passos pro proximo bloco e body caso tiver
    }
}
