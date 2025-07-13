//Applied to 'CandleManager' GameObject

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleSpawner : MonoBehaviour
{
    [Header("Candle Setup")]
    public List<GameObject> candlePrefabs;
    public int initialPoolSize = 2000;
    public float spawnRadiusScale = 0.05f;

    private List<GameObject> pool = new List<GameObject>();
    private List<GameObject> activeCandles = new List<GameObject>();

    void Start()
    {
        //Call PrewarmPool to preload a set number of candles
        PrewarmPool(initialPoolSize);
    }

    //Instantiate random candle prefab, ensures its deactivated, and store it in a pool
    public void PrewarmPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject candle = Instantiate(GetRandomCandle());

            if (candle.activeSelf)
                candle.SetActive(false);

            pool.Add(candle);
        }
    }

    //Randomly select one of the candle prefabs and then instantiate it
    GameObject GetRandomCandle()
    {
        if (candlePrefabs.Count == 0)
        {
            return null;
        }

        int index = Random.Range(0, candlePrefabs.Count);
        return Instantiate(candlePrefabs[index]);
    }

    //Start spawning candles
    public void SpawnCandles(int count, Vector3 center)
    {
        StartCoroutine(SpawnCandlesGradually(count, center));
    }

    //Spawn candles one at a time from the pool
    IEnumerator SpawnCandlesGradually(int count, Vector3 center)
    {
        int spawned = 0;

        while (spawned < count)
        {
            //Get candle from the pool
            GameObject candle = GetCandleFromPool();
            if (candle == null)
                yield break;

            //Use a randomm position
            Vector3 pos = GetScatterPosition(center, count);
            candle.transform.position = pos;

            //Randomly rotate each candle's Y-axis
            candle.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            //Set to active
            candle.SetActive(true);
            activeCandles.Add(candle);

            spawned++;

            //Pause briefly every 50 canldles
            if (spawned % 50 == 0)
                yield return null;
        }
    }

    //Calculate a random position in a scattered circular area
    Vector3 GetScatterPosition(Vector3 center, int totalCount)
    {
        //The more candles spawned, the wider the radius gets
        float radius = spawnRadiusScale * Mathf.Sqrt(totalCount);

        //Pick a random point inside a circle
        Vector2 randomCircle = Random.insideUnitCircle * radius;

        //Return a 3D position (keeping Y the same as center)
        return new Vector3(center.x + randomCircle.x, center.y, center.z + randomCircle.y);
    }

    //Retrieve an inactive candle from the pool
    GameObject GetCandleFromPool()
    {
        foreach (var candle in pool)
        {
            //Resuse inactive candles
            if (!candle.activeInHierarchy)
                return candle;
        }

        //If the pool is empty, then add a new deactivated candle to the pool and use it
        GameObject newCandle = GetRandomCandle();
        if (newCandle != null)
        {
            newCandle.SetActive(false);
            pool.Add(newCandle);
        }

        return newCandle;
    }

    //Deactivate all spawned candles and clear the active list
    public void ClearAllCandles()
    {
        foreach (var candle in activeCandles)
        {
            if (candle != null)
                candle.SetActive(false);
        }

        //Reset list
        activeCandles.Clear();
    }
}