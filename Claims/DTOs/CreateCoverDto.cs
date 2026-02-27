using Claims.Models;

namespace Claims.DTOs
{
    public class CreateCoverDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CoverType Type { get; set; }
    }
}
