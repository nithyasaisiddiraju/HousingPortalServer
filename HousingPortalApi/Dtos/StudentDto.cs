namespace HousingPortalApi.Dtos
{
    public class StudentDto
    {
        public Guid studentId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string major { get; set; }
        public int graduationYear { get; set; }
    }

}
