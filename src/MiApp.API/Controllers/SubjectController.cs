using Microsoft.AspNetCore.Mvc;
using MiApp.Application.Interfaces;
using MiApp.Application.DTOs;

namespace MiApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectController(ISubjectService subjectService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectResponseDto>>> GetAll()
    {
        var response = await subjectService.GetAllAsync();

        return Ok(response);
    }
}

