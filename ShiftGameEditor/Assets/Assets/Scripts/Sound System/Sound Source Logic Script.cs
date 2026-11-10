using UnityEngine;

public class SoundSourceLogicScript : MonoBehaviour
{
    private SoundSystemScript SoundScript;

    public float Duration;

    void Start()
    {
        SoundScript = GameObject.FindWithTag("Sound Manager").GetComponent<SoundSystemScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Duration -= Time.fixedDeltaTime;

        if (Duration <= 0)
        {
            SoundScript.DespawnSoundSource(gameObject);
        }
    }
}
