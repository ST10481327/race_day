namespace RaceDay_poe_part2.Models
{
    public class Events
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = "";
        public decimal DistanceKm { get; set; }
        public string EventType { get; set; } = "Run";       // Run, Walk or Cycle
        public int MaxParticipants { get; set; }
        public string Status { get; set; } = "Upcoming";     // Upcoming, Open, Closed, Completed, Cancelled
        public int OrganiserId { get; set; }
        public string OrganiserName { get; set; } = "";

    }
}
