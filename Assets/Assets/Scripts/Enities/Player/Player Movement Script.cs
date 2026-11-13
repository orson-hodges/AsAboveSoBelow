using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMovementScript : MonoBehaviour
{
    public float MovementSpeed;
    [NonSerialized] public Rigidbody2D PlayerBody;
    protected Vector2 InputDirection;

    public float RangedAttackFireRate;
    private float RangedAttackCooldownTimer;

    private bool Firing;
    private bool Shifting;
    [NonSerialized] public bool ShiftedThisFrame;

    public GameObject PlayerProjectilePrefab;
    [NonSerialized] public GameObject PlayerCamera;

    [NonSerialized] public bool Alive;
    [NonSerialized] public Vector2 DeathTargetVector;

    public GameObject DeathScene;

    private TilemapCollider2D AboveWallCollisionTilemap;
    private TilemapRenderer AboveWallSpriteTilemap;

    public InputActionAsset InputMap;

    void Start()
    {
        PlayerBody = gameObject.GetComponent<Rigidbody2D>();
        PlayerCamera = GameObject.FindWithTag("MainCamera");
        DeathScene.SetActive(false);

        AboveWallCollisionTilemap = GameObject.FindWithTag("Above Wall Collision Tilemap").GetComponent<TilemapCollider2D>();
        AboveWallSpriteTilemap = GameObject.FindWithTag("Above Wall Sprite Tilemap").GetComponent<TilemapRenderer>();

        AboveWallCollisionTilemap.enabled = false;
        AboveWallSpriteTilemap.enabled = false;

        RangedAttackCooldownTimer = RangedAttackFireRate;

        Firing = false;
        Shifting = false;
        ShiftedThisFrame = false;

        Alive = true;
    }

    void FixedUpdate()
    {
        if (Alive == true)
        {
            PlayerBody.linearVelocity = InputDirection * MovementSpeed;
        }
        else if (Alive == false)
        {
            PlayerBody.linearVelocity = -DeathTargetVector * 3;
        }
    }

    void Update()
    {
        RangedAttackCooldownTimer += Time.deltaTime;

        if (Firing)
        {
            if (RangedAttackCooldownTimer > RangedAttackFireRate)
            {
                Instantiate(PlayerProjectilePrefab, transform.position, transform.rotation);
                RangedAttackCooldownTimer = 0;
            }
        }

        if (ShiftedThisFrame)
        {
            ShiftedThisFrame = false;
        }

        if (!Alive)
        {
            Time.timeScale = Mathf.Clamp(Time.timeScale - Time.unscaledDeltaTime, 0, 1);
        }
    }

    public void Dying()
    {
        DeathScene.SetActive(true);
        PlayerBody.linearVelocity = Vector2.zero;
        PlayerCamera.GetComponent<CameraMovementScript>().enabled = false;
        Alive = false;
        InputMap.Disable();
    }

    void OnMove(InputValue inputvalue)
    {
        // Keep track of the player's input direction.
        InputDirection = inputvalue.Get<Vector2>();
    }

    void OnRangedAttack(InputValue inputvalue)
    {
        Firing = inputvalue.Get<float>() > 0;
    }

    void OnShift(InputValue inputvalue)
    {
        Shifting = inputvalue.Get<float>() > 0;
        ShiftedThisFrame = true;

        if (Shifting)
        {
            AboveWallCollisionTilemap.enabled = true;
            AboveWallSpriteTilemap.enabled = true;
            gameObject.GetComponent<Collider2D>().excludeLayers += LayerMask.GetMask("Below Wall Collision Tilemap", "Below Enemy");
            gameObject.layer = LayerMask.NameToLayer("Player (Above)");
        }
        else if (Alive)
        {
            AboveWallCollisionTilemap.enabled = false;
            AboveWallSpriteTilemap.enabled = false;
            gameObject.GetComponent<Collider2D>().excludeLayers -= LayerMask.GetMask("Below Wall Collision Tilemap", "Below Enemy");
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

        Collider2D[] OverlapCircleHits = Physics2D.OverlapCircleAll(transform.position, 0.4f, 1408);

        foreach (Collider2D OverlapCircleHit in OverlapCircleHits)
        {
            if (OverlapCircleHit != null)
            {
                DeathTargetVector = Vector2.zero;
                PlayerBody.GetComponent<Collider2D>().enabled = false;
                Dying();
                break;
            }
        }
    }
}
