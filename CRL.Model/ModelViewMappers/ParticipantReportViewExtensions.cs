using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.FS;
using CRL.Service.Common;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Infrastructure.Configuration;

namespace CRL.Model.ModelViewMappers
{
    public static class ParticipantReportViewMapper
    {
        public static SecuredPartyReportView ConvertToSecuredPartyReportView(this IndividualParticipant model, LookUpForFS LookUps = null)
        {
            SecuredPartyReportView iview = new SecuredPartyReportView();
            model.ConvertToSecuredPartyReportView(iview, LookUps);
            return iview;


        }

        public static void ConvertToSecuredPartyReportView(this IndividualParticipant model, SecuredPartyReportView iview, LookUpForFS LookUps = null)
        {          
            
            
            iview.Name = NameHelper.GetFullName(model.Identification.FirstName, model.Identification.MiddleName, model.Identification.Surname);

            iview.FinancialStatementId = model.FinancialStatementId;
            iview.Address = model.Address.Address;
            iview.Address2  = model.Address.Address2;
            
            iview.City = model.Address.City;
            iview.Email = model.Address.Email;
            iview.Phone = model.Address.Phone;
            iview.DOB = model.DOB;
            iview.Gender = model.Gender;
            iview.CardNo = model.Identification.CardNo;
            iview.SecuringPartyIndustryTypename ="Individual";          
            iview.Title = model.Title;
            iview.isIndividual = true;
            iview.OtherDocumentDescription = model.OtherDocumentDescription;
            if (LookUps != null)
            {
                iview.County = LookUps.Countys.Where(s => s.LkValue == (int)model.CountyId).SingleOrDefault().LkName;
                iview.Country = LookUps.Countries.Where(s => s.LkValue == (int)model.CountryId).SingleOrDefault().LkName;
               // iview.Nationality = LookUps.Nationalities.Where(s => s.LkValue == (int)model.NationalityId).SingleOrDefault().LkName;
                iview.PersonIdentificationTypename = LookUps.IdentificationCardTypes.Where(s => s.LkValue == (int)model.PersonIdentificationTypeId).SingleOrDefault().LkName;
            }
            else
            {
                iview.County = model.County.Name;
                iview.Country = model.Country.Name;
                //iview.Nationality = model.Nationality.Name;
                iview.PersonIdentificationTypename = model.PersonIdentificationType.PersonIdentificationCardCategoryName;
            }
           
            
        }

        public static SecuredPartyReportView ConvertToSecuredPartyReportView(this InstitutionParticipant model, LookUpForFS LookUps = null)
        {
            SecuredPartyReportView iview = new SecuredPartyReportView();
            model.ConvertToSecuredPartyReportView(iview, LookUps);
            return iview;


        }

        public static void ConvertToSecuredPartyReportView(this InstitutionParticipant model, SecuredPartyReportView iview, LookUpForFS LookUps = null)
        {

            iview.FinancialStatementId = model.FinancialStatementId;
            iview.Name = model.Name;          
            iview.Address = model.Address.Address;
            iview.Address2 = model.Address.Address2;
            iview.CompanyNo = model.CompanyNo;
            iview.City = model.Address.City;
            iview.Email = String.IsNullOrWhiteSpace(model.Address.Email) ? "N/A" : model.Address.Email;
            iview.Phone = String.IsNullOrWhiteSpace(model.Address.Phone) ? "N/A" : model.Address.Phone;       
            iview.isIndividual = false;
            iview.OwnerOfCompany = model.OwnerOfCompany;
            if (LookUps != null)
            {
                iview.County =( model.CountyId == Constants.CountyNotApplicable ||  model.CountyId == null)   ? "N/A" : LookUps.Countys.Where(s => s.LkValue == (int)model.CountyId).SingleOrDefault().LkName;
                
                iview.Country = LookUps.Countries.Where(s => s.LkValue == (int)model.CountryId).SingleOrDefault().LkName;
                //iview.Nationality = LookUps.Nationalities.Where(s => s.LkValue == (int)model.NationalityId).SingleOrDefault().LkName;
                iview.SecuringPartyIndustryTypename = LookUps.SecuringPartyIndustryTypes .Where(s => s.LkValue == (int)model.SecuringPartyIndustryTypeId ).SingleOrDefault().LkName;
                iview.LGA = (model.LGAId == Constants.LGANotApplicable || model .LGAId == null) ? "N/A" : LookUps.LGAs.Where(s => s.LkValue == (int)model.LGAId).SingleOrDefault().LkName; ;
                
            }
            else
            {
                iview.County = model.CountyId  != null && model.CountyId != Constants .CountyNotApplicable  ? model.County.Name : "N/A";
                iview.Country = model.Country.Name;
               // iview.Nationality = model.Nationality.Name;
                iview.SecuringPartyIndustryTypename = model.SecuringPartyIndustryType.SecuringPartyIndustryCategoryName;
                iview.LGA = model.LGAId != null && model.LGAId != Constants.LGANotApplicable ? model.LGA.Name : "N/A";
            }

        }

