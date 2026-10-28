using UnityEngine;

public class Boid : MonoBehaviour
{
    public GameObject boidPrefab;
    public Color color;
    public int boidCount = 10;
    public float spawnRadius = 5f;
    public float attractionStrength = 1f;
    public float repulsionStrength= 1f;
    public float targetStrength = 1f;
    public float repulseDistance= 1f;
    public float repulseA = 1f;
    public float collisionThreshold = 1.5f;
    public float expCoeff = 1f;
    //public float targetDistance = 2f;
    public float maxSpeed = 10f;
    public float maxStartSpeed = 5f;
    public float timeStep = 0.02f;

    private GameObject[] boids;
    private Vector3[] acceleration;
    private Vector3[] velocity;
    private MaterialPropertyBlock[] materialProperties;
    private float[] minNeighborDistances;

    public int collisions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collisions = 0;
        boids = new GameObject[boidCount];
        acceleration = new Vector3[boidCount];
        velocity = new Vector3[boidCount];
        materialProperties = new MaterialPropertyBlock[boidCount];
        minNeighborDistances = new float[boidCount];
        if (boidPrefab == null)
        {
            boidPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }

        for (int i = 0; i < boidCount; i++)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
            boids[i] = Instantiate(boidPrefab, spawnPosition, Quaternion.identity);
            acceleration[i] = Vector3.zero;
            velocity[i] = Random.insideUnitSphere * maxStartSpeed;
            materialProperties[i] = new MaterialPropertyBlock();
            materialProperties[i].SetColor("_Color", color);
            boids[i].GetComponent<Renderer>().SetPropertyBlock(materialProperties[i]);
            minNeighborDistances[i] = float.MaxValue;
        }
        //boidPrefab.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        collisions = 0;
        for (int i = 0; i < boidCount; i++)
        {
            acceleration[i] = Vector3.zero;
            float distance;
            for (int j = i + 1; j < boidCount; j++)
            {
                distance = Vector3.Distance(boids[i].transform.position, boids[j].transform.position);
                if (distance < repulseDistance)
                {
                    // Repulsion
                    Vector3 repulsionDirection = (boids[i].transform.position - boids[j].transform.position).normalized;
                    acceleration[i] += repulsionDirection * repulsionStrength / distance;
                    acceleration[j] -= repulsionDirection * repulsionStrength / distance;
                }
                // Attraction
                Vector3 attractionDirection = (boids[j].transform.position - boids[i].transform.position).normalized;
                acceleration[i] += attractionDirection * attractionStrength / distance;
                acceleration[j] -= attractionDirection * attractionStrength / distance;
                
                if (distance < minNeighborDistances[i])
                {
                    minNeighborDistances[i] = distance;
                }
                if (distance < minNeighborDistances[j])
                {
                    minNeighborDistances[j] = distance;
                }
            }
            // Alignment
            distance = Vector3.Distance(boids[i].transform.position, transform.position);
            Vector3 targetDirection = (transform.position - boids[i].transform.position).normalized;
            acceleration[i] += targetDirection * targetStrength / distance;
            velocity[i] += acceleration[i] * timeStep;
            if (velocity[i].magnitude > maxSpeed)
            {
                velocity[i] = velocity[i].normalized * maxSpeed;
            }
            boids[i].transform.position += velocity[i] * timeStep;
            float repulseDist = repulseA / (minNeighborDistances[i] + 0.001f);
            if (repulseDist > collisionThreshold)
            {
                collisions+= Mathf.CeilToInt(Mathf.Exp(expCoeff * (repulseDist - collisionThreshold)));
            }
            MaterialPropertyBlock mpb = materialProperties[i];
            mpb.SetColor("_Color", color);
            mpb.SetFloat("_RepulseDist", repulseDist);
            mpb.SetFloat("_Distance", 5f / (distance + 0.0001f));
            mpb.SetFloat("_Repulse", repulsionStrength);
            boids[i].GetComponent<Renderer>().SetPropertyBlock(mpb);
            minNeighborDistances[i] = float.MaxValue; // Reset for the next frame
        }
    }
}
