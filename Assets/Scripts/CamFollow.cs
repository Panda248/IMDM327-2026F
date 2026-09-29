using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    List<GameObject> bodies; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bodies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Body"));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = Vector3.zero;
        float furthestDistance = 0f;
        if (bodies.Count == 0) return;
        foreach(GameObject body in bodies)
        {
            position += body.transform.position;
            if(body.transform.position.sqrMagnitude > furthestDistance)
            {
                furthestDistance = body.transform.position.magnitude;
            }
        }
        position /= bodies.Count;
        position.z = -1 * (10 + furthestDistance);
        transform.position = position;
    }
}