        public static DebtorReportView ConvertToDebtorReportView(this IndividualParticipant model, LookUpForFS LookUps = null)
        {
            DebtorReportView iview = new DebtorReportView();
            model.ConvertToDebtorReportView(iview, LookUps);
            return iview;


        }

        public static void ConvertToDebtorReportView(this IndividualParticipant model, DebtorReportView iview, LookUpForFS LookUps = null)
        {


            iview.Name = NameHelper.GetFullName(model.Identification.FirstName, model.Identification.MiddleName, model.Identification.Surname);
            iview.FinancialStatementId = model.FinancialStatementId;
            iview.Address = model.Address.Address;
            iview.Address2 = model.Address.Address2;
            iview.City = model.Address.City;
            iview.Email = String.IsNullOrWhiteSpace (model.Address.Email) ? "N/A":model.Address.Email;
            iview.Phone = String.IsNullOrWhiteSpace(model.Address.Phone) ? "N/A" : model.Address.Phone;
            iview.Gender = String.IsNullOrWhiteSpace(model.Gender) ? "N/A" : model.Gender;
            iview.DOB = model.DOB;
            iview.CardNo = model.Identification.CardNo;
            iview.CardNo2 = model.Identification.CardNo2;
            iview.DebtorTypeName  = "Individual";
            iview.Title = String.IsNullOrWhiteSpace(model.Title) ? "N/A" : model.Title;
            iview.isIndividual = true;
            iview.ParticipantNo = model.ParticipantNo;
            iview.OtherDocumentDescription = model.OtherDocumentDescription;
            iview.HasOtherIdentificationInformation = model.OtherPersonIdentifications.Count > 0;
            iview.DebtorIsAlreadyClientOfSecuredParty = model.DebtorIsAlreadyClientOfSecuredParty;

            List<string> pSectorOfOperations = new List<string>();

            if (model.SectorOfOperationTypes.Count > 0)
            {
                foreach (var sector in model.SectorOfOperationTypes)
                {
                    pSectorOfOperations.Add(sector.SectorOfOperationCategoryName);
                }
            }

            if (pSectorOfOperations.Count > 0)
            {
                iview.SectorOfOperationTypes = string.Join (",", pSectorOfOperations);
            }


    

            if (LookUps != null)
            {
                iview.County = model.CountyId != null && model.CountyId != Constants.CountyNotApplicable ? LookUps.Countys.Where(s => s.LkValue == (int)model.CountyId).SingleOrDefault().LkName : "N/A";
                iview.Country = model.CountryId != null ? LookUps.Countries.Where(s => s.LkValue == (int)model.CountryId).SingleOrDefault().LkName : "N/A";
                iview.Nationality = LookUps.Nationalities.Where(s => s.LkValue == (int)model.NationalityId ).SingleOrDefault().LkName;
                iview.PersonIdentificationTypename = LookUps.IdentificationCardTypes .Where(s => s.LkValue == (int)model.PersonIdentificationTypeId ).SingleOrDefault().LkName;
                iview.PersonIdentificationTypename2 = LookUps.IdentificationCardTypes.Where(s => s.LkValue == (int)model.PersonIdentificationType2Id).SingleOrDefault().LkName;
                iview.LGA = model.LGAId != null && model.LGAId != Constants.LGANotApplicable ? LookUps.LGAs.Where(s => s.LkValue == (int)model.LGAId).SingleOrDefault().LkName : "N/A";
            }
            else
            {
                iview.County = model.CountyId != null && model.CountyId != Constants.CountyNotApplicable ? model.County.Name : "N/A";
                iview.Country = model.Country.Name;
                iview.Nationality = model.Nationality.Name;
                iview.PersonIdentificationTypename = model.PersonIdentificationType.PersonIdentificationCardCategoryName ?? "";
                if (model.PersonIdentificationType2 != null)
                    iview.PersonIdentificationTypename2 = model.PersonIdentificationType2.PersonIdentificationCardCategoryName ?? "";
                else
                    iview.PersonIdentificationTypename2 = "";
                iview.LGA = model.LGAId != null && model.LGAId != Constants.LGANotApplicable ? model.LGA.Name : "N/A";
            }

            

        }

