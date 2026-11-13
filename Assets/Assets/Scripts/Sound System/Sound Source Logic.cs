using UnityEngine;

public class SoundSourceLogic : MonoBehaviour
{
    private SoundSystem SoundSystemScript;

    // How long the sound lasts.
    public float SoundDuration;

    void Start()
    {
        SoundSystemScript = GameObject.FindWithTag("Sound Manager").GetComponent<SoundSystem>();
    }

    void FixedUpdate()
    {
        SoundDuration -= Time.fixedDeltaTime;

        // After the duration, the sound source object is despawned by the sound manager.
        if (SoundDuration <= 0)
        {
            SoundSystemScript.DespawnSoundSource(gameObject);
        }
    }
}
