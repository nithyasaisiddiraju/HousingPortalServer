using System;
using System.Threading.Tasks;
using HousingPortalApi.Dtos;
using Microsoft.EntityFrameworkCore;
using HousingPortalApi.Interfaces;
using HousingPortalApi.Data;

namespace HousingPortalApi.Services
{
    public class StudentService : IStudentService
    {
        private readonly HousingPortalDbContext _context;

        public StudentService(HousingPortalDbContext context)
        {
            _context = context;
        }

        public async Task<StudentDto> GetStudentDetails(Guid studentId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student != null)
            {
                return new StudentDto
                {
                    StudentId = student.StudentId,
                    Name = student.Name,
                    Email = student.Email,
                    Phone = student.Phone,
                    Major = student.Major,
                    GraduationYear = student.GraduationYear
                };
            }
            return null;
        }
    }
}