using UnityEngine;
using UnityEngine.InputSystem;


public class CameraMovementScript : MonoBehaviour
{
    private Camera PlayerCamera;
    private GameObject PlayerObject;
    protected Vector2 MousePosition;
    
    // Where the camera is suppose to be at the X axis. 
    protected float XPosition;
    // How far along the "line" between the player and the mouse the camera will be. Acts as a divider (2 is the exact midpoint, 3 is 1/3th along from the player, etc).
    public float Distance;  

    void Start()
    {
        PlayerCamera = Camera.main;
        PlayerObject = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        // Getting the position for the camera to be, which is a point between the player and the mouse, determined by the "Distance" variable.
        // Checking if the X axis of the mouse is larger or smaller to the X axis of the player, as this changes the maths needed for the camera's X axis. The camera's Y axis' formula is the same for both but depends on the camera's X axis.
        MousePosition = PlayerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        if (MousePosition.x > PlayerObject.transform.position.x)
        {
            XPosition = PlayerObject.transform.position.x + Mathf.Abs(PlayerObject.transform.position.x - MousePosition.x)/Distance;
            SetPosition();
        }
        else if (MousePosition.x < PlayerObject.transform.position.x)
        {
            XPosition = PlayerObject.transform.position.x - Mathf.Abs(MousePosition.x - PlayerObject.transform.position.x)/Distance;
            SetPosition();
        }
        else if(MousePosition.x == PlayerObject.transform.position.x)
        {
            // The X axis of the mouse and player being equal is a special case for both of the camera's X and Y axes.
            // Checking if the Y axis of the mouse is larger or smaller to the Y axis of the player, as this changes the maths needed for the camera's Y axis. Due to the mouse and player's X axis being the same in this case, the camera's X axis must also be the same.
            if (MousePosition.y > PlayerObject.transform.position.y)
            {
                transform.position = new Vector3(MousePosition.x, PlayerObject.transform.position.y + (Mathf.Abs(MousePosition.y - PlayerObject.transform.position.y)/Distance), -10);
            }
            else if (MousePosition.y < PlayerObject.transform.position.y)
            {
                transform.position = new Vector3(MousePosition.x, MousePosition.y + (Mathf.Abs(MousePosition.y - PlayerObject.transform.position.y)/Distance), -10);
            }
            else if (MousePosition.y == PlayerObject.transform.position.y)
            {
                // If both the X and Y axis are the same, then both the mouse and player have the same position, and as such the camera must too.
                transform.position = new Vector3(PlayerObject.transform.position.x, PlayerObject.transform.position.y, -10);
            }
        }

        // Rolating the player to always face the position of the camera. Note that it is adding 90 instead of substracting, as the "front" of the player is techanically the botton of its sprite.
        float CameraAngleRadis = Mathf.Atan2(MousePosition.y - PlayerObject.transform.position.y, MousePosition.x - PlayerObject.transform.position.x);
        float CameraAngleDegree = (180 / Mathf.PI) * CameraAngleRadis + 90;
        PlayerObject.transform.rotation = Quaternion.Euler(0, 0, CameraAngleDegree);
    }

    void SetPosition()
    {
        // Setting the camera's position.
         transform.position = new Vector3(XPosition, ((MousePosition.y - PlayerObject.transform.position.y) / (MousePosition.x - PlayerObject.transform.position.x)) * (XPosition - PlayerObject.transform.position.x) + PlayerObject.transform.position.y, -10);
    }
}
