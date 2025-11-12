using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 2f;
    public int monstersPerWave = 5;

    void Start()
    {
        InvokeRepeating(nameof(SpawnWave), 1f, spawnInterval);
    }

    void SpawnWave()
    {
        for (int i = 0; i < monstersPerWave; i++)
        {
            Vector3 spawnPos = spawnPoint.position + new Vector3(i * 0.5f, 0, 0);
            Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
        }
    }
}
