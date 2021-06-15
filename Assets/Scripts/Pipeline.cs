
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

    public abstract void init(string dataPath, string rootRoomPath, string rootMaterialPath, InputField categoriesInputField);
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

    protected int totalModelCount;

    protected List<string> roomPaths = new List<string>();
    protected List<string> roomMtlPaths = new List<string>();

    protected List<string> modelPaths = new List<string>();
    protected List<string> modelMtlPaths = new List<string>();
    protected List<string> modelCategories = new List<string>();

    protected string[] categories;
    protected string rootRoomPath, rootMaterialPath, dataPath;
    private GameObject room, model;
    protected System.Random random = new System.Random();

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
        string[] dirList = Directory.GetDirectories(rootRoomPath);
        foreach (string dir in dirList)
        {
            Debug.Log("test");
            roomPaths.AddRange(Directory.GetFiles(dir, "*.obj"));
            roomMtlPaths.AddRange(Directory.GetFiles(dir, "*.mtl"));
        }

        roomPaths.AddRange(Directory.GetFiles(rootRoomPath, "*.obj"));
        roomMtlPaths.AddRange(Directory.GetFiles(rootRoomPath, "*.mtl"));
    }

    public override void init(string dataPath, string rootRoomPath, string rootMaterialPath, InputField categoriesInputField)
    {
        this.dataPath = dataPath;
        this.rootMaterialPath = rootMaterialPath;
        this.rootRoomPath = rootRoomPath;
        this.categories = categoriesInputField.text.Split(',');

        updateRoomPathList(rootRoomPath);
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

    public override void setupCamera(GameObject model)
    {
        throw new NotImplementedException();
    }
}

public static class PipelineType
{
    public const int SINGLE_PIPELINE = 1;
    public const int ROOM_PIPELINE = 2;
}