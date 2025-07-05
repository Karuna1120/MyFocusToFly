using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public float spawnRate = 1.4f;
    public float minHeight = -1.5f;
    public float maxHeight = 1.5f;

    private float timer;
    public float towerGap = 2.5f;

    public GameObject attackplanePrefab;
    public float attactplanespawnChance = 0.08f; // ????

    private bool planeOnCooldown = false;
    public float planeCooldownDuration = 4f; // ???????????

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
        if (!planeOnCooldown && Random.value < attactplanespawnChance)
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

    public void SetTowerGap(float gap)
    {
        towerGap = gap;
    }

    private void SpawnAttackPlane()
    {
        StartCoroutine(ShowFlashAndSpawn());
    }

    private IEnumerator ShowFlashAndSpawn()
    {
        ScreenFlash screenFlash = FindObjectOfType<ScreenFlash>();

        if (screenFlash != null)
        {
            for (int i = 0; i < 3; i++)
            {
                screenFlash.Flash();
                yield return new WaitForSeconds(0.3f);
                yield return new WaitForSeconds(0.3f);
            }
        }

        yield return new WaitForSeconds(0.5f); // ????

        Vector2[] leftCorners = {
            new Vector2(-8, 5),
            new Vector2(-8, -5)
        };

        Vector2 spawnPos = leftCorners[Random.Range(0, leftCorners.Length)];
        Instantiate(attackplanePrefab, spawnPos, Quaternion.identity);

        planeOnCooldown = true;
        yield return new WaitForSeconds(planeCooldownDuration); // ????
        planeOnCooldown = false;
    }
}
