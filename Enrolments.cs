namespace RaceDay_poe_part2.Models
{
    public class Enrolments
    {
        public int EnrolmentId { get; set; }
        public DateTime EnrolmentDate { get; set; }
        public string Status { get; set; } = "";
        public int UserId { get; set; }
        public string ParticipantName { get; set; } = "";
        public int EventId { get; set; }
        public string EventName { get; set; } = "";
        public int OrganiserId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";

    }
}
