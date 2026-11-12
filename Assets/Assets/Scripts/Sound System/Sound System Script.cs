using UnityEditor;
using UnityEngine;

public class SoundSystemScript : MonoBehaviour
{
    public GameObject SoundSourcePrefab;

    // Spawn a sound source object with a specific radius at a specific position.
    public void SpawnSoundSource(float radius, Vector2 position)
    {
        SoundSourcePrefab.transform.localScale = new Vector2(radius, radius);
        SoundSourcePrefab.transform.position = position;
        Instantiate(SoundSourcePrefab);
    }

    // Destroy a sound source object.
    public void DespawnSoundSource(GameObject soundprefab)
    {
        Destroy(soundprefab);
    }
}
