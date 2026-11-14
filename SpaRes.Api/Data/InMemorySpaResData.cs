using SpaRes.Api.Models;

namespace SpaRes.Api.Data
{
    public static class InMemorySpaResData
    {
        public static List<StaffSchedule> StaffSchedules { get; } =
        [
            new StaffSchedule
            {
                StaffScheduleId = 1,
                StaffId = 1,
                LocationId = 1,
                Date = new DateOnly(2026, 9, 13),
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(18, 0)
            },
            new StaffSchedule
            {
                StaffScheduleId = 2,
                StaffId = 1,
                LocationId = 1,
                Date = new DateOnly(2026, 9, 14),
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(18, 0)
            },
           
            new StaffSchedule
            {
                StaffScheduleId = 3,
                StaffId = 1,
                LocationId = 2,
                Date = new DateOnly(2026, 9, 15),
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(18, 0)
            },
            new StaffSchedule
            {
                StaffScheduleId = 4,
                StaffId = 3,
                LocationId = 1,
                Date = new DateOnly(2026, 9, 12),
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(18, 0)
            }
        ];
        public static List<StaffBreak> StaffBreaks { get; } =
        [
            new StaffBreak
            {
                StaffBreakId = 1,
                StaffId = 1,
                LocationId = 1,
                Date = new DateOnly(2026, 9, 12),
                StartTime = new TimeOnly(12, 0),
                EndTime = new TimeOnly(15, 0),
                Note = "Doctor appointment"
            }
        ];
        public static List<StaffAbsence> StaffAbsences { get; } =
        [
            new StaffAbsence
            {
                StaffAbsenceId = 1,
                StaffId = 1,
                StartDate = new DateOnly(2026, 9, 16),
                EndDate = new DateOnly(2026, 9, 20),
                Note = "Out of town-2"
            }
        ];
        public static List<Room> Rooms { get; } =
        [
            new Room
            {
                RoomId = 1,
                LocationId = 1,
                RoomName = "Room 1",
                Capacity = 2,
                OutOfService = false
            },
            new Room
            {
                RoomId = 2,
                LocationId = 1,
                RoomName = "Room 2",
                Capacity = 1,
                OutOfService = true
            },
            new Room
            {
                RoomId = 3,
                LocationId = 2,
                RoomName = "Room 3",
                Capacity = 2,
                OutOfService = false
            },
             new Room
            {
                RoomId = 4,
                LocationId = 1,
                RoomName = "Room 4",
                Capacity = 1,
                OutOfService = false
            }
        ];

        public static List<StaffService> StaffServices { get; } =
        [
            new StaffService
            {
                StaffServiceId = 1,
                StaffId = 1,
                ServiceId = 1
            },
            new StaffService
            {
                StaffServiceId = 2,
                StaffId = 3,
                ServiceId = 1
            }
        ];
        public static List<Service> Services { get; } =
        [
            new Service
            {
                ServiceId = 1,
                TenantId = 1,
                Description = "Swedish Massage",
                Amount = 95.00m,
                Availability = true,
                CatId = 1,
                ShortName = "SWEDISH",
                ServiceDuration = 60
            },
            new Service
            {
                ServiceId = 2,
                TenantId = 1,
                Description = "Inactive Test Service",
                Amount = 100.00m,
                Availability = true,
                CatId = 1,
                ShortName = "INACTIVE",
                ServiceDuration = 60
            }
        ];

        public static List<LocationService> LocationServices { get; } =
        [
            new LocationService
            {
                LocationServiceId = 1,
                LocationId = 1,
                ServiceId = 1,
                Available = true
            },
            new LocationService
            {
                LocationServiceId = 2,
                LocationId = 2,
                ServiceId = 1,
                Available = false
            },
            new LocationService
            {
                LocationServiceId = 3,
                LocationId = 1,
                ServiceId = 2,
                Available = true
            }
        ];
    }


}