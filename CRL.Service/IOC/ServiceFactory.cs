using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Service.Implementations;
using CRL.Service.Interfaces;
using CRL.Service.Interfaces.FinancialStatement;

namespace CRL.Service.IOC
{
    public static class ServiceFactory
    {
        public static IInstitutionUnitService InstitutionUnitService()
        {
            return new InstitutionUnitService();
        }
        
        public static IInstitutionService InstitutionService()
        {
            return new InstitutionService();
        }
        public static IFinancingStatementService FinancialStatementService()
        {
            return new FinancingStatementService ();
        }

        public static ISearchService SearchService()
        {
            return new SearchService();
        }

        public static IMembershipRegistrationService  MembershipRegistrationService()
        {
            return new MembershipRegistrationService ();
        }
        public static IUserService  UserService()
        {
            return new UserService ();
        }
        public static IClientService ClientService()
        {
            return new ClientService();
        }
        public static IPaymentService PaymentService()
        {
            return new PaymentService();
        }
        public static IAuthenticationService AuthenticatonService()
        {
            return new AuthenticatonService ();
        }

    }
}
