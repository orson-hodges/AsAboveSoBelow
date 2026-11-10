using UnityEngine;

public class SoundSystemScript : MonoBehaviour
{
    public GameObject SoundSourcePrefab;

    public void SpawnSoundSource(float radis, Vector2 position)
    {
        SoundSourcePrefab.transform.localScale = new Vector2(radis, radis);
        SoundSourcePrefab.transform.position = position;
        Instantiate(SoundSourcePrefab);
    }

    public void DespawnSoundSource(GameObject soundprefab)
    {
        Destroy(soundprefab);
    }
}
