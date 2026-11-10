using UnityEngine;
using UnityEngine.InputSystem;

public class test : MonoBehaviour
{
    private InputAction Click;

    private Camera PlayerCamera;

    public SoundSystemScript SoundScript;

    void Start()
    {
        Click = InputSystem.actions.FindAction("Spawn Sound Radis");
        PlayerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Click.WasCompletedThisFrame())
        {
            Vector2 MousePosition = PlayerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            float Radis = 20;

            SoundScript.SpawnSoundSource(Radis, MousePosition);
        }
    }
}
