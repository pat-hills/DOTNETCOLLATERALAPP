using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.ModelViews.FinancingStatement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViewMappers;

namespace CRL.Model.Model.FS
{
    public abstract class FSActivityReportBuilder
    {
        public List<FSReportView> FS{get;set;}
        public List<DischargeActivityReportView> DischargeActivity{get;set;}
        public List<AssignmentActivityReportView> AssignmentActivity{get;set;}
        public List<ActivityReportView> Activity{get;set;}
        public List<SecuredPartyReportView> Lenders{get;set;}
        public List<DebtorReportView> Borrowers{get;set;}
        public List<CollateralReportView> Collaterals{get;set;}
        public List<FSActivitySummaryReportView> Activities{get;set;}
        public List<OtherIdentificationReportView> OtherIdentifications{get;set;}
        public List<ChangeDescriptionView> ChangeDescription{get;set;}
        public List<CollateralReportView> DischargedCollaterals{get;set;}
        public List<SubordinatingPartyReportView> SubordinatingParty{get;set;}
        public List<ClientReportView> AssignedParty{get;set;}
        protected FinancialStatementActivityCategory activityType;
        protected DateTime generatedDate { get; set; }
        public FSActivityReportBuilder()
        {

        }

        public void BuildVerificationReport(FinancialStatementActivity fsActivity, List<ClientReportView> AssigneeNAssignor, LookUpForFS fsLookUps, DateTime GeneratedDate)
        {
            //Here we try to convert from USA timezone to Nigeria Timezone
            TimeZoneInfo nigeriaTimezone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
            generatedDate = TimeZoneInfo.ConvertTime(GeneratedDate, TimeZoneInfo.Local, nigeriaTimezone);

            activityType = fsActivity.FinancialStatementActivityTypeId;
           
           FS = new List<FSReportView>();           
            Lenders = new List<SecuredPartyReportView>();
          Borrowers = new List<DebtorReportView>();
           Collaterals = new List<CollateralReportView>();
            OtherIdentifications = new List<OtherIdentificationReportView>();
            Activity = new List<ActivityReportView>();
            if (fsActivity.FinancialStatementActivityTypeId == FinancialStatementActivityCategory.Update)
            {
               
                string xmlchangeDescription = ((ActivityUpdate)fsActivity).UpdateXMLDescription;
              ChangeDescription = new List<ChangeDescriptionView>();
                ChangeDescription.AddRange(((ActivityUpdate)fsActivity).GetOperationDescription(xmlchangeDescription).ConvertToChangeDescriptionView());
                FSReportView fs = fsActivity.FinancialStatement.ConvertToFSReportView(fsLookUps);
                fs.Id = 0;
                FS.Add(fs);
                Lenders.AddRange(fsActivity.FinancialStatement.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsSecuredParty).ConvertToSecuredPartyReportView(fsLookUps));
                foreach (var p in Lenders) { p.FinancialStatementId = 0; }
                Borrowers.AddRange(fsActivity.FinancialStatement.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsBorrower).ConvertToDebtorReportView(fsLookUps));
                foreach (var p in Borrowers) { p.FinancialStatementId = 0; }
                Collaterals.AddRange(fsActivity.FinancialStatement.Collaterals.ConvertToCollateralReportViews(fsLookUps));
                foreach (var p in Collaterals) { p.FinancialStatementId = 0; }
                OtherIdentifications.AddRange(fsActivity.FinancialStatement.Participants.OfType<IndividualParticipant>().Where(s => s.OtherPersonIdentifications.Count > 0).GetOtherIdentificationReportView(fsLookUps));

                FSReportView fs2 = ((ActivityUpdate)fsActivity).PreviousFinancialStatement.ConvertToFSReportView(fsLookUps);
                fs2.Id = 1;
                FS.Add(fs2);
                ICollection<SecuredPartyReportView> securedparty2 = ((ActivityUpdate)fsActivity).PreviousFinancialStatement.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsSecuredParty).ConvertToSecuredPartyReportView(fsLookUps);
                foreach (var p in securedparty2) { p.FinancialStatementId = 1; }
                Lenders.AddRange(securedparty2);

