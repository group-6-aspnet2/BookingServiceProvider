namespace Data.Entities;

public class StatusEntity
{
    public string StatusName { get; set; } = null!;
    public int Id { get; set; }
    public virtual ICollection<BookingEntity> Bookings { get; set; } = [];
}
