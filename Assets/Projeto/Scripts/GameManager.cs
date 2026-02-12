using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;
    public Text textoPontos;
    public AudioSource somCacapa;
    public OponenteIA oponenteScript;
    private Rigidbody2D[] todasAsBolas;
    private int pontosPlayer = 0;
    private int pontosIA = 0;
    public bool vezDoPlayer = true;

    private bool esperandoBolasPararem = false;
    private bool marcouPontoNestaRodada = false;

    void Awake() => instancia = this;
    void Start()
    {
        // Busca todas as bolas uma única vez ao iniciar
        todasAsBolas = Object.FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None);
    }
    public void AdicionarPontuacao()
    {
        if (vezDoPlayer) pontosPlayer++;
        else pontosIA++;

        marcouPontoNestaRodada = true; // Jogador ganha outra chance se encaçapar
        AtualizarUI();
        if (somCacapa != null) somCacapa.Play();
    }

    void AtualizarUI() => textoPontos.text = $"Player: {pontosPlayer} | IA: {pontosIA}";

    // Chamado pelo ControleTaco assim que o player atira
    public void TacadaRealizada()
    {
        marcouPontoNestaRodada = false;
        StartCoroutine(AguardarParadaDasBolas());
    }

    IEnumerator AguardarParadaDasBolas()
    {
        // Espera um tempo mínimo para as bolas ganharem velocidade
        yield return new WaitForSeconds(0.7f);
        esperandoBolasPararem = true;

        float tempoSeguranca = 0; // Para evitar que o jogo trave se uma bola bugar

        while (esperandoBolasPararem && tempoSeguranca < 10f) // Máximo 10 segundos por jogada
        {
            tempoSeguranca += 0.2f;
            Rigidbody2D[] todasAsBolas = Object.FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None);
            bool algoAindaMove = false;

            foreach (var rb in todasAsBolas)
            {
                // Se qualquer bola estiver acima da velocidade de corte, ela ainda move
                if (rb.linearVelocity.magnitude > 0.15f)
                {
                    algoAindaMove = true;
                    break;
                }
            }

            if (!algoAindaMove)
            {
                esperandoBolasPararem = false;
            }

            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("Todas as bolas pararam. Verificando turno...");
        ChecarTrocaDeTurno();
    }

    public void LiberarTacoPlayer()
    {
        ControleTaco scriptTaco = FindFirstObjectByType<ControleTaco>();
        if (scriptTaco != null) scriptTaco.ResetarCadeado();
    }
    void ChecarTrocaDeTurno()
{
    if (!marcouPontoNestaRodada)
    {
        vezDoPlayer = !vezDoPlayer;
    }
    else 
    {
        marcouPontoNestaRodada = false;
    }

    if (vezDoPlayer) 
    {
        LiberarTacoPlayer(); // LIBERA O PLAYER AQUI
    }
    else 
    {
        oponenteScript.IniciarTurnoIA();
    }
}
}