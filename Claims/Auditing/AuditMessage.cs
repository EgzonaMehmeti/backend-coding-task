namespace Claims.Auditing
{
    public class AuditMessage
    {
        public string? ClaimId { get; set; }
        public string? CoverId { get; set; }
        public string HttpRequestType { get; set; } = default!;
        public DateTime Created { get; set; }
    }
}
