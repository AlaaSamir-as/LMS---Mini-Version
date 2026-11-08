using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.EnrollmentFeature.GetEnrollmentByIntern.Dtos;
using LMS___Mini_Version.features.shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.EnrollmentFeature.GetEnrollmentByIntern
{
    public record GetEnrollmentByInternQuery(int InternId) : IRequest<RequestResponse<IEnumerable<InternEnrollmentDto>>>;

    public sealed class GetEnrollmentByInternValidatorBehavior : 
        IPipelineBehavior<GetEnrollmentByInternQuery, RequestResponse<IEnumerable<InternEnrollmentDto>>>
    {
        private readonly IGetInternEnrollmentValidator _validator;

        public GetEnrollmentByInternValidatorBehavior(IGetInternEnrollmentValidator validator)
        {
            _validator = validator;
        }

        public async Task<RequestResponse<IEnumerable<InternEnrollmentDto>>> Handle(
            GetEnrollmentByInternQuery request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<RequestResponse<IEnumerable<InternEnrollmentDto>>> next)
        {
            var errors = await _validator.ValidateAsync(request, cancellationToken);
            
            return errors.Count > 0
                ? RequestResponse<IEnumerable<InternEnrollmentDto>>.Fail(400, errors)
                : await next();
        }
    }

    public class GetEnrollmentByInternQueryHandler : IRequestHandler<GetEnrollmentByInternQuery, RequestResponse<IEnumerable<InternEnrollmentDto>>>
    {
        private readonly IGeneralRepository<Domain.Entities.Enrollment> _enrollmentRepository;

        public GetEnrollmentByInternQueryHandler(IGeneralRepository<Enrollment> enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<RequestResponse<IEnumerable<InternEnrollmentDto>>> Handle(GetEnrollmentByInternQuery request, CancellationToken cancellationToken)
        {
            var enrollments = await _enrollmentRepository.GetAll()
                .Where(e => e.InternId == request.InternId)
                .Select(e => new InternEnrollmentDto
                {
                    EnrollmentId = e.Id,
                    TrackId = e.TrackId,
                    TrackName = e.Track.Name,
                    EnrollmentDate = e.EnrollmentDate.ToString("yyyy-MM-dd"),
                    EnrollmentStatus = e.Status.ToString()
                }).OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync(cancellationToken);

            return RequestResponse<IEnumerable<InternEnrollmentDto>>.OK(enrollments);
        }
    }
}
