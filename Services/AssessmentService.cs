using Microsoft.EntityFrameworkCore;
using TrackFinder.Context;
using TrackFinder.Models.Assessment_Models;
using TrackFinder.Models.AssessmentModels;
using TrackFinder.ViewModels.Assessment_ViewModels;

namespace TrackFinder.Services
{
    public class AssessmentService
    {
        private readonly AppDbContext _context;

        public AssessmentService(AppDbContext context)
        {
            _context = context;
        }
        private static string ResolveAverageSalary(string? averageSalary = null)
        {
            return string.IsNullOrWhiteSpace(averageSalary)
                ? "Not available"
                : averageSalary;
        }

        public async Task<List<QuestionOnAssessmentVM>> GetAssessmentQuestionsAsync()
        {
            var questions = await _context.Set<QuestionOnAssessment>()
                .Select(q => new QuestionOnAssessmentVM
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText
                })
                .ToListAsync();
            return [.. questions.OrderBy(_ => Random.Shared.Next())];
        }

        public async Task<AssessmentResultVM> GetAssessmentResultAsync(AssessmentAnswersVM answersVM)
        {
            if (answersVM.Answers is null || !answersVM.Answers.Any())
                throw new ArgumentException("Assessment answers cannot be empty.");

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == answersVM.UserId);

            if (student is null)
                throw new KeyNotFoundException("Student not found.");

            // ===== Safety net: duplicate submission guard =====
            // لو نفس اليوزر عمل submit خلال آخر 5 ثواني، متعملش insert جديد.
            // ده بيغطي أي double-submit ماوصلش يتمنع من الـ JS (double-click سريع جدًا،
            // إعادة إرسال الفورم، إلخ) بدون ما نضطر نغير شكل الـ API.
            var recentDuplicate = await _context.AssessmentResults
                .Where(r => r.UserId == student.UserId
                         && r.CreatedAt > DateTime.UtcNow.AddSeconds(-5))
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();

            if (recentDuplicate is not null)
            {
                var existingResult = await GetAssessmentResultByIdAsync(recentDuplicate.AssessmentResultId);
                if (existingResult is not null)
                    return existingResult;
            }

            var questionIds = answersVM.Answers.Select(a => a.QuestionId).ToList();

            var weights = await _context.Set<TrackQuestionsWeights>()
                .Where(w => questionIds.Contains(w.QuestionId))
                .Include(w => w.RelatedTrack!)
                    .ThenInclude(t => t.GainedSkills!)
                        .ThenInclude(ts => ts.GainedSkill)
                .Include(w => w.RelatedTrack!)
                    .ThenInclude(t => t.RelatedStacks)
                .ToListAsync();

            if (!weights.Any())
                throw new InvalidOperationException("No track weights found for the submitted questions.");

            var answersMap = answersVM.Answers.ToDictionary(a => a.QuestionId, a => a.OptionNumber);

            const int maxOptionValue = 5;

            var trackResults = weights
                .GroupBy(w => w.TrackId)
                .Select(g =>
                {
                    var track = g.First().RelatedTrack!;

                    double rawScore = 0;
                    double maxScore = 0;

                    foreach (var w in g)
                    {
                        if (!answersMap.TryGetValue(w.QuestionId, out var optionNumber))
                            continue;

                        rawScore += optionNumber * w.Weight;
                        maxScore += maxOptionValue * w.Weight;
                    }

                    var similarity = maxScore > 0
                        ? (int)Math.Round(rawScore / maxScore * 100)
                        : 0;

                    return new AssessmentResultTracksVM
                    {
                        TrackId = track.Id,
                        TrackName = track.TrackName,
                        TrackDescription = track.TrackDescription,
                        SimilarityScore = similarity,
                        AverageSalary = ResolveAverageSalary(),
                        RoadMapUrl = track.RoadMapUrl,
                        Skills = track.GainedSkills?.Select(ts => ts.GainedSkill.Name).ToList() ?? new(),
                        Stacks = track.RelatedStacks?.Select(s => new TrackStackResultVM
                        {
                            Id = s.Id,
                            StackName = s.StackName
                        }).ToList() ?? new()
                    };
                })
                .OrderByDescending(x => x.SimilarityScore)
                .Take(3)
                .ToList();

            var assessmentResult = new AssessmentResult
            {
                UserId = student.UserId,
                RecommendedTracks = trackResults.Select(t => new AssessmentResultTracks
                {
                    TrackId = t.TrackId,
                    SimilarityScore = t.SimilarityScore
                }).ToList()
            };

            _context.AssessmentResults.Add(assessmentResult);

            await _context.SaveChangesAsync();

