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
    string currentCategory = "";
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
        if (base.currentRoom != null)
        {
            Transform[] allChildren = base.currentRoom.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if (child.name != base.currentRoom.name)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        Debug.Log("room list" + base.roomPaths.Count.ToString());

       
        //if (currentCategory != getModelCategory())
        //{
        //    roomCacheDictionary = new Dictionary<string, string>();
        //}

        int roomIndex = 0;
        roomIndex = base.random.Next(base.roomPaths.Count);
        
        string objPath = base.roomPaths[roomIndex];
        string mtlPath = base.roomMtlPaths[roomIndex];

        if (roomCacheDictionary.ContainsKey(objPath))
        {
            //Logger.WriteLine(objPath);
            //Logger.WriteLine(roomCacheDictionary[objPath]);

            //Debug.Log(objPath);
            //Debug.Log(roomCacheDictionary[objPath]);

            base.currentRoom = GameObject.Find(roomCacheDictionary[objPath]);
            Transform[] allChildren = base.currentRoom.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                child.gameObject.SetActive(true);
            }

            base.currentRoom.transform.position = base.origin;
        }
        //else
        //{
        //    //Debug.Log("room index" + objPath);

        //    try
        //    {
        //        base.currentRoom = new OBJLoader().Load(objPath, mtlPath, base.origin);
        //    }
        //    catch
        //    {
        //        base.currentRoom = new OBJLoader().Load(objPath, base.origin);
        //    }

        //    base.currentRoom.name = "room" + roomCacheDictionary.Count.ToString();
        //    roomCacheDictionary.Add(objPath, base.currentRoom.name);

        //}
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

    public override bool replaceModel(MonoBehaviour mono, Vector3 origin)
    {
        Debug.Log("replaceModel");
        List<GameObject> matchingObjects = new List<GameObject>();
        Transform[] allChildren = base.currentRoom.GetComponentsInChildren<Transform>();
        int i = 0;
        foreach (Transform child in allChildren)
        {
            if (child.name.ToLower().Contains(getModelCategory().ToLower()))
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
            return false;
        }

        GameObject reference = matchingObjects[random.Next(matchingObjects.Count)];
        bool rotate = modelHelper.modifyScale(base.currentModel, reference);
        //model.transform.parent = reference.transform.parent;

        Vector3 referencePosition = reference.gameObject.GetComponent<MeshRenderer>().bounds.center;
        base.currentModel.transform.position = new Vector3(referencePosition.x, modelHelper.GetMeshHierarchyBounds(base.currentModel).size.y / 2, referencePosition.z);
        base.currentModel.transform.rotation = reference.transform.rotation;
        reference.SetActive(false);
        GameObject.DestroyImmediate(reference);

        checkIntersection(base.currentRoom, base.currentModel, origin);

        cameraHelper.getMainCamera().transform.LookAt(base.currentModel.transform);
        return rotate;
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