using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public float spawnRate = 1.4f;
    public float minHeight = -1.5f;
    public float maxHeight = 1.5f;

    private float timer;
    public float towerGap = 2.5f;

    public GameObject attackplanePrefab;
    public float attactplanespawnChance = 0.15f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            Spawn();
            timer = 0;
        }
    }

    private void Spawn()
    {
        if (Random.value < attactplanespawnChance)
        {
            SpawnAttackPlane();
            return;
        }

        GameObject towers = Instantiate(prefab, transform.position, Quaternion.identity);
        towers.transform.position += Vector3.up * Random.Range(minHeight, maxHeight);
    }

    public void SetSpawnRate(float rate)
    {
        spawnRate = rate;
    }

    private void SpawnAttackPlane()
    {
        Vector2[] corners = {
        new Vector2(-8, 5),   // ??
        new Vector2(8, 5),    // ??
        new Vector2(-8, -5),  // ??
        new Vector2(8, -5)    // ??
    };

        Vector2 spawnPos = corners[Random.Range(0, corners.Length)];
        GameObject plane = Instantiate(attackplanePrefab, spawnPos, Quaternion.identity);
    }

    public void SetTowerGap(float gap)
    {
        towerGap = gap;
    }
}
