using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.InternFeature.DeleteIntern
{
    public record DeleteInternCommand(int InternId) : IRequest<RequestResponse>;

    public class DeleteInternCommandHandler : IRequestHandler<DeleteInternCommand, RequestResponse>
    {
        private readonly IGeneralRepository<Intern> _internRepository;
        private readonly IGeneralRepository<Enrollment> _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteInternCommandHandler(IGeneralRepository<Intern> internRepository, IGeneralRepository<Enrollment> enrollmentRepository,IUnitOfWork unitOfWork)
        {
            _internRepository = internRepository;
            _enrollmentRepository = enrollmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse> Handle(DeleteInternCommand request, CancellationToken cancellationToken)
        {
            var intern = await _internRepository.GetByIdAsync(request.InternId);
            if (intern is null)
                return RequestResponse.Fail(404, new Dictionary<string, string[]>
                {
                    [nameof(request.InternId)] = ["Intern was not found."]
                });

            var hasEnrollments = await _enrollmentRepository.GetAll()
                .AnyAsync(e => e.InternId == request.InternId, cancellationToken);
            if (hasEnrollments)
                return RequestResponse.Fail(409, new Dictionary<string, string[]>
                {
                    [nameof(request.InternId)] = ["Cannot delete intern with existing enrollments."]
                });

            _internRepository.Delete(intern);
            await _unitOfWork.SaveChangesAsync();

            return RequestResponse.OK();
        }
    }
}