using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.FS.Enums;

using CRL.Model.ModelViews;
using CRL.Repository.EF.All;
using CRL.Repository.EF.All.Repository.FinancialStatement;
using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Service.Views.FinancialStatement;
using System.Data.Entity;
using CRL.Model.FS;
using CRL.Model.Common.Enum;
using CRL.Service.Messaging.Reporting.Request;

using CRL.Model.FS.IRepository;
using CRL.Model.Messaging;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.Memberships;


namespace CRL.Service.QueryGenerator
{
    public static class FSQueryGenerator
    {
        public static IQueryable<FSCustomReportView> CreateQueryForFSCustomQuery2(
         ViewFSCustomQueryRequest request, IFinancialStatementRepository _rpFS, bool DoCount)
        {
            CBLContext ctx = ((FinancialStatementRepository)_rpFS).ctx;
            IQueryable<Model.FS.FinancialStatement> query = _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.ClonedId == null && s.isApprovedOrDenied == 1 && s.IsActive == true );

            if (request.FSStateType == 1)
            {
                query = query.Where(s => s.AfterUpdateFinancialStatementId == null);
            }
            else
            {
                query = query.Where(s => !(ctx.FinancialStatements.Where(p => p.RegistrationNo == s.RegistrationNo && p.AfterUpdateFinancialStatementId != null).Select(c => c.AfterUpdateFinancialStatementId).Contains(s.Id)));

            }

            if (request.ClientId != null)
            {
                query = query.Where(s => s.MembershipId == request.ClientId);
            }

            if (request.RegistrationDate != null)
            {
                query = query.Where(s => s.RegistrationDate >= request.RegistrationDate.StartDate && s.RegistrationDate < request.RegistrationDate.EndDate);
            }
            if (request.FinancialStatementTransactionTypeId != null && request.FinancialStatementTransactionTypeId.Count() > 0)
            {
                query = query.Where(s => request.FinancialStatementTransactionTypeId.Contains(s.FinancialStatementTransactionTypeId));
            }
            if (request.FinancialStatementLoanTypeId != null && request.FinancialStatementLoanTypeId.Count() > 0)
            {
                query = query.Where(s => request.FinancialStatementLoanTypeId.Contains(s.FinancialStatementLoanTypeId));
            }
            if (request.MaximumCurrency != null && request.MaximumCurrency.Count() > 0)
            {
                query = query.Where(s => request.MaximumCurrency.Contains(s.MaximumAmountSecuredCurrencyId));
            }
            if (request.CollateralTypeId != null && request.CollateralTypeId.Count() > 0)
            {
                query = query.Where(s => request.CollateralTypeId.Contains(s.CollateralTypeId));
            }
            if (request.CollateralSubTypeId != null && request.CollateralSubTypeId.Count() > 0)
            {
                query = query.Where(s => s.Collaterals.Any(p => request.CollateralSubTypeId.Contains(p.CollateralSubTypeId) && p.IsDeleted == false && p.IsActive == true)); //Let's use first if elsa agrres for only first to be listed
            }

