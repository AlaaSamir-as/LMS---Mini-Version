using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.EnrollmentFeature.CompleteEnrollment
{
    public record CompleteEnrollmentCommand(int EnrollmentId) : IRequest<RequestResponse>;

    public sealed class CompleteEnrollmentValidationBehavior :
        IPipelineBehavior<CompleteEnrollmentCommand, RequestResponse>
    {
        private readonly ICompleteEnrollmentValidator _validator;

        public CompleteEnrollmentValidationBehavior(
            ICompleteEnrollmentValidator validator)
        {
            _validator = validator;
        }

        public async Task<RequestResponse> Handle(
            CompleteEnrollmentCommand request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<RequestResponse> next)
        {
            var errors = await _validator.ValidateAsync(request, cancellationToken);

            return errors.Count > 0
                ? RequestResponse.Fail(400, errors)
                : await next();
        }
    }

    public sealed class CompleteEnrollmentCommandHandler
        : IRequestHandler<CompleteEnrollmentCommand, RequestResponse>
    {
        private readonly IGeneralRepository<Enrollment> _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompleteEnrollmentCommandHandler(
            IGeneralRepository<Enrollment> enrollmentRepository,
            IUnitOfWork unitOfWork)
        {
            _enrollmentRepository = enrollmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse> Handle(
            CompleteEnrollmentCommand request,
            CancellationToken cancellationToken)
        {
            var enrollment = await _enrollmentRepository.GetAll()
                .SingleOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

            enrollment.Status = EnrollmentStatus.Completed;
            await _unitOfWork.SaveChangesAsync();

            return RequestResponse.OK(200);
        }
    }
}
