using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Enums;
using CRL.Infrastructure.Helpers;
using CRL.Service.Common;

using CRL.Service.Views.Memberships;
using CRL.Model.ModelViews;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Messaging.User.Response
{
    public class GetUserResponse : ResponseBase
    {
        public ICollection<LookUpView> InstitutionUnits { get; set; }
        public ICollection<LookUpView> Nationalities { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> Countries { get; set; }
        public ICollection<LookUpView> Countys { get; set; }
        public ICollection<LookUpView> RegularClientInstitutions { get; set; }
        public UserView UserView { get; set; }

        public TypeOfUser TypeOfUser { get; set; }
    }
    public class GetUserCreateResponse : GetUserResponse
    {
      

    }
    public class GetUserEditResponse : GetUserResponse
    {
       
                     
    }
    public class GetUserViewResponse : GetUserResponse
    {


    }
}
