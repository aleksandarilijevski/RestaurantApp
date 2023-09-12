namespace EntityFramework.Models
{
    public class Waiter : BaseEntity
    {
        public long Barcode { get; set; }

        public string FirstAndLastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public long JMBG { get; set; }
    }
}
