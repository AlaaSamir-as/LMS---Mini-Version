using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.shared;
using MediatR;

namespace LMS___Mini_Version.features.InternFeature.AddIntern
{
    public record AddInternCommand(string FullName, string Email, int BirthYear, int TrackId) : IRequest<RequestResponse<int>>;

    public sealed class AddInternValidationBehavior :
        IPipelineBehavior<AddInternCommand, RequestResponse<int>>
    {
        private readonly IInternRegistrationValidator _validator;

        public AddInternValidationBehavior(IInternRegistrationValidator validator)
        {
            _validator = validator;
        }

        public async Task<RequestResponse<int>> Handle(
            AddInternCommand request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<RequestResponse<int>> next)
        {
            var errors = await _validator.ValidateAsync(request, cancellationToken);

            return errors.Count > 0
                ? RequestResponse<int>.Fail(400, errors)
                : await next();
        }
    }

    public class AddInternCommandHandler : IRequestHandler<AddInternCommand, RequestResponse<int>>
    {
        private readonly IGeneralRepository<Intern> _internRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AddInternCommandHandler(
            IGeneralRepository<Intern> internRepository,
            IUnitOfWork unitOfWork)
        {
            _internRepository = internRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<RequestResponse<int>> Handle(AddInternCommand request, CancellationToken cancellationToken)
        {
            var intern = new Intern
            {
                FullName = request.FullName,
                Email = request.Email,
                BirthYear = request.BirthYear,
                TrackId = request.TrackId,
                Status = EnrollmentStatus.Pending.ToString()
            };
            _internRepository.Add(intern);
            await _unitOfWork.SaveChangesAsync();
            return RequestResponse<int>.OK(intern.Id);
        }
    }
}
