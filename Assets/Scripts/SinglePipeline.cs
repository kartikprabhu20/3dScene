using System;
using System.IO;
using Dummiesman;
using UnityEngine;
using UnityEngine.UI;

public class SinglePipeline : Pipeline
{
    private int pipelineType = PipelineType.SINGLE_PIPELINE;
    

    public SinglePipeline(RoomHelper roomHelper, ModelHelper modelHelper,CameraHelper cameraHelper, LightManager lightHelper)
    {
        base.roomHelper = roomHelper;
        base.modelHelper = modelHelper;
        base.cameraHelper = cameraHelper;
        base.lightHelper = lightHelper;
    }

    public override void init(string dataPath, string outputPath, string rootRoomPath, string rootMaterialPath, string categoriesInput, string imagesPerCategory, Vector3 origin)
    {
        base.init(dataPath,outputPath, default_room_path, rootMaterialPath, categoriesInput, imagesPerCategory,origin);

    }

    public override int PipeLineType
    {
        get { return this.pipelineType; }
    }

    public override GameObject getRoomObject()
    {
        string objPath = base.roomPaths[0];
        string mtlPath = base.roomMtlPaths.Count > 0 ? base.roomMtlPaths[0] : " ";

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

        if (string.IsNullOrEmpty(base.dataPath))
        {
            this.currentModelNumber = -1;
        }
        base.currentModelNumber += 1;
        string objPath = modelPaths[this.currentModelNumber];
        string mtlPath = modelMtlPaths[this.currentModelNumber];

        //Debug.Log(objPath);
        //Debug.Log(base.origin);
        //Debug.Log(categories[0]);

        try
        {
            base.currentModel = new OBJLoader().Load(objPath, mtlPath, base.origin);
        }
        catch
        {
            base.currentModel = new OBJLoader().Load(objPath, base.origin);
        }

        base.currentModel.gameObject.transform.position = base.origin;
        return base.currentModel;
    }

    public override void replaceModel(MonoBehaviour mono, Vector3 origin)
    {
        return;
    }

}