using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line_Rendering_test : MonoBehaviour {

    private LineRenderer lineRenderer;
    private float counter;
    private float dist;

    public Transform origin;
    public Transform destination;

    public float lineDrawSpeed = 6f;

    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        lineRenderer.startWidth = 0.45f;
        lineRenderer.endWidth = 0.45f;


        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        dist = Vector3.Distance(origin.position, destination.position);
        if (counter < dist)
        {
            counter += .1f / lineDrawSpeed;

            float x = Mathf.Lerp(0, dist, counter);

            Vector3 pointA = origin.position;
            Vector3 pointB = destination.position;

            Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;

            lineRenderer.SetPosition(0, origin.position);
            lineRenderer.SetPosition(1, pointAlongLine);
        }
    }
}
