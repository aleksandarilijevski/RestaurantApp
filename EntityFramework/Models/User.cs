using EntityFramework.Enums;

namespace EntityFramework.Models
{
    public class User : BaseEntity, ICloneable
    {
        public long Barcode { get; set; }

        public string FirstAndLastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public long JMBG { get; set; }

        public UserRole UserRole { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public object Clone()
        {
            return new User
            {
                ID = this.ID,
                CreatedDateTime = this.CreatedDateTime,
                ModifiedDateTime = this.ModifiedDateTime,
                Barcode = this.Barcode,
                FirstAndLastName = this.FirstAndLastName,
                DateOfBirth = this.DateOfBirth,
                JMBG = this.JMBG,
                UserRole = this.UserRole,
                IsDeleted = this.IsDeleted,
                IsActive = this.IsActive
            };
        }
    }
}
