using UnityEngine;
using UnityEngine.UI; // Adicione isso no topo

public class ControleTaco : MonoBehaviour
{
    public LayerMask camadasAtingiveis; // No Inspector, selecione "Tudo" EXCETO a camada da Bola Branca
    public Transform bolaTransform;
    public Rigidbody2D rbBola;
    public float forcaMaxima = 20f;
    public Slider barraForca; // Arraste o Slider do Canvas para cá no Inspector
    private bool estaCarregando = false;
    private float forcaAtual = 0f;
    private AudioSource somTaco;
    private bool podeJogar = true; // Novo cadeado para evitar inputs durante a jogada da IA
    public GameObject spriteDoTaco;
    public LineRenderer linhaGuia;
    public float distanciaLinha = 5f;

    private float anguloManual = 0f;

    void Start()
    {
        // somTaco = rbBola.GetComponent<AudioSource>();
        spriteDoTaco.transform.localPosition = new Vector3(-forcaAtual * 0.1f, 0, 0);


        somTaco = rbBola.GetComponent<AudioSource>();
        // Inicializa o ângulo manual com a rotação atual para não dar "pulo" ao começar
        anguloManual = transform.eulerAngles.z;
    }

    public void ResetarCadeado()
    {
        podeJogar = true;
    }
    void Update()
    {
        if (JukeboxController.MenuEstaAberto)
        {
            estaCarregando = false; // Reseta o estado de carregamento
            linhaGuia.enabled = false;
            return;
        }

        bool vezDoPlayerNoManager = GameManager.instancia.vezDoPlayer;
        bool bolaParadaFisica = rbBola.linearVelocity.magnitude < 0.1f;

        // O CADEADO SÓ ABRE VIA CHAMADA EXTERNA (ResetarCadeado)
        if (vezDoPlayerNoManager && podeJogar && bolaParadaFisica)
        {
            spriteDoTaco.SetActive(true);
            GerenciarMira();
            GerenciarTacada();
        }
        else
        {
            spriteDoTaco.SetActive(false);
            estaCarregando = false;
            linhaGuia.enabled = false;
        }

        // Estabilização (Mantido)
        if (rbBola.linearVelocity.magnitude < 0.1f) // Aumentamos um pouco o limite
        {
            rbBola.linearVelocity = Vector2.zero;
            rbBola.angularVelocity = 0f;
        }
    }

    void GerenciarMira()
    {
        transform.position = bolaTransform.position;

        // --- MIRA MULTI-INPUT ---
        float h = Input.GetAxis("Horizontal");
        if (Mathf.Abs(h) > 0.01f)
        {
            anguloManual += h * -150f * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, anguloManual);
        }
        else if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            Vector2 direcaoMouse = (Vector2)mouseWorldPos - (Vector2)bolaTransform.position;
            anguloManual = Mathf.Atan2(direcaoMouse.y, direcaoMouse.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, anguloManual);
        }

        // --- LINHA GUIA SIMPLIFICADA (PONTO ÚNICO) ---
        linhaGuia.enabled = true;
        Vector2 direcaoBranca = transform.right;

        // Ponto de partida levemente à frente para evitar colisão com a própria branca
        float raioBola = 0.3f;
        Vector2 pontoPartida = (Vector2)bolaTransform.position + (direcaoBranca * raioBola);

        // Definimos apenas 2 pontos para uma linha reta simples
        linhaGuia.positionCount = 2;
        linhaGuia.SetPosition(0, (Vector2)bolaTransform.position);

        RaycastHit2D hit = Physics2D.Raycast(pontoPartida, direcaoBranca, distanciaLinha, camadasAtingiveis);

        if (hit.collider != null)
        {
            // Se bater em algo, a linha para no ponto de impacto
            linhaGuia.SetPosition(1, hit.point);
        }
        else
        {
            // Se não bater em nada, a linha vai até o limite da distância
            linhaGuia.SetPosition(1, (Vector2)bolaTransform.position + (direcaoBranca * distanciaLinha));
        }
    }
    void GerenciarTacada()
    {
        // Detecção de inputs (Mouse, Teclado e Controle)
        bool pressionouBotao = Input.GetMouseButtonDown(0) || Input.GetButtonDown("Jump");
        bool segurandoBotao = Input.GetMouseButton(0) || Input.GetButton("Jump");
        bool soltouBotao = Input.GetMouseButtonUp(0) || Input.GetButtonUp("Jump");

        if (pressionouBotao)
        {
            estaCarregando = true;
        }

        if (segurandoBotao && estaCarregando)
        {
            // O segredo está aqui: Mathf.PingPong faz o valor ir de 0 até forcaMaxima e voltar
            // Time.time * velocidade controla a rapidez da oscilação
            float velocidadeOscilacao = 20f;
            forcaAtual = Mathf.PingPong(Time.time * velocidadeOscilacao, forcaMaxima);

            // Atualiza o recuo visual do taco baseado na força oscilante
            spriteDoTaco.transform.localPosition = new Vector3(-forcaAtual * 0.15f, 0, 0);

            // Atualiza o Slider (Barra de força)
            if (barraForca != null)
                barraForca.value = forcaAtual / forcaMaxima;
        }

        if (soltouBotao && estaCarregando)
        {
            // Toca o som da tacada
            if (somTaco != null) somTaco.Play();

            // 1. FECHA O CADEADO IMEDIATAMENTE
            podeJogar = false;

            // 2. APLICA A FORÇA
            Vector2 direcaoFinal = transform.right;
            rbBola.AddForce(direcaoFinal * forcaAtual, ForceMode2D.Impulse);

            // 3. AVISA O MANAGER
            GameManager.instancia.TacadaRealizada();
            // Reseta os visuais
            spriteDoTaco.transform.localPosition = Vector3.zero;
            estaCarregando = false;
            forcaAtual = 0; // Zera a força para a próxima tacada

            if (barraForca != null)
                barraForca.value = 0;
        }
    }
}