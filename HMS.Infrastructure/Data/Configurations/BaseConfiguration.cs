using HMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Infrastructure.Data.Configurations
{
    public class BaseConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity<TKey>
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
