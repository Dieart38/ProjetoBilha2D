using UnityEngine;

public class SomColisao : MonoBehaviour
{
    private AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D col) {
        // Toca o som apenas se a batida for minimamente forte
        if (col.relativeVelocity.magnitude > 0.5f) {
            // Ajusta o volume baseado na força da batida
            audioSource.volume = Mathf.Clamp(col.relativeVelocity.magnitude / 10f, 0.1f, 1f);
            audioSource.Play();
        }
    }
}