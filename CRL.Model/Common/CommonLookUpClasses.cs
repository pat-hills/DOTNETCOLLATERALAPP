using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Infrastructure;

namespace CRL.Model.Common
{
    /// <summary>
    /// Represents lookup values for MembershipAccount Types
    /// </summary>

    [Serializable]
    public class LKNationality : EntityBase<int>, IAggregateRoot
    {
        public string Name { get; set; }



        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Represents lookup values for MembershipAccount Types
    /// </summary>

    [Serializable]
    public class LKCountry : EntityBase<int>, IAggregateRoot
    {
        public string Name { get; set; }



        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class LKCounty : EntityBase<int>, IAggregateRoot
    {
        public string Name { get; set; }



        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }


     [Serializable]
    public class LKLGA: EntityBase<int>, IAggregateRoot
    {
        public string Name { get; set; }


        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }

        public int CountyId { get; set; }
        public LKCounty County { get; set; }
    }



     [Serializable]
     public class LKRegistrationPrefix : EntityBase<int>, IAggregateRoot
     {
         public string Name { get; set; }


         protected override void CheckForBrokenRules()
         {
             throw new NotImplementedException();
         }
     }




}
