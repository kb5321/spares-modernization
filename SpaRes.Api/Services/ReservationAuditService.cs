using SpaRes.Api.Data;
using SpaRes.Api.Models;

namespace SpaRes.Api.Services
{
    public class ReservationAuditService
    {
        private readonly SpaResDbContext _db;

        public ReservationAuditService(SpaResDbContext db)
        {
            _db = db;
        }

        public void AddLog(
            Reservation reservation,
            int updatedByStaffId,
            string logInfo,
            string valueBefore,
            string valueAfter)
        {
            var log = new ResLog
            {
                TenantId = reservation.TenantId,
                ReservationId = reservation.ReservationId,
                UpdatedByStaffId = updatedByStaffId,
                LogTime = DateTime.Now,
                LogInfo = logInfo,
                LogFieldNumber = 0,
                LogValueBefore = valueBefore,
                LogValueAfter = valueAfter
            };

            _db.ResLogs.Add(log);
        }
    }
}