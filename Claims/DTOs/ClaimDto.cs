using Claims.Models;

namespace Claims.DTOs
{
    public class ClaimDto
    {
        public string Id { get; set; }
        public string CoverId { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public ClaimType Type { get; set; }
        public decimal DamageCost { get; set; }
    }
}
