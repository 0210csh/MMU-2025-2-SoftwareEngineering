using UnityEngine;

public class Monster : MonoBehaviour
{
    public float speed = 1f;
    public int maxHp = 100;
    private int currentHp;

    private Transform target;

    void Start()
    {
        currentHp = maxHp;
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        Debug.Log($"{gameObject.name}이(가) {damage} 데미지를 입음! 남은 HP: {currentHp}");

        if (currentHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} 사망");
        Destroy(gameObject);
    }
}
