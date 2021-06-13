
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class AbstractPipeline
{
    public abstract int PipeLineType { get; set; }
    public abstract GameObject getRoomObject();
    public abstract GameObject getModelObject();

    public abstract void init(string dataPath, string rootRoomPath, string rootMaterialPath, string[] categories);
    public abstract void setupRoom(GameObject room);
    public abstract void setupModel(GameObject model);
    public abstract void execute();

}


public class Pipeline : AbstractPipeline
{
    protected RoomHelper roomHelper;
    protected ModelHelper modelHelper;

    protected List<string> roomPaths = new List<string>();
    protected List<string> roomMtlPaths = new List<string>();
    protected string[] categories;
    protected string rootRoomPath, rootMaterialPath, dataPath;
    private GameObject room, model;
    protected System.Random random = new System.Random();


    private GameObjectHandler gameObjectHandler;

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
            roomPaths.AddRange(Directory.GetFiles(dir, "*.obj"));
            roomMtlPaths.AddRange(Directory.GetFiles(dir, "*.mtl"));
        }
    }

    public override void init(string dataPath, string rootRoomPath, string rootMaterialPath, string[] categories)
    {
        this.dataPath = dataPath;
        this.rootMaterialPath = rootMaterialPath;
        this.rootRoomPath = rootRoomPath;
        this.categories = categories;
        updateRoomPathList(rootRoomPath);
        gameObjectHandler = GameObjectHandler.GetInstance();
    }

    public override void execute()
    {

        

    }

    public override void setupRoom(GameObject room)
    {
        Debug.Log(room != null);
        roomHelper.changeShader(room);
        roomHelper.preprocessRoom(room);
        roomHelper.applyTextures(room, rootMaterialPath);
        //roomHelper.attachMeshColliders(room);

    }

    public override void setupModel(GameObject model)
    {
        modelHelper.setupModel(model);
    }
}

public static class PipelineType
{
    public const int SINGLE_PIPELINE = 1;
    public const int ROOM_PIPELINE = 2;
}