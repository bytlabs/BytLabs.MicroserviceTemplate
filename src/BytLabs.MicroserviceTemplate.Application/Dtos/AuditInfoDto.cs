using BytLabs.Domain.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BytLabs.MicroserviceTemplate.Application.Dtos
{
    public abstract class AuditInfoDto: IAuditableEntity
    {
        public DateTime? CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? LastModifiedAt { get; set; }

        public string? LastModifiedBy { get; set; }

        public DateTime? DeletedAt { get; set; }

        public string? DeletedBy { get; set; }
    }
}
