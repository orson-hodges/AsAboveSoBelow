using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class NormalEnemyBehaviourScript : MonoBehaviour
{
    public float WalkingSpeed;
    public float RunningSpeed;
    public float StrafingSpeed;


    public GameObject[] PatrolPath;
    public int CurrentPartolPoint;

    [SerializeField] private State CurrentState;

    void Start()
    {
        Debug.Log(CurrentPartolPoint);
    }

    void FixedUpdate()
    {
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
        
    }

    void PatrolingUpdate()
    {
        float PatrolAngleRadis = Mathf.Atan2(PatrolPath[CurrentPartolPoint].transform.position.y - transform.position.y, PatrolPath[CurrentPartolPoint].transform.position.x - transform.position.x);
        float PatrolAngleDegree = (180 / Mathf.PI) * PatrolAngleRadis + 90;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, PatrolAngleDegree), 0.1f);

        transform.position = Vector2.MoveTowards(transform.position, PatrolPath[CurrentPartolPoint].transform.position, WalkingSpeed * Time.deltaTime);
    }

    void InvestigatingUpdate()
    {
        
    }

    void ChasingUpdate()
    {
        
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

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == PatrolPath[CurrentPartolPoint])
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
