using System.Linq.Expressions;
using AutoMapper;
using HMS.Core.Contracts;
using HMS.Core.Entities.BookingEntities;
using HMS.Core.Entities.IdentityEntities;
using HMS.Core.Entities.ServiceEntities;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.ServiceDTOs;
using HMS.Shared.QueryParameters;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HMS.Services.Services
{
    public class HotelServicesManagementService : IHotelServicesManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<HotelServicesManagementService> _logger;

        public HotelServicesManagementService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<HotelServicesManagementService> logger
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GenericResponse<bool>> AssignStaffToServiceRequestByAdminAsync(
            int serviceRequestId,
            string staffId
        )
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                var serviceRequest = await _unitOfWork
                    .GetRepository<ServiceRequest, int>()
                    .GetByIdAsync(serviceRequestId, x => x.Status == Status.Pending, null);

                if (serviceRequest is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message =
                        "Service Request not found or cannot assign staff to a non-pending request.";
                    return genericResponse;
                }

                serviceRequest.AssignedStaffId = staffId;
                serviceRequest.Status = Status.Assigned;
                serviceRequest.UpdatedAt = DateTime.Now;

                _unitOfWork.GetRepository<ServiceRequest, int>().Update(serviceRequest);

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (!result)
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to assign staff to service";
                    return genericResponse;
                }

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message =
                    $"Staff with id ; {staffId} assign with service : {serviceRequest.ServiceId} successfully";
                genericResponse.Data = true;

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error while assign staff");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to assign staff to service";
                return genericResponse;
            }
        }

        public async Task<GenericResponse<bool>> CreateHotelServiceByAdminAsync(
            CreateOrUpdateHotelServiceDTO createHotelServiceDTO
        )
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                if (createHotelServiceDTO is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Invalide service data.";
                    return genericResponse;
                }

                var serviceToAdd = _mapper.Map<Service>(createHotelServiceDTO);

                await _unitOfWork.GetRepository<Service, int>().AddAsync(serviceToAdd);

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (!result)
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to create service.";
                    return genericResponse;
                }

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Service Ceated successfully";
                genericResponse.Data = true;

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a hotel service.");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to create service.";
                return genericResponse;
            }
        }

        public async Task<GenericResponse<bool>> CreateServiceRequestByGuestAsync(
            CreateServiceRequestByGuestDTO createServiceRequestByGuestDTO
        )
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                if (createServiceRequestByGuestDTO is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Invalid fields to create service request.";
                    return genericResponse;
                }

                var booking = await _unitOfWork
                    .GetRepository<Booking, Guid>()
                    .GetByIdAsync(createServiceRequestByGuestDTO.BookingId);

                if (booking is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message =
                        "Booking you want to request a service for is not found";
                    return genericResponse;
                }

                var service = await _unitOfWork
                    .GetRepository<Service, int>()
                    .GetByIdAsync(createServiceRequestByGuestDTO.ServiceId);

                if (service is null || service!.IsAvailable == false)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = "Service you want to request is not found";
                    return genericResponse;
                }

                var mappedServiceRequest = _mapper.Map<ServiceRequest>(
                    createServiceRequestByGuestDTO
                );

                await _unitOfWork
                    .GetRepository<ServiceRequest, int>()
                    .AddAsync(mappedServiceRequest);

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (!result)
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to Request a service";
                    return genericResponse;
                }

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Service Request created successfully";
                genericResponse.Data = true;

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexcpected error to request a service");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to Request a service";
                return genericResponse;
            }
        }

        public async Task<GenericResponse<bool>> DeleteHotelServiceByAdminAsync(int id)
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                var service = await _unitOfWork.GetRepository<Service, int>().GetByIdAsync(id);

                if (service is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = $"Service with Id : {id} not found to delete";
                    return genericResponse;
                }

                service.IsAvailable = false; // Soft delete by setting IsAvailable to false
                _unitOfWork.GetRepository<Service, int>().Update(service);
                service.UpdatedAt = DateTime.Now;

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (!result)
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to Delete service.";
                    return genericResponse;
                }

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Service Deleted successfully";
                genericResponse.Data = true;

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexcpected error to delete Service");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to Delete service.";
                return genericResponse;
            }
        }

        public async Task<GenericResponse<bool>> DeleteServiceRequestByGuestAsync(
            int serviceRequestId,
            string guestId
        )
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                Expression<Func<ServiceRequest, bool>> filter = x => x.Status == Status.Pending;

                var serviceRequest = await _unitOfWork
                    .GetRepository<ServiceRequest, int>()
                    .GetByIdAsync(serviceRequestId, filter, null);

                if (serviceRequest == null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = "Service Request to delete is not found";
                    return genericResponse;
                }

                serviceRequest.Status = Status.Cancelled; // Soft delete by setting Status to Cancelled

                _unitOfWork.GetRepository<ServiceRequest, int>().Update(serviceRequest);
                serviceRequest.UpdatedAt = DateTime.Now;

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (!result)
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to cancel service request";
                    return genericResponse;
                }

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Service request cancelled successfully";
                genericResponse.Data = true;

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexcpected error to cancel service request");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to cancel service request";
                return genericResponse;
            }
        }

        public async Task<
            GenericResponse<IEnumerable<HotelServicesDTO>>
        > GetAllHotelServicesForGuestAsync()
        {
            var genericResponse = new GenericResponse<IEnumerable<HotelServicesDTO>>();

            var services = await _unitOfWork
                .GetRepository<Service, int>()
                .GetAllAsync(x => (x.IsAvailable == true), null, null, null);

            if (services is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Services not found.";
                return genericResponse;
            }

            var mappedService = _mapper.Map<IEnumerable<HotelServicesDTO>>(services);

            if (mappedService is null || !mappedService.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Services not found.";
                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Services retrieved successfully.";
            genericResponse.Data = mappedService;

            return genericResponse;
        }

        public async Task<
            GenericResponse<IEnumerable<HotelServicesAdminDTO>>
        > GetAllServiceForAdminAsync(bool? IsAvailable)
        {
            var genericResponse = new GenericResponse<IEnumerable<HotelServicesAdminDTO>>();

            var repo = _unitOfWork.GetRepository<Service, int>();

            IEnumerable<Service> services = [];

            if (IsAvailable is null)
            {
                services = await repo.GetAllAsync();
            }

            if (IsAvailable is not null)
            {
                services = await repo.GetAllAsync(
                    x => x.IsAvailable == IsAvailable,
                    null,
                    null,
                    null
                );
            }

            if (services is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Not Services found";
                return genericResponse;
            }

            var mappedServices = _mapper.Map<IEnumerable<HotelServicesAdminDTO>>(services);

            if (mappedServices is null || !mappedServices.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Services not found.";
                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Services retrieved successfully.";
            genericResponse.Data = mappedServices;

            return genericResponse;
        }

        public async Task<
            GenericResponse<IEnumerable<ServiceRequestForAdminDTO>>
        > GetAllServiceRequestsForAdminAsync(ServiceRequestQueryParam? queryParam)
        {
            var genericResponse = new GenericResponse<IEnumerable<ServiceRequestForAdminDTO>>();

            Enum.TryParse(queryParam!.Status, true, out Status statusValue);
            Expression<Func<ServiceRequest, bool>> filter = x =>
                (queryParam!.Status == null || x.Status == statusValue)
                && (queryParam.ServiceId == null || x.ServiceId == queryParam.ServiceId)
                && (queryParam.StaffId == null || x.AssignedStaffId == queryParam.StaffId);

            var serviceRequests = await _unitOfWork
                .GetRepository<ServiceRequest, int>()
                .GetAllAsync(filter, null, null, [x => x.Booking.HotelUser, x => x.Service]);

            if (serviceRequests is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Service Requests not found";
                return genericResponse;
            }

            var mappedData = _mapper.Map<IEnumerable<ServiceRequestForAdminDTO>>(serviceRequests);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Retrives Service Request Successfully";
            genericResponse.Data = mappedData;

            return genericResponse;
        }

        public async Task<
            GenericResponse<IEnumerable<ServiceRequestDTO>>
        > GetAllServiceRequestsForCurrentGuestAsync(string guestId)
        {
            var genericResponse = new GenericResponse<IEnumerable<ServiceRequestDTO>>();

            var serviceRequests = await _unitOfWork
                .GetRepository<ServiceRequest, int>()
                .GetAllAsync(null, null, null, [x => x.Service]);

            if (serviceRequests is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No ServiceRequest found";
                return genericResponse;
            }

            var mappedServiceRequests = _mapper.Map<IEnumerable<ServiceRequestDTO>>(
                serviceRequests
            );

            if (mappedServiceRequests is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No ServiceRequest found";
                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Success to return service requests";
            genericResponse.Data = mappedServiceRequests;

            return genericResponse;
        }

        public async Task<GenericResponse<HotelServicesDTO>> GetHotelServiceByIdForGuestAsync(
            int serviceId
        )
        {
            var genericResponse = new GenericResponse<HotelServicesDTO>();

            var service = await _unitOfWork
                .GetRepository<Service, int>()
                .GetByIdAsync(serviceId, x => x.IsAvailable == true, null);

            if (service is null || service!.IsAvailable == false)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = $"Service with Id : {serviceId} not found.";
                return genericResponse;
            }

            var mappedService = _mapper.Map<HotelServicesDTO>(service);

            if (mappedService is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = $"Service with Id : {serviceId} not found.";
                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = $"Service with Id : {serviceId} retrieved successfully.";
            genericResponse.Data = mappedService;

            return genericResponse;
        }

        public async Task<
            GenericResponse<ServiceRequestForAdminDTO>
        > GetServiceRequestByIdForAdminAsync(int serviceRequestId)
        {
            var genericResponse = new GenericResponse<ServiceRequestForAdminDTO>();

            var serviceRequest = await _unitOfWork
                .GetRepository<ServiceRequest, int>()
                .GetByIdAsync(serviceRequestId, null, [x => x.Booking.HotelUser, x => x.Service]);

            if (serviceRequest is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message =
                    $"Service Request with id : {serviceRequestId} is not found";
                return genericResponse;
            }

            var mappedData = _mapper.Map<ServiceRequestForAdminDTO>(serviceRequest);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Retrive service request successfully";
            genericResponse.Data = mappedData;

            return genericResponse;
        }

        public async Task<GenericResponse<bool>> UpdateHotelServiceByAdminAsync(
            int id,
            CreateOrUpdateHotelServiceDTO updateHotelServiceDTO
        )
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                var service = await _unitOfWork.GetRepository<Service, int>().GetByIdAsync(id);

                if (service is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = "Service not found";
                    return genericResponse;
                }

                var mappedService = _mapper.Map(updateHotelServiceDTO, service);

                _unitOfWork.GetRepository<Service, int>().Update(mappedService);
                service.UpdatedAt = DateTime.Now;

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (!result)
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to update service";
                    return genericResponse;
                }

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Service updated successfully";
                genericResponse.Data = true;

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a hotel service.");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to update service.";
                return genericResponse;
            }
        }

        public async Task<GenericResponse<bool>> UpdateServiceRequestStatusByStaffAsync(
            int serviceRequestId,
            string staffUserId,
            string status
        )
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                var serviceRequest = await _unitOfWork
                    .GetRepository<ServiceRequest, int>()
                    .GetByIdAsync(
                        serviceRequestId,
                        x => x.Status == Status.Assigned,
                        [x => x.AssignedStaff!]
                    );

                if (serviceRequest is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = "Service Request not found to update status";
                    return genericResponse;
                }

                if (
                    serviceRequest.AssignedStaff!.Id != staffUserId
                    || serviceRequest.AssignedStaff.GetType() != typeof(StaffUser)
                )
                {
                    genericResponse.StatusCode = StatusCodes.Status403Forbidden;
                    genericResponse.Message = "You are not assigned to this service request.";
                    return genericResponse;
                }

                if (status == "InProgress" || status == "Completed")
                {
                    Enum.TryParse(status, out Status statusValue);
                    serviceRequest.Status = statusValue;
                    serviceRequest.UpdatedAt = DateTime.Now;
                }
                else
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message =
                        "You have only two status options InProgress or Completed";
                    return genericResponse;
                }

                _unitOfWork.GetRepository<ServiceRequest, int>().Update(serviceRequest);

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (!result)
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to update serviceRequest status";
                    return genericResponse;
                }

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message =
                    $"Service Request status updated by status : {status} successfully";
                genericResponse.Data = true;

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error while update service request status");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to update serviceRequest status";
                return genericResponse;
            }
        }
    }
}
