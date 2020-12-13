using System;

namespace TaxCalculator.DataLayer.Entities
{
    public class BaseEntity<TIdentity>
    {
        public TIdentity Id { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid CreatedBy{ get; set; }
    }
}
