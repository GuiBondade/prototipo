using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ParameterSlot : MonoBehaviour//, IDropHandler
{
    // workspace
    // 

    // criação de bloco: definicao de tipo por inspector (dicionario onde pode ser adicionado parametros e cada parametro tem uma variavel tipo a ser atribuida)
        // alterar imagem do slot, que nem no scratch (int é quadrado, boolean é losango, etc) [cor/contorno também?]
    
    // drag funciona através de draggableBlock script component

    void OnDrop() //(PointerEventData eventData)
    {
        return;
        // pegar parametro sendo drag 
        
        // pegar onde foi dropado (provavel que ele pegue o raycast, entao pega o pai (slot))
        // a partir do tipo declarado vai ser feito comparação entre o bloco/parametro dropado e o tipo definido, se nao bater parenta no workspace
        // verificar se onde foi dropado tem script parameterSlot, se sim, verificar se tem tipo igual do dropado 
        // verificar se tem parametro ja nesse slot
            // se tiver, da drop nele(parametro é bem facil de configurar) ou parenta na workspace
        // parenta no slot (provavel que ele pegue o raycast, entao confirma se ta sendo parentado no slot) o parametro em drag
        // id pra associar parametro ao pai na leitura (id normal do pai?)
        // ajustar largura slot (precisa?)
        // como leria o valor do parametro? setta a variavel usada na ação do bloco como o valor dele?(parametros sao lidos primeiros?)

    }

     
    //void onClick() // so funciona pra botao, entao nem faz sentido atualmente (o drag vai vai dar, se tiver o component draggableBlock)
    
        // (ideia antiga) tipo do parametro nao definido pelo bloco (alteravel na workspace) (ideia antiga)
            // cada parametro vai ter raycast target pra type (esquerda) e raycast target pra parametro (direita e enlarga parameterslot)
            // pegar onde foi clicado (qual dos raycast)
            // if (clicou no type):
                // aparece interface de seleção de tipo overlay na posicao do mouse
                    // lista de botoes que settam 
            // elif (clicou no parametro):
                // draggable block.ondrag() no parametro
                // retirar valores do dicionario dos parametros (ou faz loucura pra tirar no drop? pra fazer undo (acho nada a ver))

        //
        // tipos do parametro constantes por bloco (exceto funcoes como planejado atualmente)
            // draggable block.ondrag() no parametro
            // retirar valores do dicionario dos parametros (ou faz loucura pra tirar no drop? pra fazer undo (acho nada a ver))
            // ajustar largura do slot (precisa?)

            // input jogador???????????
                // adicionar object botao como filho
                // quando clicar
                    // aparece interface, de seleção, overlay na posicao do mouse
                    // lista de botoes que settam o valor do parametro, dependendo do tipo, e dai fecha a interface
                    // 

    

    
}
