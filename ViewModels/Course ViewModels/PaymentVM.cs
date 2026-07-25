using System.ComponentModel.DataAnnotations;

namespace TrackFinder.ViewModels.Course_ViewModels
{
    public class PaymentVM
    {
        public Guid CourseId { get; set; }

        [Required]
        [Display(Name = "Card Number")]
        [CreditCard]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Expiry Date")]
        public string ExpiryDate { get; set; } = string.Empty;

        [Required]
        public string CVV { get; set; } = string.Empty;
    }
}
