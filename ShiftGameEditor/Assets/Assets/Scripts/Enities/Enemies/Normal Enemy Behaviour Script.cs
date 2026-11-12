using System;
using UnityEngine;
using UnityEngine.AI;

public class NormalEnemyBehaviourScript : MonoBehaviour
{
    public float WalkingSpeed;
    public float RunningSpeed;
    public float StrafingSpeed;

    public float IdleRotationDuration;
    public float PatrollingRotationDuration;
    public float InvestigatingRotationDuration;

    public float RangedAttackRange;
    public float MeleeAttackRange;

    public float RangedAttackFireRate;
    public float RangedAttackSpread;

    public float StrafeDurationMinimum;
    public float StrafeDurationMaximun;

    public float MeleeAttackPauseDuration;
    public float MeleeAttackDuration;
    public float MeleeAttackMovementSpeed;
    public float MeleeAttackCooldown;
    
    public float InvestigatingDistance;
    public float InvestigatingDuration;

    private bool UnawareOfPlayerPosition;
    private bool Alive;

    [NonSerialized] public Vector3 DeathTargetPosition;

    private Rigidbody2D EnemyBody;

    private int[] StrafeDirection;
    private int StrafeDirectionIndex;
    private float StrafeDuration;

    private Vector2 OriginalPosition;
    private Vector3 SoundSourcePosition;

    [SerializeField] private GameObject MeleeAttackHitbox;
    private Vector2 MeleeAttackTargetPosition;

    private NavMeshAgent NavigationMechAgent;
    private GameObject PlayerObject;
    [SerializeField] private GameObject FieldOfVisionObject;

    public bool IdleRotating;
    public float[] AngleOfIdleRotationArray;
    public float[] DurationOfIdleRotationArray;
    private int CurrentIdleRotation;

    [SerializeField] private GameObject ProjectilePrefab;

    private Vector3 CurrentAngle;
    private float AngleChange;

    private int UpdateTicker;
    public int UpdateTickRate;

    public float WallCollisionCheckDistance;

    private float IdleTimer;
    private float InvestigatingTimer;
    private float RangedAttackCooldownTimer;
    private float StrafeDurationTimer;
    private float MeleeAttackPauseTimer;
    private float MeleeAttackCooldownTimer;
    private float MeleeAttackTimer;
    private float RotationTimer;
    private float RotationDuration;

    public GameObject[] PatrolPath;
    private int CurrentPatrolPoint;

    public EnemyState CurrentEnemyState;
    private EnemyState DefaultState;

