using API.Entities;

namespace API.Models.Responses
{
    public class DepartmentResponse
    {
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? DepartmentType { get; set; }
    }
}
