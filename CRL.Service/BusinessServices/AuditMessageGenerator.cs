using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.ModelViews;
using CRL.Model.Memberships;
using CRL.Model.Payments;

namespace CRL.Service.BusinessService
{

    public static class PaymentAuditMsgGenerator
    {
        public static string PaymentDetails(Payment payment)
        {
            
            return String.Format("Payment Code: {0}, Payment Date: {1}, Payee: {2}, T24TransactionNo: {3},  SecurityCode: {4}, PaymentSource: {5}, PaymentType: {6}",
                payment.PaymentNo, payment.PaymentDate, payment.Payee, payment.T24TransactionNo, payment.PublicUserSecurityCode == null ? "" : payment.PublicUserSecurityCode.SecurityCode, payment.PaymentSource.ToString(), payment.PaymentType.ToString());
        }
    }
    public static class MembershipAuditMsgGenerator
    {
        public static string IndividualMembershipRegistration (User user)
        {
          return String.Format ("Client Code: {0}, Username: {1}, Firstname: {2}, Surname: {3}, Email: {4}", 
            user.Membership .ClientCode , user.Username , user.FirstName ,user.Surname ,user.Address .Email );
        }

        public static string InstitutionMembershipRegistration (Institution institution,User AdminUser)
        {
            return String.Format("Client Code: {0}, Name: {1}, Company No: {2}, Admin Username: {3}, Admin FullName: {4}",
           institution.Membership.ClientCode, institution.Name , institution.CompanyNo , AdminUser.Username , NameHelper.GetFullName (AdminUser .FirstName ,AdminUser .MiddleName ,AdminUser .Surname ));
        }

        public static string ClientDetails(Membership membership, string ClientName)
        {


            return String.Format("Client code: {0}, Client Name: {1}, Client Type: {2}", membership.ClientCode, ClientName, membership.isIndividualOrLegalEntity == 1 ? "Individual" : "Financial Institution");

        }
        public static string InstitutionDetails(Institution institution)
        {
            return String.Format("Client code: {0}, Client Name: {1}", institution.Membership.ClientCode, institution.Name);
        }

        public static string InstitutionUnitDetails(InstitutionUnit institution)
        {
            return String.Format("Client Name: {0}", institution.Name);
        }


        public static string UserDetails(User user)
        {
            if (user.InstitutionId ==null)
                return String.Format("Client Code: {0}, Username: {1}, Firstname: {2}, Surname: {3}, Email: {4}",
              user.Membership.ClientCode, user.Username, user.FirstName, user.Surname, user.Address.Email);
            else
                return String.Format(" Username: {0}, Firstname: {1}, Surname: {2}, Email: {3}",
               user.Username, user.FirstName, user.Surname, user.Address.Email);
        }

        public static string PayPointUserAssignmentDetails(Institution institution,int noOfUsersInRequest)
        {

            return String.Format("Client code: {0}, Client Name: {1}, Number of Paypoint users requested:{2}", institution.Membership.ClientCode, institution.Name, noOfUsersInRequest);
        }

    }


    public static class RegistrationFSAuditMsgGenerator
    {
        public static string SubmitResendCreate(FinancialStatement fs, string TransactionTypeName = null, string CollateralTypeName = null)
        {

            TransactionTypeName = TransactionTypeName == null ? fs.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName : TransactionTypeName;
            CollateralTypeName = CollateralTypeName == null ? fs.CollateralType.CollateralCategoryName : CollateralTypeName;
            return String.Format("Registration No:{0}, Request No: {1}, Transaction Type:{2}, Loan Amount:{3},  Expiry Date:{4}, Collateral Type:{5}, No of Collaterals:{6} , No of Secured Parties:{7} , No of Borrowers:{8}",
           fs.RegistrationNo, fs.RequestNo, TransactionTypeName,
          fs.MaximumAmountSecured.Value.ToString("#,##0.00"), fs.ExpiryDate, CollateralTypeName, fs.Collaterals.Count().ToString(), fs.Participants.Where(p => p.ParticipationTypeId == ParticipationCategory.AsSecuredParty).Count().ToString(),
          fs.Participants.Where(p => p.ParticipationTypeId == ParticipationCategory.AsBorrower).Count().ToString());
        }

        public static string SubmitResendCreate(FinancialStatementActivity fsActivity)
        {

            string TransactionTypeName = fsActivity.FinancialStatement.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName;
            string CollateralTypeName = fsActivity.FinancialStatement.CollateralType.CollateralCategoryName;
            return String.Format("Registration No:{0}, Request No: {1}, Transaction Type:{2}, Loan Amount:{3},  Expiry Date:{4}, Collateral Type:{5}, No of Collaterals:{6} , No of Secured Parties:{7} , No of Borrowers:{8}",
           fsActivity.FinancialStatement.RegistrationNo, fsActivity.FinancialStatement.RequestNo, TransactionTypeName,
         fsActivity.FinancialStatement.MaximumAmountSecured.Value.ToString("#,##0.00"), fsActivity.FinancialStatement.ExpiryDate, CollateralTypeName,
         fsActivity.FinancialStatement.Collaterals.Count().ToString(), fsActivity.FinancialStatement.Participants.Where(p => p.ParticipationTypeId == ParticipationCategory.AsSecuredParty).Count().ToString(),
         fsActivity.FinancialStatement.Participants.Where(p => p.ParticipationTypeId == ParticipationCategory.AsBorrower).Count().ToString());
        }






    }
}
