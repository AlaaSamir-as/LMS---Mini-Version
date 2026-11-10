using FluentValidation;
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.EnrollmentFeature.GetEnrollmentByIntern
{
    public sealed class GetInternEnrollmentValidator : AbstractValidator<GetEnrollmentByInternQuery>
    {
        public GetInternEnrollmentValidator(
            IGeneralRepository<Intern> internRepository)
        {
            RuleFor(request => request.InternId)
                .GreaterThan(0)
                .WithMessage("Intern ID must be greater than 0.")
                .MustAsync(async (internId, cancellationToken) =>
                    await internRepository.GetAll().AnyAsync(intern => intern.Id == internId, cancellationToken))
                .WithMessage("Intern was not found.");
        }
    }
}
