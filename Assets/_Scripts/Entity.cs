using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public abstract class Entity : MonoBehaviour {
    [Header("Stats")]
    public float MaxHealth = 10;
    public float moveSpeed = 3f;
    public Bar healthBar;
    protected float currentHealth;

    [Header("Sound Settings")] 
    public AudioClip DeathSFX;

    [Header("Damage Flash Settings")]
    public Material DamageMaterial;
    public float duration = 0.05f;


    [NonSerialized] public Animator animator;
    [NonSerialized] public Rigidbody2D rb;
    [NonSerialized] public AudioSource audioSource;
    [NonSerialized] public SpriteRenderer sr;
    [NonSerialized] public Material orgMat;

    protected virtual void Awake() {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        currentHealth = MaxHealth;

        if(sr != null) orgMat = sr.material;

    }

    public virtual void TakeDamage(float amount) {
        currentHealth -= amount;
        OnDamageTaken();

        if (currentHealth <= 0f) {
            Die();
        }
    }

    protected virtual void OnDamageTaken() {
        // Play VFX, sound, flash sprite, etc.
        StartCoroutine(DamageFlash());
    }

    public virtual void Die() {
        if(audioSource == null){
            Debug.Log("no audio source detected on: " + gameObject.name);
            return;
        }
        audioSource.PlayOneShot(DeathSFX);
        animator.SetBool("isDead", true);
        
    }

    public void OnAnimationComplete(){
        Destroy(gameObject);
    }

    public virtual void Heal(float amount) {
        currentHealth = Mathf.Min(currentHealth + amount, MaxHealth);
        if(currentHealth != MaxHealth) healthBar.Change(amount);
    }

    public float GetHealthPercent() {
        return currentHealth / MaxHealth;
    }

    private IEnumerator DamageFlash(){
        

        if(sr == null || DamageMaterial == null) yield break;

        sr.material = DamageMaterial;
        yield return new WaitForSeconds(duration);
        sr.material = orgMat;

    }

    public float GetCurrentHealth() => currentHealth;

    public float SetCurrentHealth(float amount){
        return currentHealth = Mathf.RoundToInt(amount);
    }
}
