using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;

namespace Data.Repositories;



public class BookingStatusRepository(DataContext context) : BaseRepository<StatusEntity, StatusModel>(context), IBookingStatusRepository
{
    private readonly DataContext _context = context;


}

