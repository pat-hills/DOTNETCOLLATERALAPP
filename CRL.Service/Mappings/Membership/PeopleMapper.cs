using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CRL.Model.ModelViews;
using CRL.Service.Views.Memberships;
using CRL.Model.ModelViews;
using CRL.Model.ModelViewMappers;
using CRL.Model.ModelViews.Memberships;
using CRL.Model.Memberships;

namespace CRL.Service.Mappings.Membership
{
    public static class PersonMapper
    {
        //Convert from people to PeopleView creating new
        public static IEnumerable<PersonView> ConvertToPeopleViews(
                                                this IEnumerable<Person> People)
        {
            ICollection <PersonView> ppl = new List<PersonView>();
           
            foreach (var pr in People)
            {
                if (pr.GetType () == typeof(User))
                {
                    ppl.Add (((User)pr).ConvertToUserView());
                }
                else
                {
                    ppl.Add(pr.ConvertToPersonView ());
                }
            }

            return ppl;
        }

        public static PersonView ConvertToPersonView(this Person User)
        {
            return Mapper.Map<Person, PersonView>(User);
        }
    }
}
