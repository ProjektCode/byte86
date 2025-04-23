using System.Collections;
using UnityEngine;

public class ShootingPoint : MonoBehaviour {

    public GameObject bulletPrefab;
    public Transform[] FirePoints;
    public float fireRate = 0.3f;

    public AudioSource fireSource;
    public AudioClip[] fireSounds;

    private float nextFire = 0f;
    private PlayerController playerController;

    void Awake() {
     playerController = GetComponent<PlayerController>();   
    }

    // Update is called once per frame
    void Update() {
        if(playerController.isDead) return;

        if(Input.GetKey(KeyCode.Space) && Time.time >= nextFire){

            Shoot();
            nextFire = Time.time + fireRate;

        }
        
    }

    void Shoot(){

        StartCoroutine(ShootWithDelay());

    }

    IEnumerator ShootWithDelay(){

        foreach(Transform point in FirePoints){
            Instantiate(bulletPrefab, point.position, Quaternion.identity);
                    //Play shoot sound
            if(fireSource != null && fireSounds.Length > 0){
                
                int rand = Random.Range(0, fireSounds.Length);
                fireSource.PlayOneShot(fireSounds[rand]);

            }
            yield return new WaitForSeconds(0.1f);
        }

    }


}