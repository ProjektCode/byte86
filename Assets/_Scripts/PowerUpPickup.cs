using UnityEngine;


[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class PowerUpPickup : MonoBehaviour {

    public PowerupData Powerup;

    // Update is called once per frame
    void Update() {
        transform.position += Vector3.down * Time.deltaTime;    
    }

    void OnTriggerEnter2D(Collider2D col) {

        if(col.CompareTag("Player")) {
            col.GetComponent<PlayerPowerupHandler>().ApplyPowerup(Powerup);
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible() {
        Destroy(gameObject);
    }

}
