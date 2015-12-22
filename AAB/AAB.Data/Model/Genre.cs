using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAB.Data.Model
{
    //public enum GenreList
    //{
    //    [Description("Science Fiction")]
    //    ScienceFiction = 1,
    //    Satire = 2,
    //    Drama = 3,
    //    [Description("Action and Adventure")]
    //    ActionandAdventure = 4,
    //    Romance = 5,
    //    Mystery = 6,
    //    Horror = 7,
    //    [Description("Self Help")]
    //    SelfHelp = 8
    //}
    public class Genre
    {
        public int Id { get; set; }
        [DataType(DataType.Text, ErrorMessage = "Invalid data format!")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Length must be between 3 and 50 characters long.")]
        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }
    
}
