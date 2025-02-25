using UnityEngine;

public class MenuMusicPlayer : MonoBehaviour
{
    public AudioClip[] canciones; // Array de canciones
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ReproducirCancionAleatoria();
    }

    void ReproducirCancionAleatoria()
    {
        if (canciones.Length == 0) return; // Evitar errores si no hay canciones

        int indiceAleatorio = Random.Range(0, canciones.Length); // Selecciona una canción aleatoria
        audioSource.clip = canciones[indiceAleatorio];
        audioSource.Play();

        // Llama a la función para cambiar de canción cuando termine
        Invoke("ReproducirCancionAleatoria", audioSource.clip.length);
    }
}