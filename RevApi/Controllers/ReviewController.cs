using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevApi.DbsContext;
using RevApi.Models;
using RevApi.Models.ResponseDTO;
using RevApi.Models.ReviewDTO;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RevApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly RevDbContext _context;
        private readonly ILogger<ReviewsController> _logger;


        public ReviewsController(RevDbContext context, ILogger<ReviewsController> logger)
        {
            _context = context;
            _logger = logger;
        }



        [HttpGet("GetReviewsByWorkshop/{workshopId}")]
        public async Task<IActionResult> GetReviewsByWorkshop(int workshopId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Response)
                .Where(r => r.WorkshopId == workshopId)
                .Select(r => new ReviewDetailDTO
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    Response = r.Response != null ? new ResponseUpdateDTO()
                    {
                        Id = r.Response.Id,
                        Message = r.Response.Message
                    } : null,
                    UserId = r.UserId
                })
                .ToListAsync();

            if (reviews == null || !reviews.Any())
            {
                return NotFound("Sin reviews");
            }
            return Ok(reviews);
        }

        [HttpGet("GetReviewById/{id}")]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Response)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review != null)
            {
                ReviewDetailDTO reviewDTO = new()
                {
                    Id = review.Id,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    UserId = review.UserId,
                    Response = review.Response != null ? new ResponseUpdateDTO
                    {
                        Id = review.Response.Id,
                        Message = review.Response.Message
                    } : null
                };

                return Ok(reviewDTO);
            }
            else
            {
                return NotFound("Review no encontrada");
            }
        }


        [HttpGet("CountReviews/{workshopId}")]
        public async Task<IActionResult> CountReviews(int workshopId)
        {
            var count = _context.Reviews.Where(r => r.WorkshopId == workshopId).Count();
            return Ok(count);
        }

      
        [HttpGet("GetMediaReview/{workshopId}")]
        public async Task<IActionResult> GetMediaReviews(int workshopId)
        {
            var averageRating = _context.Reviews
                .Where(r => r.WorkshopId == workshopId)
                .Average(r => r.Rating); 
            return Ok(averageRating);
        }

        [Authorize(Policy = "ClientOnly")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReviewCreateDTO review)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var existingReview = _context.Reviews.FirstOrDefault(r => r.WorkshopId == review.WorkshopId && r.UserId == userId);


            if (existingReview == null)
            {
                Review rev = new() {
                  UserId=userId,
                  WorkshopId=review.WorkshopId,
                  Rating = review.Rating,
                  Comment=review.Comment,
                  CreatedAt = DateTime.UtcNow,
                  UpdatedAt = DateTime.UtcNow,
                };

                _context.Reviews.Add(rev);
                _context.SaveChanges();
                return Ok();
            }
            else {
                return BadRequest("Ya tienes una review");
            }
             
        }

        [Authorize(Policy = "ClientOnly")]
        [HttpPut("ReviewUpdate")]
        public async Task<IActionResult> Put([FromBody] ReviewUpdateDTO review)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var existingReview = _context.Reviews.FirstOrDefault(r => r.Id == review.Id && r.UserId==userId);
            if (existingReview == null)
            {
                return NotFound("Review no encontrada");
            }

            existingReview.Comment = review.Comment; 
            existingReview.Rating = review.Rating;
            existingReview.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();

            return Ok();
        }

        [Authorize(Policy = "ClientOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var review = _context.Reviews.FirstOrDefault(r => r.Id == id && r.UserId==userId);
            if (review == null)
            {
                return NotFound("Review no encontrada");
            }

            _context.Reviews.Remove(review);
            _context.SaveChanges();

            return Ok();
        }
    }
}
