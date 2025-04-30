using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public abstract class Entity : MonoBehaviour {
    [Header("Stats")]
     public int Health = 10;
    public float moveSpeed = 3f;
    protected int currentHealth;

    [NonSerialized]
    public Animator animator;
    [NonSerialized]
    public Rigidbody2D rb;

    protected virtual void Awake() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = Health;
    }

    public virtual void TakeDamage(int amount) {
        currentHealth -= amount;
        OnDamageTaken();

        if (currentHealth <= 0f) {
            Die();
        }
    }

    protected virtual void OnDamageTaken() {
        // Play VFX, sound, flash sprite, etc.
    }

    public virtual void Die() {
        animator.SetBool("isDead", true);
        //Destroy(gameObject);
        
    }

    public void OnAnimationComplete(){
        Destroy(gameObject);
    }

    public virtual void Heal(int amount) {
        currentHealth = Mathf.Min(currentHealth + amount, Health);
    }

    public float GetHealthPercent() {
        return currentHealth / Health;
    }
}
