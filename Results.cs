namespace RaceDay_poe_part2.Models
{
    public class Results
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; } = "";
        public string ParticipantName { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public string FinishTime { get; set; } = "00:00:00";   // hh:mm:ss
        public int Position { get; set; }
        public string Notes { get; set; } = "";

    }
}
