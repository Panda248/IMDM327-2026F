using UnityEngine;
using System.Collections.Generic;


public class LeftController : MonoBehaviour
{
    MediaPipeBodyTracker mp;
    public float curlMultiplier, spreadMultiplier;
    public float minCurl, minSpread;
    public float curl, spread;
    public float thumbDot;
    public float timeStep = 0.01f;
    public List<Boid> boids;
    public bool tracking;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mp = MediaPipeBodyTracker.Instance();
    }

    // Update is called once per frame
    void Update()
    {
        curl = Curl();
        spread = FingerSpread();
        thumbDot = ThumbVector();
        UpdateBoids();
        tracking = mp.LeftHandTracked;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position, transform.position + thumb);
    //}

    void UpdateBoids()
    {
        foreach (Boid boid in boids)
        {
            boid.repulsionStrength =  minCurl + (curl * curlMultiplier);
            boid.repulseDistance = minSpread + (spread * spreadMultiplier);
            float h, s, v;
            Color.RGBToHSV(boid.color, out h, out s, out v);
            h += thumbDot * timeStep;
            if (h > 1f) h -= 1f;
            if (h < 0f) h += 1f;
            boid.color = Color.HSVToRGB(h, s, v);
            //boid.targetStrength = Vector3.Dot(boid.transform.position.normalized, thumb);
        }
    }

    float Curl()
    {
        float result = 0f;
        for (int i = 0; i < 4; i++)
        {
            Vector3 finger_tip = mp.LeftLandmarkPositions[8 + i * 4];
            Vector3 finger_base = mp.LeftLandmarkPositions[5 + i * 4];
            float curl = Vector3.Distance(finger_tip, finger_base);
            if (curl < 0.02f) return 0f;
            result += curl * 0.33f;
        }
        return result * 3f;
    }

    float FingerSpread()
    {
        float result = 0f;
        for (int i = 0; i < 3; i++)
        {
            Vector3 finger_tip = mp.LeftLandmarkPositions[8 + i * 4];
            Vector3 next_tip = mp.LeftLandmarkPositions[12 + i * 4];
            float spread = Vector3.Distance(finger_tip, next_tip);
            if (spread < 0.02f) return 0f;
            result += spread * 0.33f;
        }
        return result * 5f;
    }

    float ThumbVector()
    {
        Vector3 thumb_to_palm = mp.LeftLandmarkPositions[4] - mp.LeftLandmarkPositions[1];
        Vector3 thumb = mp.LeftLandmarkPositions[4] - mp.LeftLandmarkPositions[3];
        return 1 - Mathf.Abs(Vector3.Dot(thumb_to_palm.normalized, thumb.normalized));
    }
}
