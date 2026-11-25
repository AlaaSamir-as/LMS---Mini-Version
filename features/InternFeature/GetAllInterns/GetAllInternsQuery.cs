using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.InternFeature.GetAllInterns.Dtos;
using LMS___Mini_Version.features.shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.InternFeature.GetAllInterns
{
    public record GetAllInternsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<RequestResponse<PaginatedResult<InternDto>>>;
    public class GetAllInternsQueryHandler : IRequestHandler<GetAllInternsQuery, RequestResponse<PaginatedResult<InternDto>>>
    {
        private readonly IGeneralRepository<Intern> _internRepository;
        public GetAllInternsQueryHandler(IGeneralRepository<Intern> internRepository)
        {
            _internRepository = internRepository;
        }
        public async Task<RequestResponse<PaginatedResult<InternDto>>> Handle(GetAllInternsQuery request, CancellationToken cancellationToken)
        {
            int safePageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            int safePageSize = request.PageSize < 1 ? 10 : (request.PageSize > 100 ? 100 : request.PageSize);

            var query = _internRepository.GetAll()
                .AsNoTracking();

            var totalCount = await query.CountAsync(cancellationToken);

            var internDtos = await query
                .OrderBy(intern => intern.Id)
                .Skip((safePageNumber - 1) * safePageSize)
                .Take(safePageSize)
                .Select(intern => new InternDto
                {
                    Id = intern.Id,
                    FullName = intern.FullName,
                    Email = intern.Email,
                    Status = intern.Status,
                    TrackName = intern.Track.Name
                })
                .ToListAsync(cancellationToken);

            var result = new PaginatedResult<InternDto>(internDtos, totalCount, safePageNumber, safePageSize);
            return RequestResponse<PaginatedResult<InternDto>>.OK(result);
        }
    }
}