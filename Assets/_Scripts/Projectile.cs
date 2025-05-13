using UnityEngine;

public abstract class Projectile : MonoBehaviour {
    public float speed = 10f;
    public float damage = 1f;

    protected SpriteRenderer sr;
    protected Color orgColor;

    protected abstract string TargetTag { get; }

    protected virtual void Awake() {
        sr = GetComponent<SpriteRenderer>();
        orgColor = sr.color;
    }

    protected virtual void Start() {
        Destroy(gameObject, 3f);
    }

    protected virtual void Update() {
        transform.position += speed * Time.deltaTime * transform.up;
    }

    protected virtual void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("Bullet")) {
            Destroy(col.gameObject);
            Destroy(gameObject);
            return;
        }

        if (!col.CompareTag(TargetTag)) return;

        Entity entity = col.GetComponent<Entity>();
        if (entity != null) {
            entity.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible() => Destroy(gameObject);
}