        public static DebtorReportView ConvertToDebtorReportView(this InstitutionParticipant model, LookUpForFS LookUps = null)
        {
            DebtorReportView iview = new DebtorReportView();
            model.ConvertToDebtorReportView(iview, LookUps);
            return iview;


        }

        public static void ConvertToDebtorReportView(this InstitutionParticipant model, DebtorReportView iview, LookUpForFS LookUps = null)
        {


            iview.Name = model.Name;
            iview.FinancialStatementId = model.FinancialStatementId;
            iview.Address = model.Address.Address;
            iview.Address2 = model.Address.Address2;
            iview.City = model.Address.City;
            iview.Email =  String.IsNullOrWhiteSpace (model.Address.Email) ? "N/A":model.Address.Email;
            iview.Phone = String.IsNullOrWhiteSpace(model.Address.Phone) ? "N/A" : model.Address.Phone; 
            iview.DebtorIsAlreadyClientOfSecuredParty = model.DebtorIsAlreadyClientOfSecuredParty;
            iview.isIndividual = false;
            iview.ParticipantNo = model.ParticipantNo;
            iview.OwnerOfCompany = model.OwnerOfCompany;
            iview.CompanyNo = model.CompanyNo;
            
            iview.MajorityFemaleOrMaleOrBoth  = model.MajorityFemaleOrMaleOrBoth;

            if (LookUps != null)
            {
                iview.County = model.CountyId != null && model.CountyId != Constants.CountyNotApplicable ? LookUps.Countys.Where(s => s.LkValue == (int)model.CountyId).SingleOrDefault().LkName : "N/A";
                iview.Country = LookUps.Countries.Where(s => s.LkValue == (int)model.CountryId).SingleOrDefault().LkName;
               // iview.Nationality = LookUps.Nationalities.Where(s => s.LkValue == (int)model.NationalityId).SingleOrDefault().LkName;
                iview.DebtorTypeName = LookUps.DebtorTypes .Where(s => s.LkValue == (int)model.DebtorTypeId ).SingleOrDefault().LkName;
                iview.CompanyNo = LookUps.RegistrationNoPrefix.Where(s => s.LkValue == (int)model.BusinessRegistrationPrefixId).SingleOrDefault().LkName + model.CompanyNo;
                iview.BusinessTin = LookUps.RegistrationNoPrefix.Where(s => s.LkValue == (int)model.BusinessRegistrationPrefixId).SingleOrDefault().LkName + model.CompanyNo; ;
                iview.LGA = model.LGAId != null && model.LGAId != Constants.LGANotApplicable ? LookUps.LGAs.Where(s => s.LkValue == (int)model.LGAId).SingleOrDefault().LkName : "N/A";
                
            
            }
            else
            {
                iview.County = model.CountyId != null && model.CountyId != Constants.CountyNotApplicable ? model.County.Name : "N/A";
                iview.Country = model.Country.Name;
               // iview.Nationality = model.Nationality.Name;
                iview.DebtorTypeName = model.DebtorType.CompanyCategoryName;
                iview.CompanyNo = model.BusinessRegistrationPrefix.Name + model.CompanyNo;
                iview.BusinessTin = model.BusinessRegistrationPrefix.Name + model.CompanyNo;
                iview.LGA = model.LGAId != null && model.LGAId != Constants.LGANotApplicable ? model.LGA.Name : "N/A";
            }

            List<string> pSectorOfOperations = new List<string>();

            if (model.SectorOfOperationTypes.Count > 0)
            {
                foreach (var sector in model.SectorOfOperationTypes)
                {
                    pSectorOfOperations.Add(sector.SectorOfOperationCategoryName);
                }
            }

            if (pSectorOfOperations.Count > 0)
            {
                iview.SectorOfOperationTypes = string.Join(",", pSectorOfOperations);
            }




        }

