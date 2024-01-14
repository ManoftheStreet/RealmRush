using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField][Range(0, 50f)] int poolSize = 5;
    [SerializeField][Range(0.1f, 30f)] float spawnTimer = 1f;

    //GameObject[] pool;
    List<GameObject> pool;

    void Awake()
    {
        PopulatePool();
    }

    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    void PopulatePool()
    {
        /*pool = new GameObject[poolSize];

        for(int i =0; i < pool.Length; i++)
        {
            pool[i] = Instantiate(enemyPrefab, transform);
            pool[i].SetActive(false);
        }*/
        pool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            AddObjectToPool();
        }
    }

    void EnableObjectInPool()
    {
        /*for(int i = 0; i < pool.Length; i++)
        {
            if (pool[i].activeInHierarchy == false)
            {
                pool[i].SetActive(true);
                return;
            }
        }*/
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return;
            }
        }
        
        AddObjectToPool().SetActive(true);
    }

    GameObject AddObjectToPool()
    {
        GameObject newObject = Instantiate(enemyPrefab, transform);
        newObject.SetActive(false);
        pool.Add(newObject);
        return newObject;
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            EnableObjectInPool();
            yield return new WaitForSeconds(spawnTimer);
        }
    }
}