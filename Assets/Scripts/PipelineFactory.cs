public class PipelineFactory
{
    private static PipelineFactory instance = null;
    private static readonly object padlock = new object();
    private RoomHelper roomHelper;
    private ModelHelper modelHelper;
    private CameraHelper cameraHelper;

    private PipelineFactory(RoomHelper roomHelper, ModelHelper modelHelper, CameraHelper cameraHelper)
    {
        this.roomHelper = roomHelper;
        this.modelHelper = modelHelper;
        this.cameraHelper = cameraHelper;

    }

    public static PipelineFactory GetInstance(RoomHelper roomHelper, ModelHelper modelHelper, CameraHelper cameraHelper)
    {
        lock (padlock)
        {
            if (instance == null)
            {
                instance = new PipelineFactory(roomHelper, modelHelper,cameraHelper);
            }
            return instance;
        }
    }


    public Pipeline getPipeline(int id)
    {
        switch (id)
        {
            case PipelineType.SINGLE_PIPELINE:
                return new SinglePipeline(this.roomHelper, this.modelHelper,this.cameraHelper);
            case PipelineType.ROOM_PIPELINE:
                return new RoomPipeline(this.roomHelper, this.modelHelper, this.cameraHelper);
            default:
                return new SinglePipeline(this.roomHelper, this.modelHelper, this.cameraHelper);
        }
    }

}