        public static ICollection<SecuredPartyReportView> ConvertToSecuredPartyReportView(this IEnumerable<Participant> Participants, LookUpForFS LookUps = null)
        {
            ICollection<SecuredPartyReportView> SecuredPartyReportView = new List<SecuredPartyReportView>();
            foreach (var p in Participants)
            {
                SecuredPartyReportView _SecuredPartyReportView= new SecuredPartyReportView ();
                if (p is IndividualParticipant)
                {
                    ((IndividualParticipant)p).ConvertToSecuredPartyReportView(_SecuredPartyReportView,LookUps);
                }
                else
                {

                    ((InstitutionParticipant)p).ConvertToSecuredPartyReportView(_SecuredPartyReportView,LookUps);
                }
                SecuredPartyReportView.Add(_SecuredPartyReportView);
            }

            return SecuredPartyReportView;
        }

        public static ICollection<DebtorReportView> ConvertToDebtorReportView(this IEnumerable<Participant> Participants, LookUpForFS LookUps = null)
        {
            ICollection<DebtorReportView> DebtorReportView = new List<DebtorReportView>();
            foreach (var p in Participants)
            {
                DebtorReportView _DebtorReportView = new DebtorReportView();
                if (p is IndividualParticipant)
                {
                    ((IndividualParticipant)p).ConvertToDebtorReportView(_DebtorReportView, LookUps);
                }
                else
                {

                    ((InstitutionParticipant)p).ConvertToDebtorReportView(_DebtorReportView, LookUps);
                }
                DebtorReportView.Add(_DebtorReportView);
            }

            return DebtorReportView;
        }

        public static ICollection<OtherIdentificationReportView > GetOtherIdentificationReportView(this IEnumerable<IndividualParticipant > Participants, LookUpForFS LookUps = null)
        {
            ICollection<OtherIdentificationReportView> OtherIdentificationReportView = new List<OtherIdentificationReportView>();
            foreach (var p in Participants)
            {
                foreach (var _otherids in ((IndividualParticipant)p).OtherPersonIdentifications.Where (s=>s.IsDeleted ==false))
                {
                    OtherIdentificationReportView _OtherIdentificationReportView = new OtherIdentificationReportView();
                    _OtherIdentificationReportView.Name = _otherids.Identification.FirstName;
                    _OtherIdentificationReportView.ParticipantNo = p.ParticipantNo;
                    _OtherIdentificationReportView.ParticipantId = p.Id;
                    _OtherIdentificationReportView.PersonIdentificationTypename =LookUps==null? _otherids.PersonIdentificationType .PersonIdentificationCardCategoryName : LookUps.IdentificationCardTypes.Where(s => s.LkValue == _otherids.PersonIdentificationTypeId).SingleOrDefault().LkName;
                    _OtherIdentificationReportView.OtherDocumentDescription = _otherids.Identification.OtherDocumentDescription;
                    _OtherIdentificationReportView.CardNo = _otherids.Identification.CardNo;
                    OtherIdentificationReportView.Add(_OtherIdentificationReportView);
                }
                
            }

            return OtherIdentificationReportView;
        }

    }
}
