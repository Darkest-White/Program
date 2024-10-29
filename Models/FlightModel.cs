namespace Program.Models
{
    public class FlightModel
    {
        public int id { get; set; }
        public DateTime start_time { get; set; }
        public int route_id { get; set; }
        public Guid pilot_id { get; set; }
        public int plane_id { get; set; }
        public int passenger_count { get; set; }
    }
}
