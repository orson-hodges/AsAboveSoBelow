using System.IO.Compression;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class CameraMovementScript : MonoBehaviour
{
    public float CameraSpeed;
    private GameObject CameraObject;
    private GameObject PlayerObject;

    void Start()
    {
        CameraObject = gameObject;
        PlayerObject = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        CameraObject.transform.position = Vector3.Lerp(CameraObject.transform.position, PlayerObject.transform.position, CameraSpeed * Time.deltaTime);
        CameraObject.transform.position = new Vector3(CameraObject.transform.position.x, CameraObject.transform.position.y, -10);
    }
}
