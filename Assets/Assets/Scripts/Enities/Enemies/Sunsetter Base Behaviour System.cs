using System;
using UnityEngine;
using UnityEngine.AI;

public class SunsetterBaseBehaviourSystem : MonoBehaviour
{
    [NonSerialized] public NavMeshAgent NavigationMechAgent;

    [NonSerialized] public Rigidbody2D SunsetterBody;

    private float UpdateTickRateTimer;
    public float UpdateTickRate;

    [NonSerialized] public Vector2 HomePosition;
    public float MaximunDistanceFromHomePosition;

    private GameObject PlayerObject;
    
    [NonSerialized] public States CurrentState;

    void Start()
    {
        NavigationMechAgent = GetComponent<NavMeshAgent>();
        NavigationMechAgent.updateRotation = false;
        NavigationMechAgent.updateUpAxis = false;

        SunsetterBody = gameObject.GetComponent<Rigidbody2D>();

        PlayerObject = GameObject.FindWithTag("Player");

        HomePosition = transform.position;

        CurrentState = States.Wondering;
    }

    void FixedUpdate()
    {
        UpdateTickRateTimer += Time.fixedDeltaTime;

        switch (CurrentState)
        {
            case States.Wondering:

            WonderingUpdate();
            break;

            case States.Chasing:

            ChasingUpdate();
            break;
        }
    }

    void WonderingUpdate()
    {
        SunsetterBody.linearVelocityX += UnityEngine.Random.Range(-0.1f, 0.1f);
        SunsetterBody.linearVelocityY += UnityEngine.Random.Range(-0.1f, 0.1f);

        float DistanceFromHomePosition = Vector2.Distance(transform.position, HomePosition);

        if (transform.position.x > HomePosition.x)
        {
            SunsetterBody.linearVelocityX -= DistanceFromHomePosition/MaximunDistanceFromHomePosition;
        }
        else if (transform.position.x < HomePosition.x)
        {
            SunsetterBody.linearVelocityX += DistanceFromHomePosition/MaximunDistanceFromHomePosition;
        }

        if (transform.position.y > HomePosition.y)
        {
            SunsetterBody.linearVelocityY -= DistanceFromHomePosition/MaximunDistanceFromHomePosition;
        }
        else if (transform.position.y < HomePosition.y)
        {
            SunsetterBody.linearVelocityY += DistanceFromHomePosition/MaximunDistanceFromHomePosition;
        }

        SunsetterBody.linearVelocityX = Mathf.Clamp(SunsetterBody.linearVelocityX, -1, 1);
        SunsetterBody.linearVelocityY = Mathf.Clamp(SunsetterBody.linearVelocityY, -1, 1);

        if (UpdateTickRateTimer > UpdateTickRate)
        {
            if (SunsetterBody.linearVelocityX < 0)
            {
                if(WallCollisionCheck(new Vector2(-1, 0)) != null)
                {
                    SunsetterBody.linearVelocityX = 0;
                }
            }
            else
            {
                if(WallCollisionCheck(new Vector2(1, 0)) != null)
                {
                    SunsetterBody.linearVelocityX = 0;
                }
            }

            if (SunsetterBody.linearVelocityY < 0)
            {
                if(WallCollisionCheck(new Vector2(0, -1)) != null)
                {
                    SunsetterBody.linearVelocityY = 0;
                }
            }
            else
            {
                if(WallCollisionCheck(new Vector2(0, 1)) != null)
                {
                    SunsetterBody.linearVelocityY = 0;
                }
            }

            UpdateTickRateTimer = 0;
        }
    }

    void ChasingUpdate()
    {
        NavigationMechAgent.SetDestination(PlayerObject.transform.position);
    }

    Collider2D WallCollisionCheck(Vector3 direction)
    {
        RaycastHit2D RaycastResult = Physics2D.Raycast(transform.position, direction, Vector2.Distance(transform.position, direction), 128);
        return RaycastResult.collider;
    }

    public enum States
    {
        Wondering,
        Chasing
    }
}
