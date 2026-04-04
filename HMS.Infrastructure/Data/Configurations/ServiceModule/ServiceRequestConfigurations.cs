using HMS.Core.Entities.ServiceEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Infrastructure.Data.Configurations.ServiceModule
{
    internal class ServiceRequestConfigurations : IEntityTypeConfiguration<ServiceRequest>
    {
        public void Configure(EntityTypeBuilder<ServiceRequest> builder)
        {
            builder.Property(x => x.Notes).HasMaxLength(500);
        }
    }
}
