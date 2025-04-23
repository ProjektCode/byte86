using UnityEngine;

public class Chaser : MonoBehaviour {

    [Header("Chaser Stats")]
    //public float approachSpeed = 2f;         // Slow downward approach speed
    public float chaseSpeed = 5f;            // Speed once chasing
    public float chaseTriggerDistance = 5f;  // Distance to trigger chase

    private Transform player;
    private bool isChasing = false;
    private bool hasLockedOn = false;
    private Vector3 chaseDirection;
    private EnemyStats stats;

    void Awake() {
        stats = GetComponent<EnemyStats>();
    }

    private void Start() {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) {
            player = playerObj.transform;
        } else {
            Debug.LogWarning("Player not found by ZigZagChaser.");
        }
    }

    private void Update() {
        if (player == null) return;

        if (isChasing) {
            if (!hasLockedOn) {
                // Lock-on direction
                chaseDirection = (player.position - transform.position).normalized;

                // Instantly rotate to face player based on sprite facing up
                transform.rotation = Quaternion.FromToRotation(Vector3.up, chaseDirection);

                hasLockedOn = true;
            }

            // Move toward player
            transform.position += chaseSpeed * Time.deltaTime * chaseDirection;
        } else {
            // Move straight downward
            transform.position += stats.moveSpeed * Time.deltaTime * Vector3.down;

            // Face downward (180 degrees from up)
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);

            // Trigger chase if within range
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < chaseTriggerDistance) {
                isChasing = true;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerStats>()?.TakeDamage(10);
            Destroy(gameObject); // Or disable if pooling
        }
    }
}
