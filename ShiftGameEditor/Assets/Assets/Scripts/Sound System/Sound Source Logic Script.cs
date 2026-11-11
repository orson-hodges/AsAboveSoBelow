using UnityEngine;

public class SoundSourceLogicScript : MonoBehaviour
{
    private SoundSystemScript SoundScript;

    // How long the sound lasts.
    public float SoundDuration;

    void Start()
    {
        SoundScript = GameObject.FindWithTag("Sound Manager").GetComponent<SoundSystemScript>();
    }

    void FixedUpdate()
    {
        SoundDuration -= Time.fixedDeltaTime;

        // After the duration, the sound source object is despawned by the sound manager.
        if (SoundDuration <= 0)
        {
            SoundScript.DespawnSoundSource(gameObject);
        }
    }
}