            if (request.DebtorTypeId != null && request.DebtorTypeId.Count() > 0)
            {
                if (!request.LimitToFirstDebtor)
                {

                    if (request.DebtorTypeId.Contains(DebtorCategory.Individual))
                    {
                        query = query.Where(s =>
                            s.Participants.OfType<IndividualParticipant>().Any(p => p.ParticipationTypeId == ParticipationCategory.AsBorrower && p.IsActive == true && p.IsDeleted == false)
                            || s.Participants.OfType<InstitutionParticipant>().Any(o => request.DebtorTypeId.Contains(o.DebtorTypeId) && o.IsActive == true && o.IsDeleted == false));
                    }
                    else
                    {
                        query = query.Where(s => s.IsActive == true && s.IsDeleted == false && s.Participants.OfType<InstitutionParticipant>().Any(o => request.DebtorTypeId.Contains(o.DebtorTypeId)));
                    }
                }
                else
                {

                    if (request.DebtorTypeId.Contains(DebtorCategory.Individual))
                    {
                        query = query.Where(s =>
                            (s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.IsActive == true && d.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ||
                           (s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.IsActive == true && d.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Insititution
                        && request.DebtorTypeId.Contains(s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.IsActive == true && d.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().DebtorTypeId))));
                    }
                    else
                    {
                        query = query.Where(s => request.DebtorTypeId.Contains(s.Participants.Where(o => o.ParticipationTypeId == ParticipationCategory.AsBorrower && o.IsActive == true && o.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().DebtorTypeId));
                    }

                }

            }
            if (request.DebtorCountryId != null && request.DebtorCountryId.Count() > 0)
            {
                if (!request.LimitToFirstDebtor)
                {

                    query = query.Where(s => s.Participants.Any(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower && request.DebtorCountryId.Contains(o.CountryId)));
                }
                else
                {
                    query = query.Where(s => request.DebtorCountryId.Contains(s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).FirstOrDefault().CountryId));
                }

            }

            if (request.DebtorCountyId != null && request.DebtorCountyId.Count() > 0)
            {
                if (!request.LimitToFirstDebtor)
                {

                    query = query.Where(s => s.Participants.Any(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower && request.DebtorCountyId.Contains(o.CountyId)));
                }
                else
                {
                    query = query.Where(s => request.DebtorCountyId.Contains(s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).FirstOrDefault().CountyId));
                }

            }
           
