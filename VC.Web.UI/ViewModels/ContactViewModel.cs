namespace VillaCollective.Web.UI.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class ContactViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }
        
        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check In")]
        public DateTime CheckIn { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check Out")]
        public DateTime CheckOut { get; set; }

        [Display(Name = "Minimum Bedrooms")]
        public short BedroomsMin { get; set; }

        [Display(Name = "Maximum Bedrooms")]
        public short BedroomsMax { get; set; }
        
        [Display(Name = "Adults")]
        public short NumberOfAdults { get; set; }
        
        [Display(Name = "Children")]
        public short NumberOfChildren { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public string Location { get; set; }

        [Display(Name = "Villa Name")]
        public string VillaName { get; set; }

    }
}
