using Data.Entities;
using Domain.Models;

namespace Data.Interfaces;

public interface IBookingStatusRepository : IBaseRepository<StatusEntity, StatusModel>
{
}
