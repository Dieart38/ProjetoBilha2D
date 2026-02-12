using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JukeboxController : MonoBehaviour
{
    [Header("Configurações de Áudio")]
    public AudioClip[] listaDeMusicas;
    public AudioSource caixinhaDeSom;

    [Header("Interface")]
    public GameObject painelMenu;
    public Text textoNomeMusica; 
    public Slider sliderVolume;

    [Header("Suporte a Controle e Navegação")]
    public GameObject primeiroBotaoSelecionar; 

    // Variável global para impedir movimentação do taco
    public static bool MenuEstaAberto = false;

    private int musicaAtualIndex = 0;
    private bool playerEstaPerto = false;

    void Start()
    {
        painelMenu.SetActive(false);
        sliderVolume.onValueChanged.AddListener(MudarVolume);
        caixinhaDeSom.volume = sliderVolume.value;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            
                AlternarMenu();
            
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            AlternarMenu();
        }
    }

    private void OnMouseEnter()
    {
        Debug.Log("Mouse entrou na jukebox");
        if (!painelMenu.activeSelf ) // Adicionado playerEstaPerto para evitar abrir de longe
        {
            AlternarMenu();
        }
    }

    public void AlternarMenu()
    {
        bool abrindo = !painelMenu.activeSelf;
        
        if (abrindo) AbrirMenu();
        else FecharMenu();
    }

    public void AbrirMenu()
    {
        painelMenu.SetActive(true);
        MenuEstaAberto = true;

        // Garante que o mouse apareça e não fique preso
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Foca no botão inicial, mas limpa o foco anterior primeiro
        EventSystem.current.SetSelectedGameObject(null);
        if (primeiroBotaoSelecionar != null)
        {
            EventSystem.current.SetSelectedGameObject(primeiroBotaoSelecionar);
        }
    }

    public void FecharMenu()
    {
        painelMenu.SetActive(false);
        MenuEstaAberto = false; // Isso destrava o taco!

        // Se o seu jogo usa o mouse para mirar o taco, 
        // talvez você queira esconder o cursor de novo:
        // Cursor.visible = false; 
    }

    // --- FUNÇÕES DA JUKEBOX ---

    public void TocarMusica()
    {
        if (caixinhaDeSom.clip == null) CarregarMusica(0);
        caixinhaDeSom.Play();
        AtualizarTexto();
    }

    public void PararMusica()
    {
        caixinhaDeSom.Pause();
    }

    public void ProximaMusica()
    {
        musicaAtualIndex = (musicaAtualIndex + 1) % listaDeMusicas.Length;
        CarregarMusica(musicaAtualIndex);
        TocarMusica();
    }

    public void MusicaAnterior()
    {
        musicaAtualIndex--;
        if (musicaAtualIndex < 0) musicaAtualIndex = listaDeMusicas.Length - 1;
        CarregarMusica(musicaAtualIndex);
        TocarMusica();
    }

    public void MudarVolume(float valor)
    {
        caixinhaDeSom.volume = valor;
    }

    void CarregarMusica(int index)
    {
        caixinhaDeSom.clip = listaDeMusicas[index];
    }

    void AtualizarTexto()
    {
        if (textoNomeMusica != null && caixinhaDeSom.clip != null)
            textoNomeMusica.text = caixinhaDeSom.clip.name;
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("BolaBranca"))
    //     {
    //         playerEstaPerto = true;
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag("BolaBranca"))
    //     {
    //         playerEstaPerto = false;
    //         FecharMenu(); // Usa o método FecharMenu para resetar a variável estática
    //     }
    // }
}