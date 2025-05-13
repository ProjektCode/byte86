using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class PowerUpPickup : MonoBehaviour {

    public PowerupData Powerup;

    private Tween PulseTween;

    void Start() {
        PulseTween = transform.DOScale(0.45f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    // Update is called once per frame
    void Update() {
        transform.position += Vector3.down * Time.deltaTime;    
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("Player")) {
            if (col.TryGetComponent<PlayerPowerupHandler>(out var handler)) {
                handler.ApplyPowerup(Powerup);
            }

            CleanupAndDestroy();
        }
    }

    void OnBecameInvisible() {
        CleanupAndDestroy();
    }

    private void CleanupAndDestroy(){
        PulseTween?.Kill();
        Destroy(gameObject);
    }

}
