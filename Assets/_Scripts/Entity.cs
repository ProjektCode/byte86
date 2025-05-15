using System;
using System.Collections;
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
    public Material CritDamageMaterial;
    public float duration = 0.05f;


    [NonSerialized] public Animator animator;
    [NonSerialized] public Rigidbody2D rb;
    [NonSerialized] public AudioSource audioSource;
    [NonSerialized] public SpriteRenderer sr;
    [NonSerialized] public Material orgMat;
    [NonSerialized] public bool isCrit = false;

    protected virtual void Awake() {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        currentHealth = MaxHealth;

        if (sr != null) orgMat = sr.material;
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
        if (isCrit) {
            StartCoroutine(DamageFlash(CritDamageMaterial));
            isCrit = false;
        } else {
            StartCoroutine(DamageFlash(DamageMaterial));
        }
    }

    public virtual void Die() {
        
        if (audioSource == null) {
            Debug.Log("no audio source detected on: " + gameObject.name);
            animator.SetBool("isDead", true);
            return;
        }
        audioSource.PlayOneShot(DeathSFX);
        animator.SetBool("isDead", true);
    }

    public virtual void OnAnimationComplete(){
        Destroy(gameObject);
    }

    public virtual void Heal(float amount) {
        currentHealth = Mathf.Min(currentHealth + amount, MaxHealth);
        if(currentHealth != MaxHealth) healthBar.Change(amount);
    }

    public float GetHealthPercent() {
        return currentHealth / MaxHealth;
    }

    private IEnumerator DamageFlash(Material mat){
        if(sr == null || DamageMaterial == null || CritDamageMaterial == null) yield break;

        sr.material = mat;
        yield return new WaitForSeconds(duration);
        sr.material = orgMat;
    }

    public float GetCurrentHealth() => currentHealth;

    public float SetCurrentHealth(float amount){
        return currentHealth = Mathf.RoundToInt(amount);
    }
}
