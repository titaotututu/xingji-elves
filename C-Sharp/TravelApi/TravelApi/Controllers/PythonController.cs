using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TravelApi.Models;

[Route("api/python")]
[ApiController]
public class PythonController : ControllerBase
{
    private readonly PythonService _pythonService;

    public PythonController(PythonService pythonService)
    {
        _pythonService = pythonService;
    }

    [HttpPost("user")]
    public async Task<ActionResult<User>> RegisterUser([FromBody] User user)
    {
        try
        {
            var result = await _pythonService.RegisterUserAsync(user);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("user")]
    public async Task<ActionResult<User>> Login([FromQuery] long userId, [FromQuery] long password)
    {
        try
        {
            var result = await _pythonService.LoginAsync(userId, password);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<User>> GetUser(long userId)
    {
        try
        {
            var result = await _pythonService.GetUserAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("user/{userId}")]
    public async Task<ActionResult<User>> UpdateUser(long userId, [FromBody] User user)
    {
        try
        {
            var result = await _pythonService.UpdateUserAsync(userId, user);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("user/{userId}")]
    public async Task<ActionResult> DeleteUser(long userId)
    {
        try
        {
            await _pythonService.DeleteUserAsync(userId);
            return Ok(new { message = "用户删除成功" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        try
        {
            var result = await _pythonService.GetAllUsersAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("journal")]
    public async Task<ActionResult<Journal>> CreateJournal([FromBody] Journal journal)
    {
        try
        {
            var result = await _pythonService.CreateJournalAsync(journal);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("journal/{journalId}/image")]
    public async Task<ActionResult<string>> UploadJournalImage(long journalId, IFormFile image)
    {
        try
        {
            var result = await _pythonService.UploadJournalImageAsync(journalId, image);
            return Ok(new { picture_path = result });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("journal/{journalId}")]
    public async Task<ActionResult<Journal>> GetJournal(long journalId)
    {
        try
        {
            var result = await _pythonService.GetJournalAsync(journalId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("journals/user/{userId}")]
    public async Task<ActionResult<List<Journal>>> GetUserJournals(long userId)
    {
        try
        {
            var result = await _pythonService.GetUserJournalsAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("journal/{journalId}")]
    public async Task<ActionResult<Journal>> UpdateJournal(long journalId, [FromBody] Journal journal)
    {
        try
        {
            var result = await _pythonService.UpdateJournalAsync(journalId, journal);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("journal/{journalId}")]
    public async Task<ActionResult> DeleteJournal(long journalId)
    {
        try
        {
            await _pythonService.DeleteJournalAsync(journalId);
            return Ok(new { message = "日志删除成功" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("journals")]
    public async Task<ActionResult<List<Journal>>> GetAllJournals()
    {
        try
        {
            var result = await _pythonService.GetAllJournalsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("feedback")]
    public async Task<ActionResult<Feedback>> CreateFeedback([FromBody] Feedback feedback)
    {
        try
        {
            var result = await _pythonService.CreateFeedbackAsync(feedback);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("feedback/user/{userId}")]
    public async Task<ActionResult<List<Feedback>>> GetUserFeedbacks(long userId)
    {
        try
        {
            var result = await _pythonService.GetUserFeedbacksAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("feedbacks")]
    public async Task<ActionResult<List<Feedback>>> GetAllFeedbacks()
    {
        try
        {
            var result = await _pythonService.GetAllFeedbacksAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("feedback/{feedbackId}")]
    public async Task<ActionResult> UpdateFeedbackStatus(long feedbackId, [FromBody] Dictionary<string, string> statusUpdate)
    {
        try
        {
            if (!statusUpdate.ContainsKey("status"))
            {
                return BadRequest("状态字段是必需的");
            }
            
            await _pythonService.UpdateFeedbackStatusAsync(feedbackId, statusUpdate["status"]);
            return Ok(new { message = "反馈状态更新成功" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("journal/image/{journalId}")]
    public async Task<ActionResult> GetJournalImage(long journalId)
    {
        try
        {
            var (imageData, contentType) = await _pythonService.GetJournalImageAsync(journalId);
            return File(imageData, contentType);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("journal/image/{journalId}")]
    public async Task<ActionResult> ClearJournalImages(long journalId)
    {
        try
        {
            await _pythonService.ClearJournalImagesAsync(journalId);
            return Ok(new { message = "图片已清空" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