            return new AssessmentResultVM
            {
                AssessmentResultId = assessmentResult.AssessmentResultId,
                Tracks = trackResults,
                at = assessmentResult.CreatedAt
            };
        }

        public async Task<TrackDetailsVM> GetTrackDetailsAsync(int trackId)
        {
            var track = await _context.Set<Track>()
                .Where(t => t.Id == trackId)
                .Include(t => t.RelatedStacks!)
                    .ThenInclude(s => s.RelatedStackTools!)
                        .ThenInclude(st => st.RelatedTool)
                .Include(t => t.GainedSkills!)
                    .ThenInclude(ts => ts.GainedSkill)
                .FirstOrDefaultAsync();

            if (track is null)
                throw new KeyNotFoundException($"Track is not found.");

            return new TrackDetailsVM
            {
                Id = track.Id,
                TrackName = track.TrackName,
                TrackDescription = track.TrackDescription,
                RoadMapUrl = track.RoadMapUrl,
                Stacks = track.RelatedStacks?.Select(s => new TrackStackVM
                {
                    Id = s.Id,
                    StackName = s.StackName,
                    StackDescription = s.StackDescription,
                    Tools = s.RelatedStackTools?
                        .Select(st => new ToolVM
                        {
                            Name = st.RelatedTool.Name,
                            Description = st.RelatedTool.Description
                        })
                        .ToList() ?? new List<ToolVM>()
                }).ToList() ?? new List<TrackStackVM>(),
                GainedSkills = track.GainedSkills?.Select(ts => new GainedSkillVM
                {
                    Id = ts.GainedSkill.Id,
                    Name = ts.GainedSkill.Name,
                    Description = ts.GainedSkill.Description
                }).ToList() ?? new List<GainedSkillVM>()
            };
        }

        public async Task<TrackStackVM> GetStackDetailsAsync(int stackId)
        {
            var stack = await _context.Set<TrackStack>()
                .Where(s => s.Id == stackId)
                .Include(s => s.RelatedStackTools!)
                    .ThenInclude(st => st.RelatedTool)
                .FirstOrDefaultAsync();

            if (stack is null)
                throw new KeyNotFoundException($"Stack is not found.");

            return new TrackStackVM
            {
                Id = stack.Id,
                StackName = stack.StackName,
                StackDescription = stack.StackDescription,
                Tools = stack.RelatedStackTools?.Select(st => new ToolVM
                {
                    Name = st.RelatedTool.Name,
                    Description = st.RelatedTool.Description
                }).ToList() ?? new List<ToolVM>()
            };
        }

        public async Task<AssessmentResultVM?> GetAssessmentResultByIdAsync(Guid id)
        {
            var result = await _context.Set<AssessmentResult>()
                .Where(r => r.AssessmentResultId == id)
                .Include(r => r.RecommendedTracks!)
                    .ThenInclude(rt => rt.Track!)
                        .ThenInclude(t => t.GainedSkills!)
                            .ThenInclude(ts => ts.GainedSkill)
                .Include(r => r.RecommendedTracks!)
                    .ThenInclude(rt => rt.Track!)
                        .ThenInclude(t => t.RelatedStacks)
                .FirstOrDefaultAsync();

            if (result is null)
                return null;

            return new AssessmentResultVM
            {
                AssessmentResultId = result.AssessmentResultId,
                at = result.CreatedAt,
                Tracks = result.RecommendedTracks!
                    .Select(rt => new AssessmentResultTracksVM
                    {
                        TrackId = rt.TrackId,
                        TrackName = rt.Track!.TrackName,
                        TrackDescription = rt.Track.TrackDescription,
                        SimilarityScore = rt.SimilarityScore,
                        AverageSalary = ResolveAverageSalary(),
                        RoadMapUrl = rt.Track.RoadMapUrl,
                        Skills = rt.Track.GainedSkills?.Select(ts => ts.GainedSkill.Name).ToList() ?? new List<string>(),
                        Stacks = rt.Track.RelatedStacks?.Select(s => new TrackStackResultVM
                        {
                            Id = s.Id,
                            StackName = s.StackName
                        }).ToList() ?? new List<TrackStackResultVM>()
                    })
                    .OrderByDescending(t => t.SimilarityScore)
                    .ToList()
            };
        }

        public async Task<List<AssessmentHistoryItemVM>> GetUserAssessmentHistoryAsync(Guid userId)
        {
            var results = await _context.Set<AssessmentResult>()
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Include(r => r.RecommendedTracks!)
                    .ThenInclude(rt => rt.Track)
                .ToListAsync();

            return results.Select(r =>
            {
                var top3Tracks = r.RecommendedTracks!
                    .OrderByDescending(rt => rt.SimilarityScore)
                    .Take(3)
                    .ToList();

                var tracksNames = string.Join(", ", top3Tracks.Select(rt => rt.Track!.TrackName));
                var scores = top3Tracks.Select(rt => $"{rt.Track!.TrackName} ({rt.SimilarityScore}%)").ToList();

                return new AssessmentHistoryItemVM
                {
                    AssessmentResultId = r.AssessmentResultId,
                    CreatedAt = r.CreatedAt,
                    TopTrackName = top3Tracks.FirstOrDefault()?.Track!.TrackName,
                    RecommendedTracks = tracksNames,
                    SimilarityScores = scores
                };
            }).ToList();
        }
    }
}