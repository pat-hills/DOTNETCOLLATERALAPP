using CRL.Model.ModelViews.FinancingStatement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViewMappers;
using CRL.Model.FS.Enums;
using CRL.Infrastructure.Reporting;
using CRL.Model.ModelService;
using CRL.Model.ModelViews.Amendment;

namespace CRL.Model.FS
{
    public abstract class FSDetailReportBuilder 
    {

        public List<FSReportView> FS { get; private set; }
        public List<SecuredPartyReportView> Lenders { get; private set; }
        public List<DebtorReportView> Borrowers { get; private set; }
        public List<CollateralReportView> Collaterals { get; private set; }
        public List<FSActivitySummaryReportView> Activities { get; private set; }
        public List<OtherIdentificationReportView> OtherIdentifications { get; private set; }
        public DateTime generatedDate { get; set; }

        public FSDetailReportBuilder()
        {

        }

        public void BuildCurrentReport(FinancialStatement fs, LookUpForFS fsLookUps, DateTime reportDate)
        {
            //Here we try to convert from USA timezone to Nigeria Timezone
            TimeZoneInfo nigeriaTimezone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
            generatedDate = TimeZoneInfo.ConvertTime(reportDate, TimeZoneInfo.Local, nigeriaTimezone);

            FS = new List<FSReportView>();
            FSReportView fsReportView = fs.ConvertToFSReportView(fsLookUps);          
            FS.Add(fsReportView);

            Lenders = new List<SecuredPartyReportView>();
            Lenders.AddRange(fs.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsSecuredParty).ConvertToSecuredPartyReportView(fsLookUps));
            
            Borrowers = new List<DebtorReportView>();
            Borrowers.AddRange(fs.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsBorrower).ConvertToDebtorReportView(fsLookUps));
            
            Collaterals = new List<CollateralReportView>();
            var collateralReportView = fs.Collaterals.ConvertToCollateralReportViews(fsLookUps);
             Collaterals.AddRange(collateralReportView);

            OtherIdentifications = new List<OtherIdentificationReportView>();
            OtherIdentifications.AddRange(fs.Participants.OfType<IndividualParticipant>().Where(s => s.OtherPersonIdentifications.Count > 0).GetOtherIdentificationReportView(fsLookUps));

            Activities = new List<FSActivitySummaryReportView>();
            Activities.AddRange(fs.FinancialStatementActivities.Where(s => s.isApprovedOrDenied == 1 && s.IsDeleted == false && s.IsActive == true).ConvertToFSActivitySummaryReportView());
             
        }

        public void BuildVerificationReport(FinancialStatement fs, LookUpForFS fsLookUps, DateTime reportDate)
        {
            //Here we try to convert from USA timezone to Nigeria Timezone
            TimeZoneInfo nigeriaTimezone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
            generatedDate = TimeZoneInfo.ConvertTime(reportDate, TimeZoneInfo.Local, nigeriaTimezone);

            FS = new List<FSReportView>();
            FSReportView fsReportView = fs.ConvertToFSReportView(fsLookUps);
            fsReportView.IsDischarged = false;            
            FS.Add(fsReportView);

            Lenders = new List<SecuredPartyReportView>();
            Lenders.AddRange(fs.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsSecuredParty).ConvertToSecuredPartyReportView(fsLookUps));
            Borrowers = new List<DebtorReportView>();
            Borrowers.AddRange(fs.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsBorrower).ConvertToDebtorReportView(fsLookUps));
            Collaterals = new List<CollateralReportView>();           
            var collateralReportView = fs.Collaterals.ConvertToCollateralReportViews(fsLookUps);
            foreach (var c in collateralReportView)
            {
                c.IsDischarged = false;
                c.DischargedId = null;
                
            }
            Collaterals.AddRange(collateralReportView);
            OtherIdentifications = new List<OtherIdentificationReportView>();
            OtherIdentifications.AddRange(fs.Participants.OfType<IndividualParticipant>().Where(s => s.OtherPersonIdentifications.Count > 0).GetOtherIdentificationReportView(fsLookUps));

             
                
        }
        public void FSVerificationReportGenerator(List<FSReportView> _fs, List<SecuredPartyReportView> _lenders, List<DebtorReportView> _borrowers,
            List<CollateralReportView> _collaterals,List<OtherIdentificationReportView > _otheridentifications=null, List<FSActivitySummaryReportView> _activities=null)
        {
            FS = _fs;
            Lenders = _lenders;
            Borrowers = _borrowers;
            Collaterals = _collaterals;
            OtherIdentifications = _otheridentifications;
            Activities = _activities;
        }

        public abstract Byte[] GenerateVerificationReport(string IdentifierId);
        public abstract Byte[] GenerateCurrentReport(string IdentifierId);
       
      
    }
}
