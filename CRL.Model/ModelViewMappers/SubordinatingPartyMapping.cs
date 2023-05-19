using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.FS;
using CRL.Service.Common;
using CRL.Model.ModelViews.Amendment;

namespace CRL.Model.ModelViewMappers
{
    public static class SubordinatingPartyMapping
    {
        public static SubordinatingPartyView ConvertToSubordinatingPartyViewBase(this SubordinatingParty model)
        {

            SubordinatingPartyView iview = new SubordinatingPartyView();
            model.ConvertToSubordinatingPartyViewBase(iview);
            return iview;
        }

        public static void ConvertToSubordinatingPartyViewBase(this SubordinatingParty model, SubordinatingPartyView iview)
        {
            iview.Address = model.Address.Address  ;
            iview.Address2 = model.Address.Address2;
            
            iview.City = model.Address.City;
            iview.CountryId = model.CountryId;
            iview.CountyId = model.CountyId;            
            iview.Country = model.Country.Name;
            iview.LGAId = model.LGAId;
            iview.LGA = model.LGA.Name;
            iview.County = model.County.Name;
            iview.Email = model.Address.Email;
            iview.Phone = model.Address.Phone;       
            iview.NationalityId = model.NationalityId;
            iview.Nationality = model.Nationality.Name;
            iview.isBeneficiary = model.isBeneficiary;
            iview.Id = model.Id;

        }

        public static SubordinatingParty ConvertToSubordinatingPartyBase(this SubordinatingPartyView iview)
        {

            SubordinatingParty model = new SubordinatingParty();
            iview.ConvertToSubordinatingPartyBase(model);
            return model;
        }

        public static void ConvertToSubordinatingPartyBase(this SubordinatingPartyView iview, SubordinatingParty model)
        {

            model.Address.Email = iview.Email;
            model.Address.Phone = iview.Phone;
            model.Address.Address = iview.Address;
            model.Address.Address2 = iview.Address2;
            model.Address.City = iview.City;
            model.CountryId = iview.CountryId;
            model.CountyId = iview.CountyId;          
            model.NationalityId = iview.NationalityId;
            model.isBeneficiary = iview.isBeneficiary;
            model.LGAId = iview.LGAId;
            
            //model.IsActive = true;
            //model.IsDeleted = false;            
            model.Id = iview.Id;

        }

        public static SubordinatingParty ConvertToSubordinatingParty(
                                           this SubordinatingPartyView p)
        {
            SubordinatingParty participant=null;
          
            

                if (p is IndividualSubordinatingPartyView )
                {

                    participant = new IndividualSubordinatingParty();
                    p.ConvertToSubordinatingPartyBase(participant);
                    ((IndividualSubordinatingPartyView)p).ConvertToIndividualSubordinatingParty((IndividualSubordinatingParty)participant);
                    
                }
                else if (p is InstitutionSubordinatingPartyView)
                {
                    participant = new InstitutionSubordinatingParty();
                    p.ConvertToSubordinatingPartyBase(participant);
                    ((InstitutionSubordinatingPartyView)p).ConvertToInstitutionSubordinatingParty((InstitutionSubordinatingParty)participant);

                


                }



                return participant;
        }

        public static SubordinatingPartyView ConvertToSubordinatingPartyView(
                                        this SubordinatingParty p)
        {
            SubordinatingPartyView participant = null;



            if (p is IndividualSubordinatingParty)
            {

                participant = new IndividualSubordinatingPartyView();
                p.ConvertToSubordinatingPartyViewBase (participant);
                ((IndividualSubordinatingParty)p).ConvertToIndividualSubordinatingPartyView((IndividualSubordinatingPartyView)participant);

            }
            else if (p is InstitutionSubordinatingParty)
            {
                participant = new InstitutionSubordinatingPartyView();
                p.ConvertToSubordinatingPartyViewBase(participant);
                ((InstitutionSubordinatingParty)p).ConvertToInstitutionSubordinatingPartyView ((InstitutionSubordinatingPartyView)participant);




            }



            return participant;
        }


       
    }
}
