using HMS.Shared.DTOs.FeedbackDTOs;
using HMS.Shared.QueryParameters;
using HMS.Shared.Responses;

namespace HMS.Services.Abstraction
{
    public interface IFeedbackService
    {
        Task<GenericResponse<bool>> SubmitFeedbackAsync(
            string guestId,
            FeedbackQueryParam feedbackQueryParam
        );

        Task<GenericResponse<IEnumerable<GuestFeedbacksDTO>>> GetFeedbacksForGuestAsync(
            string guestId
        );
        Task<GenericResponse<IEnumerable<ReturnedFeedbaskForAdminDTO>>> GetFeedbacksForAdminAsync(
            FeedbackForAdminFilterQueryParam queryParam
        );
        Task<GenericResponse<ReturnedFeedbaskForAdminDTO>> GetFeedbackByIdForAdminAsync(
            int feedbackId
        );
        Task<GenericResponse<bool>> DeleteFeedbackByAdminAsync(int feedbackId);

        Task<
            GenericResponse<IEnumerable<ReturnedModerationLogsForAdmin>>
        > GetModerationLogsForAdminAsync(string? verdict, string? guestId);
    }
}
