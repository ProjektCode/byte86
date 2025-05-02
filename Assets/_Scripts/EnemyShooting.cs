using System.Collections;
using UnityEngine;

public class EnemyShooting : MonoBehaviour {
    public GameObject bulletPrefab;
    public Transform[] FirePoints;
    public float fireRate = 1.3f;
    public bool RotBullet = false;

    [Header("Sound Settings")]
    public AudioClip shootSFX;
    [Range(0.9f, 1.1f)] public float minPitch = 0.95f;
    [Range(0.9f, 1.1f)] public float maxPitch = 1.05f;

    private float nextFireTime;

    private AudioSource fireSource;

    void Awake() {
        fireSource = GetComponent<AudioSource>();
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
        foreach (Transform point in FirePoints) {
            Quaternion rot = RotBullet ? Quaternion.Euler(0, 0, 180) : Quaternion.identity;
            Instantiate(bulletPrefab, point.position, rot);

            // Play shoot sound
            if (fireSource != null) {
                fireSource.pitch = Random.Range(minPitch, maxPitch);
                fireSource.PlayOneShot(shootSFX);
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
