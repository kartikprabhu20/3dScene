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
        model.name = "target_model";
        model.transform.position = new Vector3(0, 0, 0);
        model.AddComponent<MeshRenderer>();
        //Rigidbody gameObjectsRigidBody = model.AddComponent<Rigidbody>();
        base.attachMeshColliders(model);
    }
}
