using FluentValidation;
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;

namespace LMS___Mini_Version.features.EnrollmentFeature.GetEnrollmentByIntern
{
    public sealed class GetInternEnrollmentValidator : AbstractValidator<GetEnrollmentByInternQuery>
    {
        private readonly IGeneralRepository<Intern> _InternRepository;

        public GetInternEnrollmentValidator(
            IGeneralRepository<Intern> InternRepository)
        {
            _InternRepository = InternRepository;

            RuleFor(request => request.InternId)
                .MustAsync(async (internId, cancellationToken) =>
                    await _InternRepository.GetByIdAsync(internId) is not null)
                .WithMessage("Intern was not found.");
        }
    }
}
