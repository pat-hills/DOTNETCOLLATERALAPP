using CRL.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.ModelViews;
using CRL.Model.Memberships;

namespace CRL.Model.FS
{
    [Serializable]
    public partial class IndividualParticipant:Participant,IAggregateRoot
    {
        public IndividualParticipant()
        {
            OtherPersonIdentifications = new HashSet<PersonIdentification>();
            Identification = new PersonIdentificationInfo();
        }
        public string Title { get; set; }
        public PersonIdentificationInfo Identification { get; set; }

        public string Gender { get; set; }      
        public DateTime DOB { get; set; }
        public string OtherDocumentDescription { get; set; }
        public int? RegistrantIndividualId { get; set; } //For only secured party
        public virtual Person RegistrantIndividual { get; set; }  //For only secured party


        //Relationships
        public virtual ICollection<PersonIdentification> OtherPersonIdentifications { get; set; }
        public virtual LKPersonIdentificationCategory PersonIdentificationType { get; set; }
        public virtual LKPersonIdentificationCategory PersonIdentificationType2 { get; set; }
        public int? PersonIdentificationTypeId { get; set; }
        public int? PersonIdentificationType2Id { get; set; }

        public IndividualParticipant Duplicate()
        {
            IndividualParticipant _participant = new IndividualParticipant
            {
                Title = this.Title,
                Gender = this.Gender,
                DOB = this.DOB,
                OtherDocumentDescription = this.OtherDocumentDescription,
                PersonIdentificationTypeId = this.PersonIdentificationTypeId,
                 PersonIdentificationType2Id = this.PersonIdentificationType2Id 
                 

            };

            _participant.Identification.FirstName = this.Identification.FirstName;
            _participant.Identification.MiddleName = this.Identification.MiddleName;
            _participant.Identification.Surname = this.Identification.Surname;
            _participant.Identification.CardNo = this.Identification.CardNo;
            
            _participant.Identification.OtherDocumentDescription = this.Identification.OtherDocumentDescription;
            _participant.Identification.CardNo2 = this.Identification.CardNo2;

            foreach (var OtherIds in OtherPersonIdentifications)
            {
                PersonIdentification identification = new PersonIdentification();
                identification.Identification.FirstName = OtherIds.Identification.FirstName;
                identification.Identification.MiddleName = OtherIds.Identification.MiddleName;
                identification.Identification.Surname = OtherIds.Identification.Surname;
                identification.Identification.CardNo = OtherIds.Identification.CardNo;
                identification.Identification.OtherDocumentDescription = OtherIds.Identification.OtherDocumentDescription;
                identification.IndividualParticipant = _participant;
                identification.PersonIdentificationTypeId = OtherIds.PersonIdentificationTypeId;
                
                identification.UpdatedBy = OtherIds.UpdatedBy;
                identification.CreatedBy = OtherIds.CreatedBy;
                identification.CreatedOn = OtherIds.CreatedOn;
                identification.UpdatedOn = OtherIds.UpdatedOn;
                identification.IsActive = OtherIds.IsActive;
                identification.IsDeleted = OtherIds.IsDeleted;
                identification.UniqueCode = OtherIds.UniqueCode;
                identification.Id = OtherIds.Id;
                identification.ClonedId = OtherIds.Id;
                _participant.OtherPersonIdentifications.Add(identification);
                identification.Identification.CardNo2 = OtherIds.Identification.CardNo2;
            }


            return _participant;




        }
    }
}
