using System;
using System.Threading.Tasks;
using HousingPortalApi.Models;
using HousingPortalApi.Dtos;
using Microsoft.EntityFrameworkCore;
using HousingPortalApi.Interfaces;

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
            var student = await _context.Students.FirstOrDefaultAsync(s => s.studentId == studentId);

            if (student != null)
            {
                return new StudentDto
                {
                    studentId = student.studentId,
                    name = student.name,
                    email = student.email,
                    phone = student.phone,
                    major = student.major,
                    graduationYear = student.graduationYear
                };
            }
            return null;
        }
    }
}