using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.Navigation
{
    public class MenuView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public MenuView ParentMenu { get; set; }
        public IList<MenuView> SubMenus { get; set; }
        public string[] Roles { get; set; }
        public int LimitOwnersOrClients { get; set; }
        public int LimitInstitutionOrIndividual { get; set; }
        public int LimitToInstitutionOrUnits { get; set; }
        public bool AvailableToPublic { get; set; }
        public bool OverrideRolesForPaypointUser { get; set; }
        public bool ClearAllRightsBeforeRules { get; set; }
        public bool LimitToRegularClients { get; set; }


        public bool LimitToRegularClientAndOwner { get; set; }
    }


}
