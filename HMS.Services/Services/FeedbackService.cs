using System.Linq.Expressions;
using AutoMapper;
using HMS.Core.Contracts;
using HMS.Core.Entities.FeedbackEntities;
using HMS.Core.Entities.ServiceEntities;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.FeedbackDTOs;
using HMS.Shared.QueryParameters;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HMS.Services.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOpenAiModerationService _openAiModerationService;
        private readonly ILogger<FeedbackService> _logger;
        private readonly IMapper _mapper;

        public FeedbackService(
            IUnitOfWork unitOfWork,
            IOpenAiModerationService openAiModerationService,
            ILogger<FeedbackService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _openAiModerationService = openAiModerationService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<GenericResponse<bool>> DeleteFeedbackByAdminAsync(int feedbackId)
        {
            var genericReponse = new GenericResponse<bool>();

            try
            {
                var feedback = await _unitOfWork
                    .GetRepository<Feedback, int>()
                    .GetByIdAsync(feedbackId);

                if (feedback is null)
                {
                    genericReponse.StatusCode = StatusCodes.Status404NotFound;
                    genericReponse.Message = $"Feedback with Id : {feedbackId} not found to delete";
                    return genericReponse;
                }

                _unitOfWork.GetRepository<Feedback, int>().Delete(feedback);

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (!result)
                {
                    genericReponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericReponse.Message = $"Failed to delete feedback with Id : {feedbackId}";
                    return genericReponse;
                }

                genericReponse.StatusCode = StatusCodes.Status200OK;
                genericReponse.Message = $"Feedback with Id : {feedbackId} deleted successfully";
                genericReponse.Data = true;

                return genericReponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected Error to delete feedback");
                genericReponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericReponse.Message = $"Failed to delete feedback with Id : {feedbackId}";
                return genericReponse;
            }
        }

        public async Task<
            GenericResponse<ReturnedFeedbaskForAdminDTO>
        > GetFeedbackByIdForAdminAsync(int feedbackId)
        {
            var genericResponse = new GenericResponse<ReturnedFeedbaskForAdminDTO>();

            var feedback = await _unitOfWork
                .GetRepository<Feedback, int>()
                .GetByIdAsync(feedbackId, null, [f => f.Guest]);

            if (feedback is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = $"Feedback with Id : {feedback} not found";
                return genericResponse;
            }

            var mappedData = _mapper.Map<ReturnedFeedbaskForAdminDTO>(feedback);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Feedback retrieved Successfully";
            genericResponse.Data = mappedData;

            return genericResponse;
        }

        public async Task<
            GenericResponse<IEnumerable<ReturnedFeedbaskForAdminDTO>>
        > GetFeedbacksForAdminAsync(FeedbackForAdminFilterQueryParam queryParam)
        {
            var genericResponse = new GenericResponse<IEnumerable<ReturnedFeedbaskForAdminDTO>>();

            Expression<Func<Feedback, bool>> filter = f =>
                (
                    (queryParam!.GuestId == null || f.GuestId == queryParam.GuestId)
                    && (queryParam.ServiceId == null || f.ServiceId == queryParam.ServiceId)
                    && (queryParam.Rating == null || f.Rating == queryParam.Rating)
                );

            var feedbacks = await _unitOfWork
                .GetRepository<Feedback, int>()
                .GetAllAsync(filter, null, null, [f => f.Service!, f => f.Guest]);

            if (feedbacks is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No feedbacks founded to return";
                return genericResponse;
            }

            var mappedData = _mapper.Map<IEnumerable<ReturnedFeedbaskForAdminDTO>>(feedbacks);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Feedbacks retrieved Successfully";
            genericResponse.Data = mappedData;

            return genericResponse;
        }

        public async Task<
            GenericResponse<IEnumerable<GuestFeedbacksDTO>>
        > GetFeedbacksForGuestAsync(string guestId)
        {
            var genericResponse = new GenericResponse<IEnumerable<GuestFeedbacksDTO>>();

            var feedbacks = await _unitOfWork
                .GetRepository<Feedback, int>()
                .GetAllAsync(
                    f => f.ModerationStatus == ModerationStatus.Approved,
                    null,
                    null,
                    [f => f.Service!]
                );

            if (feedbacks is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No feedbacks found for the guest.";
                return genericResponse;
            }

            var mappedData = _mapper.Map<IEnumerable<GuestFeedbacksDTO>>(feedbacks);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Feedbacks retrieved Successfully";
            genericResponse.Data = mappedData;

            return genericResponse;
        }

        public async Task<
            GenericResponse<IEnumerable<ReturnedModerationLogsForAdmin>>
        > GetModerationLogsForAdminAsync(string? verdict, string? guestId)
        {
            var genericResponse =
                new GenericResponse<IEnumerable<ReturnedModerationLogsForAdmin>>();

            Enum.TryParse(verdict, out Verdict parsedVerdict);
            Expression<Func<ModerationLog, bool>>? filter = log =>
                (verdict == null || log.Verdict == parsedVerdict)
                && (guestId == null || log.GuestId == guestId);

            var moderationLogs = await _unitOfWork
                .GetRepository<ModerationLog, int>()
                .GetAllAsync(filter, null, null, [f => f.Guest]);

            if (!moderationLogs.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No moderation logs founded";
                return genericResponse;
            }

            var mappedData = _mapper.Map<IEnumerable<ReturnedModerationLogsForAdmin>>(
                moderationLogs
            );

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Moderation Logs returned successfully";
            genericResponse.Data = mappedData;

            return genericResponse;
        }

        public async Task<GenericResponse<bool>> SubmitFeedbackAsync(
            string guestId,
            FeedbackQueryParam feedbackQueryParam
        )
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                if (string.IsNullOrWhiteSpace(feedbackQueryParam.Content))
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Invalid feedback content . Content cannot be empty";
                    return genericResponse;
                }

                if (feedbackQueryParam.Rating is not null)
                {
                    if (feedbackQueryParam.Rating < 1 || feedbackQueryParam.Rating > 5)
                    {
                        genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                        genericResponse.Message = "Rating must be between 1 and 5.";
                        return genericResponse;
                    }
                }

                if (feedbackQueryParam.ServiceId is not null)
                {
                    var service = await _unitOfWork
                        .GetRepository<Service, int>()
                        .GetByIdAsync(feedbackQueryParam.ServiceId.Value);

                    if (service is null)
                    {
                        genericResponse.StatusCode = StatusCodes.Status404NotFound;
                        genericResponse.Message = "Service not found.";
                        return genericResponse;
                    }
                }

                #region Ai moduration integration Scenario (needed payment in OpenAi)
                //var moderationResult = await _openAiModerationService.CheckContentAsync(
                //    feedbackQueryParam.Content
                //);

                //if (!moderationResult.IsSafe)
                //{
                //    var moderationLog = new ModerationLog
                //    {
                //        GuestId = guestId,
                //        Content = feedbackQueryParam.Content,
                //        Verdict = Verdict.Rejected,
                //        RejectionReason = moderationResult.Reason,
                //        AttempetedAt = DateTime.Now,
                //    };

                //    await _unitOfWork.GetRepository<ModerationLog, int>().AddAsync(moderationLog);

                //    var result = await _unitOfWork.SaveChangesAsync() > 0;

                //    if (!result)
                //    {
                //        genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                //        genericResponse.Message = "Failed to save moderation log.";
                //        return genericResponse;
                //    }

                //    genericResponse.StatusCode = StatusCodes.Status422UnprocessableEntity;
                //    genericResponse.Message =
                //        $"Feedback content rejected by AI moderation. Reason : {moderationResult.Reason}";
                //    genericResponse.Data = false;

                //    return genericResponse;
                //}
                //else
                //{
                //    var feedback = new Feedback
                //    {
                //        GuestId = guestId,
                //        ServiceId = feedbackQueryParam.ServiceId,
                //        Content = feedbackQueryParam.Content,
                //        ModerationStatus = ModerationStatus.Approved,
                //        Rating = feedbackQueryParam.Rating,
                //        SubmittedAt = DateTime.Now,
                //    };

                //    await _unitOfWork.GetRepository<Feedback, int>().AddAsync(feedback);

                //    var moderationLog = new ModerationLog
                //    {
                //        GuestId = guestId,
                //        Content = feedbackQueryParam.Content,
                //        Verdict = Verdict.Approved,
                //        AttempetedAt = DateTime.Now,
                //    };

                //    await _unitOfWork.GetRepository<ModerationLog, int>().AddAsync(moderationLog);

                //    var result = await _unitOfWork.SaveChangesAsync() > 0;

                //    if (!result)
                //    {
                //        genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                //        genericResponse.Message = "Failed to save moderation log.";
                //        return genericResponse;
                //    }

                //    genericResponse.StatusCode = StatusCodes.Status200OK;
                //    genericResponse.Message = $"Feedback submitted successfully";
                //    genericResponse.Data = true;

                //    return genericResponse;
                //}
                #endregion

                var feedback = new Feedback
                {
                    GuestId = guestId,
                    ServiceId = feedbackQueryParam.ServiceId,
                    Content = feedbackQueryParam.Content,
                    ModerationStatus = ModerationStatus.Approved,
                    Rating = feedbackQueryParam.Rating,
                    SubmittedAt = DateTime.Now,
                };

                await _unitOfWork.GetRepository<Feedback, int>().AddAsync(feedback);

                var moderationLog = new ModerationLog
                {
                    GuestId = guestId,
                    Content = feedbackQueryParam.Content,
                    Verdict = Verdict.Approved,
                    AttempetedAt = DateTime.Now,
                };

                await _unitOfWork.GetRepository<ModerationLog, int>().AddAsync(moderationLog);

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (!result)
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to save moderation log.";
                    return genericResponse;
                }

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = $"Feedback submitted successfully";
                genericResponse.Data = true;

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An unexpected error occurred while submitting feedback for guest {GuestId}.",
                    guestId
                );
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to save moderation log.";
                return genericResponse;
            }
        }
    }
}
