using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelHelper : Helper
{

    public ModelHelper()
    {

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
        //Rigidbody gameObjectsRigidBody = model.AddComponent<Rigidbody>();
        base.attachMeshColliders(model);
    }
}
