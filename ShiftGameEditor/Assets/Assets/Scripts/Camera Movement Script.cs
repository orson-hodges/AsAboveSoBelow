using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;
using UnityEngine.InputSystem;


public class CameraMovementScript : MonoBehaviour
{
    private Camera Cam;
    private GameObject PlayerObject;
    private Vector2 MousePosition;
    private float XPosition;
    public float Distance;  

    void Start()
    {
        Cam = Camera.main;
        PlayerObject = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        // Getting the position for the camera to be, which is a point between the player and the mouse, determined by the "Distance" variable.
        // Checking if the X axis of the mouse is larger or smaller to the X axis of the player, as this changes the maths needed for the camera's X axis. The camera's Y axis' formula is the same for both but depends on the camera's X axis.
        MousePosition = ((Vector2)Cam.ScreenToWorldPoint(Input.mousePosition));
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
            transform.position = new Vector3(transform.position.x + (transform.position.x/Distance), MousePosition.y, -10);
        }

        float AngleRadis = Mathf.Atan2(MousePosition.y - PlayerObject.transform.position.y, MousePosition.x - PlayerObject.transform.position.x);
        float AngleDegree = (180 / Mathf.PI) * AngleRadis + 90;
        PlayerObject.transform.rotation = Quaternion.Euler(0, 0, AngleDegree);
    }

    void SetPosition()
    {
        // Setting the camera's position.
         transform.position = new Vector3(XPosition, ((MousePosition.y - PlayerObject.transform.position.y) / (MousePosition.x - PlayerObject.transform.position.x)) * (XPosition - PlayerObject.transform.position.x) + PlayerObject.transform.position.y, -10);
    }
}