                ICollection<DebtorReportView> debtor2 = ((ActivityUpdate)fsActivity).PreviousFinancialStatement.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsBorrower).ConvertToDebtorReportView(fsLookUps);
                foreach (var p in debtor2) { p.FinancialStatementId = 1; }
                Borrowers.AddRange(debtor2);

                ICollection<CollateralReportView> collateral2 = ((ActivityUpdate)fsActivity).PreviousFinancialStatement.Collaterals.ConvertToCollateralReportViews(fsLookUps);
                foreach (var p in collateral2) { p.FinancialStatementId = 1; }
                Collaterals.AddRange(collateral2);

                OtherIdentifications.AddRange(((ActivityUpdate)fsActivity).PreviousFinancialStatement.Participants.OfType<IndividualParticipant>().Where(s => s.OtherPersonIdentifications.Count > 0).GetOtherIdentificationReportView(fsLookUps));

              
                Activity.Add(fsActivity.ConvertToActivityReportView());               

            }
            else
            {
               
                FSReportView fs = fsActivity.FinancialStatement.ConvertToFSReportView(fsLookUps);
                FS.Add(fs);
                Lenders.AddRange(fsActivity.FinancialStatement.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsSecuredParty).ConvertToSecuredPartyReportView(fsLookUps));
                Borrowers.AddRange(fsActivity.FinancialStatement.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsBorrower).ConvertToDebtorReportView(fsLookUps));
                Collaterals.AddRange(fsActivity.FinancialStatement.Collaterals.Where(s => s.IsDeleted == false).ConvertToCollateralReportViews(fsLookUps));
                OtherIdentifications.AddRange(fsActivity.FinancialStatement.Participants.OfType<IndividualParticipant>().Where(s => s.OtherPersonIdentifications.Count > 0).GetOtherIdentificationReportView(fsLookUps));
                if (fsActivity.FinancialStatementActivityTypeId == FinancialStatementActivityCategory.FullDicharge || fsActivity.FinancialStatementActivityTypeId == FinancialStatementActivityCategory.PartialDischarge)
                {
                    DischargeActivity = new List<DischargeActivityReportView>();
                    DischargeActivity.Add((DischargeActivityReportView)fsActivity.ConvertToActivityReportView());
                    //Now create the view for the activity summary and the discharged collaterals
                   DischargedCollaterals = new List<CollateralReportView>();
                    if (DischargeActivity[0].DischargeType == 2)
                    {
                        DischargedCollaterals.AddRange(((ActivityDischarge)fsActivity).Collaterals.ConvertToCollateralReportViews(fsLookUps));
                    }


                   
                }
                else if (fsActivity.FinancialStatementActivityTypeId == FinancialStatementActivityCategory.Subordination)
                {
                   Activity = new List<ActivityReportView>();
                    Activity.Add(fsActivity.ConvertToActivityReportView());
                    //Now create the view for the activity summary and the discharged collaterals
                    SubordinatingParty = new List<SubordinatingPartyReportView>();
                    SubordinatingParty.Add(((ActivitySubordination)fsActivity).SubordinatingParticipant.ConvertToSubordinatingPartyReportView(fsLookUps));



                  

                }
                else if (fsActivity.FinancialStatementActivityTypeId == FinancialStatementActivityCategory.FullAssignment ||
                    fsActivity.FinancialStatementActivityTypeId == FinancialStatementActivityCategory.PartialAssignment)
                {
                    AssignmentActivity = new List<AssignmentActivityReportView>();
                    AssignmentActivity.Add((AssignmentActivityReportView)fsActivity.ConvertToActivityReportView());
                    ////Now create the view for the activity summary and the discharged collaterals
                    AssignedParty = AssigneeNAssignor;

                   //CRL.Model.Membership.Membership  _membership = ((ActivityAssignment)fsActivity).AssignedMembership ;
                    //AssignedParty.Add(((ActivityAssignment)fsActivity).AssignedMembership.ConvertToClientReportView(_institutionRepository, _userRepository, LookUpForFS));
                    //Model.Membership.Membership _membershipFrom = _membershipRepository.FindBy(((ActivityAssignment)fsActivity).AssignedMembershipId);
                    //AssignedParty.Add(((ActivityAssignment)fsActivity).AssignedMembershipFrom.ConvertToClientReportView(_institutionRepository, _userRepository, LookUpForFS));
                    //report = new AmendmentVerificationReportGenerator(FS, Lenders, Borrowers, Collaterals, OtherIdentifications, AssignedParty, AssignmentActivity);

                }
            }

        }

     


        public abstract Byte[] GenerateVerificationReport(string IdentifierId);
    }
}
