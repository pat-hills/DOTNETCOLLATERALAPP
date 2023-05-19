using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.ModelViews;


using CRL.Service.Views.Payments;
using CRL.Model.Payments;
using CRL.Model.ModelViews.Payments;
using CRL.Model.Memberships.IRepository;
using CRL.Model.Memberships;

namespace CRL.Service.Mappings.Payments
{
  
    public static class PaymentMapper
    {


        public static PaymentView ConvertToPaymentView(this Payment Payment, IInstitutionRepository _institutionRepository, IUserRepository _userRepository)
        {

            PaymentView iview = new PaymentView();
            Payment.ConvertToPaymentView(iview,_institutionRepository ,_userRepository );
            return iview;
        }

        public static void ConvertToPaymentView(this Payment Payment, PaymentView iview, IInstitutionRepository _institutionRepository , IUserRepository _userRepository)
        {
            iview.Amount = Payment.Amount;
            iview.Payee = Payment.Payee;
            iview.PaymentDate = Payment.PaymentDate;
            iview.PaymentNo = Payment.PaymentNo;
            iview.MembershipId = Payment.MembershipId;
            iview.T24TransactionNo = Payment.T24TransactionNo;
            iview.PaymentType = Convert.ToInt32(Payment.PaymentType);
            iview.PaymentSourceName = (Payment.PaymentSource == PaymentSource.Paypoint ? "Paypoint" : Payment.PaymentSource == PaymentSource.Settlement ? "Settlement" :
            Payment.PaymentSource == PaymentSource.InterSwitchWebPay ? "Interswitch WebPay" :
            Payment.PaymentSource == PaymentSource.InterswitchDirectPay ? "Interswitch DirectPay" : "");

            if (Payment.PaymentType  == PaymentType.Normal )
            {
                iview.PaymentTypeName = "Normal";
            }
            else if (Payment.PaymentType == PaymentType.Reversal )
            {
                iview.PaymentTypeName = "Reversal";
            }
            if (Payment.PublicUserSecurityCode != null)
            {
                iview.PublicUserEmail = Payment.PublicUserSecurityCode.PublicUserEmail;
                iview.SecurityCode = Payment.PublicUserSecurityCode.SecurityCode;
                iview.IsPublicUser = true;
                iview.Client = Payment.Payee;
              
            }
            else
            {
                Institution institution = _institutionRepository.GetDbSet().Where(m => m.MembershipId == Payment.MembershipId).SingleOrDefault();
                if (institution != null)
                    iview.Client  = institution.Name;
                else
                {
                    User user = _userRepository.GetDbSet().Where(m => m.MembershipId == Payment.MembershipId).SingleOrDefault();
                    iview.Client  = NameHelper.GetFullName(user.FirstName, user.MiddleName, user.Surname);
                }

            }
           
            iview.PaypointUser = NameHelper.GetFullName(Payment.CreatedByUser.FirstName, Payment.CreatedByUser.MiddleName , Payment.CreatedByUser.Surname );
            iview.Paypoint = Payment.CreatedByUser.Institution.Name;
            
            
            iview.Id = Payment.Id;

        }

        public static Payment ConvertToPayment(this PaymentView PaymentView)
        {

           Payment i = new Payment();
            PaymentView.ConvertToPayment(i);
            return i;
        }

        public static void ConvertToPayment(this PaymentView PaymentView, Payment i)
        {
            i.Amount = PaymentView.Amount;
            i.Payee = PaymentView.Payee;
            i.PaymentDate = PaymentView.PaymentDate;            
            i.MembershipId = PaymentView.MembershipId;
            i.PaymentSource = PaymentView.PaymentSource;
            i.T24TransactionNo = PaymentView.T24TransactionNo;
         
         
            
         
        

        }

        public static IEnumerable<PaymentView> ConvertToPaymentViews(
                                              this IEnumerable<Payment> Payments, IInstitutionRepository _institutionRepository, IUserRepository _userRepository)
        {
            ICollection<PaymentView> iviews = new List<PaymentView>();
            foreach (Payment i in Payments)
            {
                iviews.Add(i.ConvertToPaymentView(_institutionRepository ,_userRepository ));
            }

            return iviews;


        }

        public static IEnumerable<Payment> ConvertToPayments(
                                               this IEnumerable<PaymentView> PaymentViews)
        {
            ICollection<Payment> ins = new List<Payment>();
            foreach (PaymentView iview in PaymentViews)
            {
                ins.Add(iview.ConvertToPayment());
            }

            return ins;

        }


       
    }
}
