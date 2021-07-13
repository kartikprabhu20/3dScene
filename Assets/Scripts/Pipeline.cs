
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractPipeline
{
    public abstract int PipeLineType { get; set; }
    public abstract GameObject getRoomObject();
    public abstract GameObject getModelObject();
    public abstract int getModelCount();
    public abstract void setModelCount(int modelCount);
    public abstract string getModelCategory();
    public abstract string getModelName();

    public abstract void init(string dataPath, string outputPath, string rootRoomPath, string rootMaterialPath, string categoriesInput, string imagesPerCategory);
    public abstract void setupRoom(GameObject room);
    public abstract void setupModel(GameObject model);
    public abstract void setupCamera(GameObject model);
    public abstract List<GameObject> setupLigtSources(GameObject model, GameObject room);

    public abstract void execute(MonoBehaviour mono);

}


public class Pipeline : AbstractPipeline
{
    protected RoomHelper roomHelper;
    protected ModelHelper modelHelper;
    protected CameraHelper cameraHelper;
    protected LightManager lightHelper;
    protected CustomImageSynthesis customImageSynthesis;

    protected int totalModelCount = 0;
    protected int imagesPerCategory = 0;
    protected int currentModelNumber = -1;
    protected bool incrementCategoryCounter = false;

    protected List<string> roomPaths = new List<string>();
    protected List<string> roomMtlPaths = new List<string>();

    protected List<string> modelPaths = new List<string>();
    protected List<string> modelMtlPaths = new List<string>();
    protected List<string> modelCategories = new List<string>();
    protected List<string> modelNames = new List<string>();

    protected string[] categories;
    protected string rootRoomPath, rootMaterialPath, dataPath,outputPath;
    protected GameObject currentRoom, currentModel;
    protected System.Random random = new System.Random();

    protected const string default_room_path = "Assets/resources/default_room/";
    protected const string default_model_path = "Assets/resources/default_model/model.obj";

    public override int PipeLineType { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    
    public override GameObject getRoomObject()
    {
        throw new System.NotImplementedException();
    }

    public override GameObject getModelObject()
    {
        throw new System.NotImplementedException();
    }

    void updateRoomPathList(string rootRoomPath)
    {
        //Debug.Log("updateRoomPathList");
        string[] dirList = Directory.GetDirectories(rootRoomPath);
        foreach (string dir in dirList)
        {
            roomPaths.AddRange(Directory.GetFiles(dir, "*.obj"));
            roomMtlPaths.AddRange(Directory.GetFiles(dir, "*.mtl"));
        }

        roomPaths.AddRange(Directory.GetFiles(rootRoomPath, "*.obj"));
        roomMtlPaths.AddRange(Directory.GetFiles(rootRoomPath, "*.mtl"));
    }

    public override void init(string dataPath, string outputPath, string rootRoomPath, string rootMaterialPath, string categoriesInput, string imagesPerCategory)
    {
        //Debug.Log("init1");

        this.dataPath = dataPath;
        this.outputPath = outputPath;
        this.rootMaterialPath = rootMaterialPath;
        this.rootRoomPath = rootRoomPath;
        this.categories = categoriesInput.Split(',');

        updateRoomPathList(rootRoomPath);

        if (!String.IsNullOrEmpty(imagesPerCategory))
        {
            this.imagesPerCategory = Int32.Parse(imagesPerCategory);
            this.incrementCategoryCounter = true;
        }

        this.customImageSynthesis = new CustomImageSynthesis(cameraHelper.getMainCamera());

    }

    public override void execute(MonoBehaviour mono)
    {
        string destinationPath = outputPath + Path.DirectorySeparatorChar + getModelCategory() + Path.DirectorySeparatorChar + getModelName();

        //Debug.Log(getModelCategory()+ " "+ getModelName());
        //Debug.Log(destinationPath);
        //Debug.Log(getModelCategory());

        Directory.CreateDirectory(destinationPath);
        var files = Directory.GetFiles(destinationPath, "*.*", SearchOption.AllDirectories);
        int targetName = -1;
        string str2 = string.Empty;
        foreach (string filePath in files)
        {
            string fileName = Path.GetFileName(filePath);
            string number = new String(fileName.Where(Char.IsDigit).ToArray());
            Debug.Log(number);
            targetName = (targetName <= Int32.Parse(number)) ? Int32.Parse(number) : targetName;
        }

        targetName = (targetName == -1) ? 0 : targetName + 1;

        this.customImageSynthesis.OnSceneChange();
        mono.StartCoroutine(this.customImageSynthesis.Save(targetName.ToString(), currentRoom, currentModel, -1, -1, destinationPath));

    }

    public override void setupRoom(GameObject room)
    {
        roomHelper.changeShader(room);
        roomHelper.preprocessRoom(room);
        roomHelper.applyTextures(room, rootMaterialPath);
    }

    public override void setupModel(GameObject model)
    {
        modelHelper.setupModel(model);
    }

    public override int getModelCount()
    {
        return totalModelCount;
    }

    public override void setModelCount(int modelCount)
    {
        this.totalModelCount = modelCount;
    }

    public override void setupCamera(GameObject model)
    {
        cameraHelper.randomizeCamera(currentRoom, model, true);
    }

    public override List<GameObject> setupLigtSources(GameObject model, GameObject room)
    { 
        return lightHelper.randomizeLight(model,room);
    }

    public override string getModelCategory()
    {
        return modelCategories[currentModelNumber];
    }

    public override string getModelName()
    {
        return modelNames[currentModelNumber];
    }
}

public static class PipelineType
{
    public const int SINGLE_PIPELINE = 1;
    public const int ROOM_PIPELINE = 2;
}