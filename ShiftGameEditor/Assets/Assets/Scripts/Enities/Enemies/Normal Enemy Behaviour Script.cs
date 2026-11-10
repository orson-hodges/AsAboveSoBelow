using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NormalEnemyBehaviourScript : MonoBehaviour
{
    public float WalkingSpeed;
    public float RunningSpeed;
    public float IdleRotationDuration;
    public float PatrollingRotationDuration;
    public float InvestigatingRotationDuration;
    
    public float InvestigatingDistance;
    public float InvestigatingDuration;

    private bool Unaware;

    private Vector2 OriginalPosition;
    private Vector3 SoundSourcePosition;

    private NavMeshAgent NavigationMechAgent;
    private GameObject PlayerObject;
    public GameObject FieldOfVisionObject;

    public bool IdleRotating;
    public float[] AngleOfIdleRotationArray;
    public float[] DurationOfIdleRotationArray;
    private int CurrentIdleRotation;

    private Vector3 CurrentAngle;
    private float AngleChange;

    private int UpdateTicker;
    public int LineOfSightUpdateTickRate;

    private float IdleTimer;
    private float InvestigatingTimer;
    private float RotationTimer;
    private float RotationDuration;

    public GameObject[] PatrolPath;
    private int CurrentPatrolPoint;

    public State CurrentState;
    private State DefaultState;

    void Start()
    {
        NavigationMechAgent = GetComponent<NavMeshAgent>();
        NavigationMechAgent.updateRotation = false;
        NavigationMechAgent.updateUpAxis = false;
        NavigationMechAgent.speed = RunningSpeed;

        PlayerObject = GameObject.FindWithTag("Player");

        SetRotationDuration();
        DefaultState = CurrentState;
        OriginalPosition = transform.position;

        Unaware = true;

        RotationTimer = 0;
        IdleTimer = 0;
        
        CurrentPatrolPoint = 0;
        CurrentIdleRotation = 0;

        CurrentAngle = transform.eulerAngles;

        AngleChange = 0;
    }

    void FixedUpdate()
    {   
        UpdateTicker += 1;

        IdleTimer += Time.fixedDeltaTime;
        InvestigatingTimer += Time.fixedDeltaTime;
        RotationTimer += Time.fixedDeltaTime;

        switch (CurrentState)
        {
            case State.Idling:

                IdlingUpdate();
                break;

            case State.Patrolling:

                PatrollingUpdate();
                break;
            
            case State.MovingToInvestigate:

                MovingToInvestigateUpdate();
                break;

            case State.Investigating:

                InvestigatingUpdate();
                break;

            case State.Returning:

                ReturningUpdate();
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
        if (IdleRotating == true)
        {
            float AngleOfIdlePosition = AngleOfIdleRotationArray[CurrentIdleRotation];        
            Rotate(CurrentAngle, AngleOfIdlePosition, RotationTimer/RotationDuration);

            if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, AngleOfIdlePosition)) < 0.01)
            {
                if (IdleTimer > DurationOfIdleRotationArray[CurrentIdleRotation])
                {
                    CurrentIdleRotation += 1;

                    if(CurrentIdleRotation == AngleOfIdleRotationArray.Length)
                    {
                        CurrentIdleRotation = 0;
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

    void PatrollingUpdate()
    {
        LookTowardsObject(CurrentAngle, PatrolPath[CurrentPatrolPoint].transform.position, RotationTimer/RotationDuration);

        transform.position = Vector2.MoveTowards(transform.position, PatrolPath[CurrentPatrolPoint].transform.position, WalkingSpeed * Time.deltaTime);
    }

    void MovingToInvestigateUpdate()
    {
        LookTowardsObject(transform.eulerAngles, NavigationMechAgent.steeringTarget, 0.05f);

        if (UpdateTicker % LineOfSightUpdateTickRate == 0)
        {   
            if (NavigationMechAgent.remainingDistance < InvestigatingDistance)
            {
                RaycastHit2D RaycastResult = Physics2D.Raycast(transform.position, SoundSourcePosition - transform.position, Vector2.Distance(transform.position, SoundSourcePosition));

                if (RaycastResult.collider == null)
                {
                    NavigationMechAgent.ResetPath();

                    RotationTimer = 0;
                    CurrentAngle = transform.eulerAngles;

                    InvestigatingTimer = 0;
                    CurrentState = State.Investigating;
                    SetRotationDuration();
                }  
            }
        }
        
    }

    void InvestigatingUpdate()
    {
        float SoundSourceAngleInRadii = Mathf.Atan2(SoundSourcePosition.y - transform.position.y, SoundSourcePosition.x - transform.position.x);
        float SoundSourceAngleInDegrees = (180 / Mathf.PI) * SoundSourceAngleInRadii + 90;
        float FacingAngle = SoundSourceAngleInDegrees + AngleChange;

        Rotate(CurrentAngle, FacingAngle, RotationTimer/RotationDuration);

        if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, FacingAngle)) < 0.01)
        {
            CurrentAngle = transform.eulerAngles;

            if (SoundSourceAngleInDegrees < FacingAngle)
            {
                AngleChange = -60;
            }
            else if (SoundSourceAngleInDegrees >= FacingAngle)
            {
                AngleChange = +60;
            }
            
            RotationTimer = 0;
        }

        if (InvestigatingTimer > InvestigatingDuration)
        {
            NavigationMechAgent.SetDestination(OriginalPosition);

            CurrentState = State.Returning;
        }
    }

    void ReturningUpdate()
    {
        LookTowardsObject(transform.eulerAngles, NavigationMechAgent.steeringTarget, 0.05f);

        if (NavigationMechAgent.remainingDistance < 0.01)
        {   
            IdleTimer = 0;

            NavigationMechAgent.ResetPath();
            CurrentState = DefaultState;
            SetRotationDuration();
        }
    }

    void ChasingUpdate()
    {
        NavigationMechAgent.SetDestination(PlayerObject.transform.position);

        LookTowardsObject(transform.eulerAngles, NavigationMechAgent.steeringTarget, 0.05f);
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

    void LookTowardsObject(Vector3 currentangle, Vector2 targetobjectposition,  float interpolation)
    {
        float TargetObjectAngleInRadii = Mathf.Atan2(targetobjectposition.y - transform.position.y, targetobjectposition.x - transform.position.x);
        float TargetObjectAngleInDegrees = (180 / Mathf.PI) * TargetObjectAngleInRadii + 90;

        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(currentangle.z, TargetObjectAngleInDegrees, interpolation/(Mathf.Abs(Mathf.DeltaAngle(CurrentAngle.z, TargetObjectAngleInDegrees))/360)));     
    }

    void Rotate(Vector3 currentangle, float targetangle, float interpolation)
    {
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(currentangle.z, targetangle, interpolation/(Mathf.Abs(Mathf.DeltaAngle(CurrentAngle.z, targetangle))/360)));
    }

    void SetRotationDuration()
    {
        if (CurrentState == State.Idling)
        {
            RotationDuration = IdleRotationDuration;
        }
        else if (CurrentState == State.Patrolling)
        {
            RotationDuration = PatrollingRotationDuration;
        }
        else if (CurrentState == State.Investigating)
        {
            RotationDuration = InvestigatingRotationDuration;
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject == PatrolPath[CurrentPatrolPoint])
        {
            if (Vector2.Distance(transform.position, PatrolPath[CurrentPatrolPoint].transform.position) < 0.01)
            {
                CurrentPatrolPoint += 1;

                if (CurrentPatrolPoint == PatrolPath.Length)
                {
                    CurrentPatrolPoint = 0;
                }
            }

            CurrentAngle = transform.eulerAngles;

            RotationTimer = 0;
        }
        
        if (UpdateTicker % LineOfSightUpdateTickRate == 0)
        {
            if (collider.gameObject == PlayerObject)
            {
                RaycastHit2D RaycastResult = Physics2D.Raycast(transform.position, PlayerObject.transform.position - transform.position);

                if (RaycastResult.collider == PlayerObject.GetComponent<Collider2D>())
                {
                    Unaware = false;
                    NavigationMechAgent.isStopped = false;
                    NavigationMechAgent.speed = RunningSpeed;
                    FieldOfVisionObject.SetActive(false);
                    CurrentState = State.Chasing;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Sound Source"))
        {;
            if (Unaware == true)
            {
                NavigationMechAgent.isStopped = false;
                SoundSourcePosition = collider.gameObject.transform.position;
                NavigationMechAgent.SetDestination(SoundSourcePosition);
                
                if (CurrentState == DefaultState)
                {
                    OriginalPosition = transform.position;
                }

                CurrentState = State.MovingToInvestigate;
            }
        }
    }

    public enum State
    {
        Idling,
        Patrolling,
        MovingToInvestigate,
        Investigating,
        Returning,
        Chasing,
        AttackingRanged,
        AttackingMelee,
        Searching,
        Dying
    }
}
