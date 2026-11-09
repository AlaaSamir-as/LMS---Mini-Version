using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.shared;
using MediatR;

namespace LMS___Mini_Version.features.InternFeature.DeleteIntern
{
    public record DeleteInternCommand(int InternId) : IRequest<RequestResponse>;

    public class DeleteInternCommandHandler : IRequestHandler<DeleteInternCommand, RequestResponse>
    {
        private readonly IGeneralRepository<Intern> _internRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteInternCommandHandler(IGeneralRepository<Intern> internRepository)
        {
            _internRepository = internRepository;
        }
        public async Task<RequestResponse> Handle(DeleteInternCommand request, CancellationToken cancellationToken)
        {
            var intern = await _internRepository.GetByIdAsync(request.InternId);

             _internRepository.Delete(intern);
            await _unitOfWork.SaveChangesAsync();

            return RequestResponse.OK();
        }
    }
}