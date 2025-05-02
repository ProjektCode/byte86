using UnityEngine;

public class Chaser : MonoBehaviour {

    [Header("Chaser Stats")]
    public float chaseSpeed = 5f;
    public float chaseTriggerDistance = 5f;
    public int damage = 5;

    private Transform player;
    private bool isChasing = false;
    private bool hasLockedOn = false;
    private Vector3 chaseDirection;
    private EnemyStats stats;

    private bool isOffScreen = false;
    private float offScreenTimer = 0f;
    private float offScreenLifetime = 2f;


    void Awake() {
        stats = GetComponent<EnemyStats>();
        stats.audioSource = GetComponent<AudioSource>();
        
    }

    private void Start() {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) {
            player = playerObj.transform;
        } else {
            Debug.LogWarning("Player not found by Chaser.");
        }
    }

    private void Update() {
        if (player == null) return;
        //if (!stats.canMove) return;

        // Handle off-screen lifetime
        if (isOffScreen) {
            offScreenTimer += Time.deltaTime;
            if (offScreenTimer >= offScreenLifetime) {
                Destroy(gameObject);
                return;
            }
        } else {
            offScreenTimer = 0f;
        }

        if (isChasing) {
            if (!hasLockedOn) {
                chaseDirection = (player.position - transform.position).normalized;
                transform.rotation = Quaternion.FromToRotation(Vector3.up, chaseDirection);
                hasLockedOn = true;
            }
            transform.position += chaseSpeed * Time.deltaTime * chaseDirection;
        } else {
            transform.position += stats.moveSpeed * Time.deltaTime * Vector3.down;
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);

            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < chaseTriggerDistance) {
                isChasing = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerStats>().TakeDamage(damage);
            stats.Die();
            //Destroy(gameObject);
        }
    }

    private void OnBecameInvisible() => isOffScreen = true;
    private void OnBecameVisible() => isOffScreen = false;
}
