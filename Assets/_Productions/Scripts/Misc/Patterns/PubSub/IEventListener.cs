namespace CapsaBanting
{
    public interface IEventListener<T> : IEventListenerBase
    {
        void OnEvent(T e);
    }
}