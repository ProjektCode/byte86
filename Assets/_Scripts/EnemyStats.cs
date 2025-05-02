using System;
using UnityEngine;

public class EnemyStats : Entity {
    public int score = 10;

    [NonSerialized] public bool canMove = true;

    private new Collider2D collider2D;
    private PowerupDropper powerupDropper;

    protected override void Awake() {
        currentHealth = MaxHealth;
        healthBar.MaxValue = Mathf.RoundToInt(currentHealth);

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        audioSource  = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
        powerupDropper = GetComponent<PowerupDropper>();

        if(sr != null) orgMat = sr.material;
    }

    protected override void OnDamageTaken() {
        base.OnDamageTaken();
        // Add enemy-specific reactions like flashing red or playing a sound
    }

    public override void Die() {
        collider2D.enabled = false;
        healthBar.enabled = false;
        GameManager.Instance.UpdateScore(score);
        canMove = false;
        powerupDropper.TryDropPowerup();
        base.Die();
        
    }

    public override void TakeDamage(float amount) {
        currentHealth -= amount;
        healthBar.Change(-amount);
        OnDamageTaken();

        if (currentHealth <= 0f) {
            Die();
            healthBar.Change(0);
        }
    }
}
