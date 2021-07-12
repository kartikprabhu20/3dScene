using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelHelper : Helper
{
    private Shader shader;

    public ModelHelper(Shader shader)
    {
        this.shader = shader;
    }

    public void setupModel(GameObject model)
    {
        //Debug.Log(model.transform.localScale.y);
        //Debug.Log(model.transform.lossyScale.y);
        //Debug.Log(model.transform.position);
        //Debug.Log(model.transform.localPosition);


        model.name = "target_model";
        model.transform.position = new Vector3(0, 0, 0) + new Vector3(0, GetMeshHierarchyBounds(model).size.y/2,0);
        model.AddComponent<MeshRenderer>();
        Rigidbody gameObjectsRigidBody = model.AddComponent<Rigidbody>();
        model.GetComponent<Rigidbody>().useGravity = false;
        //model.GetComponent<Rigidbody>().drag = 10;
        //model.GetComponent<Rigidbody>().mass = 10;
        base.attachMeshColliders(model);
        changeShader(model);
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
