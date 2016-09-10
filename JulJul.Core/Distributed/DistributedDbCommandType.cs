namespace JulJul.Core.Distributed
{
    public enum DistributedDbCommandType
    {
        None,
        Add,
        Update,
        Delete
    }

    public enum CommandBehavior
    {
        PubSub,
        Queue,
        Stack
    }
}