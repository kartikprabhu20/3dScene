using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper
{

    public void attachMeshRenderer(GameObject gameObject)
    {
        Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            child.gameObject.AddComponent<MeshRenderer>();
        }
    }

    public void attachMeshColliders(GameObject gameObject)
    {
        Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
        for (int i = 1; i < allChildren.Length; i++)//Skip first child as parent gets returned
        {
                attachMeshCollider(allChildren[i].gameObject);
        }

        attachMeshCollider(gameObject);
    }

    void attachMeshCollider(GameObject child)
    {
        child.AddComponent<MeshCollider>();
        child.GetComponent<MeshCollider>().convex = true;
        child.GetComponent<MeshCollider>().isTrigger = true;
    }


    public Bounds GetMeshHierarchyBounds(GameObject go)
    {
        Bounds bounds = new Bounds(); // Not used, but a struct needs to be instantiated

        if (go.GetComponent<Renderer>() != null)
        {
            bounds = go.GetComponent<Renderer>().bounds;
            // Make sure the parent is included
            //Debug.Log("Found parent bounds: " + bounds);
            //bounds.Encapsulate(go.renderer.bounds);
        }
        foreach (var c in go.GetComponentsInChildren<MeshRenderer>())
        {
            //Debug.Log("Found {0} bounds are {1}", c.name, c.bounds);
            if (bounds.size == Vector3.zero)
            {
                bounds = c.bounds;
            }
            else
            {
                bounds.Encapsulate(c.bounds);
            }
        }
        return bounds;
    }


    public void modifyScale(GameObject target, GameObject reference)
    {
        var currentBounds = GetMeshHierarchyBounds(target);
        var currentSize = currentBounds.size;
        var categoryReferenceBounds = reference.GetComponent<Renderer>().bounds;
        var categoryReferenceSize = categoryReferenceBounds.size;
        float minimumNewSizeRatio = Math.Min(categoryReferenceSize.x / currentSize.x, Math.Min(categoryReferenceSize.y / currentSize.y, categoryReferenceSize.z / currentSize.z));
        //float minimumNewSizeRatio = (categoryReferenceSize.y / currentSize.y); //Only match height
        target.transform.localScale = target.transform.localScale * minimumNewSizeRatio;
    }

}
