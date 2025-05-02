namespace RevApi.Models
{
    public class Response
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime RespondedAt { get; set; }

  
        public int ReviewId { get; set; }
        public  Review Review { get; set; }
    }
}
