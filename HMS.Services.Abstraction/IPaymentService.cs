using HMS.Shared.Responses;

namespace HMS.Services.Abstraction
{
    public interface IPaymentService
    {
        Task<GenericResponse<string>> CreatePaymentUrlAsync(Guid bookingId);
    }
}
