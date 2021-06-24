using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dummiesman;
using UnityEngine;
using UnityEngine.UI;

public class RoomPipeline : Pipeline
{
    private int pipelineType = PipelineType.ROOM_PIPELINE;
   
    public RoomPipeline(RoomHelper roomHelper, ModelHelper modelHelper, CameraHelper cameraHelper, LightManager lightHelper)
    {
        base.roomHelper = roomHelper;
        base.modelHelper = modelHelper;
        base.cameraHelper = cameraHelper;
        base.lightHelper = lightHelper;
    }

    public override void init(string dataPath, string rootRoomPath, string rootMaterialPath, string categoriesInput, string totalModelCount, string imagesPerCategory)
    {
        base.init(dataPath, rootRoomPath, rootMaterialPath, categoriesInput, totalModelCount, imagesPerCategory);
        int modelCount = 0;

        foreach (string category in this.categories)
        {
            Debug.Log("category");
            Debug.Log(category);
            int catagoryCount = 0;
            string categoryPath = this.dataPath + Path.DirectorySeparatorChar + category;
            string[] folders = Directory.GetDirectories(categoryPath, "*", System.IO.SearchOption.TopDirectoryOnly);

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

                //Debug.Log("catagoryCount");
                //Debug.Log(catagoryCount);
                //Debug.Log("modelCount");
                //Debug.Log(modelCount);
            }

        }
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
        string objPath = modelPaths[base.currentModelNumber];
        string mtlPath = modelMtlPaths[base.currentModelNumber];
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