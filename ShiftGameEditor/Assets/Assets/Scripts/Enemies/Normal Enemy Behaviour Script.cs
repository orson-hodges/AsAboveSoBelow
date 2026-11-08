using UnityEngine;
using UnityEngine.AI;

public class NormalEnemyBehaviourScript : MonoBehaviour
{
    public float WalkingSpeed;
    public float RunningSpeed;

    private NavMeshAgent NavigationMechAgent;
    private GameObject PlayerObject;
    [SerializeField] private GameObject FieldofVisionObject;

    public bool RolatingIdlePositions;
    public float[] AngleOfIdlePositionArray;
    public float[] DurationOfIdlePositionArray;

    private int CurrentIdlePosition;
    private Vector3 CurrentAngle;

    private int UpdateTicker;
    public int LineOfSightUpdatePeriod;

    private float IdleTimer;
    private float RotationTimer;
    private float RotationDuration;

    public GameObject[] PatrolPath;
    private int CurrentPartolPoint;

    [SerializeField] private State CurrentState;

    void Start()
    {
        NavigationMechAgent = GetComponent<NavMeshAgent>();
        NavigationMechAgent.updateRotation = false;
        NavigationMechAgent.updateUpAxis = false;
        NavigationMechAgent.speed = RunningSpeed;

        PlayerObject = GameObject.Find("Player");

        if (CurrentState == State.Idling)
        {
            RotationDuration = 6;
        }
        else if (CurrentState == State.Patroling)
        {
            RotationDuration = 0.3f;
        }

        RotationTimer = 0;
        IdleTimer = 0;
        
        CurrentPartolPoint = 0;
        CurrentIdlePosition = 0;

        CurrentAngle = transform.eulerAngles;
    }

    void FixedUpdate()
    {   
        UpdateTicker += 1;

        RotationTimer += Time.fixedDeltaTime;
        IdleTimer += Time.fixedDeltaTime;

        switch (CurrentState)
        {
            case State.Idling:

                IdlingUpdate();
                break;

            case State.Patroling:

                PatrolingUpdate();
                break;

            case State.Investigating:

                InvestigatingUpdate();
                break;

            case State.Chasing:

                ChasingUpdate();
                break;

            case State.AttackingRanged:

                AttackingRangedUpdate();
                break;

            case State.AttackingMelee:

                AttackingMeleeUpdate();
                break;

            case State.Searching:

                SearchingUpdate();
                break;

            case State.Dying:

                DyingUpdate();
                break;
        }
    }

    void IdlingUpdate()
    {
        if (RolatingIdlePositions == true)
        {
            float AngleOfIdlePosition = AngleOfIdlePositionArray[CurrentIdlePosition];
            float RotationPercentage = (RotationTimer/RotationDuration)/(Mathf.Abs(Mathf.DeltaAngle(CurrentAngle.z, AngleOfIdlePosition))/360);

            transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(CurrentAngle.z, AngleOfIdlePosition, RotationPercentage));

            float AngularDistanceToIdlePosition = Mathf.DeltaAngle(transform.eulerAngles.z, AngleOfIdlePosition);

            if (Mathf.Abs(AngularDistanceToIdlePosition) < 0.01)
            {
                if (IdleTimer > DurationOfIdlePositionArray[CurrentIdlePosition])
                {
                    CurrentIdlePosition += 1;

                    if(CurrentIdlePosition == AngleOfIdlePositionArray.Length)
                    {
                        CurrentIdlePosition = 0;
                    }

                    CurrentAngle = transform.eulerAngles;

                    RotationTimer = 0;
                    IdleTimer = 0;
                }
            }
            else
            {
                IdleTimer = 0;
            }
        }
    }

    void PatrolingUpdate()
    {
        LookingTowardsObject(CurrentAngle, PatrolPath[CurrentPartolPoint].transform.position, RotationTimer/RotationDuration);

        transform.position = Vector2.MoveTowards(transform.position, PatrolPath[CurrentPartolPoint].transform.position, WalkingSpeed * Time.deltaTime);
    }

    void InvestigatingUpdate()
    {
        
    }

    void ChasingUpdate()
    {
        NavigationMechAgent.SetDestination(PlayerObject.transform.position);

        LookingTowardsObject(transform.eulerAngles, NavigationMechAgent.steeringTarget, 0.5f);
    }

    void AttackingRangedUpdate()
    {
        
    }

    void AttackingMeleeUpdate()
    {
        
    }

    void SearchingUpdate()
    {
        
    }

    void DyingUpdate()
    {
        
    }

    void LookingTowardsObject(Vector3 currentangle, Vector2 targetobjectposition,  float interpolation)
    {
        float PatrolAngleRadii = Mathf.Atan2(targetobjectposition.y - transform.position.y, targetobjectposition.x - transform.position.x);
        float PatrolAngleDegrees = (180 / Mathf.PI) * PatrolAngleRadii + 90;

        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(currentangle.z, PatrolAngleDegrees, interpolation));
        
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject == PatrolPath[CurrentPartolPoint])
        {
            float DistanceToPartolPoint = Vector2.Distance(transform.position, PatrolPath[CurrentPartolPoint].transform.position);

            if (DistanceToPartolPoint < 0.01)
            {
                CurrentPartolPoint += 1;

                if (CurrentPartolPoint == PatrolPath.Length)
                {
                    CurrentPartolPoint = 0;
                }
            }

            CurrentAngle = transform.eulerAngles;

            RotationTimer = 0;
            return;
        }
        
        if (UpdateTicker % LineOfSightUpdatePeriod == 0)
        {
            if (collider.gameObject == PlayerObject)
            {
                RaycastHit2D RaycastResult = Physics2D.Raycast(transform.position, PlayerObject.transform.position - transform.position);

                if (RaycastResult.collider == PlayerObject.GetComponent<Collider2D>())
                {
                    CurrentState = State.Chasing;
                    FieldofVisionObject.SetActive(false);
                }
            }
        }
    }

    public enum State
    {
        Idling,
        Patroling,
        Investigating,
        Chasing,
        AttackingRanged,
        AttackingMelee,
        Searching,
        Dying
    }
}
