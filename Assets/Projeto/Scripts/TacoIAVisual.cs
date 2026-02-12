using UnityEngine;

public class TacoIAVisual : MonoBehaviour
{[Header("Configurações de Movimento")]
   public float velocidade = 5f;
    public float distancia = 1.5f;
    
    private Vector3 posicaoLocalInicial;
    private bool estaAtivo = false;

    void Start()
    {
        // Salva a posição relativa ao PIVÔ
        posicaoLocalInicial = transform.localPosition;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (estaAtivo)
        {
            // O movimento PingPong agora afeta o X local (frente e trás do taco)
            float deslocamento = Mathf.PingPong(Time.time * velocidade, distancia);
            
            // Subtraímos o deslocamento para ele ir para TRÁS antes de bater
            transform.localPosition = new Vector3(posicaoLocalInicial.x - deslocamento, 0, 0);
        }
    }

    public void AparecerTaco(Quaternion rotacaoDesejada)
    {
        // O PAI do taco (o Pivô) recebe a rotação da mira
        if (transform.parent != null)
        {
            transform.parent.rotation = rotacaoDesejada;
        }

        estaAtivo = true;
        gameObject.SetActive(true);
    }

    public void EsconderTaco()
    {
        estaAtivo = false;
        gameObject.SetActive(false);
        transform.localPosition = posicaoLocalInicial;
    }
}