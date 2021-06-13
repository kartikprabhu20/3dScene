using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper
{
    public void attachMeshColliders(GameObject gameObject)
    {
        Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            attachMeshCollider(child.gameObject);
        }
    }

    void attachMeshCollider(GameObject child)
    {
        child.AddComponent<MeshCollider>();
        child.GetComponent<MeshCollider>().convex = true;
    }

}
