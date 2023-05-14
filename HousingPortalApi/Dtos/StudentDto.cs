namespace HousingPortalApi.Dtos
{
    public class StudentDto
    {
        public Guid StudentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Major { get; set; }
        public int GraduationYear { get; set; }
    }

}
