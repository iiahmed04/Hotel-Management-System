namespace HMS.Shared.DTOs.AuthDTOs
{
    public class GetUserDTO
    {
        public string Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
        public bool IsActive { get; set; }
    }
}
