namespace HMS.Core.Entities.ServiceEntities
{
    public class Service : BaseEntity<int>
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = [];
    }
}
