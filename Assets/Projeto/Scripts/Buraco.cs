using UnityEngine;

public class Buraco : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BolaBranca"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            other.transform.position = new Vector2(-3, 0);
        }
        else if (other.CompareTag("Bola"))
        {
            // AVISA O GAME MANAGER
            GameManager.instancia.AdicionarPontuacao();
            
            Destroy(other.gameObject);
        }
    }
}