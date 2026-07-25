using System.ComponentModel.DataAnnotations;

namespace TrackFinder.Models.AchievementModels
{
    public class Badges
    {
        [Key]
        public int BadgeId { get; set; }
        public string? BadgeName { get; set; }
        public string? BadgeDescription { get; set; }
        public string? BadgeImageUrl { get; set; }

        // حقول جديدة
        public string? BadgeIconClass { get; set; }
        public int Level { get; set; } = 1;
        public string? UnlockCondition { get; set; }

        public virtual ICollection<UserBadge>? UserBadges { get; set; }
    }
}