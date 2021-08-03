using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dummiesman;
using UnityEngine;
using UnityEngine.UI;

public class RoomPipeline : Pipeline
{
    public string[] exceptionObjects = { "wall", "floor" };

    private int pipelineType = PipelineType.ROOM_PIPELINE;
   
    public RoomPipeline(RoomHelper roomHelper, ModelHelper modelHelper, CameraHelper cameraHelper, LightManager lightHelper)
    {
        base.roomHelper = roomHelper;
        base.modelHelper = modelHelper;
        base.cameraHelper = cameraHelper;
        base.lightHelper = lightHelper;
    }

    public override void init(string dataPath, string outputPath, string rootRoomPath, string rootMaterialPath, string categoriesInput, string imagesPerCategory, Vector3 origin)
    {
        base.init(dataPath, outputPath, rootRoomPath, rootMaterialPath, categoriesInput, imagesPerCategory,origin);

    }

    public override int PipeLineType
    {
        get { return this.pipelineType; }
    }

    public override GameObject getRoomObject()
    {
        int roomIndex = base.random.Next(base.roomPaths.Count);
        string objPath = base.roomPaths[roomIndex];
        string mtlPath =  base.roomMtlPaths[roomIndex];

        try
        {
            base.currentRoom = new OBJLoader().Load(objPath, mtlPath, base.origin);
        }
        catch
        {
            base.currentRoom = new OBJLoader().Load(objPath, base.origin);
        }

        base.currentRoom.name = "room";
        return base.currentRoom;
    }

    public override GameObject getModelObject()
    {
        base.currentModelNumber += 1;

        Debug.Log(base.currentModelNumber);
        string objPath = modelPaths[base.currentModelNumber];
        string mtlPath = modelMtlPaths[base.currentModelNumber];

        try
        {
            base.currentModel = new OBJLoader().Load(objPath, mtlPath, base.origin);
        }
        catch
        {
            base.currentModel = new OBJLoader().Load(objPath,base.origin);
        }

        return base.currentModel;
    }

    public override void replaceModel(MonoBehaviour mono, Vector3 origin)
    {
        Debug.Log("replaceModel");
        List<GameObject> matchingObjects = new List<GameObject>();
        Transform[] allChildren = base.currentRoom.GetComponentsInChildren<Transform>();
        int i = 0;
        foreach (Transform child in allChildren)
        {
            if (child.name == getModelCategory())
            {
                i += 1;
                matchingObjects.Add(child.gameObject);
                //Debug.Log(child.name+ i.ToString());
                //child.name = child.name + i.ToString();
            }
        }

        Debug.Log("matchingObjects.Count:" + matchingObjects.Count.ToString());
        if (matchingObjects.Count < 1)
        {
            checkIntersection(base.currentRoom, base.currentModel, origin);
            return;
        }

        GameObject reference = matchingObjects[random.Next(matchingObjects.Count)];
        modelHelper.modifyScale(base.currentModel, reference);
        //model.transform.parent = reference.transform.parent;

        base.currentModel.transform.position = reference.gameObject.GetComponent<MeshRenderer>().bounds.center;
        base.currentModel.transform.rotation = reference.transform.rotation;
        reference.SetActive(false);
        GameObject.DestroyImmediate(reference);

        checkIntersection(base.currentRoom, base.currentModel, origin);

        cameraHelper.getMainCamera().transform.LookAt(base.currentModel.transform);
        return;
    }

    private void checkIntersection(GameObject room, GameObject model, Vector3 origin)
    {
        Debug.Log("checkIntersection");
        Transform[] allChildren = room.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child != null && model.GetComponent<Collider>().bounds.Intersects(child.gameObject.GetComponent<Collider>().bounds))//If target object is intersecting with gameobject
            {
                int pos = Array.IndexOf(exceptionObjects, child.name);
                if (pos == -1) //If exceptionObjects string array doesnt have the name of the child, delete the child
                {
                    Debug.Log("Bounds intersecting");
                    if (child.name == "bed")
                    {
                        string[] objectsToDestory = { "duvet", "pillow" };
                        foreach (string objectToDestroy in objectsToDestory)
                        {
                            FindAndDestroy(room, objectToDestroy);
                        }
                    }
                    GameObject.DestroyImmediate(child.gameObject);
                }
                //else if (pos == 0)//if wall is intersecting then go back to origin
                //{
                //    Debug.Log("Bounds intersecting");
                //    model.transform.position = origin;
                //    checkIntersection(room,model, origin);
                //}
            }
        }
        return;
    }

    private void FindAndDestroy(GameObject room, string objectName)
    {
        try
        {
            Transform[] allChildren = room.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if (child.name == objectName)
                {
                    GameObject objectToDestroy = child.gameObject;
                    if (objectToDestroy != null)
                    {
                        GameObject.DestroyImmediate(objectToDestroy);
                    }
                }
            }
        }
        catch
        {

        }
    }
}