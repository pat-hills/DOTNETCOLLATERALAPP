using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.Messaging;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.Search;
using CRL.Model.ModelViews.FinancingStatement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViewMappers;
using CRL.Model.ModelViews.Search;

namespace CRL.Model.Model.Search
{
    public abstract class SearchFSReportBuilder
    {
        protected List<FSReportView> FS { get; set; }
        protected List<SearchParamView> SearchParams = new List<SearchParamView>();
        protected List<CACSearchView> CACSearches = new List<CACSearchView>();
        protected List<SecuredPartyReportView> Lenders { get; set; }
        protected List<DebtorReportView> Borrowers { get; set; }
        protected List<CollateralReportView> Collaterals { get; set; }
        protected List<UpdateActivityReportView> Updates { get; set; }
        protected List<DischargeActivityReportView> DischargeActivity { get; set; }
        protected List<AssignmentActivityReportView> AssignmentActivity { get; set; }
        protected List<SubordinationActivityReportView> SubordinationActivity { get; set; }
        protected List<ClientReportView> AssignedParty { get; set; }
        protected List<SubordinatingPartyReportView> SubordinatingParty { get; set; }
        protected List<CollateralReportView> DischargedCollaterals { get; set; }
        protected List<ChangeDescriptionView> ChangeDescription { get; set; }
        protected List<OtherIdentificationReportView> OtherIdentifications { get; set; }
        protected List<FSActivitySummaryReportView> Activities { get; set; }
        protected DateTime generatedDate { get; set; }

        public SearchFSReportBuilder()
        {

        }
        
