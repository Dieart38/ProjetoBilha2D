using UnityEngine;

public class ControleBola : MonoBehaviour
{
    [Header("Física e Movimento")]
    private Rigidbody2D rb;
    public float forcaMax = 15f;           // Potência máxima da tacada
    public float sensibilidadeGiro = 150f;  // Velocidade de giro (Teclado/Controle)
    public float atritoParada = 0.15f;      // Velocidade mínima para a bola ser considerada 'parada'

    [Header("Visuais da Mira e Taco")]
    public LineRenderer linhaVisual;       // Arraste a linha aqui
    public Transform tacoTransform;        // Arraste o objeto do Taco aqui
    public float comprimentoLinha = 3f;    // Tamanho da linha de mira
    public float velocidadeTracejado = 2f; // Velocidade da animação dos pontinhos

    public float velocidadeRecuo = 2f;     // Velocidade de oscilação da força (quanto mais alto, mais rápido sobe/desce)

    private float anguloMira = 0f;         // Direção atual para onde você aponta
    private float forcaAtual = 0f;         // Força acumulada no momento
    private bool carregandoTacada = false; // Define se o jogador está segurando o botão

    void Start()
    {
        // Pega o componente físico da bola
        rb = GetComponent<Rigidbody2D>();

        // Configura o atrito para a bola parar naturalmente
        // rb.linearDamping = 1.0f;
        //rb.angularDamping = 1.0f;
    }

    void Update()
    {
        // Verifica se a velocidade da bola é menor que o limite de parada
        bool bolaParada = rb.linearVelocity.magnitude < atritoParada;

        if (bolaParada)
        {
            // Se estiver quase parada, forçamos a parada total para habilitar a mira
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            linhaVisual.enabled = true; // Mostra a linha
            if (tacoTransform != null) tacoTransform.gameObject.SetActive(true); // Mostra o taco

            ProcessarEntrada();
            AtualizarVisuais();
        }
        else
        {
            // Se a bola estiver correndo, esconde os visuais de mira
            linhaVisual.enabled = false;
            if (tacoTransform != null) tacoTransform.gameObject.SetActive(false);
            carregandoTacada = false;
        }
    }

    void ProcessarEntrada()
    {
        // 1. MIRA PELO MOUSE
        Vector3 posMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direcaoMouse = (Vector2)(posMouse - transform.position);

        // Só atualiza pelo mouse se houver movimento do hardware
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            // Atan2 converte a distância X e Y em um ângulo de 0 a 360 graus
            anguloMira = Mathf.Atan2(direcaoMouse.y, direcaoMouse.x) * Mathf.Rad2Deg;
        }

        // 2. MIRA PELO TECLADO/CONTROLE
        float h = Input.GetAxis("Horizontal"); // Setas Esquerda/Direita ou A/D
        anguloMira += h * -sensibilidadeGiro * Time.deltaTime;

        // 3. CARREGAMENTO DA FORÇA
        if (Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0))
        {
            carregandoTacada = true;
        }

        if (carregandoTacada)
        {
            // Efeito de "PingPong": a força sobe e desce enquanto você segura
            forcaAtual = Mathf.PingPong(Time.time * velocidadeRecuo, forcaMax);

            // Ao soltar o botão, a tacada é executada
            if (Input.GetButtonUp("Jump") || Input.GetMouseButtonUp(0))
            {
                // Converte o ângulo final em um Vetor de direção
                Vector2 direcaoFinal = new Vector2(Mathf.Cos(anguloMira * Mathf.Deg2Rad), Mathf.Sin(anguloMira * Mathf.Deg2Rad));

                // Aplica o impulso físico na bola
                rb.AddForce(direcaoFinal * forcaAtual, ForceMode2D.Impulse);

                carregandoTacada = false;
                forcaAtual = 0f;
            }
        }
    }

    void AtualizarVisuais()
    {
        // 1. CONFIGURAÇÕES INICIAIS
        Vector3 direcaoMira = new Vector3(Mathf.Cos(anguloMira * Mathf.Deg2Rad), Mathf.Sin(anguloMira * Mathf.Deg2Rad), 0);
        float raioBola = 0.35f;
        Vector3 pontoAtual = transform.position + (direcaoMira * raioBola);
        Vector3 direcaoAtual = direcaoMira;

        // Precisamos de 3 pontos para mostrar: Origem -> Parede 1 -> Parede 2
        linhaVisual.positionCount = 3;
        linhaVisual.SetPosition(0, pontoAtual);

        // 2. PRIMEIRO IMPACTO (Tabela 1)
        RaycastHit2D hit1 = Physics2D.Raycast(pontoAtual, direcaoAtual, comprimentoLinha);

        if (hit1.collider != null)
        {
            linhaVisual.SetPosition(1, hit1.point);

            // Calcula a primeira reflexão
            direcaoAtual = Vector2.Reflect(direcaoAtual, hit1.normal);

            // 3. SEGUNDO IMPACTO (Tabela 2)
            // Disparamos um novo raio a partir do ponto de impacto anterior
            // Usamos um pequeno "offset" (0.01f) para o raio não bater na própria parede de onde saiu
            RaycastHit2D hit2 = Physics2D.Raycast(hit1.point + (hit1.normal * 0.01f), direcaoAtual, comprimentoLinha * 0.5f);

            if (hit2.collider != null)
            {
                linhaVisual.SetPosition(2, hit2.point);
            }
            else
            {
                // Se não bater em nada na segunda vez, apenas estica a linha na direção refletida
                linhaVisual.SetPosition(2, (Vector3)hit1.point + (Vector3)direcaoAtual * (comprimentoLinha * 0.5f));
            }
        }
        else
        {
            // Caso não bata em nada nem na primeira parede
            linhaVisual.positionCount = 2; // Volta para 2 pontos para não bugar o visual
            linhaVisual.SetPosition(1, pontoAtual + (direcaoMira * comprimentoLinha));
        }

        // --- MANTER ANIMAÇÃO E TACO ---
        AplicarEstiloLinha();
        AtualizarPosicaoTaco();
    }

    void AplicarEstiloLinha()
    {
        float off = Time.time * velocidadeTracejado;
        linhaVisual.material.mainTextureOffset = new Vector2(-off, 0);

        // Cor dinâmica baseada na força
        Color corDinamica = Color.Lerp(Color.cyan, Color.red, forcaAtual / forcaMax);
        linhaVisual.startColor = corDinamica;
        linhaVisual.endColor = new Color(corDinamica.r, corDinamica.g, corDinamica.b, 0);
    }
    void AtualizarPosicaoTaco()
    {
        if (tacoTransform != null)
        {
            float anguloTaco = anguloMira + 180f;
            tacoTransform.rotation = Quaternion.Euler(0, 0, anguloTaco);
            float distRecuo = 0.7f + (forcaAtual / forcaMax) * 0.8f;
            Vector3 dirTaco = new Vector3(Mathf.Cos(anguloTaco * Mathf.Deg2Rad), Mathf.Sin(anguloTaco * Mathf.Deg2Rad), 0);
            tacoTransform.position = transform.position + (dirTaco * distRecuo);
        }
    }
}