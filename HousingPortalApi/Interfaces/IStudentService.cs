using HousingPortalApi.Dtos;

namespace HousingPortalApi.Interfaces
{
    public interface IStudentService
    {
        Task<StudentDto> GetStudentDetails(Guid studentId);
    }
}
