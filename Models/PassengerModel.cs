namespace Program.Models
{
    public class PassengerModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public int flight_id { get; set; }
        public string? full_name { get; set; }
    }
}
