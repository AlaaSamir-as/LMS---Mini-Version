using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.EnrollmentFeature.CompleteEnrollment;

namespace LMS___Mini_Version.features.EnrollmentFeature.GetEnrollmentByIntern
{
    public interface IGetInternEnrollmentValidator
    {
        Task<IDictionary<string, string[]>> ValidateAsync(
            GetEnrollmentByInternQuery request,
            CancellationToken cancellationToken);
    }

    public sealed class GetInternEnrollmentValidator : IGetInternEnrollmentValidator
    {
        private readonly IGeneralRepository<Intern> _InternRepository;
        public GetInternEnrollmentValidator(
            IGeneralRepository<Intern> InternRepository)
        {
            _InternRepository = InternRepository;
        }
        public async Task<IDictionary<string, string[]>> ValidateAsync(
            GetEnrollmentByInternQuery request,
            CancellationToken cancellationToken)
        {
            var errors = new Dictionary<string, string[]>();
            var intern = await _InternRepository.GetByIdAsync(request.InternId);
            if (intern is null)
            {
                errors[nameof(request.InternId)] = new[]
                {
                    "Intern was not found."
                };
            }
            return errors;
        }
    }
}
