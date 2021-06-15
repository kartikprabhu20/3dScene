using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dummiesman;
using UnityEngine;
using UnityEngine.UI;

public class RoomPipeline : Pipeline
{
    private int pipelineType = PipelineType.ROOM_PIPELINE;
    private IEnumerator models;
   
    private int modelNumber;

    public RoomPipeline(RoomHelper roomHelper, ModelHelper modelHelper, CameraHelper cameraHelper)
    {
        base.roomHelper = roomHelper;
        base.modelHelper = modelHelper;
        base.cameraHelper = cameraHelper;
    }

    public override void init(string dataPath, string rootRoomPath, string rootMaterialPath, InputField categoriesInputField)
    {
        base.init(dataPath, rootRoomPath, rootMaterialPath, categoriesInputField);
        this.modelNumber = 0;

        foreach (string category in base.categories)
        {
            //Debug.Log(category);
            string categoryPath = base.dataPath + Path.DirectorySeparatorChar + category;
            //Debug.Log(categoryPath);

            string[] folders = Directory.GetDirectories(categoryPath, "*", System.IO.SearchOption.TopDirectoryOnly);

            //To teest single model
            //string[] folders = { "/Users/apple/OVGU/Thesis/Dataset/pix3d/model/chair/IKEA_EKENAS/" };

            foreach (string folderPath in folders)
            {
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
            return new OBJLoader().Load(objPath, mtlPath);
        }
        catch
        {
           return new OBJLoader().Load(objPath);
        }
    }

    public override GameObject getModelObject()
    {
        string objPath = modelPaths[this.modelNumber];
        string mtlPath = modelMtlPaths[this.modelNumber];
        this.modelNumber += 1;
        try
        {
            return new OBJLoader().Load(objPath, mtlPath);
        }
        catch
        {
            return new OBJLoader().Load(objPath);
        }
    }
}