        public void BuildSearchReport(SearchFinancialStatement searchFS, bool IsCertified, List<FinancialStatement> financialStatements, List<ClientReportView> AssigneeNAssignor, LookUpForFS fsLookUps, DateTime GeneratedDate)
        {

            SearchParam _legalSearchParam = SearchParameterHelper.GetObjectFromXML<SearchParam>(searchFS.SearchParamXML);
            //Here we try to convert from USA timezone to Nigeria Timezone
            TimeZoneInfo nigeriaTimezone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
            DateTime convertedTime = TimeZoneInfo.ConvertTime(searchFS.CreatedOn,TimeZoneInfo.Local, nigeriaTimezone);

            generatedDate = TimeZoneInfo.ConvertTime(GeneratedDate, TimeZoneInfo.Local, nigeriaTimezone);

            //searchFS.GeneratedReport = new FileUpload();
            //var reportDate = searchFS.GeneratedReport.CreatedOn;
            SearchParamView _searchParamView = new SearchParamView
            {
                BorrowerFirstName = _legalSearchParam.BorrowerFirstName,
                BorrowerIDNo = _legalSearchParam.BorrowerIDNo,
                BorrowerType = _legalSearchParam.SearchType == 1 ? "Individual" : "Financial Institution",
                BorrowerLastName = _legalSearchParam.BorrowerLastName,
                BorrowerMiddleName = _legalSearchParam.BorrowerMiddleName,
                BorrowerName = _legalSearchParam.BorrowerName,
                CollateralDescription = _legalSearchParam.CollateralDescription,
                CollateralSerialNo = _legalSearchParam.CollateralSerialNo,
                IsNonLegalEffectSearch = searchFS.IsLegalOrFlexible == 1 ? "Legally Effective Search" : "Flexible Search",
                DebtorEmail = _legalSearchParam.DebtorEmail,
                SearchCode = searchFS.SearchCode,
                ReportType = IsCertified == true ? "Certified Search Result" : "Uncertified Search Result",
                DateOfSearch = convertedTime

            };

            if (_legalSearchParam.DebtorDateOfBirth != null)
            {
                _searchParamView.DebtorDateOfBirth = _legalSearchParam.DebtorDateOfBirth.StartDate;
            }

            SearchParams.Add(_searchParamView);

            if (searchFS.CACResultsXML != null)
            {
                CACSearch CACResult = SearchParameterHelper.GetObjectFromXML<CACSearch>(searchFS.CACResultsXML);
                if (CACResult != null)
                {
                    CACSearchView _cacResults = new CACSearchView
                    {
                        Name = CACResult.Name,
                        Deed_Title = CACResult.Deed_Title == null ? "N/A" : CACResult.Deed_Title,
                        Asset = CACResult.Asset == null ? "N/A" : CACResult.Asset,
                        Description = CACResult.Description == null ? "N/A" : CACResult.Description,
                        Type = CACResult.Type == null ? "N/A" : CACResult.Type,
                        Serial_No = CACResult.Serial_No == null ? "N/A" : CACResult.Serial_No,
                        Amount = CACResult.Amount == null ? "N/A" : CACResult.Amount,
                        In_Words = CACResult.In_Words == null ? "N/A" : CACResult.In_Words,
                        Currency = CACResult.Currency == null ? "N/A" : CACResult.Currency,
                        Bank = CACResult.Bank == null ? "N/A" : CACResult.Bank,
                        Date = CACResult.Date == null ? "N/A" : CACResult.Date,
                        BVN = CACResult.BVN == null ? "N/A" : CACResult.BVN
                    };
                    CACSearches.Add(_cacResults);
                }
            }

            //        var financialStatements = _financialStatementRepository.GetDbSet().Where(s => request.SelectedFS.Contains(s.Id)).ToList();


            //        //Now we need to get the full details for each of the financingstatement
            FS = new List<FSReportView>();
            Activities = new List<FSActivitySummaryReportView>();
            Lenders = new List<SecuredPartyReportView>();
            Borrowers = new List<DebtorReportView>();
            Collaterals = new List<CollateralReportView>();
            OtherIdentifications = new List<OtherIdentificationReportView>();
            Updates = new List<UpdateActivityReportView>();
            ChangeDescription = new List<ChangeDescriptionView>();
            SubordinationActivity = new List<SubordinationActivityReportView>();
            DischargeActivity = new List<DischargeActivityReportView>();
            AssignmentActivity = new List<AssignmentActivityReportView>();
            SubordinatingParty = new List<SubordinatingPartyReportView>();

            OtherIdentifications = new List<OtherIdentificationReportView>();
            foreach (var financialStatement in financialStatements)
            {
                FS.Add(financialStatement.ConvertToFSReportView());
                Lenders.AddRange(financialStatement.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsDeleted == false).ConvertToSecuredPartyReportView());
                Borrowers.AddRange(financialStatement.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsDeleted == false).ConvertToDebtorReportView());
                Collaterals.AddRange(financialStatement.Collaterals.Where(s => s.IsDeleted == false && (s.IsDischarged == false || (s.DischargeActivityId == s.FinancialStatement.DischargeActivityId))).ConvertToCollateralReportViews());
                Activities.AddRange(financialStatement.FinancialStatementActivities.Where(s => s.isApprovedOrDenied == 1 && s.IsDeleted == false && s.IsActive == true).ConvertToFSActivitySummaryReportView());
                OtherIdentifications.AddRange(financialStatement.Participants.OfType<IndividualParticipant>().Where(s => s.OtherPersonIdentifications.Count > 0).GetOtherIdentificationReportView());

