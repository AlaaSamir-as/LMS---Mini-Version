using FluentValidation;
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;

namespace LMS___Mini_Version.features.EnrollmentFeature.CompleteEnrollment
{
    public sealed class CompleteEnrollmentValidator : AbstractValidator<CompleteEnrollmentCommand>
    {
        private readonly IGeneralRepository<Enrollment> _enrollmentRepository;

        public CompleteEnrollmentValidator(
            IGeneralRepository<Enrollment> enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;

            RuleFor(request => request.EnrollmentId)
                .CustomAsync(async (enrollmentId, context, cancellationToken) =>
                {
                    var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
                    var errorMessage = enrollment switch
                    {
                        null => "Enrollment was not found.",
                        { Status: EnrollmentStatus.Completed } => "Enrollment is already completed.",
                        { Status: EnrollmentStatus.Cancelled } => "Enrollment is cancelled.",
                        { Status: not EnrollmentStatus.Active } => "Only an active enrollment can be completed.",
                        { Payment: not { Status: PaymentStatus.Completed } } => "The enrollment must have a completed payment.",
                        _ => null
                    };

                    if (errorMessage is not null)
                        context.AddFailure(nameof(CompleteEnrollmentCommand.EnrollmentId), errorMessage);
                });
        }
    }
}
