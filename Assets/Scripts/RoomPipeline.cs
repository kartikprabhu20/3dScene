using System.Collections;
using System.IO;
using Dummiesman;
using UnityEngine;

public class RoomPipeline : Pipeline
{
    private int pipelineType = PipelineType.ROOM_PIPELINE;
    private IEnumerator models;

    public RoomPipeline(RoomHelper roomHelper, ModelHelper modelHelper)
    {
        base.roomHelper = roomHelper;
        base.modelHelper = modelHelper;
    }

    public override void init(string dataPath, string rootRoomPath, string rootMaterialPath, string[] categories)
    {
        base.init(dataPath, rootRoomPath, rootMaterialPath, categories);
        this.models = getModelObj();
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

        Debug.Log(objPath);
         Debug.Log(mtlPath);

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
        models.MoveNext();
        return (GameObject)this.models.Current;
    }

    private IEnumerator getModelObj()
    {
        GameObject model;
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
                try
                {
                    string mtlPath = Directory.GetFiles(folderPath, "*.mtl")[0];
                    model =  new OBJLoader().Load(objPath, mtlPath);
                }
                catch
                {
                    model = new OBJLoader().Load(objPath);
                }

                Debug.Log(objPath);
                yield return model;

            }
        }
    }
}