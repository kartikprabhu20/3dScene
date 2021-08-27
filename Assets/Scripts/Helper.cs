using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Linq;

public class Helper
{
    public System.Random random = new System.Random();
    private Texture2D tex;

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


    public bool modifyScale(GameObject target, GameObject reference)
    {
        var currentBounds = GetMeshHierarchyBounds(target);
        var currentSize = currentBounds.size;
        var categoryReferenceBounds = reference.GetComponent<Renderer>().bounds;
        var categoryReferenceSize = categoryReferenceBounds.size;

        bool rotate = false;
        if ((categoryReferenceSize.z > categoryReferenceSize.x && currentSize.x > currentSize.z) || (categoryReferenceSize.x > categoryReferenceSize.z && currentSize.z > currentSize.x))
        {
            Debug.Log("modifyScale rotate");
            rotate = true;
        }
        float minimumNewSizeRatio = rotate ? Math.Min(categoryReferenceSize.x / currentSize.z, Math.Min(categoryReferenceSize.y / currentSize.y, categoryReferenceSize.z / currentSize.x))
                                           : Math.Min(categoryReferenceSize.x / currentSize.x, Math.Min(categoryReferenceSize.y / currentSize.y, categoryReferenceSize.z / currentSize.z));
        target.transform.localScale = target.transform.localScale * minimumNewSizeRatio;

        return rotate;
    }

    public void textureChildren(GameObject child, string texturePath)
    {
        Renderer rend = child.GetComponent<Renderer>();
        if (rend != null)
        {
            //Debug.Log("applying texture");
            loadTexture(texturePath, rend);
            if (child.name == "floor")
            {
                rend.material.mainTextureScale = new Vector2(5, 5);//Tiling 10x10
            }
        }

        Transform[] childList = child.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child1 in childList)
        {
            if (child1.name != child.name)
            {
                textureChildren(child1.gameObject, texturePath);
            }
        }
    }

    public void loadTexture(string filePath, Renderer rend)
    {

        if (File.Exists(filePath))
        {
            this.tex = new Texture2D(2, 2);
            byte[] fileData = File.ReadAllBytes(filePath);
            this.tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            rend.material.mainTexture = tex;
        }
        else if (Directory.Exists(filePath))
        {

            string[] textureList = Directory.GetFiles(filePath);

            Debug.Log(filePath);
            rend.material.EnableKeyword("_NORMALMAP");
            rend.material.EnableKeyword("_METALLICGLOSSMAP");

            TextureHelper texHelper = new TextureHelper();
            foreach (string texture in textureList)
            {
                string textureKey = texHelper.getTextureKey(texture);
                rend.material.EnableKeyword(textureKey);

                this.tex = new Texture2D(2, 2);
                byte[] fileData = File.ReadAllBytes(texture);
                this.tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                rend.material.SetTexture(textureKey, this.tex);

            }

        }
    }

}
