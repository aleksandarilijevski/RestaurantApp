using EntityFramework.Enums;

namespace EntityFramework.Models
{
    public class User : BaseEntity
    {
        public long Barcode { get; set; }

        public string FirstAndLastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public long JMBG { get; set; }

        public UserRole UserRole { get; set; }

        public bool IsDeleted { get; set; }
    }
}
