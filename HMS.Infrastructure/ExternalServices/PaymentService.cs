using System.Net.Http.Json;
using System.Text.Json;
using HMS.Core.Contracts;
using HMS.Core.Entities.BookingEntities;
using HMS.Services.Abstraction;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace HMS.Infrastructure.ExternalServices
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<PayMobSettings> _options;
        private readonly HttpClient _httpClient;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IOptions<PayMobSettings> options,
            HttpClient httpClient
        )
        {
            _unitOfWork = unitOfWork;
            _options = options;
            _httpClient = httpClient;
        }

        public async Task<GenericResponse<string>> CreatePaymentUrlAsync(Guid bookingId)
        {
            var genericResponse = new GenericResponse<string>();

            var booking = await _unitOfWork
                .GetRepository<Booking, Guid>()
                .GetByIdAsync(bookingId, null, [B => B.HotelUser]);

            if (booking is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Booking not found to Pay";

                return genericResponse;
            }

            //Get Auth Token
            var authToken = await AuthenticateAsync();

            //Create Order [Intent]
            var orderId = await CreateOrderAsync(authToken, booking.TotalAmount, booking.Currency);

            if (orderId is null)
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to create intent in PayMob";

                return genericResponse;
            }

            booking.PayMobOrderId = orderId;

            //Create PaymentKey
            var paymentKey = await CreatePaymentKeyAsync(
                authToken,
                orderId,
                booking.TotalAmount,
                booking.Currency,
                booking.HotelUser.Email!,
                booking.HotelUser.FullName,
                booking.HotelUser.PhoneNumber!
            );

            if (paymentKey is null)
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to create payment in PayMob";

                return genericResponse;
            }

            booking.PayMobPaymentKey = paymentKey;
            booking.PaidDate = DateTime.Now;

            booking.Status = BookingStatus.Paid;
            booking.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Booking, Guid>().Update(booking);
            booking.UpdatedAt = DateTime.Now;

            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (!result)
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to create payment Link";

                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Success to create payment";
            genericResponse.Data =
                $"{_options.Value.BaseUrl}/acceptance/iframes/{_options.Value.IFrameId}?payment_token={paymentKey}";

            return genericResponse;
        }

        private async Task<string> AuthenticateAsync()
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_options.Value.BaseUrl}/auth/tokens",
                new { api_key = _options.Value.ApiKey }
            );

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            return json.GetProperty("token").GetString()!;
        }

        private async Task<string> CreateOrderAsync(
            string authToken,
            decimal amount,
            string currency
        )
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_options.Value.BaseUrl}/ecommerce/orders",
                new
                {
                    auth_token = authToken,
                    delivery_needed = false,
                    amount_cents = (int)(amount * 100),
                    currency = currency,
                    items = Array.Empty<object>(),
                }
            );

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            return json.GetProperty("id").GetInt32().ToString();
        }

        private async Task<string> CreatePaymentKeyAsync(
            string authToken,
            string orderId,
            decimal amount,
            string currency,
            string email,
            string fullName,
            string phoneNumber
        )
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_options.Value.BaseUrl}/acceptance/payment_keys",
                new
                {
                    auth_token = authToken,
                    amount_cents = (int)(amount * 100),
                    currency = currency,
                    order_id = orderId,
                    expiration = 3600,
                    integration_id = _options.Value.IntegrationId,
                    billing_data = new
                    {
                        email = email,
                        first_name = fullName.Split(' ')[0],
                        last_name = fullName.Split(' ')[1],
                        phone_number = phoneNumber,
                        apartment = "NA",
                        floor = "NA",
                        street = "NA",
                        building = "NA",
                        city = "Cairo",
                        country = "EG",
                        state = "cairo",
                    },
                }
            );

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            return json.GetProperty("token").GetString()!;
        }
    }
}
