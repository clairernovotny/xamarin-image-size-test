namespace ImageBrowser.Data
{
    public interface ISyncItem<T>
    {
        void Sync(T other);
        bool NeedSync(T other);
    }
}
