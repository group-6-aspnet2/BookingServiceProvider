using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;
using Domain.Responses;
using System.Diagnostics;

namespace Data.Repositories;

public class BookingRepository(DataContext context) : BaseRepository<BookingEntity, BookingModel>(context), IBookingRepository
{

    private readonly DataContext _context = context;

 
}
