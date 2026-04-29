using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace ProductManager.EntityFrameworkCore;

public static class ProductManagerDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<ProductManagerDbContext> builder, string connectionString)
    {
        builder.UseSqlServer(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<ProductManagerDbContext> builder, DbConnection connection)
    {
        builder.UseSqlServer(connection);
    }
}
