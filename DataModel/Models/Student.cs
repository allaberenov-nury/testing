using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InnovationTest.Models
{
    public class Student
    {
        public int Id { get; set; }
       
       
        [MaxLength(450)]
        [Required]
        public string OwnerID { get; set; }

        [RegularExpression(@"^[\S]+([\s]?[\S])*$", ErrorMessage = @"Adyň başynda, ahyrynda we arasynda artyk boş ýer goýmaň.")]
        [Required(ErrorMessage = "Adyňyzy giriziň.")]
        [MaxLength(50, ErrorMessage = "Adyň uzynlygy 50 simwoldan geçmeli däl.")]
        [Display(Name = "Ady")]
        public string Name { get; set; }

        [RegularExpression(@"^[\S]+([\s]?[\S])*$", ErrorMessage = @"Familýaňyň başynda, ahyrynda we arasynda artyk boş ýer goýmaň.")]
        [Required(ErrorMessage = "Familýaňyzy giriziň.")]
        [MaxLength(50, ErrorMessage = "Familýaňyň uzynlygy 50 simwoldan geçmeli däl.")]
        [Display(Name = "Familýasy")]
        public string LastName { get; set;}

        [RegularExpression(@"^[\S]+([\s]?[\S])*$", ErrorMessage = @"Familýaňyzyň başynda, ahyrynda we arasynda artyk boş ýer goýmaň.")]
        [MaxLength(50, ErrorMessage = "Ata adynyň 50 simwoldan geçmeli däl.")]
        [Display(Name = "Atasynyň ady")]
        public string Patronymic { get; set; }


        [Required(ErrorMessage ="Telefon belgiňizi giriziň.")]
        [Phone]
        [Display(Name ="Telefon")]
        [MaxLength(25)]
        
        public string PhoneNumber { get; set; }




        [Required]
        [Display(Name = "Okuw ýyly")]
        [Range(1,10,ErrorMessage ="1-11 aralygynda bolmaly.")]
        public int StudyYear { get; set; }
        public DateTime ModifiedTime { get; set; }

        public string FullName
        {
            get
            {
                return LastName + " " + Name + " " + Patronymic;
            }
        }
        [Required(ErrorMessage = "Okuw jaýyny saýlaň.")]
        [Display(Name = "Okuw jayy")]

        public int UniversityId { get; set; }

        
        public University University { get; set; }

        public ICollection<TestResult> TestResults { get; set; }
    }
}
