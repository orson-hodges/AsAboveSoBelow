using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private Camera PlayerCamera;
    private GameObject PlayerObject;
    public Texture2D CursorSprite;
    
    // How far along the "line" between the player and the mouse the camera will be. Acts as a divider (2 is the exact midpoint, 3 is 1/3th along from the player, etc).
    public float DistanceRatio;  

    void Start()
    {
        PlayerCamera = Camera.main;
        PlayerObject = GameObject.FindWithTag("Player");

        // Setting the cursor to the custom cursor sprite and confining it to the game space so it cannot leave the window.
        Cursor.SetCursor(CursorSprite, new Vector2(50, 50), CursorMode.ForceSoftware);
        Cursor.lockState = CursorLockMode.Confined;
    }

    void FixedUpdate()
    {
        // Gets the camera target position by getting the mouse position in world space, getting the vector to the mouse's position from the player's position, and then dividing by DistanceRatio. Then sets the camera's position to the target position.
        Vector3 MousePosition = PlayerCamera.ScreenToWorldPoint(new Vector2(Mathf.Clamp(Mouse.current.position.x.ReadValue(), 0, 800), Mathf.Clamp(Mouse.current.position.y.ReadValue(), 0, 800)));
        Vector2 CameraTargetPosition = PlayerObject.transform.position + (MousePosition - PlayerObject.transform.position)/DistanceRatio;
        PlayerCamera.transform.position = new Vector3(CameraTargetPosition.x, CameraTargetPosition.y, -10);
    }
}
