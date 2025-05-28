using Data.Entities;

namespace Repositories_Tests.SeedData;

public static class TestData
{

    public static readonly StatusEntity[] StatusEntities = [
        new StatusEntity{
            Id = 1,
            StatusName = "Pending"
        },
        new StatusEntity{
            Id = 2,
            StatusName = "Confirmed"
        },
        new StatusEntity{
            Id = 3,
            StatusName = "Cancelled"
        }
        ];

    public static readonly BookingEntity[] BookingEntities = [
        new BookingEntity{
            Id = "832697b2-57b7-4c5c-a033-f444f5415588",
            StatusId = 1,
            EventId = "a2beec0a-7ef7-4617-9901-50cfc5fab680" ,
            InvoiceId = "c9755db1-c394-4e84-b425-b4d1801ef079",
            UserId = "52df0a8f-66b1-4fcd-85ca-63bb95634526",
            TicketCategoryName = "Gold",
            TicketPrice = 500,
            TicketQuantity = 2,
            CreateDate = DateTime.Parse("2025-04-10 14:00:00")
        },

         new BookingEntity{
            Id = "9510c850-fd24-486e-8154-c71e0859cdd2",
            StatusId = 1,
            EventId = "a2beec0a-7ef7-4617-9901-50cfc5fab680" ,
            InvoiceId = "a2be8f7d-8e19-4c62-b610-8863ffef6b3c",
            UserId = "925839c4-441a-4db7-98e9-f3ed8e290f3b",
            TicketCategoryName = "Gold",
            TicketPrice = 500,
            TicketQuantity = 1,
            CreateDate = DateTime.Parse("2025-04-10 14:20:00")
        },
         new BookingEntity{
            Id = "e46f1c1d-26e5-432f-8897-7aad8d0d525b",
            StatusId = 1,
            EventId = "46fd63f2-e309-40d3-8ada-0981230c2247" ,
            InvoiceId = "07639695-29a0-465c-83cf-f62889d3c076",
            UserId = "925839c4-441a-4db7-98e9-f3ed8e290f3b",
            TicketCategoryName = "Silver",
            TicketPrice = 200,
            TicketQuantity = 2,
            CreateDate = DateTime.Parse("2025-03-29 09:30:00")
        },
           new BookingEntity{
            Id = "28f123a8-664c-4153-9904-22ed5358da57",
            StatusId = 1,
            EventId = "46fd63f2-e309-40d3-8ada-0981230c2247" ,
            InvoiceId = "7f032bd1-9e53-4a0e-9df4-8f3fa5746822",
            UserId = "ee78a1a7-0e35-4e1c-b2ae-2228e748f292",
            TicketCategoryName = "Standard",
            TicketPrice = 100,
            TicketQuantity = 3,
            CreateDate = DateTime.Parse("2025-05-12 15:45:00")
        }
        ];

}
