using UnityEngine;

public class Tower : MonoBehaviour
{
    public float range = 5f;  
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    private float fireCooldown = 0f;

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        GameObject target = FindNearestMonster();
        if (target != null && fireCooldown <= 0f)
        {
            Fire(target);
            fireCooldown = 1f / fireRate;
        }
    }

    GameObject FindNearestMonster()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        GameObject nearest = null;
        float shortestDist = Mathf.Infinity;

        foreach (GameObject monster in monsters)
        {
            float dist = Vector2.Distance(transform.position, monster.transform.position);
            if (dist < shortestDist && dist <= range)
            {
                shortestDist = dist;
                nearest = monster;
            }
        }
        return nearest;
    }

    void Fire(GameObject target)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetTarget(target);
    }
}