            if (request.MajorityOwnershipId != null && request.MajorityOwnershipId != 0)
            {


                if (!request.LimitToFirstDebtor)
                {
                    query = query.Where(s => s.Participants.OfType<IndividualParticipant>().Any(p => p.IsActive == true && p.IsDeleted == false && p.ParticipationTypeId == ParticipationCategory.AsBorrower && (request.MajorityOwnershipId == 1 || request.MajorityOwnershipId == 2) && p.Gender == (request.MajorityOwnershipId == 1 ? "Male" : "Female"))
                         || s.Participants.OfType<InstitutionParticipant>().Any(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower && o.MajorityFemaleOrMaleOrBoth == request.MajorityOwnershipId)
                         );
                }
                else
                {


                    query = query.Where(s => s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).OfType<InstitutionParticipant>().FirstOrDefault().MajorityFemaleOrMaleOrBoth == request.MajorityOwnershipId
                        || ((request.MajorityOwnershipId == 1 || request.MajorityOwnershipId == 2) && s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).OfType<IndividualParticipant>().FirstOrDefault().Gender == (request.MajorityOwnershipId == 1 ? "Male" : "Female")));
                }
            }
            if (request.ExistingRelationshipId != null)
            {


                if (!request.LimitToFirstDebtor)
                {

                    query = query.Where(s => s.FinancialStatementTransactionTypeId != FinancialStatementTransactionCategory.Lien && s.Participants.OfType<InstitutionParticipant>().Any(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower && o.DebtorIsAlreadyClientOfSecuredParty == (request.ExistingRelationshipId == 1 ? true : false)));
                }
                else
                {
                    query = query.Where(s => s.FinancialStatementTransactionTypeId != FinancialStatementTransactionCategory.Lien && s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).OfType<InstitutionParticipant>().FirstOrDefault().DebtorIsAlreadyClientOfSecuredParty == (request.ExistingRelationshipId == 1 ? true : false));
                }
            }
            if (request.DebtorNationalityId != null && request.DebtorNationalityId.Count() > 0)
            {


                if (!request.LimitToFirstDebtor)
                {

                    query = query.Where(s => s.Participants.Any(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower && request.DebtorNationalityId.Contains(o.NationalityId)));
                }
                else
                {
                    query = query.Where(s => request.DebtorNationalityId.Contains(s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).FirstOrDefault().NationalityId));
                }
            }
            if (request.SectorOfOperationId != null && request.SectorOfOperationId.Count() > 0)
            {


                if (!request.LimitToFirstDebtor)
                {

                    query = query.Where(s => s.Participants.Any(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower && o.SectorOfOperationTypes.Any(m => request.SectorOfOperationId.Contains(m.Id))));
                }
                else
                {
                    query = query.Where(s => s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).FirstOrDefault().SectorOfOperationTypes.Any(m => request.SectorOfOperationId.Contains(m.Id)));
                }

            }
            if (request.SecuredPartyTypeId != null && request.SecuredPartyTypeId.Count() > 0)
            {
                if (!request.LimitToFirstSecuredParty)
                {

                    if (request.SecuredPartyTypeId.Contains(0))
                    {
                        query = query.Where(s => s.Participants.OfType<IndividualParticipant>().Any(p => p.IsActive == true && p.IsDeleted == false && p.ParticipationTypeId == ParticipationCategory.AsSecuredParty)
                          || s.Participants.OfType<InstitutionParticipant>().Any(o => o.IsActive == true && o.IsDeleted == false && request.SecuredPartyTypeId.Contains(o.SecuringPartyIndustryTypeId)));
                    }
                    else
                    {
                        query = query.Where(s => s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsSecuredParty).OfType<InstitutionParticipant>().Any(o => request.SecuredPartyTypeId.Contains(o.SecuringPartyIndustryTypeId)));
                    }
                }
                else
                {

                    if (request.SecuredPartyTypeId.Contains(0))
                    {
                        query = query.Where(s => s.Participants.Where(d => d.IsActive == true && d.IsDeleted == false && d.ParticipationTypeId == ParticipationCategory.AsSecuredParty).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ||
                           (s.Participants.Where(d => d.IsActive == true && d.IsDeleted == false && d.ParticipationTypeId == ParticipationCategory.AsSecuredParty).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Insititution
                        && request.SecuredPartyTypeId.Contains(s.Participants.Where(d => d.IsActive == true && d.IsDeleted == false && d.ParticipationTypeId == ParticipationCategory.AsSecuredParty).OfType<InstitutionParticipant>().FirstOrDefault().SecuringPartyIndustryTypeId)));
                    }
                    else
                    {
                        query = query.Where(s => request.SecuredPartyTypeId.Contains(s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsSecuredParty).OfType<InstitutionParticipant>().FirstOrDefault().SecuringPartyIndustryTypeId));
                    }

                }


            }


            if (request.SecurityUser.IsOwnerUser == false)
            {
                query = query.Where(s => s.MembershipId == request.SecurityUser.MembershipId);
            }

            if (request.FSState == 2)
            {
                //Limit to only active non expired and non discharged
                query = query.Where(s => s.IsDischarged == false && s.ExpiryDate > DateTime.Now);

            }

            var query2 = query.Select(s => new FSCustomReportView
            {
                Id = s.Id,
                
                CollateralTypeId = s.CollateralTypeId,              
                ExpiryDate = s.ExpiryDate,
                MaximumAmountSecuredCurrencyName = s.MaximumAmountSecuredCurrency.CurrencyCode,                
                FinancialStatementLoanTypeId = s.FinancialStatementLoanTypeId,
                FinancialStatementTransactionTypeId = s.FinancialStatementTransactionTypeId,
                MaximumAmountSecured = s.MaximumAmountSecured,
                MaximumAmountSecuredCurrencyId = s.MaximumAmountSecuredCurrencyId,
                RegistrationDate = s.RegistrationDate,
                RegistrationNo = s.RegistrationNo,
                IsDischarged = s.IsDischarged,
                FinancialStatementLastActivity = s.FinancialStatementLastActivity,
                IsPendingAmendment = s.isPendingAmendment,
                IsExpired = s.ExpiryDate < DateTime.Now,
                CollateralTypeName = s.CollateralType.CollateralCategoryName,
                FinancialStatementLoanTypeName = s.FinancialStatementLoanTypeId == null ? "N/A" : s.FinancialStatementLoanType.FinancialStatementCategoryName,
                FinancialStatementTransactionTypeName = s.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName,
                MembershipId = s.MembershipId,
               // HasFileAttachment = s.FinancialStatementAttachmentId != null,
                MembershipName =
                          (
                          s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => r.FirstName + " " + r.MiddleName ?? "" + " " + r.Surname).FirstOrDefault() :
                          s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() : "Unknown"

                          ),
                MembershipType =
                         (
                         s.Membership.isIndividualOrLegalEntity == 1 ? "Individual" :
                         s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).FirstOrDefault().SecuringPartyType.SecuringPartyIndustryCategoryName : "Unknown"

                         )


            });

            return query2;


        }

        public static IQueryable<FSGridView> CreateQueryForFSCustomQuery(
          ViewFSCustomQueryRequest  request, IFinancialStatementRepository _rpFS, bool DoCount)
        {
            CBLContext ctx = ((FinancialStatementRepository)_rpFS).ctx;
            IQueryable<Model.FS.FinancialStatement> query = _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.ClonedId == null && s.isApprovedOrDenied==1 && s.IsActive ==true);

            if (request.RegistrationDate != null)
            {
                query = query.Where(s => s.RegistrationDate >= request.RegistrationDate.StartDate && s.RegistrationDate < request.RegistrationDate.EndDate);
            }
            if (request.FinancialStatementTransactionTypeId != null && request.FinancialStatementTransactionTypeId.Count() > 0)
            {
                query = query.Where(s => request.FinancialStatementTransactionTypeId .Contains(s.FinancialStatementTransactionTypeId));
            }
            if (request.FinancialStatementLoanTypeId != null && request.FinancialStatementLoanTypeId.Count() > 0)
            {
                query = query.Where(s => request.FinancialStatementLoanTypeId .Contains(s.FinancialStatementLoanTypeId ));
            }
            if (request.MaximumCurrency != null && request.MaximumCurrency.Count() > 0)
            {
                query = query.Where(s => request.MaximumCurrency .Contains(s.MaximumAmountSecuredCurrencyId ));
            }
            if (request.CollateralTypeId   != null && request.CollateralTypeId  .Count() > 0)
            {
                query = query.Where(s => request.CollateralTypeId .Contains(s.CollateralTypeId));
            }
            if (request.CollateralSubTypeId != null && request.CollateralSubTypeId.Count() > 0)
            {
                query = query.Where(s => s.Collaterals.Any(p => request.CollateralSubTypeId.Contains(p.CollateralSubTypeId) && p.IsDeleted ==false && p.IsActive ==true)); //Let's use first if elsa agrres for only first to be listed
            }

            if (request.DebtorTypeId != null && request.DebtorTypeId.Count() > 0)
            {
                if (!request.LimitToFirstDebtor)
                {

                    if (request.DebtorTypeId.Contains(DebtorCategory.Individual))
                    {
                        query = query.Where(s =>
                            s.Participants.OfType<IndividualParticipant>().Any(p => p.ParticipationTypeId == ParticipationCategory.AsBorrower && p.IsActive == true && p.IsDeleted == false)
                            || s.Participants.OfType<InstitutionParticipant>().Any(o => request.DebtorTypeId.Contains(o.DebtorTypeId) && o.IsActive == true && o.IsDeleted == false));
                    }
                    else
                    {
                        query = query.Where(s => s.IsActive == true && s.IsDeleted == false && s.Participants.OfType<InstitutionParticipant>().Any(o => request.DebtorTypeId.Contains(o.DebtorTypeId)));
                    }
                }
                else
                {

                    if (request.DebtorTypeId.Contains(DebtorCategory.Individual))
                    {
                        query = query.Where(s =>
                            (s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.IsActive == true && d.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ||
                           (s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.IsActive == true && d.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Insititution
                        && request.DebtorTypeId.Contains(s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.IsActive == true && d.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().DebtorTypeId))));
                    }
                    else
                    {
                        query = query.Where(s => request.DebtorTypeId.Contains(s.Participants.Where(o => o.ParticipationTypeId == ParticipationCategory.AsBorrower && o.IsActive == true && o.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().DebtorTypeId));
                    }

                }

            }
            if (request.DebtorCountryId != null && request.DebtorCountryId.Count() > 0)
            {
                if (!request.LimitToFirstDebtor)
                {

                    query = query.Where(s => s.Participants.Any(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower && request.DebtorCountryId.Contains(o.CountryId)));
                }
                else
                {
                    query = query.Where(s => request.DebtorCountryId.Contains(s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).FirstOrDefault().CountryId));
                }

            }
            if (request.MajorityOwnershipId != null)
            {


                if (!request.LimitToFirstDebtor)
                {

                    //query = query.Where(s => s.FinancialStatementTransactionTypeId != FinancialStatementTransactionCategory.Lien && s.Participants.OfType<InstitutionParticipant>().Any(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower && o.MajorityFemaleOrMaleOrBoth == (request.MajorityOwnershipId == 1 ? true : false)));
                }
                else
                {
                    //query = query.Where(s => s.FinancialStatementTransactionTypeId != FinancialStatementTransactionCategory.Lien && s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).OfType<InstitutionParticipant>().FirstOrDefault().MajorityFemaleOrMaleOrBoth == (request.MajorityOwnershipId == 1 ? true : false));
                }
            }
            if (request.ExistingRelationshipId != null)
            {


                if (!request.LimitToFirstDebtor)
                {

                    query = query.Where(s => s.FinancialStatementTransactionTypeId != FinancialStatementTransactionCategory.Lien && s.Participants.OfType<InstitutionParticipant>().Any(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower && o.DebtorIsAlreadyClientOfSecuredParty == (request.ExistingRelationshipId == 1 ? true : false)));
                }
                else
                {
                    query = query.Where(s => s.FinancialStatementTransactionTypeId != FinancialStatementTransactionCategory.Lien && s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).OfType<InstitutionParticipant>().FirstOrDefault().DebtorIsAlreadyClientOfSecuredParty == (request.ExistingRelationshipId == 1 ? true : false));
                }
            }
            if (request.DebtorNationalityId != null && request.DebtorNationalityId.Count() > 0)
            {


                if (!request.LimitToFirstDebtor)
                {

                    query = query.Where(s => s.Participants.Any(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower && request.DebtorNationalityId.Contains(o.NationalityId)));
                }
                else
                {
                    query = query.Where(s => request.DebtorNationalityId.Contains(s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).FirstOrDefault().NationalityId));
                }
            }
            if (request.SectorOfOperationId != null && request.SectorOfOperationId.Count() > 0)
            {


                if (!request.LimitToFirstDebtor)
                {

                    query = query.Where(s => s.Participants.Any(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower && o.SectorOfOperationTypes.Any(m => request.SectorOfOperationId.Contains(m.Id))));
                }
                else
                {
                    query = query.Where(s => s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).FirstOrDefault().SectorOfOperationTypes.Any(m => request.SectorOfOperationId.Contains(m.Id)));
                }

            }
            if (request.SecuredPartyTypeId != null && request.SecuredPartyTypeId.Count() > 0)
            {
                if (!request.LimitToFirstSecuredParty)
                {

                    if (request.SecuredPartyTypeId.Contains(0))
                    {
                        query = query.Where(s => s.Participants.OfType<IndividualParticipant>().Any(p => p.IsActive == true && p.IsDeleted == false && p.ParticipationTypeId == ParticipationCategory.AsSecuredParty)
                          || s.Participants.OfType<InstitutionParticipant>().Any(o => o.IsActive == true && o.IsDeleted == false && request.SecuredPartyTypeId.Contains(o.SecuringPartyIndustryTypeId)));
                    }
                    else
                    {
                        query = query.Where(s => s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsSecuredParty).OfType<InstitutionParticipant>().Any(o => request.SecuredPartyTypeId.Contains(o.SecuringPartyIndustryTypeId)));
                    }
                }
                else
                {

                    if (request.SecuredPartyTypeId.Contains(0))
                    {
                        query = query.Where(s => s.Participants.Where(d => d.IsActive == true && d.IsDeleted == false && d.ParticipationTypeId == ParticipationCategory.AsSecuredParty).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ||
                           (s.Participants.Where(d => d.IsActive == true && d.IsDeleted == false && d.ParticipationTypeId == ParticipationCategory.AsSecuredParty).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Insititution
                        && request.SecuredPartyTypeId.Contains(s.Participants.Where(d => d.IsActive == true && d.IsDeleted == false && d.ParticipationTypeId == ParticipationCategory.AsSecuredParty).OfType<InstitutionParticipant>().FirstOrDefault().SecuringPartyIndustryTypeId)));
                    }
                    else
                    {
                        query = query.Where(s => request.DebtorTypeId.Contains(s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).OfType<InstitutionParticipant>().FirstOrDefault().DebtorTypeId));
                    }

                }


            }


            if (request.SecurityUser.IsOwnerUser == false)
            {
                query = query.Where(s => s.MembershipId == request.SecurityUser.MembershipId);
            }


                var query2 = query.Select(s => new FSGridView
            {
                Id = s.Id,
                CollateralTypeId = s.CollateralTypeId,
                RequestNo = s.RequestNo,
                ExpiryDate = s.ExpiryDate,
                MaximumAmountSecuredCurrencyName = s.MaximumAmountSecuredCurrency.CurrencyCode,
                FinancialStatementLoanTypeId = s.FinancialStatementLoanTypeId,
                FinancialStatementTransactionTypeId = s.FinancialStatementTransactionTypeId,
                MaximumAmountSecured = s.MaximumAmountSecured,
                MaximumAmountSecuredCurrencyId = s.MaximumAmountSecuredCurrencyId,
                RegistrationDate = s.RegistrationDate,
                RegistrationNo = s.RegistrationNo,
                IsDischarged = s.IsDischarged,
                FinancialStatementLastActivity = s.FinancialStatementLastActivity,
                IsPendingAmendment = s.isPendingAmendment,
                IsExpired = s.ExpiryDate < DateTime.Now,
                CollateralTypeName = s.CollateralType.CollateralCategoryName,
                FinancialStatementLoanTypeName = s.FinancialStatementLoanType.FinancialStatementCategoryName,
                FinancialStatementTransactionTypeName = s.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName,
                MembershipId = s.MembershipId,
               // HasFileAttachment = s.FinancialStatementAttachmentId != null,
                MembershipName =
                          (
                          s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => r.FirstName + " " + r.MiddleName ?? "" + " " + r.Surname).FirstOrDefault() :
                          s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() : "Unknown"

                          ),
                MembershipType =
                         (
                         s.Membership.isIndividualOrLegalEntity == 1 ? "Individual":
                         s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).FirstOrDefault().SecuringPartyType .SecuringPartyIndustryCategoryName : "Unknown"

                         )


            });

                return query2;
            

        }
        
        public static IQueryable<FSGridView> CreateQueryForFindFS(
           ViewFSRequest request, IFinancialStatementRepository _rpFS, bool DoCount)
        {
           
              CBLContext ctx = ((FinancialStatementRepository )_rpFS).ctx;
            IQueryable<Model.FS.FinancialStatement> query = _rpFS.GetDbSet ().Where (s=>s.IsDeleted ==false && s.ClonedId == null);
            if (!(String.IsNullOrEmpty(request.RegistrationNo )))
            {
                query = query.Where(s => s.RegistrationNo.ToLower().Contains(request.RegistrationNo.ToLower()));
            }
            
            if (!(String.IsNullOrEmpty(request.RequestNo )))
            {
                query = query.Where(s => s.RequestNo.ToLower() == request.RequestNo.ToLower());
            }


            if (request .FinancialStatementTransactionTypeId !=null && request .FinancialStatementTransactionTypeId >0)
            {
                query = query.Where(s => s.FinancialStatementTransactionTypeId == (FinancialStatementTransactionCategory)request.FinancialStatementTransactionTypeId);
            }

            if (request.FinancialStatementLoanTypeId != null && request.FinancialStatementLoanTypeId > 0)
            {
                query = query.Where(s => s.FinancialStatementLoanTypeId == (FinancialStatementLoanCategory)request.FinancialStatementLoanTypeId);
            }

            if (request.CollateralTypeId != null && request.CollateralTypeId > 0)
            {
                query = query.Where(s => s.CollateralTypeId == (CollateralCategory )request.CollateralTypeId);
            }

            if (!(String.IsNullOrEmpty(request.FinancialStatementLastActivity)))
            {
                query = query.Where(s => s.FinancialStatementLastActivity.ToLower().StartsWith (request.FinancialStatementLastActivity.ToLower()));
            }

            if (request.RegistrationDate !=null)
            {
                query = query.Where(s => s.RegistrationDate >= request.RegistrationDate.StartDate && s.RegistrationDate < request.RegistrationDate.EndDate);
            }
          


            if (request.SecurityUser.IsOwnerUser == false)
            {
                query = query.Where(s => s.MembershipId  == request.SecurityUser .MembershipId );
            }

            var query2 = query.Select(s => new FSGridView
          {
              Id = s.Id,
              CollateralTypeId = s.CollateralTypeId,
              MaximumAmountSecuredCurrencyName = s.MaximumAmountSecuredCurrency .CurrencyCode ,
              RequestNo = s.RequestNo,
              ExpiryDate = s.ExpiryDate,
              FinancialStatementLoanTypeId = s.FinancialStatementLoanTypeId,
              FinancialStatementTransactionTypeId = s.FinancialStatementTransactionTypeId,
              MaximumAmountSecured = s.MaximumAmountSecured,
              MaximumAmountSecuredCurrencyId = s.MaximumAmountSecuredCurrencyId,
              RegistrationDate = s.RegistrationDate,
              RegistrationNo = s.RegistrationNo,
              IsDischarged = s.IsDischarged,
              FinancialStatementLastActivity = s.FinancialStatementLastActivity,
              IsPendingAmendment = s.isPendingAmendment,
              IsExpired = s.ExpiryDate < DateTime.Now,
              CollateralTypeName = s.CollateralType.CollateralCategoryName,
              FinancialStatementLoanTypeName = s.FinancialStatementLoanType.FinancialStatementCategoryName,
              FinancialStatementTransactionTypeName = s.FinancialStatementTransactionType .FinancialStatementTransactionCategoryName ,
              MembershipId = s.MembershipId,
              //HasFileAttachment = s.FinancialStatementAttachmentId != null,
              MembershipName =
                        (
                        s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId  && a.InstitutionId == null).Select(r => r.FirstName + " " + r.MiddleName ?? "" + " " + r.Surname).FirstOrDefault() :
                        s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() : "Unknown"

                        )


          });


            if(!(String.IsNullOrEmpty(request.MembershipName)))
            {
                query2 = query2.Where(s => s.MembershipName.Contains(request.MembershipName));
            }

            
         
            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "RegistrationDate";
                    request.SortOrder = "desc";
                }

                if (request.SortColumn == "RegistrationDate")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.RegistrationDate );
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.RegistrationDate );
                    }
                }

                if (request.SortColumn == "RegistrationNo")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.Id );
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Id );
                    }
                }

                if (request.SortColumn == "MaximumAmountSecured")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.MaximumAmountSecured );
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.MaximumAmountSecured );
                    }
                }

                if (request.SortColumn == "ExpiryDate")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.ExpiryDate );
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.ExpiryDate);
                    }
                }

              
            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }

             return query2;
         
        }
    }
}
