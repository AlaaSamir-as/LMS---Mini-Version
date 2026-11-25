using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.EnrollmentFeature.CompleteEnrollment
{
    public record CompleteEnrollmentCommand(int EnrollmentId) : IRequest<RequestResponse>;

    public sealed class CompleteEnrollmentCommandHandler
        : IRequestHandler<CompleteEnrollmentCommand, RequestResponse>
    {
        private readonly IGeneralRepository<Enrollment> _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompleteEnrollmentCommandHandler(
            IGeneralRepository<Enrollment> enrollmentRepository,
            IUnitOfWork unitOfWork)
        {
            _enrollmentRepository = enrollmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse> Handle(
            CompleteEnrollmentCommand request,
            CancellationToken cancellationToken)
        {
            var enrollment = await _enrollmentRepository.GetAll()
                .SingleOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

            if (enrollment is null)
                return RequestResponse.Fail(404, new Dictionary<string, string[]>
                {
                    [nameof(request.EnrollmentId)] = ["Enrollment was not found."]
                });

            if (enrollment.Status == EnrollmentStatus.Completed)
                return RequestResponse.Fail(409, new Dictionary<string, string[]>
                {
                    [nameof(enrollment.Status)] = ["Enrollment is already completed."]
                });

            if (enrollment.Status == EnrollmentStatus.Cancelled)
                return RequestResponse.Fail(409, new Dictionary<string, string[]>
                {
                    [nameof(enrollment.Status)] = ["Enrollment is cancelled."]
                });

            if (enrollment.Status != EnrollmentStatus.Active)
                return RequestResponse.Fail(409, new Dictionary<string, string[]>
                {
                    [nameof(enrollment.Status)] = ["Only an active enrollment can be completed."]
                });

            if (enrollment.Payment is null || enrollment.Payment.Status != PaymentStatus.Completed)
                return RequestResponse.Fail(409, new Dictionary<string, string[]>
                {
                    ["Payment"] = ["The enrollment must have a completed payment."]
                });

            enrollment.Status = EnrollmentStatus.Completed;
            await _unitOfWork.SaveChangesAsync();

            return RequestResponse.OK(200);
        }
    }
}
