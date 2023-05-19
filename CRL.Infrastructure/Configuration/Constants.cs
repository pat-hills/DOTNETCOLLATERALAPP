using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CRL.Infrastructure.Configuration
{
    public enum Project { Liberia=1, Nigeria=2};
    public static class Constants
    {
        //DB VALUES
        public static readonly int OtherIdDocumentConstant = 8;

        public static readonly int CountyNotApplicable = 37;
        public static readonly int LGANotApplicable = 775;
        public static readonly int DefaultCurrency = 306;
        public static readonly bool WorkflowIsOn = true;
        public const string RegionLabel = "State";
        public const string DischargeLabel = "Submit Cancellation";
        public const string LenderLabel = "Secured Party";
        public const string BorrowerLabel = "Debtor";
        public const bool EnablePartialDischarge = false;

        public static readonly bool EnableCancellationDueToError = true;    

        public static readonly int MaxFileSizeForUpload = 9;
       
        public static readonly int InBuiltUser = 1;
        public static readonly int PublicUser = 2;
      
        public static readonly bool TransitionPeriodOn = false;

        public static readonly int MaxDatabaseConcurrencyConflictRetries = 5;
        public static readonly bool NotifyAfterRegistrationOrChangeOfFS = true;
        public static readonly Project Project = Configuration.Project.Nigeria;   
        public static readonly int DefaultTransactionType = 1;
        public static readonly bool AllowClientToSelectTransactionType = true;
        public static readonly int DefaultWorkflowPostpadAccountSetup = 6;
        public static readonly int RegistryMembershipId = 1;
        public static readonly bool ApplySearchRoleToSearch = true;
        public static readonly bool ApplyGenerateSearchReportRole= true;
        public static readonly bool EnforceOnlyBanksAsPostpaidClients = true;
        //public static readonly int SecuredPartyBankType = 7;
        public static readonly int[] SecuredPartyBankType = new int[] {7,16 };
        public static readonly int PassportID = 6;
        public static readonly int BVNID = 5;
        public static readonly bool EnableBatchUpload = false;


        public static readonly int[] RequiredCollateralSubtypes = new int[] { 26, 29, 47, 48 };
        public static readonly int PageSize = 100;
        public static readonly int Timeout = 30;
        public static readonly bool ExpireMembershipRegistraqtionAfterInActivity = true;

        public static readonly int DefaultLenderTypeIdForPublicUsers = 360;
        
        //USER ACCOUNT
        public static readonly int ResetPasswordRequestExpiryDays = 1;
        public static readonly bool PasswordExpiry = false;
        public static readonly short PasswordExpiryDays = 90;
        public static readonly short PasswordWarnDays = 5;
        public static readonly int MaxUserLoginAttempts = 3;
        public static readonly bool EnableUnlockAfterUnlockMinutes = true;
        public static readonly int MinToUnlockUser = 15;
        public static readonly bool LockOutUserAfterMaxRetry = true;

        //CONSTANT ID VALUES
        public static readonly int DefaultCountry = 128;
        public static readonly int DefaultNationality = 128;


        public static readonly bool EnableDeletionOfUsers = true;
        public static readonly bool EnableDeletionOfInstitutions = true;
        public static readonly bool EnableDeletionOfInstitutionUnits = true;
        public static readonly int NumOfDaysToExpireAccount = 3;
        public static readonly bool NumOfDaysExpiry = false;


        public static readonly bool EXPIRE_RESULTINSEARCH = false; 
        public static readonly int MAX_NUM_SEARCHRESULTS = 3;
        public static readonly int RESULTSEARCH_LIMIT = 1;
        public static readonly bool LIMIT_SEARCH_REPORT = false;
        public static readonly int MAX_NUM_SEARCHREPORTS = 0;
        public static readonly bool SHOW_GENERATED_REPORTS_LIST = true;

        public static readonly int DEFAULT_NUM_RECORDS = 2;
        public static readonly int MAX_NUM_RECORDS = -1;

        public static string GetApplicationPath
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    string appPath = HttpContext.Current.Request.ApplicationPath;
                    string physicalPath = HttpContext.Current.Request.MapPath(appPath);
                    return physicalPath;
                }
                else
                {
                    return AppDomain.CurrentDomain.BaseDirectory;
                }
            }
        }
        public static string GetReportPath
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    string appPath = HttpContext.Current.Request.ApplicationPath;
                    string physicalPath = HttpContext.Current.Request.MapPath(appPath);
                    string end = physicalPath.Substring ((physicalPath .Length-1),1);
                    if (!(end== "\\"))
                    {
                        physicalPath += "\\";
                    }
                    physicalPath += "Reporting\\";
                    return physicalPath;
                }
                else
                {
                    return AppDomain.CurrentDomain.BaseDirectory + "Reporting\\";
                }
            }
        }
        public static bool OnTestSite
        {
            get
            {
                return ConfigurationManager.AppSettings["onTestSite"] != null ? Boolean.Parse(ConfigurationManager.AppSettings["onTestSite"]) : false;
               
            }
        }
        public static string OnTestSiteSubjectPrefix
        {
            get
            {
                return ConfigurationManager.AppSettings["onTestSiteSubjectPrefix"] != null ? ConfigurationManager.AppSettings["onTestSiteSubjectPrefix"] : "";
            }
        }    
        public static int? CurrentBatchDesktopVersion
        {
            get
            {
                if (ConfigurationManager.AppSettings["CurrentBatchDesktopVersion"] != null)
                {
                    try
                    {
                        return Convert.ToInt32(ConfigurationManager.AppSettings["CurrentBatchDesktopVersion"]);
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

            }
        }
        public static int? LeastSupportedBatchDesktopVersion
        {
            get
            {
                if ( ConfigurationManager.AppSettings["LeastSupportedBatchDesktopVersion"] != null)
                {
                    try
                    {
                        return Convert.ToInt32(ConfigurationManager.AppSettings["LeastSupportedBatchDesktopVersion"]); }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
               
            }
        }      
      
    }
}
