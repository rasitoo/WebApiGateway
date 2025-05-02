using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevApi.DbsContext;
using RevApi.Models.ResponseDTO;
using RevApi.Models;
using System.Security.Claims;

namespace RevApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResponseController : ControllerBase
    {
        private readonly RevDbContext _context;
        private readonly ILogger<ResponseController> _logger;


        public ResponseController(RevDbContext context, ILogger<ResponseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("GetResponseById/{id}")]
        public async Task<IActionResult> GetResponseById(int id)
        {
            var response = await _context.Responses
                .FirstOrDefaultAsync(r => r.Id == id);

            if (response == null)
            {
                return NotFound("Response no encontrada");
            }
            else
            {
                ResponseUpdateDTO responseDTO = new()
                {
                    Id = response.Id,
                    Message = response.Message
                };

                return Ok(responseDTO);
            }
        }

        [Authorize(Policy = "WorkshopOnly")]
        [HttpPost("CreateResponse")]
        public async Task<IActionResult> CreateResponse([FromBody] ResponseCreateDTO dto)
        {
            var workshopId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == dto.ReviewId && r.WorkshopId == workshopId);

            if (review == null)
            {
                return NotFound("Review no encontrada o no te pertenece.");
            }
            else
            {
                if (review.Response != null)
                {
                    return BadRequest("Ya existe una respuesta para esta review.");
                }
                else
                {
                    _context.Responses.Add(new Response
                    {
                        ReviewId = dto.ReviewId,
                        Message = dto.Message,
                        RespondedAt = DateTime.UtcNow,
                    });

                    await _context.SaveChangesAsync();
                    return Ok("Respuesta creada correctamente.");
                }
            }
        }

        [Authorize(Policy = "WorkshopOnly")]
        [HttpPut("UpdateResponse")]
        public async Task<IActionResult> UpdateResponse([FromBody] ResponseUpdateDTO dto)
        {
            var workshopId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var response = await _context.Responses
                .FirstOrDefaultAsync(r => r.Id == dto.Id && r.Review.WorkshopId == workshopId);

            if (response == null)
            {
                return NotFound("Respuesta no encontrada o no tienes permisos.");
            }
            else
            {
                response.Message = dto.Message;
                await _context.SaveChangesAsync();
                return Ok("Respuesta actualizada.");
            }
        }

        [Authorize(Policy = "WorkshopOnly")]
        [HttpDelete("DeleteResponse/{responseId}")]
        public async Task<IActionResult> DeleteResponse(int responseId)
        {
            var workshopId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var response = await _context.Responses.FirstOrDefaultAsync(r => r.Id == responseId && r.Review.WorkshopId == workshopId);

            if (response == null)
            {
                return NotFound("Respuesta no encontrada");
            }
            else
            {
                _context.Responses.Remove(response);
                await _context.SaveChangesAsync();

                return Ok("Respuesta eliminada.");
            }
        }
    }
}
