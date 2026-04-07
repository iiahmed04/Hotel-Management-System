using System.Security.Claims;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.FeedbackDTOs;
using HMS.Shared.QueryParameters;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers
{
    public class FeedbacksController : ApiBaseController
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbacksController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        // POST: api/Feedbacks
        [Authorize(Roles = "Guest")]
        [HttpPost]
        public async Task<ActionResult<GenericResponse<bool>>> SubmitFeedback(
            [FromBody] FeedbackQueryParam feedbackQueryParam
        )
        {
            var guestIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _feedbackService.SubmitFeedbackAsync(
                guestIdFromToken!,
                feedbackQueryParam
            );
            return HandleResponse(result);
        }

        // GET: api/Feedbacks
        [Authorize(Roles = "Guest")]
        [HttpGet("my")]
        public async Task<
            ActionResult<GenericResponse<IEnumerable<GuestFeedbacksDTO>>>
        > GetFeedbacksForGuest()
        {
            var guestIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _feedbackService.GetFeedbacksForGuestAsync(guestIdFromToken!);
            return HandleResponse(result);
        }

        // GET : api/Feedbacks
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<
            ActionResult<GenericResponse<IEnumerable<ReturnedFeedbaskForAdminDTO>>>
        > GetFeedbacksForAdmin([FromQuery] FeedbackForAdminFilterQueryParam queryParam)
        {
            var result = await _feedbackService.GetFeedbacksForAdminAsync(queryParam);
            return HandleResponse(result);
        }

        // GET : api/Feedbacks/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<
            ActionResult<GenericResponse<ReturnedFeedbaskForAdminDTO>>
        > GetFeedbackByIdForAdmin(int id)
        {
            var result = await _feedbackService.GetFeedbackByIdForAdminAsync(id);
            return HandleResponse(result);
        }

        // DELETE : api/Feedbacks/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<GenericResponse<bool>>> DeleteFeedbackByAdmin(int id)
        {
            var result = await _feedbackService.DeleteFeedbackByAdminAsync(id);
            return HandleResponse(result);
        }

        // GET : api/Feedbacks/moderation-logs
        [Authorize(Roles = "Admin")]
        [HttpGet("moderation-logs")]
        public async Task<
            ActionResult<GenericResponse<IEnumerable<ReturnedModerationLogsForAdmin>>>
        > GetModerationLogsForAdmin([FromQuery] string? verdict, [FromQuery] string? guestId)
        {
            var result = await _feedbackService.GetModerationLogsForAdminAsync(verdict, guestId);
            return HandleResponse(result);
        }
    }
}
