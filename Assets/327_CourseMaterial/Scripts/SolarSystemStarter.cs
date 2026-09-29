// IMDM327 Material
// Use CSV or JSON to load data into the simulation. Both formats are supported, but they use different data types. 
// The CSV format uses a struct, while the JSON format uses a class. This script demonstrates how to load both formats and access their data.
using UnityEngine;
public class SolarSystemStarter : MonoBehaviour
{
    // These components can be attached independently.
    DataCSV solarCSV;
    DataJSON solarJSON;
    public float timeStep = 1.0f;
    const float G = 6.674e-11f; // Gravitational constant
    PlanetProperty[] planetProperties;
    private int numberOfSphere = 10;
    struct PlanetProperty // why struct?
    {                   // https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
        public GameObject planet;
        public float mass;
        public float radius;
        public Vector3 velocity;
        public Vector3 acceleration;
        public Vector3 actualPosition;
    }

    // CSV data and JSON data use their own data types.
    public BodyProperty[] solarBodiesCSV;
    // public SolarBody[] solarBodiesJSON;

    // Both loader scripts finish reading their files in Awake().
    void Start()
    {
        // CSV: use this block when a DataCSV component is attached.
        solarCSV = GetComponent<DataCSV>();
        if (solarCSV != null)
        {
            solarBodiesCSV = solarCSV.bp;
            Debug.Log("Loaded " + solarBodiesCSV.Length + " bodies from solar.csv.");
            Debug.Log("First body: mass = " + solarBodiesCSV[0].mass + ", distance = " + solarBodiesCSV[0].distance + ", initial_velocity = " + solarBodiesCSV[0].initial_velocity);
        }

        // JSON: use this block when a DataJSON component is attached.
        // solarJSON = GetComponent<DataJSON>();
        // if (solarJSON != null)
        // {
        //     solarBodiesJSON = solarJSON.solarData.bodies;
        //     Debug.Log("Loaded " + solarBodiesJSON.Length + " bodies from solar.json.");
        //     Debug.Log("First body: " + solarBodiesJSON[0].name + ", mass: " + solarBodiesJSON[0].mass);
        // }


        // GameObject array to hold the planets in the simulation.
        planetProperties = new PlanetProperty[numberOfSphere];
        for (int i = 0; i < numberOfSphere; i++)
        {
            // Our gameobjects are created here:
            planetProperties[i] = new PlanetProperty();
            planetProperties[i].planet = GameObject.CreatePrimitive(PrimitiveType.Sphere); 
            planetProperties[i].planet.name = solarBodiesCSV[i].name;
        }

        // Apply the loaded data to the simulation. This is where you would set up your bodies in the scene based on the loaded data.
        for (int i = 0; i < solarBodiesCSV.Length; i++)
        {
            planetProperties[i].mass = solarBodiesCSV[i].mass;
            planetProperties[i].radius = solarBodiesCSV[i].radius;

            // What is missing here? You need to set the initial position and velocity of each planet based on the loaded data.
            // ***WRITE YOUR CODE HERE***

            float initialTheta = Random.Range(0, 2 * Mathf.PI);
            float cos = Mathf.Cos(initialTheta);
            float sin = Mathf.Sin(initialTheta);
            planetProperties[i].actualPosition = new Vector3(cos * solarBodiesCSV[i].distance, sin * solarBodiesCSV[i].distance, 0);
            planetProperties[i].velocity = new Vector3(-sin * solarBodiesCSV[i].initial_velocity, cos * solarBodiesCSV[i].initial_velocity, 0);


        }

        for (int i = 0; i < planetProperties.Length; i++)
        {
            // + This is just pretty trails
            TrailRenderer trailRenderer = planetProperties[i].planet.AddComponent<TrailRenderer>();
            // Configure the TrailRenderer's properties
            trailRenderer.time = 10.0f;  // Duration of the trail
            trailRenderer.startWidth = 0.5f;  // Width of the trail at the start
            trailRenderer.endWidth = 0.1f;    // Width of the trail at the end
            // a material to the trail
            trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
            // Set the colour gradient along the trail.
            Gradient gradient = new Gradient();
            Color targetColor = Color.HSVToRGB((float)i / planetProperties.Length, 1f, 1f);
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.white, 0f), // (color, normalized position)
                    new GradientColorKey(targetColor, 0.8f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f), // (alpha, normalized position) 
                    new GradientAlphaKey(0f, 1f)
                }
            );
            trailRenderer.colorGradient = gradient;
        }
    }
    void Update()
    {
        // Loop for N-body gravity
        // How should we design the loop?

        // 00. Initialize the acceleration for each body to zero at the start of each frame
       for (int i = 0; i < numberOfSphere; i++)
        {
            planetProperties[i].acceleration = Vector3.zero;
        }
        // 01. Loop through each body to calculate the gravitational forces acting on it
        for (int i = 0; i < numberOfSphere; i++)
        {
            for (int j = i + 1; j < numberOfSphere; j++)
            {
                Vector3 displacement = planetProperties[j].actualPosition - planetProperties[i].actualPosition;
                Vector3 gravForce = CalculateGravity(displacement, planetProperties[i].mass, planetProperties[j].mass); // force of j on i

                planetProperties[i].acceleration += gravForce / planetProperties[i].mass;
                planetProperties[j].acceleration -= gravForce / planetProperties[j].mass;

            }
        }
        // 02. Loop through each body to update its velocity and position based on the calculated acceleration
       for (int i = 0; i < numberOfSphere; i++)
        {
            planetProperties[i].velocity += planetProperties[i].acceleration * Time.deltaTime * timeStep;
            planetProperties[i].actualPosition += planetProperties[i].velocity * Time.deltaTime * timeStep;
             //Scale: float scaledDistance = Mathf.Sqrt(actualPosition[i].magnitude / 1e8f);

             float scaledDistance = Mathf.Sqrt(planetProperties[i].actualPosition.magnitude / 1e8f);
             planetProperties[i].planet.transform.position = scaledDistance * planetProperties[i].actualPosition.normalized; 
        }
    }

    // Gravity Fuction to finish
    private Vector3 CalculateGravity(Vector3 distanceVector, float m1, float m2)
    {
        Vector3 gravity = Vector3.zero; // note this is also Vector3
        gravity = G * m1 * m2 / (distanceVector.sqrMagnitude) * distanceVector.normalized;
        return gravity;
    }
}
