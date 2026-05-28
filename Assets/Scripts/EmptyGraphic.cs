using UnityEngine;
using UnityEngine.UI; // Dá acesso às ferramentas de UI do Unity

// Ao herdar de Graphic, o Unity entende que este objeto faz parte do sistema de cliques e hovers
public class EmptyGraphic : Graphic
{
    // Este método é o que o Unity usa para construir os triângulos e a malha (mesh) da imagem na tela
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // Nós limpamos a malha! Dizemos ao Unity: "Não desenhe nenhum pixel aqui".
        vh.Clear();
    }
}
