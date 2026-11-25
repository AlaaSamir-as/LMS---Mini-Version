using LMS___Mini_Version.features.shared;
using LMS___Mini_Version.features.TrackFeature.GetAllTracks.Dtos;
using MediatR;

namespace LMS___Mini_Version.features.TrackFeature.GetAllTracks.Query
{
    public record GetAllTracksQuery(int PageNumber = 1, int PageSize = 10) : IRequest<RequestResponse<IEnumerable<TrackDto>>>;

}
