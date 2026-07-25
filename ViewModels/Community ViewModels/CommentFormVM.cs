using System.ComponentModel.DataAnnotations;

namespace TrackFinder.ViewModels.Community_ViewModels
{
    public class CommentFormVM
    {
        [Required]
        public Guid PostId { get; set; }

        public Guid? ParentCommentId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
