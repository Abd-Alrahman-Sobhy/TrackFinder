using System.ComponentModel.DataAnnotations;
using TrackFinder.Models.CourseModels;
using TrackFinder.Models.UserModels;

namespace TrackFinder.Models.OrdersAndPaymentsModels
{
    public class Purchasing
    {
        [Key]
        public Guid PurchasingId { get; set; } = Guid.NewGuid();
        public string type { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; } 

        public Guid PurchasedItemId { get; set; }
        public Guid StudentId { get; set; }
        public Guid InstructorId { get; set; }
        public virtual Course? Course { get; set; }
        public virtual Lesson? Lesson { get; set; }
        public virtual Student Student { get; set; } = null!;
        public virtual Instructor Instructor { get; set; } = null!;
    }
}
