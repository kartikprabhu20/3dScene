
using System;
using System.Collections.Generic;
using System.IO;
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



    public abstract void init(string dataPath, string rootRoomPath, string rootMaterialPath, string categoriesInput, string totalModelCount, string imagesPerCategory);
    public abstract void setupRoom(GameObject room);
    public abstract void setupModel(GameObject model);
    public abstract void setupCamera(GameObject model);

    public abstract void execute();

}


public class Pipeline : AbstractPipeline
{
    protected RoomHelper roomHelper;
    protected ModelHelper modelHelper;
    protected CameraHelper cameraHelper;
    protected LightManager lightHelper;

    protected int totalModelCount = 0;
    protected int imagesPerCategory = 0;
    protected int currentModelNumber = 0;

    protected List<string> roomPaths = new List<string>();
    protected List<string> roomMtlPaths = new List<string>();

    protected List<string> modelPaths = new List<string>();
    protected List<string> modelMtlPaths = new List<string>();
    protected List<string> modelCategories = new List<string>();

    protected string[] categories;
    protected string rootRoomPath, rootMaterialPath, dataPath;
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
        Debug.Log("updateRoomPathList");
        string[] dirList = Directory.GetDirectories(rootRoomPath);
        foreach (string dir in dirList)
        {
            roomPaths.AddRange(Directory.GetFiles(dir, "*.obj"));
            roomMtlPaths.AddRange(Directory.GetFiles(dir, "*.mtl"));
        }

        roomPaths.AddRange(Directory.GetFiles(rootRoomPath, "*.obj"));
        roomMtlPaths.AddRange(Directory.GetFiles(rootRoomPath, "*.mtl"));
    }

    public override void init(string dataPath, string rootRoomPath, string rootMaterialPath, string categoriesInput, string totalModelCount, string imagesPerCategory)
    {
        Debug.Log("init1");

        this.dataPath = dataPath;
        this.rootMaterialPath = rootMaterialPath;
        this.rootRoomPath = rootRoomPath;
        this.categories = categoriesInput.Split(',');

        updateRoomPathList(rootRoomPath);

        if (!String.IsNullOrEmpty(totalModelCount))
        {
            this.totalModelCount = Int32.Parse(totalModelCount);
        }

        if (!String.IsNullOrEmpty(imagesPerCategory))
        {
            this.imagesPerCategory = Int32.Parse(imagesPerCategory);
        }

    }

    public override void execute()
    {

        

    }

    public override void setupRoom(GameObject room)
    {
        roomHelper.changeShader(room);
        roomHelper.preprocessRoom(room);
        roomHelper.applyTextures(room, rootMaterialPath);
        roomHelper.attachMeshColliders(room);
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
        lightHelper.randomizeLight(model);
    }

    public override string getModelCategory()
    {
        throw new NotImplementedException();
    }
}

public static class PipelineType
{
    public const int SINGLE_PIPELINE = 1;
    public const int ROOM_PIPELINE = 2;
}