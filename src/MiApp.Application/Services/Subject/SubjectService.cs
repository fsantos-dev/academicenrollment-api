using MiApp.Application.DTOs;
using MiApp.Application.Interfaces;
using MiApp.Application.Mappings;
using MiApp.Domain.Interfaces;

namespace MiApp.Application.Services.Subject;


public class SubjectService (ISubjectRepository subjectRepository) : ISubjectService
{
    public async Task<IEnumerable<SubjectResponseDto>> GetAllAsync()
    {
        var subjects = await subjectRepository.GetAllAsync();
        //select = por cada elemento de la coleccion haz... recorrer y transformar cualquier collecion
        return  subjects.Select(SubjectMapper.ToSubjectResponseDto);
    }
}