                //            //Now let's get the updates
                foreach (FinancialStatementActivity fsActivity in financialStatement.FinancialStatementActivities.Where(s => s.isApprovedOrDenied == 1 && s.IsDeleted == false && s.IsActive == true).OrderByDescending(d => d.CreatedOn))
                {
                    if (fsActivity is ActivityUpdate)
                    {

                        //Updates = new List<UpdateActivityReportView>();
                        //ChangeDescription = new List<ChangeDescriptionView>();
                        FinancialStatement previousFS = ((ActivityUpdate)fsActivity).PreviousFinancialStatement;
                        if (FS.Where(s => s.Id == previousFS.Id).Count() < 1)
                        {
                            FS.Add(previousFS.ConvertToFSReportView(fsLookUps));

                            Lenders.AddRange(previousFS.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsDeleted == false).ConvertToSecuredPartyReportView());
                            Borrowers.AddRange(previousFS.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsDeleted == false).ConvertToDebtorReportView());
                            Collaterals.AddRange(previousFS.Collaterals.Where(s => s.IsDeleted == false && (s.IsDischarged == false || (s.DischargeActivityId != s.FinancialStatement.DischargeActivityId))).ConvertToCollateralReportViews());
                            OtherIdentifications.AddRange(previousFS.Participants.OfType<IndividualParticipant>().Where(s => s.OtherPersonIdentifications.Count > 0).GetOtherIdentificationReportView());
                        }
                        if (previousFS.AfterUpdateFinancialStatementId != financialStatement.Id)
                        {
                            FinancialStatement afterUpdateFS = previousFS.AfterUpdateFinancialStatement;
                            if (FS.Where(s => s.Id == afterUpdateFS.Id).Count() < 1)
                            {
                                FS.Add(afterUpdateFS.ConvertToFSReportView());


                                Lenders.AddRange(afterUpdateFS.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsDeleted == false).ConvertToSecuredPartyReportView());
                                Borrowers.AddRange(afterUpdateFS.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsDeleted == false).ConvertToDebtorReportView());
                                Collaterals.AddRange(afterUpdateFS.Collaterals.Where(s => s.IsDeleted == false && (s.IsDischarged == false || (s.DischargeActivityId != s.FinancialStatement.DischargeActivityId))).ConvertToCollateralReportViews());
                                OtherIdentifications.AddRange(afterUpdateFS.Participants.OfType<IndividualParticipant>().Where(s => s.OtherPersonIdentifications.Count > 0).GetOtherIdentificationReportView());
                            }
                        }
                        //Add the updates details
                        UpdateActivityReportView updatereport = (UpdateActivityReportView)fsActivity.ConvertToActivityReportView();
                        updatereport.BeforeUpdateFinancialStatementId = previousFS.Id;
                        updatereport.FinancialStatementId = (int)previousFS.AfterUpdateFinancialStatementId;
                        Updates.Add(updatereport);

                        string xmlchangeDescription = ((ActivityUpdate)fsActivity).UpdateXMLDescription;
                        IEnumerable<ChangeDescriptionView> _ChangeDescription = new List<ChangeDescriptionView>();
                        _ChangeDescription = ((ActivityUpdate)fsActivity).GetOperationDescription(xmlchangeDescription).ConvertToChangeDescriptionView();
                        foreach (var item in _ChangeDescription)
                        {
                            item.UpdateId = fsActivity.Id;
                        }
                        ChangeDescription.AddRange(_ChangeDescription);
                        //**Add the change description details
                    }


                    else if (fsActivity is ActivitySubordination)
                    {
                        // SubordinationActivity = new List<SubordinationActivityReportView>();
                        SubordinationActivity.Add((SubordinationActivityReportView)fsActivity.ConvertToActivityReportView());
                        //Now create the view for the activity summary and the discharged collaterals

                        SubordinatingParty.Add(((ActivitySubordination)fsActivity).SubordinatingParticipant.ConvertToSubordinatingPartyReportView());
                    }
                    else if (fsActivity is ActivityDischarge)
                    {
                        //DischargeActivity = new List<DischargeActivityReportView>();
                        DischargeActivity.Add((DischargeActivityReportView)fsActivity.ConvertToActivityReportView());
                        //Now create the view for the activity summary and the discharged collaterals

                        if (((ActivityDischarge)fsActivity).DischargeType == 2)
                        {
                            DischargedCollaterals.AddRange(((ActivityDischarge)fsActivity).Collaterals.ConvertToCollateralReportViews());
                        }

                    }
                    else if (fsActivity is ActivityAssignment)
                    {

                        AssignmentActivity.Add((AssignmentActivityReportView)fsActivity.ConvertToActivityReportView());
                        //Now create the view for the activity summary and the discharged collaterals

                        //Model.Membership.Membership _membership = _membershipRepository.FindBy(((ActivityAssignment)fsActivity).AssignedMembershipId);
                        AssignedParty = AssigneeNAssignor;

                    }
                }

            }






        }

        public abstract Byte[] GenerateVerificationReport(string IdentifierId);

    }
}
