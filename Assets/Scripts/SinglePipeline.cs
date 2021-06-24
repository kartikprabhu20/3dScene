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

    public override void init(string dataPath, string rootRoomPath, string rootMaterialPath, string categoriesInput, string totalModelCount, string imagesPerCategory)
    {
        base.init(dataPath, default_room_path, rootMaterialPath, categoriesInput, totalModelCount, imagesPerCategory);

        int modelCount = 0;

        foreach (string category in this.categories)
        {
            Debug.Log("category");
            Debug.Log(category);
            int catagoryCount = 0;
            string categoryPath = this.dataPath + Path.DirectorySeparatorChar + category;
            string[] folders = Directory.GetDirectories(categoryPath, "*", System.IO.SearchOption.TopDirectoryOnly);

            while (catagoryCount != this.imagesPerCategory)
            {
                foreach (string folderPath in folders)
                {
                    catagoryCount++;
                    modelCount++;

                    string objPath = Directory.GetFiles(folderPath, "*.obj")[0];
                    string mtlPath = "";
                    try
                    {
                        mtlPath = Directory.GetFiles(folderPath, "*.mtl")[0];
                    }
                    catch
                    {
                    }

                    modelPaths.Add(objPath);
                    modelMtlPaths.Add(mtlPath);
                    modelCategories.Add(category);
                    this.totalModelCount += 1;

                    if(catagoryCount == this.imagesPerCategory)
                    {
                        break;
                    }

                    //Debug.Log("catagoryCount");
                    //Debug.Log(catagoryCount);
                    //Debug.Log("modelCount");
                    //Debug.Log(modelCount);
                }
            }

        }

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
            base.currentRoom = new OBJLoader().Load(objPath, mtlPath);
        }
        catch
        {
            base.currentRoom = new OBJLoader().Load(objPath);
        }
        return base.currentRoom;
    }

    public override GameObject getModelObject()
    {

        if (string.IsNullOrEmpty(base.dataPath))
        {
            this.currentModelNumber = 0;
        }
        string objPath = modelPaths[this.currentModelNumber];
        string mtlPath = modelMtlPaths[this.currentModelNumber];
        base.currentModelNumber += 1;

        try
        {
            base.currentModel = new OBJLoader().Load(objPath, mtlPath);
        }
        catch
        {
            base.currentModel = new OBJLoader().Load(objPath);
        }

        return base.currentModel;
    }

    public override string getModelCategory()
    {
        return modelCategories[base.currentModelNumber];
    }
}