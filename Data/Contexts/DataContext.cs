using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;
public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public virtual DbSet<BookingEntity> Bookings { get; set; }
    public virtual DbSet<StatusEntity> Statuses { get; set; }
}

