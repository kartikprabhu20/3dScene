public class PipelineFactory
{
    private static PipelineFactory instance = null;
    private static readonly object padlock = new object();
    private RoomHelper roomHelper;
    private ModelHelper modelHelper;

    private PipelineFactory(RoomHelper roomHelper, ModelHelper modelHelper)
    {
        this.roomHelper = roomHelper;
        this.modelHelper = modelHelper;

    }

    public static PipelineFactory GetInstance(RoomHelper roomHelper, ModelHelper modelHelper)
    {
        lock (padlock)
        {
            if (instance == null)
            {
                instance = new PipelineFactory(roomHelper, modelHelper);
            }
            return instance;
        }
    }


    public Pipeline getPipeline(int id)
    {
        switch (id)
        {
            case PipelineType.SINGLE_PIPELINE:
                return new SinglePipeline(this.roomHelper, this.modelHelper);
            case PipelineType.ROOM_PIPELINE:
                return new RoomPipeline(this.roomHelper, this.modelHelper);
            default:
                return new SinglePipeline(this.roomHelper, this.modelHelper);
        }
    }

}