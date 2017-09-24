namespace VinhShopApi.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        private VinhShopDbContext dbContext;

        public VinhShopDbContext Init()
        {
            return dbContext ?? (dbContext = new VinhShopDbContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}