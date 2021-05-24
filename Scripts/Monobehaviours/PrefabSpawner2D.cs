using UnityEngine;

/// <summary>
/// Script to instantiate prefabs continuously in a 2D environment
/// </summary>
public class PrefabSpawner2D : MonoBehaviour
{
    public PrefabCollection prefabCollection;
    public float prefabsSize = 1;
    public Vector2 direction;
    public float yRandomness = 0;
    public float zRandomness = 0;
    public float speed = 1;
    public float spawnRate = 1;
    public float spawnRateRandomness = 0;
    public float prefabLifeTime = 5;
    public bool instantiateAtStart = true;
    public bool preCreatePrefabs = true;

    private float timer = 0;
    private float calculatedSpawnRate;

    public void SetPrefabCollection(PrefabCollection prefabCollection)
    {
        this.prefabCollection = prefabCollection;

        // TODO Using objects pool technique here implies updating the object pool content at this point
    }

    private void Start()
    {
        calculatedSpawnRate = Random.Range(spawnRate - spawnRateRandomness, spawnRate + spawnRateRandomness);

        if (instantiateAtStart)
        {
            timer = calculatedSpawnRate; // to make it create one at the beginning
        }

        if (preCreatePrefabs)
        {
            PreInstantiateRandomPrefabs();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Count time to know when to spawn the next prefab
        timer += Time.deltaTime;
        if (timer >= calculatedSpawnRate)
        {
            InstantiateRandomPrefab(transform.position.x, transform.position.y);
            timer = 0;
            calculatedSpawnRate = Random.Range(spawnRate - spawnRateRandomness, spawnRate + spawnRateRandomness);
        }
    }

    // Create random prefabs in a line
    private void PreInstantiateRandomPrefabs()
    {
        int quantity = (int)(prefabLifeTime / spawnRate);
        float distancePerPrefab = spawnRate * speed;

        while (quantity > 0)
        {
            InstantiateRandomPrefab(
                transform.position.x + direction.x * distancePerPrefab * quantity,
                transform.position.y + direction.y * distancePerPrefab * quantity);
            quantity--;
        }
    }

    private void InstantiateRandomPrefab(float x, float y)
    {
        if (prefabCollection.prefabs.Length == 0)
        {
            return;
        }

        // Calculating new position considering randomness of Z dimension
        float randomY = Random.Range(transform.position.y - yRandomness, transform.position.y + zRandomness);
        float randomZ = Random.Range(transform.position.z - zRandomness, transform.position.z + zRandomness);
        Vector3 newPosition = new Vector3(x, randomY, randomZ);

        // Random index in the prefab list and instantiating
        int randomIndex = Random.Range(0, prefabCollection.prefabs.Length);

        GameObject prefab =
            Instantiate(prefabCollection.prefabs[randomIndex], newPosition, transform.rotation);
        prefab.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x * speed, direction.y * speed);
        prefab.transform.localScale = prefab.transform.localScale * prefabsSize;
        prefab.transform.SetParent(transform);
        Destroy(prefab, prefabLifeTime);

        // IDEA. Set the prefab destroy in the instantiated prefab, triggered by distance from the parent. 
        // (If it's too far, destroy it)
    }
}