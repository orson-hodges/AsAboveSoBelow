using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelTransitionSystem : MonoBehaviour
{
    private bool Transitioning = false;

    private GameObject PlayerObject;
    private PlayerInputSystem PlayerInputSystemScript;
    private PlayerInput PlayerInputMap;
    private CameraMovement CameraMovementScript;

    public GameObject TargetPosition;
    public string NextScene;
    public bool Entrance;
    public float TransitionTime;
    private float TransitionTimer;

    void Start()
    {
        PlayerObject = GameObject.FindWithTag("Player");
        PlayerInputSystemScript = PlayerObject.GetComponent<PlayerInputSystem>();
        PlayerInputMap = PlayerObject.GetComponent<PlayerInput>();
        CameraMovementScript = GameObject.FindWithTag("MainCamera").GetComponent<CameraMovement>();

        if (Entrance)
        {
            PlayerInputSystemScript.enabled = false;
            CameraMovementScript.enabled = false;
            PlayerInputMap.enabled = false;
        }
    }

    void FixedUpdate()
    {
        if (Transitioning)
        {
            PlayerObject.transform.position = Vector2.MoveTowards(PlayerObject.transform.position, TargetPosition.transform.position, PlayerInputSystemScript.MovementSpeed * Time.fixedDeltaTime);
            
            float TargetPositionAngleInRadii = Mathf.Atan2(TargetPosition.transform.position.y - PlayerObject.transform.position.y, TargetPosition.transform.position.x - PlayerObject.transform.position.x);
            float TargetPositionAngleInDegrees = (180 / Mathf.PI) * TargetPositionAngleInRadii + 90;
            PlayerObject.transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(PlayerObject.transform.eulerAngles.z, TargetPositionAngleInDegrees, 0.25f));

            TransitionTimer += Time.fixedDeltaTime;

            if (TransitionTimer > TransitionTime)
            {
                SceneManager.LoadScene(NextScene);
            }
        }
        else if(Entrance)
        {
            PlayerObject.transform.position = Vector2.MoveTowards(PlayerObject.transform.position, TargetPosition.transform.position, PlayerInputSystemScript.MovementSpeed * Time.fixedDeltaTime);
            
            float TargetPositionAngleInRadii = Mathf.Atan2(TargetPosition.transform.position.y - PlayerObject.transform.position.y, TargetPosition.transform.position.x - PlayerObject.transform.position.x);
            float TargetPositionAngleInDegrees = (180 / Mathf.PI) * TargetPositionAngleInRadii + 90;
            PlayerObject.transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(PlayerObject.transform.eulerAngles.z, TargetPositionAngleInDegrees, 0.25f));

            if (Vector2.Distance(PlayerObject.transform.position, TargetPosition.transform.position) < 0.01)
            {
                PlayerInputSystemScript.enabled = true;
                CameraMovementScript.enabled = true;
                PlayerInputMap.enabled = true;
                Entrance = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerInputSystemScript.PlayerBody.linearVelocity = Vector2.zero;
        PlayerInputSystemScript.enabled = false;
        CameraMovementScript.enabled = false;
        PlayerInputMap.enabled = false;
        Transitioning = true;
    }
}
