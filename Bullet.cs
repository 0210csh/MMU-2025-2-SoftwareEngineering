using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 100;
    public float explosionRadius = 1f;
    private GameObject target;

    public void SetTarget(GameObject _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 direction = (target.transform.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.transform.position) < 0.2f)
        {
            Explode();
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        

        foreach (Collider2D hit in hits)
        {
            Monster monster = hit.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
