using UnityEngine;

public class SemaforoController : MonoBehaviour
{
    public GameObject luzRoja;
    public GameObject luzAmarilla;
    public GameObject luzVerde;

    public AudioSource cambioAudio;

    public float tiempoRojo = 5f;
    public float tiempoAmarillo = 2f;
    public float tiempoVerde = 5f;

    private enum Estado { Rojo, Amarillo, Verde }
    private Estado estadoActual;

    private float temporizador = 0f;

    void Start()
    {
        CambiarEstado(Estado.Rojo);
    }

    void Update()
    {
        temporizador -= Time.deltaTime;

        if (temporizador <= 0)
        {
            switch (estadoActual)
            {
                case Estado.Rojo:
                    CambiarEstado(Estado.Verde);
                    break;
                case Estado.Verde:
                    CambiarEstado(Estado.Amarillo);
                    break;
                case Estado.Amarillo:
                    CambiarEstado(Estado.Rojo);
                    break;
            }
        }
    }

    void CambiarEstado(Estado nuevoEstado)
    {
        estadoActual = nuevoEstado;

        luzRoja.SetActive(nuevoEstado == Estado.Rojo);
        luzAmarilla.SetActive(nuevoEstado == Estado.Amarillo);
        luzVerde.SetActive(nuevoEstado == Estado.Verde);

        if (cambioAudio) cambioAudio.Play();

        switch (nuevoEstado)
        {
            case Estado.Rojo:
                temporizador = tiempoRojo;
                break;
            case Estado.Amarillo:
                temporizador = tiempoAmarillo;
                break;
            case Estado.Verde:
                temporizador = tiempoVerde;
                break;
        }
    }
}
