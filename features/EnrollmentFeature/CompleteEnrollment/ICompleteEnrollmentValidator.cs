using FluentValidation;
using FluentValidation.Results;
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
                .WithErrorCode("400")
                .CustomAsync(async (enrollmentId, context, cancellationToken) =>
                {
                    var enrollment = await enrollmentRepository.GetAll()
                        .Where(e => e.Id == enrollmentId)
                        .Select(e => new
                        {
                            e.Id,
                            e.Status,
                            HasPayment = e.Payment != null,
                            PaymentStatus = e.Payment != null ? (PaymentStatus?)e.Payment.Status : null
                        })
                        .FirstOrDefaultAsync(cancellationToken);

                    if (enrollment is null)
                    {
                        context.AddFailure(new ValidationFailure(nameof(CompleteEnrollmentCommand.EnrollmentId), "Enrollment was not found.")
                        {
                            ErrorCode = "404"
                        });
                        return;
                    }

                    if (enrollment.Status == EnrollmentStatus.Completed)
                    {
                        context.AddFailure(new ValidationFailure(nameof(CompleteEnrollmentCommand.EnrollmentId), "Enrollment is already completed.")
                        {
                            ErrorCode = "409"
                        });
                        return;
                    }

                    if (enrollment.Status == EnrollmentStatus.Cancelled)
                    {
                        context.AddFailure(new ValidationFailure(nameof(CompleteEnrollmentCommand.EnrollmentId), "Enrollment is cancelled.")
                        {
                            ErrorCode = "409"
                        });
                        return;
                    }

                    if (enrollment.Status != EnrollmentStatus.Active)
                    {
                        context.AddFailure(new ValidationFailure(nameof(CompleteEnrollmentCommand.EnrollmentId), "Only an active enrollment can be completed.")
                        {
                            ErrorCode = "409"
                        });
                        return;
                    }

                    if (!enrollment.HasPayment)
                    {
                        context.AddFailure(new ValidationFailure("Payment", "The enrollment must have a completed payment.")
                        {
                            ErrorCode = "409"
                        });
                        return;
                    }

                    if (enrollment.PaymentStatus != PaymentStatus.Completed)
                    {
                        context.AddFailure(new ValidationFailure("Payment", "The enrollment payment is not completed.")
                        {
                            ErrorCode = "409"
                        });
                        return;
                    }
                });
        }
    }
}
