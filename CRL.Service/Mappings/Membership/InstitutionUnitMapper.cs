using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CRL.Model.ModelViews;
using CRL.Service.Views.Memberships;
using CRL.Model.ModelViews.Memberships;
using CRL.Model.Memberships;

namespace CRL.Service.Mappings.Membership
{
    public static class InstitutionUnitMapper
    {
        public static InstitutionUnitView ConvertToInstitutionUnitView(this InstitutionUnit InstitutionUnit)
        {
            InstitutionUnitView view = new InstitutionUnitView();
            InstitutionUnit.ConvertToInstitutionUnitView(view);
            return view;

         
        }
        public static void ConvertToInstitutionUnitView(this InstitutionUnit InstitutionUnit, InstitutionUnitView view)
        {
             Mapper.Map<InstitutionUnit, InstitutionUnitView>(InstitutionUnit, view);
        }

        public static InstitutionUnit ConvertToInstitutionUnit(this InstitutionUnitView InstitutionUnitView)
        {
            InstitutionUnit InstitutionUnit = new InstitutionUnit();
            InstitutionUnitView.ConvertToInstitutionUnit(InstitutionUnit);
            return InstitutionUnit;

           
        }

        public static void ConvertToInstitutionUnit(this InstitutionUnitView view, InstitutionUnit InstitutionUnit)
        {
            Mapper.Map<InstitutionUnitView, InstitutionUnit>(view, InstitutionUnit);
        }


        public static IEnumerable<InstitutionUnitView> ConvertToInstitutionUnitViews(
                                                this IEnumerable<InstitutionUnit> InstitutionUnits)
        {
            return Mapper.Map<IEnumerable<InstitutionUnit>,
                              IEnumerable<InstitutionUnitView>>(InstitutionUnits);
        }



        public static IEnumerable<InstitutionUnit> ConvertToInstitutionUnits(
                                               this IEnumerable<InstitutionUnitView> InstitutionUnitViews)
        {
            return Mapper.Map<IEnumerable<InstitutionUnitView>,
                              IEnumerable<InstitutionUnit>>(InstitutionUnitViews);
        }





        public static InstitutionUnitGridView ConvertToInstitutionUnitGridView(this InstitutionUnit InstitutionUnit)
        {

            InstitutionUnitGridView view = new InstitutionUnitGridView();
            InstitutionUnit.ConvertToInstitutionUnitGridView(view);
            return view;
        }

        public static void ConvertToInstitutionUnitGridView(this InstitutionUnit InstitutionUnit, InstitutionUnitGridView view)
        {
            view.Id = InstitutionUnit.Id;
            view.Name = InstitutionUnit.Name;
            view.Email = InstitutionUnit.Email ;
            view.CreatedOn = InstitutionUnit.CreatedOn;
            view.HasUsers = InstitutionUnit.People.OfType <User>().Where(p => p.IsDeleted == false).Count() > 0;
        }




        public static IEnumerable<InstitutionUnitGridView> ConvertToInstitutionUnitGridViews(
                                              this IEnumerable<InstitutionUnit> InstitutionUnits)
        {
            ICollection<InstitutionUnitGridView> iviews = new List<InstitutionUnitGridView>();
            foreach (InstitutionUnit i in InstitutionUnits)
            {
                iviews.Add(i.ConvertToInstitutionUnitGridView());
            }

            return iviews;
        }
    }
}
