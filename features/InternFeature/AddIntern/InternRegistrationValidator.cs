using FluentValidation;
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.InternFeature.AddIntern
{
    public sealed class InternRegistrationValidator : AbstractValidator<AddInternCommand>
    {
        public InternRegistrationValidator(
            IGeneralRepository<Intern> internRepository,
            IGeneralRepository<Track> trackRepository)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MustAsync(async (email, cancellationToken) =>
                {
                    if (string.IsNullOrWhiteSpace(email)) return true;
                    var normalizedEmail = email.Trim().ToLowerInvariant();
                    return !await internRepository.GetAll()
                        .AnyAsync(intern => intern.Email.ToLower() == normalizedEmail, cancellationToken);
                })
                .WithMessage("An intern with this email already exists.");

            RuleFor(x => x.TrackId)
                .GreaterThan(0).WithMessage("TrackId must be greater than 0.")
                .MustAsync(async (trackId, cancellationToken) =>
                {
                    var track = await trackRepository.GetByIdAsync(trackId);
                    return track is not null && track.IsActive;
                })
                .WithMessage("The selected track does not exist or is not active.");
        }
    }
}
