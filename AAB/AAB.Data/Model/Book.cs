using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAB.Data.Model
{
    public class Book
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required!")]
        [Display(Name = "Title")]
        [DataType(DataType.Text, ErrorMessage = "Invalid data format!")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Length must be between 3 and 50 characters long.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Author is required!")]
        [Display(Name = "Author")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Length must be between 3 and 50 characters long.")]
        public string WrittenBy { get; set; }
        [Required(ErrorMessage = "Narrator is required!")]
        [Display(Name = "Narrator")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Length must be between 3 and 50 characters long.")]
        public string NarratedBy { get; set; }
        public TimeSpan? Length { get; set; }
        public string Description { get; set; }
        public float? Rate { get; set; }
        [Display(Name = "Sample Audio")]
        [DataType(DataType.Url, ErrorMessage = "Invalid data format!")]
        public string SampleAudio { get; set; }
        [Display(Name = "Full Audio")]
        [DataType(DataType.Url, ErrorMessage = "Invalid data format!")]
        public string FullAudio { get; set; }
        [Display(Name = "Cover Art")]
        [DataType(DataType.ImageUrl, ErrorMessage = "Invalid data format!")]
        public string CoverArt { get; set; }
        [Display(Name = "Genre")]
        public virtual ICollection<Genre> Genres { get; set; }
    }
}
