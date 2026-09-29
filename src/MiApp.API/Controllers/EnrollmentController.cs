using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiApp.Application.DTOs.Enrollment;
using MiApp.Application.Interfaces;

namespace MiApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentController(
    IEnrollmentService enrollmentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnrollmentResponseDto>>> GetAll()
    {
        var response = await enrollmentService.GetAllAsync();

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<EnrollmentResponseDto>> Create(
        EnrollmentRequestDto request)
    {
        var response = await enrollmentService.CreateAsync(request);

        return Ok(response);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await enrollmentService.DeleteAsync(id);

        return NoContent();
    }
}
