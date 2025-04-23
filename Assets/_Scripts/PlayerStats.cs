using System;
using UnityEngine;

public class PlayerStats : Entity {

    public int MaxHealth = 9;

    public float MaxSpeed = 5;

    public Bar healthBar;



    protected override void Awake() {
        Health = MaxHealth;
        moveSpeed = MaxSpeed;
        currentHealth = Health;
        healthBar.MaxValue = currentHealth;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        Debug.Log(gameObject.name + ": " + currentHealth + " | " + moveSpeed);
    }

    protected override void OnDamageTaken() {
        base.OnDamageTaken();
        // Add enemy-specific reactions like flashing red or playing a sound
    }

    public override void Die() {
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
