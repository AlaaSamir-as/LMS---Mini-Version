using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.InternFeature.GetAllInterns.Dtos;
using LMS___Mini_Version.features.shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.InternFeature.GetAllInterns
{
    public record GetAllInternsQuery(int Page, int PageSize) : IRequest<RequestResponse<IEnumerable<InternDto>>>;
    public class GetAllInternsQueryHandler : IRequestHandler<GetAllInternsQuery, RequestResponse<IEnumerable<InternDto>>>
    {
        private readonly IGeneralRepository<Intern> _internRepository;
        public GetAllInternsQueryHandler(IGeneralRepository<Intern> internRepository)
        {
            _internRepository = internRepository;
        }
        public async Task<RequestResponse<IEnumerable<InternDto>>> Handle(GetAllInternsQuery request, CancellationToken cancellationToken)
        {
            var internDtos = await _internRepository.GetAll()
                .AsNoTracking()
                .OrderBy(intern => intern.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(intern => new InternDto
                {
                    Id = intern.Id,
                    FullName = intern.FullName,
                    Email = intern.Email,
                    Status = intern.Status,
                    TrackName = intern.Track.Name
                })
                .ToListAsync(cancellationToken);

            return RequestResponse<IEnumerable<InternDto>>.OK(internDtos);
        }
    }
}