using FluentValidation;
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.InternFeature.AddIntern
{
    public sealed class InternRegistrationValidator : AbstractValidator<AddInternCommand>
    {
        private readonly IGeneralRepository<Intern> _internRepository;
        private readonly IGeneralRepository<Track> _trackRepository;

        public InternRegistrationValidator(
            IGeneralRepository<Intern> internRepository,
            IGeneralRepository<Track> trackRepository)
        {
            _internRepository = internRepository;
            _trackRepository = trackRepository;
        }

        public async Task<IDictionary<string, string[]>> ValidateAsync(
            AddInternCommand request,
            CancellationToken cancellationToken)
        {
            var errors = new Dictionary<string, string[]>();
            var normalizedEmail = request.Email.ToLowerInvariant();

            if (await _internRepository.GetAll()
                .AnyAsync(intern => intern.Email.ToLower() == normalizedEmail, cancellationToken))
            {
                errors[nameof(request.Email)] = new[]
                {
                    "An intern with this email already exists."
                };
            }

            var track = await _trackRepository.GetByIdAsync(request.TrackId);
            if (track is null || !track.IsActive)
            {
                errors[nameof(request.TrackId)] = new[]
                {
                    "The selected track does not exist or is not active."
                };
            }

            return errors;
        }
    }
}
