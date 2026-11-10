
using LMS___Mini_Version.features.InternFeature.AddIntern;
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
        public async Task<ActionResult<EndpointResponse<IEnumerable<InternVM>>>> GetAll()
        {
            var result = await Mediator.Send(new GetAllInternsQuery(1, 20));
            if (!result.Success)
                return StatusCode(result.StatusCode,
                    EndpointResponse.Fail(result.StatusCode, "Failed to get interns", result.Errors));

            var internsVM = result.Data.Select(x => new InternVM
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                Status = x.Status,
                TrackName = x.TrackName
            });

            return StatusCode(result.StatusCode, EndpointResponse<IEnumerable<InternVM>>.OK(internsVM, result.StatusCode, "success"));
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<>> GetById(int id)
        //{

        //}

        [HttpPost]
        public async Task<ActionResult<EndpointResponse<int>>> Create([FromBody] AddInternCommand command)
        {
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
