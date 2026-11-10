using FluentValidation;
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.InternFeature.DeleteIntern
{
    public class DeleteInternValidator : AbstractValidator<DeleteInternCommand>
    {
        public DeleteInternValidator(IGeneralRepository<Enrollment> enrollmentRepository, IGeneralRepository<Intern> internRepository)
        {
            RuleFor(x => x.InternId)
                .GreaterThan(0).WithMessage("InternId must be greater than 0.")
                .MustAsync(async (internId, cancellationToken) =>
                    await internRepository.GetAll().AnyAsync(i => i.Id == internId, cancellationToken))
                .WithMessage("Intern does not exist.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.InternId)
                        .MustAsync(async (internId, cancellationToken) =>
                            !await enrollmentRepository.GetAll().AnyAsync(e => e.InternId == internId, cancellationToken))
                        .WithMessage("Cannot delete intern with existing enrollments.");
                });
        }
    }
}
