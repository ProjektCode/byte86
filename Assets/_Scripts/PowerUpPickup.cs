using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class PowerUpPickup : MonoBehaviour {

    public PowerupData Powerup;

    void Start() {
        transform.DOScale(0.45f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    // Update is called once per frame
    void Update() {
        transform.position += Vector3.down * Time.deltaTime;    
    }

    void OnTriggerEnter2D(Collider2D col) {

        if(col.CompareTag("Player")) {
            PlayerPowerupHandler handler = col.GetComponent<PlayerPowerupHandler>();

            //if(handler != null && handler.CheckPowerUp(Powerup)) return;

            handler.ApplyPowerup(Powerup);
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible() {
        Destroy(gameObject);
    }

}
