using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class ModelHelper : Helper
{
    private Shader shader;

    public ModelHelper(Shader shader)
    {
        this.shader = shader;
    }

    public void setupModel(GameObject model, Vector3 origin)
    {
        //Debug.Log(model.transform.localScale.y);
        //Debug.Log(model.transform.lossyScale.y);
        //Debug.Log(model.transform.position);
        //Debug.Log(model.transform.localPosition);


        model.name = "target_model";
        model.transform.position = origin + new Vector3(0, GetMeshHierarchyBounds(model).size.y / 2, 0);
        model.AddComponent<MeshRenderer>();
        Rigidbody gameObjectsRigidBody = model.AddComponent<Rigidbody>();
        model.GetComponent<Rigidbody>().isKinematic = false;
        model.GetComponent<Rigidbody>().collisionDetectionMode = UnityEngine.CollisionDetectionMode.Continuous;
        model.GetComponent<Rigidbody>().useGravity = false;

        //model.GetComponent<Rigidbody>().drag = 10;
        //model.GetComponent<Rigidbody>().mass = 10;
        base.attachMeshColliders(model);
        changeShader(model);

    }

    public void changeTexutre(GameObject model, string category, string rootTexturePath)
    {
        string[] dirList = Directory.GetDirectories(rootTexturePath);
        string textureFolder = rootTexturePath + Path.DirectorySeparatorChar + category;
        if (dirList.Contains(textureFolder))//Check if there is a texturefolder with name same as category
        {
            string[] textureList = Directory.GetFiles(textureFolder);
            string[] textureDirList = Directory.GetDirectories(textureFolder);

            string[] combinedList = textureList.ToList().Concat(textureDirList.ToList()).ToArray();

            Transform[] allChildren = model.GetComponentsInChildren<Transform>();
            string texturePath = combinedList[random.Next(combinedList.Length)];

            textureChildren(model.gameObject, texturePath);
        }
    }

    public void changeShader(GameObject gameObject)
    {
        Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            Renderer rend = child.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.shader = shader;
            }
        }
    }
}
