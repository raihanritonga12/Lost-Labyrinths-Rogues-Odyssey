using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : AttackState {
	private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
	private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }

	private Movement movement;
	private CollisionSenses collisionSenses;

	protected D_MeleeAttack stateData;
	private float effect = 0;

	public MeleeAttackState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttack stateData) : base(etity, stateMachine, animBoolName, attackPosition) {
		this.stateData = stateData;
        this.effect = stateData.effect;
	}

	public override void DoChecks() {
		base.DoChecks();
	}

	public override void Enter() {
		base.Enter();
	}

	public override void Exit() {
		base.Exit();
	}

	public override void FinishAttack() {
		base.FinishAttack();
	}

	public override void LogicUpdate() {
		base.LogicUpdate();
	}

	public override void PhysicsUpdate() {
		base.PhysicsUpdate();
	}

	public override void TriggerAttack() {
		base.TriggerAttack();

		Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackPosition.position, stateData.attackRadius, stateData.whatIsPlayer);

		foreach (Collider2D collider in detectedObjects) {
			IDamageable damageable = collider.GetComponent<IDamageable>();

			if (damageable != null) {
				damageable.Damage(stateData.attackDamage);
				if (effect == 1)
				{
					BleedPlayer();
				} else if (effect == 2)
				{
					StunPlayer();
				}
			}

			IKnockbackable knockbackable = collider.GetComponent<IKnockbackable>();

			if (knockbackable != null) {
				knockbackable.Knockback(stateData.knockbackAngle, stateData.knockbackStrength, Movement.FacingDirection);
			}
		}
	}

    public void BleedPlayer()
    {
        Player player = SessionManager.player;

        float bleedAmount = 2;
        float bleedDuration = 6;

        CoroutineRunner.RunCoroutine(ApplyBleed(player, bleedAmount, bleedDuration));
    }

    private IEnumerator ApplyBleed(Player player, float bleedAmount, float bleedDuration)
    {
        float startTime = Time.time;

        while (Time.time < startTime + bleedDuration)
        {
            // Damage player with bleed amount every second for bleedDuration
            player.GetComponentInChildren<Combat>().Damage(bleedAmount);

            yield return new WaitForSeconds(1f);
        }
    }

    public void StunPlayer()
    {
        // 20% chance of trigger
        if (Random.value <= 0.33f)
        {
            Player player = SessionManager.player;

            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            Color originalColor = sr.color;
            // Change sr color to yellow
            sr.color = Color.yellow;

            float originalPlayerVelocity = player.playerData.movementVelocity;
            player.playerData.movementVelocity = 0;

            // Restore sr color and player velocity after 3 seconds
            CoroutineRunner.RunCoroutine(RestoreStun(player, sr, originalColor, originalPlayerVelocity, 1f));
        }
    }

    private IEnumerator RestoreStun(Player player, SpriteRenderer sr, Color originalColor, float originalPlayerVelocity, float duration)
    {
        yield return new WaitForSeconds(duration);

        // Restore sr color and player velocity
        sr.color = originalColor;
        player.playerData.movementVelocity = originalPlayerVelocity;
    }

}
