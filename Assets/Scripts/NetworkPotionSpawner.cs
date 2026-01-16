using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkPotionSpawner : NetworkBehaviour
{
    [Header("Spawn")]
    public float everySeconds = 5f;
    public Vector2 xRange = new Vector2(-20f, 20f);
    public Vector2 zRange = new Vector2(-20f, 20f);

    [Header("Potions")]
    public List<GameObject> potionsPrefab;

    public override void OnStartServer()
    {
        StartCoroutine(SpawnLoop());
    }

    [Server]
    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(everySeconds);

            var prefab = potionsPrefab[Random.Range(0, potionsPrefab.Count)];

            float x = Random.Range(xRange.x, xRange.y);
            float z = Random.Range(zRange.x, zRange.y);

            Vector3 pos = new Vector3(x, 0.2f, z);

            GameObject potion = Instantiate(prefab, pos, Quaternion.identity);
            NetworkServer.Spawn(potion);
        }
    }
}

