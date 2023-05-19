using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.Infrastructure;

namespace CRL.Model.FS
{
    [Serializable ]
    public partial class PersonIdentification : AuditedEntityBaseModel<int>, IAggregateRoot 
    {
        public PersonIdentification()
        {
            Identification = new PersonIdentificationInfo();
        }
        public PersonIdentificationInfo Identification { get; set; }
        public string UniqueCode { get; set; }
        //Relationships
        public int? ClonedId { get; set; }

        public virtual LKPersonIdentificationCategory PersonIdentificationType { get; set; }
        public int IndividualParticipantId { get; set; }
        public virtual IndividualParticipant IndividualParticipant { get; set; }

        public int PersonIdentificationTypeId { get; set; }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public partial class LKPersonIdentificationCategory : EntityBase<int>, IAggregateRoot
    {
        [MaxLength(50)]
        public string PersonIdentificationCardCategoryName { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
