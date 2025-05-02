namespace RevApi.Models.ReviewDTO
{
    public class ReviewCreateDTO
    {
        public int Rating { get; set; }
        public string Comment { get; set; }   
        public int WorkshopId { get; set; }
    }
}
