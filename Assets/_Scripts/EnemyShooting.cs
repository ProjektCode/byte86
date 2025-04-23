using System.Collections;
using UnityEngine;

public class EnemyShooting : MonoBehaviour {
    public GameObject bulletPrefab;
    public Transform[] firePoint;
    public float fireRate = 1.3f;
    public bool RotBullet = false;

    public AudioSource fireSource;
    public AudioClip[] fireSounds;

    private float nextFireTime;

    private Renderer enemyRenderer;

    void Awake() {
        // In case the visual part is on a child (like a sprite)
        enemyRenderer = GetComponentInChildren<Renderer>();
    }

    public void TryShoot() {
        // Only shoot if visible and cooldown has passed
        if (IsVisibleOnScreen() && Time.time >= nextFireTime) {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot() {
        StartCoroutine(ShootWithDelay());
    }

    IEnumerator ShootWithDelay() {
        foreach (Transform point in firePoint) {
            Quaternion rot = RotBullet ? Quaternion.Euler(0, 0, 180) : Quaternion.identity;
            Instantiate(bulletPrefab, point.position, rot);

            // Play shoot sound
            if (fireSource != null && fireSounds.Length > 0) {
                int rand = Random.Range(0, fireSounds.Length);
                fireSource.PlayOneShot(fireSounds[rand]);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    // Check if enemy is inside the camera view
    bool IsVisibleOnScreen() {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        return viewPos.x >= 0 && viewPos.x <= 1 &&
            viewPos.y >= 0 && viewPos.y <= 1 &&
            viewPos.z > 0; // Must be in front of the camera
    }

}
