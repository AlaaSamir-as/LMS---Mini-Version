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
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required.")
                .WithErrorCode("400");

            RuleFor(x => x.BirthYear)
                .InclusiveBetween(1900, DateTime.UtcNow.Year)
                .WithMessage($"BirthYear must be between 1900 and {DateTime.UtcNow.Year}.")
                .WithErrorCode("400");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .WithErrorCode("400")
                .EmailAddress().WithMessage("A valid email address is required.")
                .WithErrorCode("400")
                .MustAsync(async (email, cancellationToken) =>
                {
                    if (string.IsNullOrWhiteSpace(email)) return true;
                    var normalizedEmail = email.Trim().ToLowerInvariant();
                    return !await internRepository.GetAll()
                        .AnyAsync(intern => intern.Email.ToLower() == normalizedEmail, cancellationToken);
                })
                .WithMessage("An intern with this email already exists.")
                .WithErrorCode("409");

            RuleFor(x => x.TrackId)
                .GreaterThan(0).WithMessage("TrackId must be greater than 0.")
                .WithErrorCode("400")
                .MustAsync(async (trackId, cancellationToken) =>
                {
                    var track = await trackRepository.GetByIdAsync(trackId);
                    return track is not null;
                })
                .WithMessage("The selected track does not exist.")
                .WithErrorCode("404")
                .DependentRules(() =>
                {
                    RuleFor(x => x.TrackId)
                        .MustAsync(async (trackId, cancellationToken) =>
                        {
                            var track = await trackRepository.GetByIdAsync(trackId);
                            return track is not null && track.IsActive;
                        })
                        .WithMessage("The selected track is not active.")
                        .WithErrorCode("409");
                });
        }
    }
}
