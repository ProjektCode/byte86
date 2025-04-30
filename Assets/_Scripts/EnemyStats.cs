using System;
using UnityEngine;

public class EnemyStats : Entity {
    // Now has access to health, speed, TakeDamage, Die, etc.

    [Header("Enemy Stats")]
    public int MinHealth = 3;

    public float MinSpeed = 1.5f;
    public int score = 10;

    public Bar healthBar;

    [NonSerialized] public bool canMove = true;

    private new Collider2D collider2D;

    protected override void Awake() {
        // Optional: Randomize stats
        Health = UnityEngine.Random.Range(MinHealth, Health);
        moveSpeed = UnityEngine.Random.Range(MinSpeed, moveSpeed);
        currentHealth = Health;
        healthBar.MaxValue = currentHealth;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();

        Debug.Log(gameObject.name + ": " + currentHealth + " | " + moveSpeed);
    }

    protected override void OnDamageTaken() {
        base.OnDamageTaken();
        // Add enemy-specific reactions like flashing red or playing a sound
    }

    public override void Die() {
        collider2D.enabled = false;
        GameManager.Instance.UpdateScore(score);
        canMove = false;
        healthBar.enabled = false;
        base.Die();
        
    }

    public override void TakeDamage(int amount) {
        currentHealth -= amount;
        healthBar.Change(-amount);
        OnDamageTaken();

        if (currentHealth <= 0f) {
            Die();
            healthBar.Change(0);
        }
    }
}