    void Start()
    {
        NavigationMechAgent = GetComponent<NavMeshAgent>();
        NavigationMechAgent.updateRotation = false;
        NavigationMechAgent.updateUpAxis = false;
        NavigationMechAgent.speed = RunningSpeed;

        PlayerObject = GameObject.FindWithTag("Player");

        SetRotationDuration();
        DefaultState = CurrentEnemyState;
        OriginalPosition = transform.position;

        MeleeAttackHitbox.SetActive(false);

        UnawareOfPlayerPosition = true;
        Alive = true;

        EnemyBody = gameObject.GetComponent<Rigidbody2D>();

        StrafeDirection = new int[]
        {
            1,
            -1
        };

        IdleTimer = 0;
        InvestigatingTimer = 0;
        RangedAttackCooldownTimer = 0;
        StrafeDurationTimer = 0;
        MeleeAttackPauseTimer = MeleeAttackPauseDuration;
        MeleeAttackCooldownTimer = MeleeAttackCooldown;
        MeleeAttackTimer = 0;
        RotationTimer = 0;
        
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
        RangedAttackCooldownTimer += Time.fixedDeltaTime;
        StrafeDurationTimer += Time.fixedDeltaTime;
        MeleeAttackPauseTimer += Time.fixedDeltaTime;
        MeleeAttackCooldownTimer += Time.fixedDeltaTime;
        RotationTimer += Time.fixedDeltaTime;

        switch (CurrentEnemyState)
        {
            case EnemyState.Idling:

                IdlingUpdate();
                break;

            case EnemyState.Patrolling:

                PatrollingUpdate();
                break;
            
            case EnemyState.MovingToInvestigate:

                MovingToInvestigateUpdate();
                break;

            case EnemyState.Investigating:

                InvestigatingUpdate();
                break;

            case EnemyState.Returning:

                ReturningUpdate();
                break;

            case EnemyState.Chasing:

                ChasingUpdate();
                break;

            case EnemyState.AttackingRanged:

                AttackingRangedUpdate();
                break;

            case EnemyState.AttackingMelee:

                AttackingMeleeUpdate();
                break;

            case EnemyState.Searching:

                SearchingUpdate();
                break;

            case EnemyState.Dying:

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

        if (UpdateTicker % UpdateTickRate == 0)
        {   
            if (NavigationMechAgent.remainingDistance < InvestigatingDistance)
            {
                if (LineOfSightCheck(SoundSourcePosition) == null)
                {
                    NavigationMechAgent.ResetPath();

                    RotationTimer = 0;
                    CurrentAngle = transform.eulerAngles;

                    InvestigatingTimer = 0;
                    CurrentEnemyState = EnemyState.Investigating;
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
            else
            {
                AngleChange = +60;
            }
            
            RotationTimer = 0;
        }

        if (InvestigatingTimer > InvestigatingDuration)
        {
            NavigationMechAgent.SetDestination(OriginalPosition);

            CurrentEnemyState = EnemyState.Returning;
        }
    }

    void ReturningUpdate()
    {
        LookTowardsObject(transform.eulerAngles, NavigationMechAgent.steeringTarget, 0.05f);

        if (NavigationMechAgent.remainingDistance < 0.01)
        {   
            IdleTimer = 0;

            NavigationMechAgent.ResetPath();
            CurrentEnemyState = DefaultState;
            SetRotationDuration();
        }
    }

    void ChasingUpdate()
    {
        NavigationMechAgent.SetDestination(PlayerObject.transform.position);

        LookTowardsObject(transform.eulerAngles, NavigationMechAgent.steeringTarget, 0.05f);
        


        if (UpdateTicker % UpdateTickRate == 0)
        {
            AttackRangeCheck(Vector2.Distance(transform.position, PlayerObject.transform.position));
        }
    }

    void AttackingRangedUpdate()
    {   
        if (UpdateTicker % UpdateTickRate == 0)
        {
            AttackRangeCheck(Vector2.Distance(transform.position, PlayerObject.transform.position));
        }

        transform.position += transform.right * StrafeDirectionIndex * StrafingSpeed * Time.fixedDeltaTime;

        if (StrafeDurationTimer > StrafeDuration)
        {
            StrafeDurationTimer = 0;
            StrafeDirectionIndex = StrafeDirectionIndex * -1;
            StrafeDuration = UnityEngine.Random.Range(StrafeDurationMinimum, StrafeDurationMaximun);
        }

        if (UpdateTicker % UpdateTickRate == 0)
        {

            RaycastHit2D RaycastResult = Physics2D.Raycast(transform.position + transform.right * WallCollisionCheckDistance, (transform.position - transform.right * WallCollisionCheckDistance * 2) - transform.position, WallCollisionCheckDistance * 2);
            RaycastHit2D CircleCastResult = Physics2D.CircleCast(transform.position, 1f, PlayerObject.transform.position - transform.position, Vector2.Distance(transform.position, PlayerObject.transform.position));

            if (RaycastResult.collider != null || CircleCastResult.collider != PlayerObject.gameObject.GetComponent<Collider2D>())
            {
                StrafeDirectionIndex = StrafeDirectionIndex * -1;
                StrafeDuration = UnityEngine.Random.Range(StrafeDurationMinimum, StrafeDurationMaximun);
            }
        }

        LookTowardsObject(transform.eulerAngles, PlayerObject.transform.position, 0.05f);

        if (RangedAttackCooldownTimer > RangedAttackFireRate)
        {
            Vector3 ProjectileRotation = transform.eulerAngles;
            ProjectileRotation.z += UnityEngine.Random.Range(-RangedAttackSpread, RangedAttackSpread);
            
            Instantiate(ProjectilePrefab, transform.position, Quaternion.Euler(ProjectileRotation));

            RangedAttackCooldownTimer = 0;
        }
    }

    void AttackingMeleeUpdate()
    {
        if (UpdateTicker % UpdateTickRate == 0 && MeleeAttackCooldownTimer <= MeleeAttackCooldown)
        {
            AttackRangeCheck(Vector2.Distance(transform.position, PlayerObject.transform.position));
        }

        if (MeleeAttackPauseTimer < MeleeAttackPauseDuration)
        {
            LookTowardsObject(transform.eulerAngles, MeleeAttackTargetPosition, 0.05f);
        }
        else if (MeleeAttackPauseTimer > MeleeAttackPauseDuration)
        {
            EnemyBody.linearVelocity = (new Vector2(-transform.up.x, -transform.up.y)) * MeleeAttackMovementSpeed;

            MeleeAttackHitbox.SetActive(true);

            MeleeAttackTimer += Time.fixedDeltaTime;

            if (MeleeAttackTimer > MeleeAttackDuration)
            {
                MeleeAttackHitbox.SetActive(false);
                MeleeAttackCooldownTimer = 0;

                EnemyBody.linearVelocity = Vector2.zero;
            }
        }
    }

    void SearchingUpdate()
    {
        
    }

    void DyingUpdate()
    {
        if (Alive == true)
        {
            Debug.Log("He is dead :O");
            Alive = false;

            Vector2 StoredPosition = transform.eulerAngles;
            LookTowardsObject(transform.eulerAngles, DeathTargetPosition, 1);
            DeathTargetPosition += transform.up * 1;
            transform.eulerAngles = StoredPosition;
            gameObject.GetComponent<Collider2D>().excludeLayers = LayerMask.GetMask("Player", "Ignore Raycast", "Default");
            FieldOfVisionObject.SetActive(false);
        }

        EnemyBody.position = Vector2.Lerp(transform.position, DeathTargetPosition, 0.5f);
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
        if (CurrentEnemyState == EnemyState.Idling)
        {
            RotationDuration = IdleRotationDuration;
        }
        else if (CurrentEnemyState == EnemyState.Patrolling)
        {
            RotationDuration = PatrollingRotationDuration;
        }
        else if (CurrentEnemyState == EnemyState.Investigating)
        {
            RotationDuration = InvestigatingRotationDuration;
        }
    }

    Collider2D LineOfSightCheck(Vector3 targetobjectposition)
    {
        RaycastHit2D RaycastResult = Physics2D.Raycast(transform.position, targetobjectposition - transform.position, Vector2.Distance(transform.position, targetobjectposition));
        return RaycastResult.collider;
    }

    void AttackRangeCheck(float distance)
    {
        if (distance <= MeleeAttackRange && MeleeAttackCooldownTimer > MeleeAttackCooldown)
        {
            if (LineOfSightCheck(PlayerObject.transform.position) == PlayerObject.GetComponent<Collider2D>())
            {
                NavigationMechAgent.ResetPath();

                if (CurrentEnemyState != EnemyState.AttackingMelee)
                {
                    MeleeAttackPauseTimer = 0;
                    MeleeAttackTimer = 0;
                    MeleeAttackTargetPosition = PlayerObject.transform.position;
                }

                CurrentEnemyState = EnemyState.AttackingMelee;
            }
            else
            {
                CurrentEnemyState = EnemyState.Chasing;
            }
        }
        else if (distance <= RangedAttackRange)
        {
            if (LineOfSightCheck(PlayerObject.transform.position) == PlayerObject.GetComponent<Collider2D>())
            {
                NavigationMechAgent.ResetPath();
                
                if(CurrentEnemyState != EnemyState.AttackingRanged)
                {
                    RangedAttackCooldownTimer = 0;

                    StrafeDurationTimer = 0;
                    StrafeDirectionIndex = StrafeDirection[UnityEngine.Random.Range(0, 1)];
                    StrafeDuration = UnityEngine.Random.Range(StrafeDurationMinimum, StrafeDurationMaximun);
                }

                CurrentEnemyState = EnemyState.AttackingRanged;
            }
            else
            {
                CurrentEnemyState = EnemyState.Chasing;
            }            
        }
        else
        {
            CurrentEnemyState = EnemyState.Chasing;
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
        
        if (UpdateTicker % UpdateTickRate == 0)
        {
            if (collider.gameObject == PlayerObject && UnawareOfPlayerPosition == true)
            {
                if (LineOfSightCheck(PlayerObject.transform.position) == PlayerObject.GetComponent<Collider2D>())
                {
                    UnawareOfPlayerPosition = false;
                    NavigationMechAgent.speed = RunningSpeed;
                    FieldOfVisionObject.SetActive(false);
                    CurrentEnemyState = EnemyState.Chasing;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Sound Source"))
        {;
            if (UnawareOfPlayerPosition == true)
            {
                SoundSourcePosition = collider.gameObject.transform.position;
                NavigationMechAgent.SetDestination(SoundSourcePosition);
                
                if (CurrentEnemyState == DefaultState)
                {
                    OriginalPosition = transform.position;
                }

                CurrentEnemyState = EnemyState.MovingToInvestigate;
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject == PlayerObject)
        {
            EnemyBody.linearVelocity = Vector2.zero;
        }
    }
    public enum EnemyState
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
