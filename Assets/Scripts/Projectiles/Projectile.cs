﻿using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //private AttackDetails attackDetails;

    private float speed;
    private float travelDistance;
    private float xStartPos;

    [SerializeField]
    private float gravity;
    [SerializeField]
    private float damageRadius;

    private Rigidbody2D rb;

    private bool isGravityOn;
    private bool hasHitGround;
    public float damage = 15f;
    public float knockbackAmount = 3f;

    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private LayerMask whatIsPlayer;
    [SerializeField]
    private Transform damagePosition;

    public int effect = 0;

    private Player player;
    private SpriteRenderer playerSpriteRenderer;
    private Color originalColor;

    private float freezeDuration = 5f; // Duration to freeze the player in seconds
    private float freezeEndTime; // Time when freezing ends
    private bool isFreezing = false;
    private float burnDuration = 5f;
    private float burnDamage = 3f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0.0f;
        rb.velocity = transform.right * speed;

        isGravityOn = false;

        xStartPos = transform.position.x;
    }

    private void Update()
    {
        if (!hasHitGround)
        {
            //attackDetails.position = transform.position;

            if (isGravityOn)
            {
                float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!hasHitGround)
        {
            Collider2D damageHit = Physics2D.OverlapCircle(damagePosition.position, damageRadius, whatIsPlayer);
            Collider2D groundHit = Physics2D.OverlapCircle(damagePosition.position, damageRadius, whatIsGround);

            if (damageHit)
            {
                //damageHit.transform.SendMessage("Damage", attackDetails);
                GameObject hitObject = damageHit.gameObject;
                hitObject.GetComponentInChildren<Combat>().Damage(damage);
                // Get the direction based on arrow's rotation or facing direction
                int angle = (int)transform.rotation.y;
                if (angle == 0) angle++;
        
                hitObject.GetComponentInChildren<Combat>().Knockback(new Vector2(1, 1), knockbackAmount, angle);
                if (effect == 1)
                {
                    FreezePlayer();
                }
                else if (effect == 2)
                {
                    BurnPlayer();
                }
                Destroy(gameObject);
            }

            if (groundHit)
            {
                hasHitGround = true;
                rb.gravityScale = 0f;
                rb.velocity = Vector2.zero;
            }


            if (Mathf.Abs(xStartPos - transform.position.x) >= travelDistance && !isGravityOn)
            {
                isGravityOn = true;
                rb.gravityScale = gravity;
            }
        }        
    }

    public void FireProjectile(float speed, float travelDistance, float damage)
    {
        this.speed = speed;
        this.travelDistance = travelDistance;
        //attackDetails.damageAmount = damage;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(damagePosition.position, damageRadius);
    }

    public void FreezePlayer()
    {
        if (isFreezing) // If already freezing, do nothing
            return;

        player = SessionManager.player;
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();

        // Store original color of the player's sprite
        originalColor = playerSpriteRenderer.color;

        // Change sprite color to be bluish
        playerSpriteRenderer.color = Color.blue;

        // Reduce player movement velocity
        player.playerData.movementVelocity -= player.playerData.baseMovementVelocity * 0.4f;

        // Set the time when freezing will end
        freezeEndTime = Time.time + freezeDuration;

        // Set flag to indicate player is freezing
        isFreezing = true;

        // Invoke method to restore player movement velocity and color after freezeDuration
        Invoke(nameof(RestorePlayer), freezeDuration);
    }

    private void RestorePlayer()
    {
        if (!isFreezing) // If not freezing, do nothing
            return;

        // Restore player movement velocity
        player.playerData.movementVelocity += player.playerData.baseMovementVelocity * 0.4f;

        // Restore player sprite color to original color
        playerSpriteRenderer.color = originalColor;

        // Reset flag indicating player is freezing
        isFreezing = false;
    }

    public void BurnPlayer()
    {
        if (player == null) // Ensure player reference is valid
            return;

        if (isFreezing) // If freezing, cancel freezing
        {
            CancelInvoke(nameof(RestorePlayer));
            RestorePlayer();
        }

        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();

        // Store original color of the player's sprite
        originalColor = playerSpriteRenderer.color;

        // Change sprite color to be reddish
        playerSpriteRenderer.color = Color.red;

        // Start burning effect
        StartCoroutine(BurnEffect());
    }

    private IEnumerator BurnEffect()
    {
        Stats playerStats = player.GetComponentInChildren<Stats>();
        float startTime = Time.time;

        while (Time.time < startTime + burnDuration)
        {
            // Reduce player health every second
            player.GetComponentInChildren<Combat>().Damage(burnDamage * Time.deltaTime);

            yield return null;
        }

        // Restore player sprite color to original color
        playerSpriteRenderer.color = originalColor;
    }
}
