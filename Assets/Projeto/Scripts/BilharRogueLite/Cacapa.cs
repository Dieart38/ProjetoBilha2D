using UnityEngine;

public class Cacapa : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Inimigo"))
        {
            // Inimigo caiu! Você pode dar pontos ao jogador aqui
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            // O jogador caiu! Reinicia a posição ou tira vida
            other.transform.position = Vector3.zero;
            other.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            Debug.Log("Você caiu! Posicionando de volta ao centro.");
        }
    }
}