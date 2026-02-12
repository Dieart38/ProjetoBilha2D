using UnityEngine;
using System.Collections;

public class OponenteIA : MonoBehaviour
{
    public Rigidbody2D rbBolaBranca;
    [Header("Configurações")]
    [Range(0, 100)] public float dificuldade = 10;

    // 1. Adicione a referência para o script que criamos antes
    public TacoIAVisual scriptTaco;

    private bool minhaVez = false;

    // alteração 
    private GameObject[] cacapas;


    void Start()
    {
        // alteração: encontramos as cacapas no início para usar depois
        cacapas = GameObject.FindGameObjectsWithTag("Buraco");
    }
    public void IniciarTurnoIA()
    {
        if (!minhaVez)
        {
            minhaVez = true;
            StartCoroutine(PensarTacada());
        }
    }

    IEnumerator PensarTacada()
    {
        Debug.Log("IA observando a mesa...");
        yield return new WaitForSeconds(1.0f);

         GameObject[] bolas = GameObject.FindGameObjectsWithTag("Bola");
        // if (bolas.Length == 0) yield break;
       
        // GameObject alvo = bolas[Random.Range(0, bolas.Length)];
        // Vector2 direcaoParaAlvo = (alvo.transform.position - rbBolaBranca.transform.position).normalized;

        // modificação
        if (bolas.Length == 0) yield break;

        GameObject melhorBola = null;
        GameObject melhorCacapa = null;
        float menorDistancia = float.MaxValue;

        foreach (GameObject bola in bolas)
        {
            foreach (GameObject cacapa in cacapas)
            {
                float dist = Vector2.Distance(bola.transform.position, cacapa.transform.position);
                if (dist < menorDistancia)
                {
                    menorDistancia = dist;
                    melhorBola = bola;
                    melhorCacapa = cacapa;
                }
            }
        }

        // float anguloBase = Mathf.Atan2(direcaoParaAlvo.y, direcaoParaAlvo.x) * Mathf.Rad2Deg;
        // float margemErro = (100f - dificuldade) * 0.15f;
        // float desvioAleatorio = Random.Range(-margemErro, margemErro);
        // Quaternion rotacaoFinal = Quaternion.Euler(0, 0, anguloBase + desvioAleatorio);

        // --- NOVO: LÓGICA DO TACO ---

        // 2. Faz o taco aparecer e rotaciona ele para a direção do alvo
        // if (scriptTaco != null)
        // {
        //     // Passamos a rotacaoFinal que você já calculou para o método
        //     scriptTaco.AparecerTaco(rotacaoFinal);
        // }
         if (melhorBola == null || melhorCacapa == null) yield break;

        // --- AJUSTE DE PRECISÃO ---
        Vector2 posCacapa = melhorCacapa.transform.position;
        Vector2 posBolaAlvo = melhorBola.transform.position;
        
        Vector2 direcaoCacapaParaBola = (posBolaAlvo - posCacapa).normalized;
        
        // Alterado de 0.5f para 0.38f (distância ideal para o contato de duas esferas padrão)
        Vector2 pontoImpacto = posBolaAlvo + (direcaoCacapaParaBola * 0.38f);

        Vector2 direcaoFinal = (pontoImpacto - (Vector2)rbBolaBranca.transform.position).normalized;
        float anguloBase = Mathf.Atan2(direcaoFinal.y, direcaoFinal.x) * Mathf.Rad2Deg;
        
        // Se dificuldade for 100, o erro será zero absoluto
        float margemErro = (100f - dificuldade) * 0.1f;
        float desvioAleatorio = (dificuldade >= 100) ? 0 : Random.Range(-margemErro, margemErro);
        
        Quaternion rotacaoFinal = Quaternion.Euler(0, 0, anguloBase + desvioAleatorio);

        if (scriptTaco != null)
        {
            scriptTaco.AparecerTaco( rotacaoFinal);
            yield return new WaitForSeconds(1.5f);
        }

        Vector2 forcaDirecao = (Vector2)(rotacaoFinal * Vector3.right);
        float distTotal = Vector2.Distance(rbBolaBranca.transform.position, pontoImpacto) + menorDistancia;
        
        // Aumentei levemente a força mínima para garantir que a bola chegue ao buraco
        float forcaFinal = Mathf.Clamp(distTotal * 4.5f, 8f, 25f);

        rbBolaBranca.AddForce(forcaDirecao * forcaFinal, ForceMode2D.Impulse);

        if (scriptTaco != null) scriptTaco.EsconderTaco();

        GameManager.instancia.TacadaRealizada();
        minhaVez = false;
    }
        // 3. Espera um pouco com o taco visível (fazendo o movimento de vai e vem) 
        // para parecer que a IA está mirando/carregando
    //     yield return new WaitForSeconds(1.5f);

    //     // 4. Executa a tacada física
    //     Vector2 direcaoComErro = (Vector2)(rotacaoFinal * Vector3.right);
    //     float distancia = Vector2.Distance(rbBolaBranca.transform.position, alvo.transform.position);
    //     float forcaFinal = Mathf.Clamp(distancia * 5f, 5f, 20f);

    //     rbBolaBranca.AddForce(direcaoComErro * forcaFinal, ForceMode2D.Impulse);

    //     // 5. Esconde o taco imediatamente após a batida
    //     if (scriptTaco != null)
    //     {
    //         scriptTaco.EsconderTaco();
    //     }

    //     // --- FIM DA LÓGICA DO TACO ---

    //     GameManager.instancia.TacadaRealizada();
    //     minhaVez = false;
    // }
}