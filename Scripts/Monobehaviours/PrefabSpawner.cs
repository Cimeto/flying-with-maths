using UnityEngine;

/// <summary>
/// Script to instantiate prefabs continuously in a 3D environment
/// </summary>
public class PrefabSpawner : MonoBehaviour
{
    public PrefabCollection prefabCollection;
    public Transform[] spawnPoints;

    public Vector3 positionRandomness;
    public Vector3 rotationRandomness;
    public float speed = 1;
    public float spawnRate = 1;
    public float prefabLifeTime = 10;
    public float delay = 0;
    public bool instantiateAtStart = true;
    public bool attachToParent = true;

    private float timer = 0;

    private void Start()
    {
        if (instantiateAtStart)
        {
            timer = spawnRate; // to make it create one at the beginning
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Wait until delay time has been consumed.
        if (delay > 0)
        {
            delay -= Time.deltaTime;
            return;
        }

        // Count time to know when to spawn the next prefab
        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            InstantiateRandomPrefab();
            timer = 0;
        }
    }

    // TODO I would refactor this in smaller chuncks
    private void InstantiateRandomPrefab()
    {
        // Use this transform as spawnPoint
        Transform spawnPoint = transform;

        if (prefabCollection.prefabs.Length == 0)
        {
            return;
        }

        // Use a random spawnpoint if any
        if (spawnPoints.Length > 0)
        {
            int randomSPIndex = Random.Range(0, spawnPoints.Length);
            spawnPoint = spawnPoints[randomSPIndex];
        }

        // Calculate random position
        Vector3 newPosition = GenerateRandomVector(spawnPoint.position, positionRandomness);

        // Calculate random rotation
        Quaternion newRotation = Quaternion.Euler(
            GenerateRandomVector(spawnPoint.rotation.eulerAngles, rotationRandomness));

        // Select a random prefab from the collection
        int randomIndex = Random.Range(0, prefabCollection.prefabs.Length);
        GameObject prefab = Instantiate(
            prefabCollection.prefabs[randomIndex], newPosition, newRotation);

        // Set speed and size
        prefab.GetComponent<Rigidbody>().velocity = spawnPoint.forward * speed;

        if (attachToParent)
        {
            prefab.transform.SetParent(spawnPoint);
        }

        // Set death time
        Destroy(prefab, prefabLifeTime);
    }

    private Vector3 GenerateRandomVector(Vector3 origin, Vector3 randomness)
    {
        float randomX = Random.Range(origin.x - randomness.x, origin.x + randomness.x);
        float randomY = Random.Range(origin.y - randomness.y, origin.y + randomness.y);
        float randomZ = Random.Range(origin.z - randomness.z, origin.z + randomness.z);

        return new Vector3(randomX, randomY, randomZ);
    }
}