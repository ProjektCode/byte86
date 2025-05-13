using System.Collections;
using UnityEngine;

public class ShieldHandler : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;
    private Coroutine shieldRoutine;
    private bool shieldActive = false;
    private int remainingHits;

    private PlayerStats stats;
    private Color flashColor = Color.cyan;
    private Material flashMat;
    private Material orgMat;

    [SerializeField] private float flashDuration = 0.1f;


    void Awake() {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        stats = sr.GetComponent<PlayerStats>();
        flashMat = stats.DamageMaterial;
        orgMat = sr.material;
    }

    public void EnableShield(float duration, int maxHits, Color tint) {
        if (shieldRoutine != null)
            StopCoroutine(shieldRoutine);

        remainingHits = maxHits;
        shieldActive = true;
        sr.color = tint;
        shieldRoutine = StartCoroutine(ShieldDuration(duration));
    }

    private IEnumerator ShieldDuration(float duration) {
        yield return new WaitForSeconds(duration);
        DisableShield(); // Ends due to duration
    }

    public void ConsumeHit() {
        if (!shieldActive) return;

        StartCoroutine(FlashShield());

        remainingHits--;
        if (remainingHits <= 0)
        {
            DisableShield(); // Ends due to hit count
        }
    }

    public void ForceDisableShield() {
        if (shieldRoutine != null)
            StopCoroutine(shieldRoutine);
        DisableShield();
    }

    private void DisableShield() {
        shieldActive = false;
        StartCoroutine(FadeToOriginal());
    }

    private IEnumerator FadeToOriginal() {
        float t = 0f;
        Color startColor = sr.color;
        while (t < 1f)
        {
            t += Time.deltaTime;
            sr.color = Color.Lerp(startColor, originalColor, t);
            yield return null;
        }
    }

    private IEnumerator FlashShield() {

        sr.material = flashMat;
        sr.material.SetColor("_TextColor", flashColor);

        yield return new WaitForSeconds(flashDuration);
        sr.material = orgMat;

    }

    public bool IsShieldActive() => shieldActive;
}
