using System;

namespace VinhShopApi.Data.Infrastructure
{
    public interface IDbFactory : IDisposable
    {
        VinhShopDbContext Init();
    }
}