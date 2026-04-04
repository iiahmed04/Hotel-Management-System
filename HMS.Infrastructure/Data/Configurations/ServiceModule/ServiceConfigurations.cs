using HMS.Core.Entities.ServiceEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Infrastructure.Data.Configurations.ServiceModule
{
    internal class ServiceConfigurations : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.Property(x => x.Description).HasMaxLength(500);

            builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        }
    }
}
