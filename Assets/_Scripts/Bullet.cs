using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float speed = 10f;
    public float damage = 1;
    public Target target = Target.Enemy;

    public enum Target {
        Player,
        Enemy
    }

    [NonSerialized] public static bool boostedActive = false;

    private SpriteRenderer sr;
    private Color orgColor;
    private bool isBoosted = false;
    private Coroutine FlashRoutine;

    void Awake() {
        sr = GetComponent<SpriteRenderer>();
        orgColor = sr.color;
    }

    void Start() {
        Destroy(gameObject, 3f);

        // Only apply boosted effect for player bullets
        if (target == Target.Enemy && boostedActive) {
            SetBoosted(true);
        }
    }

    void Update() {
        MoveForward();
    }

    private void MoveForward() {
        transform.position += speed * Time.deltaTime * transform.up;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("Bullet")) {
            Destroy(col.gameObject);
            Destroy(gameObject);
            return;
        }

        Entity entity = col.GetComponent<Entity>();
        if (entity == null) return;

        if ((target == Target.Player && col.CompareTag("Player")) ||
            (target == Target.Enemy && col.CompareTag("Enemy"))) {
            entity.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    public void SetBoosted(bool boosted) {
        isBoosted = boosted;

        if (isBoosted && FlashRoutine == null) {
            FlashRoutine = StartCoroutine(FlashRed());
        }
    }

    private IEnumerator FlashRed() {
        float flashDuration = 0.2f;
        Color flashColor = Color.red;

        while (isBoosted) {
            // Fade to red
            float t = 0f;
            while (t < 1f) {
                t += Time.deltaTime / flashDuration;
                sr.color = Color.Lerp(orgColor, flashColor, t);
                yield return null;
            }

            // Fade back to original
            t = 0f;
            while (t < 1f) {
                t += Time.deltaTime / flashDuration;
                sr.color = Color.Lerp(flashColor, orgColor, t);
                yield return null;
            }
        }

        sr.color = orgColor;
    }

    void OnBecameInvisible() => Destroy(gameObject);
}