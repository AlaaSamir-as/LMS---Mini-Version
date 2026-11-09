using FluentValidation;
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.InternFeature.DeleteIntern
{
    public class DeleteInternValidator : AbstractValidator<DeleteInternCommand>
    {
        private readonly IGeneralRepository<Enrollment> _enrollmentRepository;
        private readonly IGeneralRepository<Intern> _internRepository;
        public DeleteInternValidator(IGeneralRepository<Enrollment> enrollmentRepository, IGeneralRepository<Intern> internRepository)
        {
            RuleFor(x => x.InternId)
                .GreaterThan(0).WithMessage("InternId must be greater than 0");
            _enrollmentRepository = enrollmentRepository;
            _internRepository = internRepository;
        }

        public async Task<IDictionary<string, string[]>> ValidateAsync(DeleteInternCommand request, CancellationToken cancellationToken)
        {
            var errors = new Dictionary<string, string[]>();
            var hasEnrollments = await _enrollmentRepository.GetAll()
                .AnyAsync(e => e.InternId == request.InternId, cancellationToken);
            if (hasEnrollments)
            {
                errors[nameof(request.InternId)] = new[]
                {
                    "Cannot delete intern with existing enrollments."
                };
            }

            var internExists = await _internRepository.GetAll()
                .AnyAsync(i => i.Id == request.InternId, cancellationToken);
            if (!internExists)
            {
                errors[nameof(request.InternId)] = new[]
                {
                    "Intern does not exist."
                };
            }

            return errors;
        }

    }
}
