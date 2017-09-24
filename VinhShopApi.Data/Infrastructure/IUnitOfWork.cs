namespace VinhShopApi.Data.Infrastructure
{
    public interface IUnitOfWork
    {
        void Commit();
    }
}