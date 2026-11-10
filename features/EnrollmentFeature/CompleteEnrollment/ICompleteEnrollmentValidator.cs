using FluentValidation;
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.EnrollmentFeature.CompleteEnrollment
{
    public sealed class CompleteEnrollmentValidator : AbstractValidator<CompleteEnrollmentCommand>
    {
        public CompleteEnrollmentValidator(
            IGeneralRepository<Enrollment> enrollmentRepository)
        {
            RuleFor(request => request.EnrollmentId)
                .GreaterThan(0).WithMessage("Enrollment ID must be greater than 0.")
                .CustomAsync(async (enrollmentId, context, cancellationToken) =>
                {
                    var enrollment = await enrollmentRepository.GetAll().Select(e =>
                        new Enrollment
                        {
                            Id = e.Id,
                            Status = e.Status,
                            Track = new Track
                            {
                                Fees = e.Track.Fees
                            },
                            Payment = new Payment
                            {
                                Status = e.Payment.Status
                            }
                        })
                        .FirstOrDefaultAsync(e => e.Id == enrollmentId, cancellationToken);

                    var errorMessage = enrollment switch
                    {
                        null => "Enrollment was not found.",
                        { Status: EnrollmentStatus.Completed } => "Enrollment is already completed.",
                        { Status: EnrollmentStatus.Cancelled } => "Enrollment is cancelled.",
                        { Status: not EnrollmentStatus.Active } => "Only an active enrollment can be completed.",
                        { Track: { Fees: > 0 }, Payment: not { Status: PaymentStatus.Completed } } => "The enrollment must have a completed payment.",
                        { Payment: { Status: not PaymentStatus.Completed } } => "The enrollment payment is not completed.",
                        _ => null
                    };

                    if (errorMessage is not null)
                        context.AddFailure(nameof(CompleteEnrollmentCommand.EnrollmentId), errorMessage);
                });
        }
    }
}
