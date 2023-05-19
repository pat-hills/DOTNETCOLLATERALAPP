using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CRL.Model.FS;
using CRL.Model.ModelViews;
using CRL.Service.Views;

using CRL.Service.Views.FinancialStatement;
using CRL.Service.Views.Memberships;
using CRL.Model.ModelViews;
using CRL.Service.Views.Configuration;
using CRL.Model.Configuration;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews.Memberships;
using CRL.Model.Memberships;

namespace CRL.Service
{
    public class AutoMapperBootStrapper
    {
        public static void ConfigureAutoMapper()
        {
            // FinancialStatement
            Mapper.CreateMap<FinancialStatement, FSView>();//.ForMember(c => c.CollateralsView, m => m.MapFrom(s => s.Collaterals));

            Mapper.CreateMap<FSView, FinancialStatement>();//.ForMember(c => c.FinancialStatementType, option => option.Ignore()).ForMember(c => c.Participants, option => option.Ignore());
            Mapper.CreateMap<FinancialStatement, FSSummaryView>().ForMember(c => c.CollateralTypeName, m => m.MapFrom(s => s.CollateralType.CollateralCategoryName)); //.ForMember(c => c.FinancialStatementType, option => option.Ignore()).ForMember(c => c.Participants, option => option.Ignore());

            Mapper.CreateMap<Collateral, CollateralView>();
            Mapper.CreateMap<Collateral, CollateralSummaryView>();
            Mapper.CreateMap<CollateralView, Collateral>(); //.ForMember(c => c.CollateralSubTypeTypes , option => option.Ignore()).ForMember(c => c.AssetType, option => option.Ignore());
           
            //Mapper.CreateMap<Participant, ParticipantView>().ForMember(c => c.SectorOfOperationTypes , option => option.Ignore()); ;
           // Mapper.CreateMap<ParticipantView, Participant>().ForMember(c => c.SectorOfOperationTypes , option => option.Ignore());


         

            Mapper.CreateMap<IndividualSPView, IndividualParticipant>().ForMember(c => c.Address, option => option.Ignore())
                .ForMember(c => c.SectorOfOperationTypes, option => option.Ignore());
               


            Mapper.CreateMap<InstitutionSPView, InstitutionParticipant>().ForMember(c => c.Address, option => option.Ignore())
                .ForMember(c => c.FinancialStatement, option => option.Ignore())
                .ForMember(c => c.SectorOfOperationTypes, option => option.Ignore());

            // Membership
            Mapper.CreateMap<Institution, InstitutionView>();
            Mapper.CreateMap<InstitutionView, Institution>().ForMember(c => c.Address, option => option.Ignore()); 
            //Mapper.CreateMap<Institution, InstitutionGridView>().ForMember(c => c.InstitutionType , m => m.MapFrom(s => s.CompanyType.CompanyCategoryName))
            //    .ForMember(c => c.AccountType , m => m.MapFrom(d=>d.Membership.MembershipAccountType.MembershipAccountCategoryName));


            // Membership
            Mapper.CreateMap<InstitutionUnit, InstitutionUnitView>();
            Mapper.CreateMap<InstitutionUnitView, InstitutionUnit>();

            Mapper.CreateMap<Person, PersonView>();
            Mapper.CreateMap<PersonView, Person>();
            // Membership
            Mapper.CreateMap<User, UserView >();
            //Mapper.CreateMap<UserView, User>().ForMember(c => c.Address, option => option.Ignore())
            //    .ForMember (d=>d.Nationality, option=>option.Ignore())
            //    .ForMember(d => d.Country , option => option.Ignore());
              

            Mapper.CreateMap<Role, RoleView>();
            Mapper.CreateMap<RoleView, Role>();
            Mapper.CreateMap<ConfigurationWorkflowView, ConfigurationWorkflow>();
            Mapper.CreateMap<ConfigurationWorkflow,ConfigurationWorkflowView>();

        
         
            Mapper.CreateMap<User,UserGridView>();

            Mapper.CreateMap<ChangeDescription, ChangeDescriptionView>();

            
            
              
          
        }
    }
}
