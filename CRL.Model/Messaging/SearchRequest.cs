using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;


namespace CRL.Model.Messaging
{

    public class VerifyPublicUserRequest : RequestBase
    {
        public string SecurityCodeForPublicUser { get; set; }
    }
    public class GetSearchRequest : RequestBase
    {
        public string SecurityCodeForPublicUser { get; set; }
        public bool Reload { get; set; }
    }

    public class CACSearchrequest : RequestBase
    {
        public string apiUrl { get; set; }
        public string uniqueId { get; set; }
        public string CACResults { get; set; }
    }

    public class SearchRequest : PaginatedRequest    {

        public SearchParam SearchParam { get; set; }
        public bool IsNonLegalEffectSearch { get; set; }
        public string UniqueIdentifier { get; set; }
        public string SecurityCodeForPublicUser { get; set; }
        public string PublicUserEmail { get; set; }
        public string PublicUserName { get; set; }
        public string PublicUserPhoneNo { get; set; }
        public string PublicUserBVN { get; set; }
        public bool IsPayable { get; set; }
        public FilterSearch FilterSearch { get; set; }
        public bool isFilterSearch { get; set; }
        public int? SearchId { get; set; }
        public string apiUrl { get; set; }
        public string clas { get; set; }
        public string Prefix { get; set; }
        public string CACResults { get; set; }
    }

    public class FilterSearch
    {

        public int SearchId { get; set; }
        public string[] RegistrationNo { get; set; }
    }


    [Serializable]
    public class SearchParam
    {
        [Display(Name = "Search By")]
        public int SearchType { get; set; }
        [Display(Name = "Name")]
        public string BorrowerName { get; set; }
        [Display(Name = "First name")]
        public string BorrowerFirstName { get; set; }
        [Display(Name = "Middle Name")]
        public string BorrowerMiddleName { get; set; }
        [Display(Name = "Surname")]
        public string BorrowerLastName { get; set; }


        public int BusinessPrefix { get; set; }
        public string BusinessPrefixName { get; set; }
        [Display(Name = " ")]
        public string BorrowerIDNo { get; set; }
        public int BorrowerIDType { get; set; }
        [Display(Name = "Collateral Serial No")]
        public string CollateralSerialNo { get; set; }

        [Display(Name = "Date of Birth")]
        public DateRange DebtorDateOfBirth { get; set; }
        public string RegistrationNo { get; set; }

        public int RegistrationPrefix { get; set; }

        [Display(Name = "Collateral Description")]
        public string CollateralDescription { get; set; }
        [Display(Name = "Email")]
        public string DebtorEmail { get; set; }

        public string ParameterString
        {
            get
            {
                string paramString = "";
                if (BorrowerIDType == 1)
                    paramString = "Debtor Type: Individual";
                else
                    paramString = "Debtor Type: Institution";
                if (!String.IsNullOrWhiteSpace(BorrowerName) ||
                    !String.IsNullOrWhiteSpace(BorrowerFirstName) ||
                    !String.IsNullOrWhiteSpace(BorrowerMiddleName) ||
                    !String.IsNullOrWhiteSpace(BorrowerLastName) ||
                    DebtorDateOfBirth != null)
                {
                    paramString = BorrowerIDType == 1 ? "Debtor Type: Individual" : "Debtor Type: Institution";
                }

                paramString += (!String.IsNullOrWhiteSpace(BorrowerName) ? ", Debtor Name: " + BorrowerName : "");
                paramString += (!String.IsNullOrWhiteSpace(BorrowerFirstName) ? ", Debtor Firstname: " + BorrowerFirstName : "");
                paramString += (!String.IsNullOrWhiteSpace(BorrowerMiddleName) ? ", Debtor Middlename: " + BorrowerMiddleName : "");
                paramString += (!String.IsNullOrWhiteSpace(BorrowerLastName) ? ", Debtor Surname: " + BorrowerLastName : "");
                paramString += (DebtorDateOfBirth != null ? ", Debtor Date of Birth: " + DebtorDateOfBirth.StartDate.ToShortDateString() : "");
                paramString += (!String.IsNullOrWhiteSpace(DebtorEmail) ? ", Debtor Email: " + DebtorEmail : "");
                paramString += (!String.IsNullOrWhiteSpace(CollateralSerialNo) ? ", Collateral Serial No: " + CollateralSerialNo : "");
                paramString += (!String.IsNullOrWhiteSpace(CollateralDescription) ? ", Collateral Description: " + CollateralDescription : "");
                paramString.TrimStart(',');
                return paramString;
            }

        }

    }

    public class GenerateSearchReportRequest : RequestBase
    {

        public bool isCertified { get; set; }
        public int SelectedFS { get; set; }
        public bool SendAsMail { get; set; }
        public string publicUsrEmail { get; set; }
        public bool PayableTransaction { get; set; }
        //public string UniqueIdentifier { get; set; }
        //public bool CheckUniqueIdentifier { get; set; }// Necessary if this was called from a link
        public bool IsPublicUser { get; set; }
        public string SecurityCodeForPublicUser { get; set; }
    }

    public class DownloadSearchReportRequest : RequestBase
    {
        public int SearchId { get; set; }
        public bool isCertified { get; set; }
        public int[] SelectedFS { get; set; }
        public string RegistrationNo { get; set; }
        public bool SendAsMail { get; set; }
        public string publicUsrEmail { get; set; }
        public string SecurityCodeForPublicUser { get; set; }
        public bool IsPublicUser { get; set; }
    }

    public class ValidateSecurityCodeRequest : RequestBase
    {
        public string SecurityCode { get; set; }
    }

    public class AssignFSToSearchRequest : RequestBase
    {
        public int SearchId { get; set; }
        public int SelectedFS { get; set; }
        public string SecurityCodeForPublicUser { get; set; }
    }

    public class TrackSearchResultRequest : RequestBase
    {
        public int SearchFinancialStatementId { get; set; }
        public string RegistrationNo { get; set; }
        public DateTime? ReportGenerationDate { get; set; }
        public bool IsGenerateReport { get; set; }
        public string SecurityCodeForPublicUser { get; set; }
        public int? FileUploadId { get; set; }
        public bool PayableTransaction { get; set; }
    }

    public class GetExpiredSearchResultRequest : RequestBase
    {
        public int SearchFinancialStatementId { get; set; }
        public string SecurityCodeForPublicUser { get; set; }
    }
}
