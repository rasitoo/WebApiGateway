using RevApi.Models.ResponseDTO;

namespace RevApi.Models.ReviewDTO
{
    public class ReviewDetailDTO
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public int UserId { get; set; }
        public ResponseUpdateDTO? Response { get; set; }
    }
}
