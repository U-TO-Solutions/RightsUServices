namespace UTO.Framework.Shared.Interfaces
{
    public interface INoSQLProvider<T>
    {
        bool Get(string collectionName, int collectionId);

        void Insert(string strInputMessage);

        string Delete();
    }
}
