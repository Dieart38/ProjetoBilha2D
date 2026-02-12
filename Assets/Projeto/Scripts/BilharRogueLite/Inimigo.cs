using UnityEngine;
using System.Collections; // Necessário para o efeito de piscar

public class Inimigo : MonoBehaviour
{
    [Header("Status")]
    public int vidaMax = 3; 
    private float vidaAtual;
    
    [Header("Configurações de Dano")]
    public float forcaNecessaria = 5f; // Velocidade mínima para registrar dano
    public float forcaParaDanoDuplo = 15f; // Se bater muito forte, tira 2 de vida

    private SpriteRenderer sprite;
    private Color corOriginal;

    void Start()
    {
        vidaAtual = vidaMax;
        sprite = GetComponent<SpriteRenderer>();
        corOriginal = sprite.color;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            float impacto = col.relativeVelocity.magnitude;

            // Se o impacto for maior que o mínimo necessário
            if (impacto > forcaNecessaria)
            {
                int danoParaAplicar = 1;

                // Se o impacto for uma "paulada", tira mais vida
                if (impacto > forcaParaDanoDuplo) {
                    danoParaAplicar = 2;
                }

                ReceberDano(danoParaAplicar);
            }
        }
    }

    public void ReceberDano(int dano)
    {
        vidaAtual -= dano;
        
        // Feedback visual: Pisca em branco
        StartCoroutine(EfeitoPiscar());

        Debug.Log($"Inimigo atingido! Dano: {dano} | Vida: {vidaAtual}/{vidaMax}");

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    IEnumerator EfeitoPiscar()
    {
        sprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sprite.color = corOriginal;
    }

    void Morrer()
    {
        // Aqui você pode instanciar partículas de explosão depois
        Destroy(gameObject);
    }
}