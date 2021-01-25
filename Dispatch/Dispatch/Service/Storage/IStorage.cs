namespace Dispatch.Service.Storage
{
    public interface IStorage<T>
    {
        T Load(string path);

        void Save(T value, string path);
    }
}
