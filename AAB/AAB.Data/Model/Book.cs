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
        [Required(ErrorMessage ="Title is required!")]
        [DataType(DataType.Text)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Author is required!")]
        public string WrittenBy { get; set; }
        [Required(ErrorMessage = "Narrator is required!")]
        public string NarratedBy { get; set; }
        public TimeSpan Length { get; set; }
        public string Description { get; set; }
        public float Rate { get; set; }
        public string SampleAudio { get; set; }
        public string FullAudio { get; set; }
        public string CoverArt { get; set; }
    }
}
