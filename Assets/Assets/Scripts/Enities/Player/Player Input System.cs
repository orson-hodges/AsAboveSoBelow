using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerInputSystem : MonoBehaviour
{
    public float MovementSpeed;
    [NonSerialized] public Rigidbody2D PlayerBody;
    protected Vector2 InputDirection;

    public float RangedAttackFireRate;
    private float RangedAttackCooldownTimer;

    private bool RangedAttacking;
    private bool MeleeAttacking;
    private bool Shifting;
    [NonSerialized] public bool ShiftedThisFrame;

    public float MeleeAttackCooldown;
    private float MeleeAttackCooldownTimer;
    public float MeleeAttackDuration;
    private float MeleeAttackDurationTimer;
    public GameObject MeleeAttackHitbox;

    public GameObject PlayerProjectilePrefab;
    [NonSerialized] public GameObject PlayerCamera;

    [NonSerialized] public bool Alive;
    [NonSerialized] public Vector2 DeathTargetVector;

    public GameObject DeathScene;

    private TilemapRenderer AboveWallSpriteTilemap;

    public InputActionAsset InputMap;

    void Start()
    {
        PlayerBody = gameObject.GetComponent<Rigidbody2D>();
        PlayerCamera = GameObject.FindWithTag("MainCamera");
        DeathScene.SetActive(false);

        AboveWallSpriteTilemap = GameObject.FindWithTag("Above Wall Sprite Tilemap").GetComponent<TilemapRenderer>();

        AboveWallSpriteTilemap.enabled = false;

        MeleeAttackHitbox.SetActive(false);

        RangedAttackCooldownTimer = RangedAttackFireRate;
        MeleeAttackCooldownTimer = MeleeAttackCooldown;
        MeleeAttackDurationTimer = MeleeAttackDuration;

        RangedAttacking = false;
        MeleeAttacking = false;
        Shifting = false;
        ShiftedThisFrame = false;

        Alive = true;
    }

    void FixedUpdate()
    {
        if (Alive == true)
        {
            PlayerBody.linearVelocity = InputDirection * MovementSpeed;

            float CameraAngleRadii = Mathf.Atan2(PlayerCamera.transform.position.y - transform.position.y, PlayerCamera.transform.position.x - transform.position.x);
            float CameraAngleDegrees = (180 / Mathf.PI) * CameraAngleRadii + 90;
            transform.eulerAngles = new Vector3(0, 0, CameraAngleDegrees);
        }
        else if (Alive == false)
        {
            PlayerBody.linearVelocity = -DeathTargetVector * 3;
        }
    }

    void Update()
    {
        RangedAttackCooldownTimer += Time.deltaTime;
        MeleeAttackCooldownTimer += Time.deltaTime;

        if (RangedAttacking)
        {
            if (RangedAttackCooldownTimer > RangedAttackFireRate)
            {
                Instantiate(PlayerProjectilePrefab, transform.position, transform.rotation);
                RangedAttackCooldownTimer = 0;
            }
        }

        if (MeleeAttacking)
        {
            MeleeAttackDurationTimer += Time.deltaTime;
            
            if (MeleeAttackDurationTimer > MeleeAttackDuration)
            {
                MeleeAttackHitbox.SetActive(false);
                MeleeAttacking = false;
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
        PlayerCamera.GetComponent<CameraMovement>().enabled = false;
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
        RangedAttacking = inputvalue.Get<float>() > 0;
    }

    void OnMeleeAttack()
    {
        if (MeleeAttackCooldownTimer > MeleeAttackCooldown)
        {
            MeleeAttackHitbox.SetActive(true);
            MeleeAttackCooldownTimer = 0;
            MeleeAttackDurationTimer = 0;
            MeleeAttacking = true;
        }
    }

    void OnShift(InputValue inputvalue)
    {
        Shifting = inputvalue.Get<float>() > 0;
        ShiftedThisFrame = true;

        if (Shifting)
        {
            AboveWallSpriteTilemap.enabled = true;
            gameObject.GetComponent<Collider2D>().excludeLayers += LayerMask.GetMask("Below Wall Collision Tilemap", "Below Enemy");
            gameObject.GetComponent<Collider2D>().excludeLayers -= LayerMask.GetMask("Above Wall Collision Tilemap", "Above Enemy");
            gameObject.layer = LayerMask.NameToLayer("Player (Above)");
        }
        else if (Alive)
        {
            AboveWallSpriteTilemap.enabled = false;
            gameObject.GetComponent<Collider2D>().excludeLayers -= LayerMask.GetMask("Below Wall Collision Tilemap", "Below Enemy");
            gameObject.GetComponent<Collider2D>().excludeLayers += LayerMask.GetMask("Above Wall Collision Tilemap", "Above Enemy");
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
