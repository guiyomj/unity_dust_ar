using UnityEngine;
using System.Collections;

public class RotateMarker : MonoBehaviour
{

    private Camera cameraToLookAt;

    // Use this for initialization
    void Start()
    {
        cameraToLookAt = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = 180;
        v.z = 0;
        transform.LookAt(cameraToLookAt.transform.position - v);
    }
}
