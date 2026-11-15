using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixGrassBounds : MonoBehaviour
{
    void Start()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter != null && filter.sharedMesh != null)
        {
            // Expands the CPU bounding box into a 3x3x3 cube
            filter.sharedMesh.bounds = new Bounds(Vector3.zero, new Vector3(3f, 3f, 3f));
        }
    }
}
