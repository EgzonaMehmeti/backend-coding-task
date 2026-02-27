using Claims.Models;

namespace Claims.DTOs
{
    public class CoverDto
    {
        public string Id { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CoverType Type { get; set; }
        public decimal Premium { get; set; }
    }
}
