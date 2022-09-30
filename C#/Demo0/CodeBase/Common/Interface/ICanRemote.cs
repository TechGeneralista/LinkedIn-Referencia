namespace Common.Interface
{
    public interface ICanRemote
    {
        void Remote(ref Response response, Command command, string[] ids);
    }
}
