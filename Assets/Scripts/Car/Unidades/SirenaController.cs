using UnityEngine;
using System.Collections;

public class SirenaController : MonoBehaviour
{
    public Light[] lucesSirena;
    public AudioSource sirena;
    public float pitchNormal = 1f;
    public float pitchRapido = 2f;
    public float parpadeoIntervalo = 0.3f;

    private bool sirenaActiva = false;
    private Coroutine parpadeoCoroutine;

    void Update()
    {
        // Pulsación única para encender/apagar sirena y luces
        if (Input.GetKeyDown(KeyCode.E))
        {
            sirenaActiva = !sirenaActiva;

            if (sirenaActiva)
            {
                EncenderSirena();
                parpadeoCoroutine = StartCoroutine(ParpadearLuces());
            }
            else
            {
                ApagarSirena();
                if (parpadeoCoroutine != null)
                    StopCoroutine(parpadeoCoroutine);
                ApagarLuces();
            }
        }

        // Aumentar pitch si se mantiene pulsada la E
        if (sirenaActiva)
        {
            sirena.pitch = Input.GetKey(KeyCode.E) ? pitchRapido : pitchNormal;
        }
    }

    void EncenderSirena()
    {
        if (sirena && !sirena.isPlaying)
        {
            sirena.loop = true;
            sirena.pitch = pitchNormal;
            sirena.Play();
        }
    }

    void ApagarSirena()
    {
        if (sirena && sirena.isPlaying)
        {
            sirena.Stop();
        }
    }

    void ApagarLuces()
    {
        foreach (Light luz in lucesSirena)
        {
            luz.enabled = false;
        }
    }

    IEnumerator ParpadearLuces()
    {
        bool estado = false;
        while (true)
        {
            estado = !estado;
            foreach (Light luz in lucesSirena)
            {
                luz.enabled = estado;
            }
            yield return new WaitForSeconds(parpadeoIntervalo);
        }
    }
}
