using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedListObject
{
    public GameObject gameObject;
    public float weight;
}

public class AsteroidFieldGenerator : MonoBehaviour
{

    public List<WeightedListObject> randomAsteroids = new List<WeightedListObject> { };
    public CircleCollider2D deleteArea;
    public float maxSpawnRadius = 150f;
    public int maxNumberOfAsteroids = 100;
    private int currentNumberofAsteroids = 0;
    public float noSpawnRadiusCenter = 10f;
    public Transform noSpawnTransformCenter;

    public List<GravityObject> asteroids = new List<GravityObject> { };

    List<GameObject> gameObjects = new List<GameObject> { };
    List<float> weights = new List<float> { };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        if (noSpawnTransformCenter == null)
        {
            noSpawnTransformCenter = transform;
        }
        foreach (WeightedListObject go in randomAsteroids)
        {
            gameObjects.Add(go.gameObject);
            weights.Add(go.weight);
        }
        while (currentNumberofAsteroids < maxNumberOfAsteroids)
        {
            SpawnNewAsteroid();
            yield return new WaitForEndOfFrame();
        }

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (enabled)
        {
            if (collision.gameObject.GetComponent<GravityObject>() != null && asteroids.Contains(collision.gameObject.GetComponent<GravityObject>()))
            {
                asteroids.Remove(collision.gameObject.GetComponent<GravityObject>());
                Destroy(collision.gameObject);
                currentNumberofAsteroids--;
                if (currentNumberofAsteroids < maxNumberOfAsteroids)
                {
                    SpawnNewAsteroid();
                }
            }
        }
    }

    void SpawnNewAsteroid()
    {
        if (enabled)
        {
            GameObject randomObject = Extension.GetWeighedRandom(gameObjects, weights);
            GameObject instantiatedObject = Instantiate(randomObject);
            instantiatedObject.transform.position = RandomPointInAnnulus(noSpawnTransformCenter.position, noSpawnRadiusCenter, maxSpawnRadius);
            currentNumberofAsteroids++;
            asteroids.Add(instantiatedObject.GetComponent<GravityObject>());
        }
    }

    public Vector2 RandomPointInAnnulus(Vector2 origin, float minRadius, float maxRadius)
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized; // There are more efficient ways, but well
        float minRadius2 = minRadius * minRadius;
        float maxRadius2 = maxRadius * maxRadius;
        float randomDistance = Mathf.Sqrt(Random.value * (maxRadius2 - minRadius2) + minRadius2);
        return origin + randomDirection * randomDistance;
    }
}
