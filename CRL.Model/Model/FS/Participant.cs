using CRL.Model.FS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Common;
using CRL.Model.Infrastructure;
using CRL.Infrastructure.Domain;
using System.ComponentModel.DataAnnotations;
using CRL.Model.Memberships;
using CRL.Infrastructure.Helpers;

namespace CRL.Model.FS
{
    [Serializable]
    public partial class Participant : AuditedEntityBaseModel<int>,IAggregateRoot
    {
        public Participant()
        {
            Address = new AddressInfo();
            SectorOfOperationTypes = new HashSet<LKSectorOfOperationCategory>();
        }

        /// <summary>
        /// Indicate the participant leader for each participation type for a specific financial statement
        /// </summary>

        public AddressInfo Address { get; set; }
        public virtual LKNationality Nationality { get; set; }
        public string ParticipantNo { get; set; }

        public int? ClonedId { get; set; }
        
        public virtual LKCountry Country { get; set; }
        public int? NationalityId { get; set; }
        public int? RegistrantMembershipId { get; set; }
        public virtual Membership  RegistrantMembership { get; set; }
        public int? CountryId { get; set; }

        public int? LGAId { get; set; }
        public virtual LKLGA LGA { get; set; }

        public virtual LKCounty County { get; set; }

        public int? CountyId { get; set; }

        //Relationship fields
        public int FinancialStatementId { get; set; }
        public virtual FinancialStatement FinancialStatement { get; set; }
        /// <summary>
        /// Represensts the id type of the participation type association
        /// </summary>
        public  ParticipationCategory ParticipationTypeId { get; set; }
        /// <summary>
        /// Represent the id of the participant type association
        /// </summary>
        public ParticipantCategory ParticipantTypeId { get; set; }  //BUS: We can have only one lead for each participant type for the same registration
        public virtual LKParticipationCategory ParticipationType { get; set; }
        public virtual LKParticipantCategory ParticipantType { get; set; }  //BUS: We can have only one lead for each participant type for the same registration
        public virtual ICollection<LKSectorOfOperationCategory> SectorOfOperationTypes { get; set; }  //BUS: We can have only one lead for each participant type for the same registration
        public System.Nullable<DateTime> AuthorizedDate { get; set; }
        public string OtherSectorOfOperation { get; set; }
        public bool? DebtorIsAlreadyClientOfSecuredParty { get; set; }

        public string GetName(bool abbreviateMiddlename)
        {
            if (this is IndividualParticipant)
            {
                var individual = ((IndividualParticipant)(this));
                string middleName = "";
                if (abbreviateMiddlename)
                {
                    if (!String.IsNullOrWhiteSpace(individual.Identification.MiddleName))
                    {
                        middleName = individual.Identification.MiddleName.Substring(0, 1);
                    }
                }
                else
                {
                    middleName = individual.Identification.MiddleName;
                }
                return individual.Title + " " +NameHelper.GetFullName(individual.Identification.FirstName, middleName, individual.Identification.Surname);
            }
            else
            {
                var institution = ((InstitutionParticipant )(this));
                return institution.Name;
            }
        }
        public void AddSectorsOfOperationsForDebtor(int[] SectorOfOperationIds, IEnumerable<LKSectorOfOperationCategory> sectors)
        {

            foreach (int c in SectorOfOperationIds)
            {

                this.SectorOfOperationTypes.Add(sectors.Where(s => s.Id == c).Single());
            }

        }


        public Participant Duplicate(FinancialStatement fs = null, IEnumerable<LKSectorOfOperationCategory> _sectorOfOperations = null)
        {
            Participant _participant = null;
            if (this is IndividualParticipant)
            {
                _participant = ((IndividualParticipant)this).Duplicate();

            }
            else
            {
                _participant = ((InstitutionParticipant)this).Duplicate();
            }

            //_participant.Id = this.Id;
            _participant.CountryId = this.CountryId;
            _participant.LGAId = this.LGAId;
            _participant.CountyId = this.CountyId;           
            _participant.Address.Address  = this.Address.Address ;
            _participant .Address .City = this.Address .City ;
            _participant .Address .Email = this.Address .Email ;
            _participant .Address .Phone = this.Address .Phone ;
            _participant.FinancialStatement = fs;
            _participant.NationalityId = this.NationalityId;
            _participant.ParticipantTypeId = this.ParticipantTypeId;
            _participant.ParticipationTypeId = this.ParticipationTypeId;
            _participant.ParticipantNo = this.ParticipantNo;
            _participant.IsActive = this.IsActive;
            _participant.CreatedBy = this.CreatedBy;
            _participant.UpdatedBy = this.UpdatedBy;
            _participant.CreatedOn = this.CreatedOn;
            _participant.UpdatedOn = this.UpdatedOn;
            _participant.IsDeleted = this.IsDeleted;
            _participant.ClonedId = this.Id;
            _participant.DebtorIsAlreadyClientOfSecuredParty = this.DebtorIsAlreadyClientOfSecuredParty;

            foreach (var sector in this.SectorOfOperationTypes )
            {
                _participant.SectorOfOperationTypes.Add(_sectorOfOperations.Where (s=>s.Id == sector.Id ).Single ());
            }

            return _participant;


        }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }

    }
    [Serializable]
    public partial class LKParticipationCategory : EntityBase<ParticipationCategory>, IAggregateRoot
        {
        [MaxLength(50)]
            public string ParticipationCategoryName { get; set; }
            protected override void CheckForBrokenRules()
            {
                throw new NotImplementedException();
            }
        }
    [Serializable]
    public partial class LKParticipantCategory : EntityBase<ParticipantCategory>, IAggregateRoot
        {
        [MaxLength(50)]
            public string ParticipantCategoryName { get; set; }

            protected override void CheckForBrokenRules()
            {
                throw new NotImplementedException();
            }
        }
    [Serializable]
    public partial class LKSectorOfOperationCategory : EntityBase<int>, IAggregateRoot
    {
        [MaxLength(100)]
        public string SectorOfOperationCategoryName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Participant> Participants { get; set; } 

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
    public static class ParticipantExtensions
    {

        public static void MakeInActive(this ICollection<Participant> Participants)
        {
            foreach (Participant c in Participants)
            {
                c.IsActive = false;
            }
        }

        public static void MakeSelectedInactive(this ICollection<Participant> Participants, int[] ppIds)
        {
            foreach (int c in ppIds)
            {
                Participant cv = Participants.Where(s => s.Id == c).Single();
                cv.IsActive = false;
            }
        }

    }
    
}
