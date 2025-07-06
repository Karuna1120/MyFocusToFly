using UnityEngine;

public class Spawner2 : MonoBehaviour
{
    public GameObject prefab;
    public float spawnRate = 1.4f;
    public float minHeight = -1.5f;
    public float maxHeight = 1.5f;

    private float timer;
    public float towerGap = 2.5f;

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
        GameObject towers = Instantiate(prefab, transform.position, Quaternion.identity);
        towers.transform.position += Vector3.up * Random.Range(minHeight, maxHeight);
    }

    public void SetSpawnRate(float rate)
    {
        spawnRate = rate;
    }

    public void SetTowerGap(float gap)
    {
        towerGap = gap;
    }
}
