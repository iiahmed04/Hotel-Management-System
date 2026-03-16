using HMS.Core.Entities.RoomModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Infrastructure.Data.Configurations.RoomModule
{
    public class RoomImageConfigurations : BaseConfiguration<RoomImage, int>, IEntityTypeConfiguration<RoomImage>
    {
        public new void Configure(EntityTypeBuilder<RoomImage> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500);
        }
    }
}
