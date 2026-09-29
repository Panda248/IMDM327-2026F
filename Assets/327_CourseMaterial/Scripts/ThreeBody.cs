// 3-body Starter Code
// Fall 2026. IMDM 327
// Instructor. Myungin Lee
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ThreeBody : MonoBehaviour
{
    public float G = 500f; // Gravitational constant for this simulation, not the real-world value.
    BodyProperty[] bp;
    public int numberOfSphere = 3;
    public float timeStep = 0.01f;
    public float separationDistance = 3f;
    public float separationForce = 10f;
    public float alignForce = 10f;
    class BodyProperty // why struct?
    {                   // https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
        public GameObject body;
        public float mass;
        public Vector3 velocity;
        public Vector3 acceleration;
    }


    void Start()
    {
        // Allocate an array to store each body's properties.
        bp = new BodyProperty[numberOfSphere];
        Random.InitState(System.DateTime.Now.Second);
        // Loop generating the gameobject and assign initial conditions (type, position, (mass/velocity/acceleration)
        for (int i = 0; i < numberOfSphere; i++)
        {
            // Our gameobjects are created here:
            bp[i] = new BodyProperty();
            bp[i].body = GameObject.CreatePrimitive(PrimitiveType.Sphere); // why sphere? try different options.
            // https://docs.unity3d.com/ScriptReference/GameObject.CreatePrimitive.html

            bp[i].body.tag = "Body";

            // initial conditions
            float r = 100f;
            float deltaTheta = 2 * Mathf.PI / numberOfSphere;
            // position is (x,y,z). In this case, I want to plot them on the circle with r

            // ******** Fill in this part ********
            bp[i].body.transform.position = new Vector3( Mathf.Cos(deltaTheta * i) * r, Mathf.Sin(deltaTheta * i) * r, 180);
            //z = 180 places the bodies in front of a camera near the origin looking along +Z. Try other positions too.

            bp[i].velocity = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10)); 
            bp[i].mass = Random.Range(1,10); 


            // + This is just pretty trails
            TrailRenderer trailRenderer = bp[i].body.AddComponent<TrailRenderer>();
            // Configure the TrailRenderer's properties
            trailRenderer.time = 10.0f;  // Duration of the trail
            trailRenderer.startWidth = 0.5f;  // Width of the trail at the start
            trailRenderer.endWidth = 0.1f;    // Width of the trail at the end
            // a material to the trail
            trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
            // Set the colour gradient along the trail.
            Gradient gradient = new Gradient();
            Color targetColor = Color.HSVToRGB((float)i / numberOfSphere, 1f, 1f);
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
            bp[i].acceleration = Vector3.zero;
        }
        // 01. Loop through each body to calculate the gravitational forces acting on it
        for (int i = 0; i < numberOfSphere; i++)
        {
            for (int j = i + 1; j < numberOfSphere; j++)
            {
                // Cluster
                Vector3 displacement = (bp[i].body.transform.position - bp[j].body.transform.position);
                float numerator = G * bp[i].mass * bp[j].mass;
                Vector3 force = CalculateGravity(displacement, bp[i].mass, bp[j].mass);
                bp[i].acceleration -= force / bp[i].mass;
                bp[j].acceleration += force / bp[j].mass;

                // Separate
                if ((bp[i].body.transform.position - bp[j].body.transform.position).magnitude < separationDistance)
                {
                    bp[i].acceleration += force * separationForce / bp[i].mass;
                    bp[j].acceleration -= force * separationForce / bp[j].mass;
                }

                //Align
                Vector3 align = CalculateGravity(-bp[i].body.transform.position, bp[i].mass, bp[i].mass);
                bp[i].acceleration += align * alignForce / bp[i].mass;

            }
        }
        // 02. Loop through each body to update its velocity and position based on the calculated acceleration
       for (int i = 0; i < numberOfSphere; i++)
        {
            bp[i].acceleration.z = 0; // Keep the motion in the XY plane
            bp[i].velocity += bp[i].acceleration * timeStep * Time.deltaTime;
            bp[i].body.transform.position += bp[i].velocity * timeStep * Time.deltaTime;

        }
    }

    // Gravity Fuction to finish
    private Vector3 CalculateGravity(Vector3 distanceVector, float m1, float m2)
    {
        Vector3 gravity = Vector3.zero; // note this is also Vector3
                                        // **** Fill in the function below. 
                                        // gravity = ****;
        gravity  = G * m1 * m2 * distanceVector.normalized / (distanceVector.sqrMagnitude + 1f);
        return gravity;
    }
}

