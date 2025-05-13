using System.Collections;
using MoreMountains.Tools;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {

    [Header("Settings")]
    public GameObject bulletPrefab;
    public Transform[] FirePoints;

    [Header("Stats")]
    public float FireRate = 0.3f;
    public float ShotDelay = 0.1f;

    [Header("Scattershot")]
    public bool scattershotActive = false;
    public int scatterCount = 1; // Total bullets per fire point (1 = normal, 3 = scattershot)
    public float scatterAngle = 15f; // degrees between each bullet in the spread


    [Header("Sound Settings")]
    public AudioClip shootSFX;
    [Range(0.5f, 1.1f)] public float minPitch = 0.9f;
    [Range(0.5f, 1.1f)] public float maxPitch = 1f;

    private float nextFire = 0f;

    private PlayerController playerController;
    private AudioSource fireSource;
    private PlayerStats stats;

    private bool isPiercingActive = false;
    private bool isExplosiveActive = false;
    private bool isVampiricActive = false;
    private float explosiveRadius = 0f;
    private LayerMask enemyLayer;

    private GameObject bullet;

    void Awake() {
     playerController = GetComponent<PlayerController>();
     fireSource = GetComponent<AudioSource>(); 
     stats = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update() {
        if(playerController.isDead) return;

        if(Input.GetKey(KeyCode.Space) && Time.time >= nextFire){

            Shoot();
            nextFire = Time.time + FireRate;

        }
        
    }

    void Shoot(){

        StartCoroutine(ShootWithDelay());

    }

    IEnumerator ShootWithDelay() {
        foreach (Transform point in FirePoints) {
            if (scattershotActive && scatterCount > 1) {
                float startAngle = -scatterAngle * (scatterCount - 1) / 2f;
                for (int i = 0; i < scatterCount; i++) {
                    float angle = startAngle + i * scatterAngle;
                    Quaternion rotation = Quaternion.Euler(0, 0, angle);
                 bullet = Instantiate(bulletPrefab, point.position, rotation);
                }
            } else {
              bullet = Instantiate(bulletPrefab, point.position, Quaternion.identity);
            }
             PlayerBullet p_bullet = bullet.GetComponent<PlayerBullet>();
             p_bullet.SetPiercing(isPiercingActive);
             p_bullet.SetExplosive(isExplosiveActive, explosiveRadius, enemyLayer);
             p_bullet.SetVampiric(isVampiricActive);
             p_bullet.GetStats(stats);
            // ✅ Play sound once per fire point, regardless of scatter count
            if (fireSource != null && shootSFX != null) {
                fireSource.pitch = Random.Range(minPitch, maxPitch);
                fireSource.PlayOneShot(shootSFX);
            }

            yield return new WaitForSeconds(ShotDelay);
        }
    }

    public void SetExplosiveBulletsActive(bool active, float radius = 0f, LayerMask layer = default) {
        isExplosiveActive = active;
        explosiveRadius = radius;
        enemyLayer = layer;
    }

    public void EnablePiercing(bool active){
        isPiercingActive = active;
    }

    public void EnableVampiricRounds(bool active){
        isVampiricActive = active;
    }


}