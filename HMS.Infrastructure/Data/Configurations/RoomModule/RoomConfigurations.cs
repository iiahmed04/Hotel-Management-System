using HMS.Core.Entities.RoomEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Infrastructure.Data.Configurations.RoomModule
{
    public class RoomConfigurations : BaseConfiguration<Room, int>, IEntityTypeConfiguration<Room>
    {
        public new void Configure(EntityTypeBuilder<Room> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Id)
                .UseIdentityColumn(100, 1);

            builder.Property(x => x.Description)
                .HasMaxLength(150);

            builder.Property(x => x.PricePerNight)
                .HasPrecision(18, 2);

        }
    }
}
