using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleSpawner : MonoBehaviour
{
    [Header("Candle Setup")]
    public List<GameObject> candlePrefabs; // Assign your 3 prefab variations here
    public int initialPoolSize = 2000;
    public float spawnRadiusScale = 0.05f; // Controls candle field spread

    private List<GameObject> pool = new List<GameObject>();
    private List<GameObject> activeCandles = new List<GameObject>();

    void Start()
    {
        PrewarmPool(initialPoolSize);
    }

    public void PrewarmPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject candle = Instantiate(GetRandomCandle());

            // Ensure it's hidden and inactive
            if (candle.activeSelf)
                candle.SetActive(false);

            pool.Add(candle);
        }
    }

    GameObject GetRandomCandle()
    {
        if (candlePrefabs.Count == 0)
        {
            Debug.LogWarning("⚠️ No candle prefabs assigned to CandleSpawner.");
            return null;
        }

        int index = Random.Range(0, candlePrefabs.Count);
        return Instantiate(candlePrefabs[index]);
    }

    public void SpawnCandles(int count, Vector3 center)
    {
        StartCoroutine(SpawnCandlesGradually(count, center));
    }

    IEnumerator SpawnCandlesGradually(int count, Vector3 center)
    {
        int spawned = 0;

        while (spawned < count)
        {
            GameObject candle = GetCandleFromPool();
            if (candle == null)
                yield break;

            Vector3 pos = GetScatterPosition(center, count);
            candle.transform.position = pos;
            candle.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            candle.SetActive(true);
            activeCandles.Add(candle);

            spawned++;

            if (spawned % 50 == 0)
                yield return null; // Yield every 50 for smoother frame pacing
        }
    }

    Vector3 GetScatterPosition(Vector3 center, int totalCount)
    {
        float radius = spawnRadiusScale * Mathf.Sqrt(totalCount);
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        return new Vector3(center.x + randomCircle.x, center.y, center.z + randomCircle.y);
    }

    GameObject GetCandleFromPool()
    {
        foreach (var candle in pool)
        {
            if (!candle.activeInHierarchy)
                return candle;
        }

        // Expand pool if needed
        GameObject newCandle = GetRandomCandle();
        if (newCandle != null)
        {
            newCandle.SetActive(false);
            pool.Add(newCandle);
        }

        return newCandle;
    }

    public void ClearAllCandles()
    {
        foreach (var candle in activeCandles)
        {
            if (candle != null)
                candle.SetActive(false);
        }

        activeCandles.Clear();
    }
}
