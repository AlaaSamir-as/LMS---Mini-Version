using LMS___Mini_Version.features.EnrollmentFeature.GetEnrollmentByIntern;
using LMS___Mini_Version.features.EnrollmentFeature.GetEnrollmentByIntern.VM;
using LMS___Mini_Version.features.EnrollmentFeature.CompleteEnrollment;
using LMS___Mini_Version.features.shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS___Mini_Version.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EnrollmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<>>> GetAll()
        //{

        //}

        //[HttpGet("{id}")]
        //public async Task<ActionResult<>> GetById(int id)
        //{

        //}

        [HttpGet("intern/{internId}")]
        public async Task<ActionResult<EndpointResponse<IEnumerable<InternEnrollmentVM>>>> GetByIntern(int internId)
        {
            var result = await _mediator.Send(new GetEnrollmentByInternQuery(internId));

            if (!result.Success)
                return StatusCode(result.StatusCode,
                    EndpointResponse.Fail(result.StatusCode, "Failed to get enrollments for the intern", result.Errors));

            var enrollmentsVM = result.Data.Select(x => new InternEnrollmentVM
            {
                EnrollmentId = x.EnrollmentId,
                TrackId = x.TrackId,
                TrackName = x.TrackName,
                EnrollmentDate = x.EnrollmentDate,
                EnrollmentStatus = x.EnrollmentStatus
            });

            return StatusCode(result.StatusCode, EndpointResponse<IEnumerable<InternEnrollmentVM>>.OK(enrollmentsVM, result.StatusCode, "success"));
        }

        //[HttpPost]
        //public async Task<ActionResult<>> Enroll( )
        //{



        //}

        [HttpPost("{enrollmentId}/complete")]
        public async Task<ActionResult<EndpointResponse>> Complete(int enrollmentId)
        {
            var result = await _mediator.Send(new CompleteEnrollmentCommand(enrollmentId));

            if (!result.Success)
                return StatusCode(result.StatusCode,
                    EndpointResponse.Fail(result.StatusCode,
                        "Failed to complete enrollment.", result.Errors));

            var response = EndpointResponse.OK(result.StatusCode,
                "Enrollment completed successfully.");

            return StatusCode(response.StatusCode, response);
        }

        //[HttpPost("{id}/cancel")]
        //public async Task<ActionResult> Cancel()
        //{

        //}

        //[HttpPost("{id}/transfer/{newTrackId}")]
        //public async Task<ActionResult> Transfer()
        //{

        //}
    }
}
