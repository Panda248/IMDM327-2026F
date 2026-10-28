using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class HandFollow : MonoBehaviour
{
    MediaPipeBodyTracker mp;
    bool prevHandTracked;
    bool visible;

    public float track_scale = 10f;

    public GameObject thumb_nbody;
    public GameObject index_nbody;
    public GameObject middle_nbody;
    public GameObject ring_nbody;
    public GameObject pinky_nbody;
    public GameObject palm_nbody;

    public bool show_vis = true;

    GameObject thumb_flock;
    GameObject index_flock;
    GameObject middle_flock;
    GameObject ring_flock;
    GameObject pinky_flock;
    GameObject palm_flock;
    Camera main_cam;
    private void OnEnable()
    {
        mp = MediaPipeBodyTracker.Instance();
        prevHandTracked = mp.RightHandTracked;
        visible = true;

        thumb_flock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        index_flock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        middle_flock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ring_flock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pinky_flock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        palm_flock = GameObject.CreatePrimitive(PrimitiveType.Cube);

        main_cam = Camera.main;

    }

    private void Update()
    {
        if (thumb_flock.activeSelf != show_vis)
        {
            thumb_flock.SetActive(show_vis);
            index_flock.SetActive(show_vis);
            middle_flock.SetActive(show_vis);
            ring_flock.SetActive(show_vis);
            pinky_flock.SetActive(show_vis);
            palm_flock.SetActive(show_vis);
        }
    }


    void FixedUpdate()
    {
        if (!mp.RightHandTracked) return;

        palm_flock.transform.position = TrackedToWorld(mp.RightHandPosition);
        palm_nbody.transform.position = TrackedToWorld(mp.RightHandPosition);

        thumb_flock.transform.position = TrackedToWorld(mp.RightLandmarkPositions[4]);
        thumb_nbody.transform.position = TrackedToWorld(mp.RightLandmarkPositions[4]);

        index_flock.transform.position = TrackedToWorld(mp.RightLandmarkPositions[8]);
        index_nbody.transform.position = TrackedToWorld(mp.RightLandmarkPositions[8]);

        middle_flock.transform.position = TrackedToWorld(mp.RightLandmarkPositions[12]);
        middle_nbody.transform.position = TrackedToWorld(mp.RightLandmarkPositions[12]);

        ring_flock.transform.position = TrackedToWorld(mp.RightLandmarkPositions[16]);
        ring_nbody.transform.position = TrackedToWorld(mp.RightLandmarkPositions[16]);

        pinky_flock.transform.position = TrackedToWorld(mp.RightLandmarkPositions[20]);
        pinky_nbody.transform.position = TrackedToWorld(mp.RightLandmarkPositions[20]);
    }

    Vector3 TrackedToWorld(Vector3 trackedPos)
    {
        Vector3 converted = new Vector3(trackedPos.x, trackedPos.y, 0f);
        converted *= -track_scale;
        converted.x *= main_cam.aspect;
        converted += new Vector3(track_scale / 2f, track_scale / 2f, 0f);
        return converted;
    }

    //void SetAcceleration(GameObject[] group)
    //{
    //    float closeDistance = 0.1f;
    //    int numberOfSphere = group.Length;
    //    GameObject[] bp = group;
    //    for (int i = 0; i < numberOfSphere; i++)
    //    {
    //        bp[i].acceleration = Vector3.zero; // important
    //    }

    //    // Acceleration (Force)  
    //    for (int i = 0; i < numberOfSphere; i++)
    //    {
    //        for (int j = i + 1; j < numberOfSphere; j++)
    //        {
    //            // Vector from i to j body. Make sure which vector you are getting.
    //            Vector3 distance = body[j].transform.position - body[i].transform.position;
    //            // Gravity
    //            Vector3 gravity = CalculateGravity(distance, bp[i].mass, bp[j].mass);
    //            // Apply Gravity
    //            // F = ma -> a = F/m
    //            // Gravity is push and pull with same amount. Force: m1 <-> m2

    //            // .. only if it is not too close
    //            if (distance.sqrMagnitude > closeDistance)
    //            {
    //                bp[i].acceleration += gravity / bp[i].mass; // why is this +?
    //                bp[j].acceleration -= gravity / bp[j].mass; // why is this -? What decided the direction?                   
    //            }
    //            else // apply opposite gravity (push) if too close. 
    //            { // Hatred is stronger than attraction.
    //                bp[i].acceleration -= 3f * gravity / bp[i].mass; // 
    //                bp[j].acceleration += 3f * gravity / bp[j].mass; // 
    //            }

    //        }
    //    }

    //    // (Force) Hesitation: randomly hover the space for natural behavior  
    //    for (int i = 0; i < numberOfSphere; i++)
    //    {
    //        float randomScale = 1f;
    //        if (Random.Range(0f, 1.05f) > 1f)
    //        {
    //            bp[i].acceleration += new Vector3(randomScale * Random.Range(-1f, 1f), randomScale * Random.Range(-1f, 1f), randomScale * Random.Range(-1f, 1f));
    //        }
    //    }
    //}

    //private Vector3 CalculateGravity(Vector3 distanceVector, float m1, float m2)
    //{
    //    Vector3 gravity = new Vector3(0f, 0f, 0f); // note this is also Vector3
    //                                               // **** Fill in the function below.
    //    float eps = 0.1f;
    //    gravity = G * m1 * m2 / (distanceVector.magnitude + eps) * distanceVector.normalized;
    //    return gravity;
    //}
}
