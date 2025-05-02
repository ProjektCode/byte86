using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour {


    public float speed = 10f;
    public float damage = 1;
    public Direction direction = Direction.Up;
    public Target target = Target.Enemy;

    public enum Direction{
        Up,
        Down
    }

    public enum Target{
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

    //Figure out why OnBecomeInvisible() doesn't work
    void Start() {
        Destroy(gameObject, 3f);

        if(boostedActive){
            isBoosted = true;
            StartCoroutine(FlashRed());
        }
    }

    // Update is called once per frame
    void Update() {
        switch(direction){
            case Direction.Up:
                transform.position += speed * Time.deltaTime * Vector3.up; //Move upward
                break;
            case Direction.Down :
                transform.position += speed * Time.deltaTime * Vector3.down; //Move upward
                break;
            default:
                transform.position += speed * Time.deltaTime * Vector3.up; //Move upward
                break;

        }

    }

    void OnTriggerEnter2D(Collider2D col) {

        if(col.CompareTag("Bullet")){
            Debug.Log("Bullet hit");
            Destroy(col.gameObject);
            Destroy(gameObject);
            return;
        }

        Entity entity = col.GetComponent<Entity>();
        
        switch(target){
            case Target.Player:
                //Destroy bullet when it leaves screen or hits something
                if(col.CompareTag("Player")){
                    entity.TakeDamage(damage);
                    Destroy(gameObject);
                }
                break;
            case Target.Enemy:
                //Destroy bullet when it leaves screen or hits something
                if(col.CompareTag("Enemy")){
                    entity.TakeDamage(damage);
                    Destroy(gameObject);
                }
                break;
        }

    }

    public void SetBoosted(bool boosted){
        isBoosted = boosted;

        if(isBoosted == true && FlashRoutine == null) FlashRoutine = StartCoroutine(FlashRed());

    }

    private IEnumerator FlashRed() {
        float flashDuration = 0.2f;
        Color flashColor = Color.red;

        while (isBoosted) {
            Debug.Log("Boosted Bullets");
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

    void OnBecameInvisible() => Destroy(gameObject); //Destroys bullet when out of camera view

}
