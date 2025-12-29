namespace Rh.Models
{
    public class RapportViewModel
    {
        public List<Conge> CongesApprouves { get; set; }
        public int TotalEmployes { get; set; }
        public int TotalJoursConges { get; set; }
        public DateTime DateGeneration { get; set; }
    }
}