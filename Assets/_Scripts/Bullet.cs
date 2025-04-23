using UnityEngine;

public class Bullet : MonoBehaviour {


    public float speed = 10f;
    public int damage = 1;
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

    void OnBecameInvisible() => Destroy(gameObject); //Destroys bullet when out of camera view

}
