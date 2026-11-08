using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.shared;

namespace LMS___Mini_Version.features.EnrollmentFeature.CompleteEnrollment
{
    public interface ICompleteEnrollmentValidator
    {
        Task<IDictionary<string, string[]>> ValidateAsync(
            CompleteEnrollmentCommand request,
            CancellationToken cancellationToken);
    }

    public sealed class CompleteEnrollmentValidator : ICompleteEnrollmentValidator
    {
        private readonly IGeneralRepository<Enrollment> _enrollmentRepository;
        public CompleteEnrollmentValidator(
            IGeneralRepository<Enrollment> enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }
        public async Task<IDictionary<string, string[]>> ValidateAsync(
            CompleteEnrollmentCommand request,
            CancellationToken cancellationToken)
        {
            var errors = new Dictionary<string, string[]>();
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId);
            if (enrollment is null)
            {
                errors[nameof(request.EnrollmentId)] = new[]
                {
                    "Enrollment was not found."
                };
            }
            else if (enrollment.Status == EnrollmentStatus.Completed)
            {
                errors[nameof(request.EnrollmentId)] = new[]
                {
                    "Enrollment is already completed."
                };
            }
            else if (enrollment.Status == EnrollmentStatus.Cancelled)
            {
                errors[nameof(request.EnrollmentId)] = new[]
                {
                    "Enrollment is cancelled."
                };
            }
            else if (enrollment.Status != EnrollmentStatus.Active)
            {

                errors[nameof(request.EnrollmentId)] = new[]
                {
                    "Only an active enrollment can be completed."
                };
            }
            else if (enrollment.Payment?.Status != PaymentStatus.Completed)
            {
                errors[nameof(request.EnrollmentId)] = new[]
                {
                    "The enrollment must have a completed payment."
                };
            }
            return errors;
        }
    }
}
