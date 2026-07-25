using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrackFinder.Services;
using TrackFinder.ViewModels.Assessment_ViewModels;

namespace TrackFinder.Controllers
{
    [AllowAnonymous]
    [Route("Assessment")]
    public class AssessmentController : Controller
    {
        private readonly AssessmentService _assessmentService;

        private static readonly Guid TestUserId = Guid.Parse("73d86ddb-3834-42d6-83ae-1f4b6746ce22");

        public AssessmentController(AssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out var userId))
                return userId;

            return TestUserId;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View("~/Views/Assessment/Index.cshtml");
        }

        [HttpGet("Take")]
        public async Task<IActionResult> Take()
        {
            var questions = await _assessmentService.GetAssessmentQuestionsAsync();
            return View(questions);
        }

        [HttpPost("Take")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Take(AssessmentAnswersVM answersVM)
        {
            answersVM.UserId = GetCurrentUserId();
            if (!ModelState.IsValid)
            {
                var questions = await _assessmentService.GetAssessmentQuestionsAsync();
                return View(questions);
            }
            try
            {
                var result = await _assessmentService.GetAssessmentResultAsync(answersVM);
                return RedirectToAction(nameof(Result), new { id = result.AssessmentResultId });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            var reloadedQuestions = await _assessmentService.GetAssessmentQuestionsAsync();
            return View(reloadedQuestions);
        }

        [HttpGet("Result/{id:guid}")]
        public async Task<IActionResult> Result(Guid id)
        {
            var result = await _assessmentService.GetAssessmentResultByIdAsync(id);
            if (result is null)
                return NotFound();
            return View(result);
        }

        [HttpGet("Skills/{trackId:int}")]
        public async Task<IActionResult> Skills(int trackId)
        {
            try
            {
                var track = await _assessmentService.GetTrackDetailsAsync(trackId);
                return View(track);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("Stack/{stackId:int}")]
        public async Task<IActionResult> Stack(int stackId)
        {
            try
            {
                var stack = await _assessmentService.GetStackDetailsAsync(stackId);
                return View(stack);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("History")]
        public async Task<IActionResult> History()
        {
            var userId = GetCurrentUserId();
            var history = await _assessmentService.GetUserAssessmentHistoryAsync(userId);
            return View(history);
        }
    }
}