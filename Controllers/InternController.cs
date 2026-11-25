using LMS___Mini_Version.features.InternFeature.AddIntern;
using LMS___Mini_Version.features.InternFeature.AddIntern.VM;
using LMS___Mini_Version.features.InternFeature.DeleteIntern;
using LMS___Mini_Version.features.InternFeature.GetAllInterns;
using LMS___Mini_Version.features.InternFeature.GetAllInterns.VM;
using LMS___Mini_Version.features.shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS___Mini_Version.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class InternController : ControllerBase
    {
        private readonly IMediator Mediator;
        public InternController(IMediator mediator)
        {
            Mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<EndpointResponse<PaginatedResult<InternVM>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            int safePageNumber = pageNumber < 1 ? 1 : pageNumber;
            int safePageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var result = await Mediator.Send(new GetAllInternsQuery(safePageNumber, safePageSize));
            if (!result.Success)
                return StatusCode(result.StatusCode,
                    EndpointResponse.Fail(result.StatusCode, "Failed to get interns", result.Errors));

            var internsVM = new PaginatedResult<InternVM>(
                result.Data!.Items.Select(x => new InternVM
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    Status = x.Status,
                    TrackName = x.TrackName
                }),
                result.Data.TotalCount,
                result.Data.PageNumber,
                result.Data.PageSize
            );

            return StatusCode(result.StatusCode, EndpointResponse<PaginatedResult<InternVM>>.OK(internsVM, result.StatusCode, "success"));
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<>> GetById(int id)
        //{

        //}

        [HttpPost]
        public async Task<ActionResult<EndpointResponse<int>>> Create([FromBody] CreateInternRequestVM request)
        {
            var command = new AddInternCommand(request.FullName, request.Email, request.BirthYear, request.TrackId);
            var result = await Mediator.Send(command);

            var response = result.Success ? EndpointResponse<int>.OK(result.Data, 201, "Intern created successfully") :
                EndpointResponse<int>.Fail(result.StatusCode, "Failed to create intern", result.Errors);

            return StatusCode(response.StatusCode, response);
        }

        //[HttpPut("{id}")]
        //public async Task<ActionResult> Update()
        //{

        //}

        [HttpDelete("{id}")]
        public async Task<ActionResult<EndpointResponse>> Delete(int id)
        {
            var result = await Mediator.Send(new DeleteInternCommand(id));

            return result.Success ? StatusCode(result.StatusCode, EndpointResponse.OK(result.StatusCode, "Intern deleted successfully")) :
                StatusCode(result.StatusCode, EndpointResponse.Fail(result.StatusCode, "Failed to delete intern", result.Errors));
        }
    }
}
