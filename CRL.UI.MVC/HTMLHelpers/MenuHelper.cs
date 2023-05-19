using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;
using CRL.Service.Views.Navigation;
using CRL.Infrastructure.Configuration;
using System.Configuration;

namespace CRL.UI.MVC.HTMLHelpers
{
    public static class MenuHelper
    {
        private static IEnumerable<MenuView> BuildSampleMenu()
        {

            bool EnableBatchUpload = false;

            var menuList = new List<MenuView>();
            //menuList.Add(new MenuView { Id = 1, Name = "Dashboard", Url = "Home/Dashboard", AvailableToPublic = false });
            menuList.Add(new MenuView { Id = 23, Name = "My Tasks", Url = "#" });

            menuList.Add(new MenuView { Id = 2, Name = "Search", Url = "#", Roles = new string[] { "Search" }, AvailableToPublic = true });

            menuList.Add(new MenuView { Id = 4, Name = "Financing Statement", Url = "#" });
            menuList.Add(new MenuView
            {
                Id = 3,
                Name = "Payment",
                Url = "#",
                AvailableToPublic = true

            });
            //menuList.Add(new MenuView { Id = 5, Name = "Amendment", Url = "FinancialStatement/FinancialStatementActivity/Index" });                           
            menuList.Add(new MenuView { Id = 6, Name = "Reports", Url = "Report/Index" });
            menuList.Add(new MenuView
            {
                Id = 7,
                Name = "Administration",
                Url = "#",
                Roles = new string[] { "Administrator (Client)", "Administrator (Owner)", "Unit Administrator (Owner)", "Unit Administrator (Client)", "Registrar", "Support" }
            });
            menuList.Add(new MenuView { Id = 43, Name = "Configuration", Url = "#" });
            menuList.Add(new MenuView { Id = 60, Name = "Notifications", Url = "#" });

            var emailMenu = menuList.Where(s => s.Id == 60).Single();
            emailMenu.SubMenus = new List<MenuView>(){
                  new MenuView { Id =61, Name ="Email Messages", Url="Membership/Client/EmailIndex", AvailableToPublic=false},
                  new MenuView { Id =62,Name = "Global Messages",Url="Administration/Configuration/GlobalMessageIndex", AvailableToPublic=false}
                 };

            var adminmenu = menuList.Where(s => s.Id == 7).Single();
            adminmenu.SubMenus = new List<MenuView>(){
                 new MenuView { Id =8,Name = Constants.Project == Project.Liberia? "Manage Clients": "Clients",Url="Membership/Institution/Index",LimitOwnersOrClients = 1,Roles=new string[]{"Administrator (Owner)", "CRL Registrar"}
                 },//LIBERIA OFF
                 //new MenuView 
                 //{ Id =9,
                 //  Name ="Individual Clients", 
                 //  Url="Membership/User/Index?IndividualUsers=true", 
                 //  LimitOwnersOrClients = 1,
                 //  Roles=new string[]{"Administrator (Owner)","CRL Registrar"}
                 //},                
                 new MenuView { Id =27, Name ="Client Profile", Url="Membership/Institution/View",  LimitOwnersOrClients = 2, LimitInstitutionOrIndividual =1,Roles=new string[]{"Administrator (Client)","Administrator (Owner)","Registrar","Support"}},
                  new MenuView { Id =28, Name ="Units", Url="Membership/InstitutionUnit/Index",   LimitInstitutionOrIndividual =1,Roles=new string[]{"Administrator (Client)","Administrator (Owner)","Registrar","Support"}},
                  new MenuView { Id =29, Name ="Unit Profile", Url="Membership/InstitutionUnit/view",  LimitInstitutionOrIndividual =1, LimitToInstitutionOrUnits =2,Roles=new string[]{"Unit Administrator (Client)","Unit Administrator (Owner)"}},
                  new MenuView { Id =30, Name ="My Users", Url="Membership/User/Index", LimitInstitutionOrIndividual =1,Roles=new string[]{"Administrator (Client)","Administrator (Owner)","Unit Administrator (Owner)","Unit Administrator (Client)" ,"Registrar","Support"}}  ,
                 new MenuView { Id =33, Name ="Submit Postpaid Request",  Url="Membership/Client/SubmitPostpaidRequest",LimitOwnersOrClients=2,Roles=new string[]{"Administrator (Client)"}},
                 new MenuView { Id =38, Name ="Setup Postpaid Account", Url="Membership/Client/SubmitPostpaidRequest",LimitOwnersOrClients=2, LimitInstitutionOrIndividual =2},
                 // new MenuView { Id =34, Name ="My Paypoint Users", Url="Membership/User/IndexOfPayPointUsers",LimitInstitutionOrIndividual=1, Roles=new string[]{"Administrator (Client)","Administrator (Owner)","CRL Registrar"}},
                //  new MenuView{Id = 45 , Name="Submit Paypoint Users", Url="Membership/User/SubmitPayPointUsers",Roles=new string[]{"Administrator (Client)"} },
                   new MenuView { Id =35, Name ="My Postpaid Clients", Url="Membership/Client/PostPaidClients",LimitInstitutionOrIndividual=1,Roles=new string[]{"Administrator (Client)","Administrator (Owner)","Registrar","Support"}},
                       
                     new MenuView { Id =37, Name ="Audit Trail", Url="Membership/Client/IndexOfAudits",Roles=new string[]{"Audit"}},
                     new MenuView { Id =32, Name ="Emails", Url="Administration/EmailAdministration", LimitOwnersOrClients = 1,Roles=new string[]{"Administrator (Owner)","Registrar","Support"} }
                     
                    // new MenuView { Id =39, Name ="Configuration", Url="Configuration/Configuration/Create",Roles=new string[]{"Administrator (Client)","Administrator (Owner)"}},
                     
            //new MenuView { Id =10, Name ="Client Profile", Url=""},
            //new MenuView { Id =11, Name ="Units", Url=""},
            //new MenuView { Id =12, Name ="Users", Url=""}
            };
            var searchmenu = menuList.Where(s => s.Id == 2).Single();
            searchmenu.SubMenus = new List<MenuView>(){
                new MenuView { Id =18, Name ="Search", Url="Search/Search/Search", AvailableToPublic = true },
                //new MenuView { Id =31, Name ="Flexible Search", Url="Search/Search/Create?NonLegalEffect=true",AvailableToPublic = true },
                 new MenuView { Id =19, Name ="My Searches", Url="Search/Search/Index", AvailableToPublic = true}
            //new MenuView { Id =10, Name ="Client Profile", Url=""},
            //new MenuView { Id =11, Name ="Units", Url=""},
            //new MenuView { Id =12, Name ="Users", Url=""}


            
            };

            var configurationMenu = menuList.Where(s => s.Id == 43).Single();
            configurationMenu.SubMenus = new List<MenuView>(){
            
            new MenuView { Id =43, Name ="Fee Configuration", Url="Configuration/Configuration/FeeConfigurationDetails",LimitOwnersOrClients=1, AvailableToPublic = false },
            new MenuView { Id =63, Name ="Upload Bank Codes", Url="Configuration/Configuration/BVCUpload", LimitOwnersOrClients=1, AvailableToPublic = false, Roles= new string[]{"Administrator (Owner)"} },
            new MenuView { Id =44, Name ="Workflow Configuration", Url="Configuration/Configuration/WorkflowConfiguration", AvailableToPublic = false, Roles=new string[]{"Administrator (Client)","Administrator (Owner)","Unit Administrator (Owner)","Unit Administrator (Client)"} }
            };





            var regmen2 = menuList.Where(s => s.Id == 4).Single();
            regmen2.SubMenus = new List<MenuView>(){
                new MenuView { Id =16, 
                               Name ="My Financing Statements",
                               Url="FinancialStatement/FinancialStatement/Index"
                },
                new MenuView { Id =17, 
                               Name ="Create New Financing Statement", 
                               Url="FinancialStatement/FinancialStatement/CreateSubmit", 
                               LimitOwnersOrClients = 2, 
                               Roles= new string[]{"FS Officer","Client Officer"}},
                               
                              
                 new MenuView { Id =14, Name ="Saved Drafts", Url="FinancialStatement/FinancialStatement/IndexOfSavedDrafts"}
                     //new MenuView { Id =20, Name ="My Financing Statement Activties",  Url = "FinancialStatement/FinancialStatementActivity/Index" },
                //new MenuView { Id =21, Name ="New Financing Statement Activity", Url = "FinancialStatement/FinancialStatementActivity/SelectAmendmentType" , LimitOwnersOrClients = 2},
               // new MenuView { Id =22, Name ="Pending Financing Statement Activities", Url =  "FinancialStatement/FinancialStatementActivity/Index?InRequestMode=true"  , LimitOwnersOrClients = 2}
                   };

            EnableBatchUpload = ConfigurationManager.AppSettings["EnableBatchUpload"] != null ? Boolean.Parse(ConfigurationManager.AppSettings["EnableBatchUpload"]) : false;

            if (EnableBatchUpload)
            {
                regmen2.SubMenus.Add(
                    new MenuView
                    {
                        Id = 41,
                        Name = "Upload FS Batch",
                        Url = "FinancialStatement/FinancialStatementBatchUpload/Upload",
                        LimitOwnersOrClients = 2,
                        Roles = new string[] { "FS Officer", "Client Officer" }
                    });
            }

            if (EnableBatchUpload)
            {
                regmen2.SubMenus.Add(
                    new MenuView
                    {
                        Id = 42,
                        Name = "FS Batches",
                        Url = "FinancialStatement/FinancialStatementBatchUpload/IndexOfBatches",
                        LimitOwnersOrClients = 2,
                        Roles = new string[] { "FS Officer", "Client Officer" }
                    });
            }

            //PAYMENTS
            var regmen3 = menuList.Where(s => s.Id == 3).Single();
            regmen3.SubMenus = new List<MenuView>()
            {
            
            //    new MenuView { Id =20,
            //               Name ="Receive payment from registered client", 
            //               Url = "Payment/Payment/SelectRegisteredClient",
            //               ClearAllRightsBeforeRules =true,
            //               OverrideRolesForPaypointUser =true
                           
            //},            
            //new MenuView { Id =21, 
            //               Name ="Receive payment from public client", 
            //               Url = "Payment/Payment/CreatePayment",
            //               ClearAllRightsBeforeRules =true,
            //               OverrideRolesForPaypointUser =true},
            ////Paypoint user(s), Client Admin, CRL Admin, Finance Officer, CRL Officer, 
            new MenuView { Id =22, 
                           Name ="Payments", 
                           Url =  "Payment/Payment/Index?ShowType=1" ,
                           ClearAllRightsBeforeRules =true,
                           OverrideRolesForPaypointUser =true,
                           Roles = new string[] {"CRL Finance Officer","Administrator (Owner)", "Registrar", "Support"}                            
            },
            new MenuView { Id =39, 
                           Name ="My payments", 
                           Url =  "Payment/Payment/Index?ShowType=2" ,
                           ClearAllRightsBeforeRules =true,                           
                           Roles = new string[] {"Administrator (Client)","Finance Officer"}                            
            },
               new MenuView { Id =40, 
                           Name ="Generate Batch", 
                           Url =  "Payment/Payment/GenerateBatch" ,
                           ClearAllRightsBeforeRules =true,                           
                           Roles = new string[]{"Finance Officer"} ,
                           LimitToRegularClients = true                           
            },
             new MenuView { Id =41, 
                           Name ="View Client Postpaid Batches", 
                           Url =  "Payment/Payment/ViewBatches" ,
                           ClearAllRightsBeforeRules =true,                           
                           Roles = new string[] {"CRL Finance Officer","Administrator (Owner)","Registrar","Support"} ,
                           LimitToRegularClientAndOwner = true   
            },
                new MenuView { Id =42, 
                           Name ="View My Postpaid Batches", 
                           Url =  "Payment/Payment/ViewBatches" ,
                           ClearAllRightsBeforeRules =true,                           
                           Roles = new string[] {"Administrator (Client)","Finance Officer"} ,
                           LimitToRegularClientAndOwner = true   
            },
            new MenuView { Id =101, 
                           Name ="View WebPay Transactions", 
                           Url =  "Payment/InterSwitch/InterSwitchTransactionsIndex"  ,
                           ClearAllRightsBeforeRules =true,                           
                           Roles = new string[] {"CRL Finance Officer","Administrator (Owner)"}                       
            },
            new MenuView { Id =102, 
                           Name ="View DirectPay Transactions", 
                           Url =  "Payment/InterSwitch/DirectPayIndex"  ,
                           ClearAllRightsBeforeRules =true,                           
                           Roles = new string[] {"CRL Finance Officer","Administrator (Owner)"}                       
            },
             new MenuView { Id =103, 
                           Name ="Make An Online Payment", 
                           Url =  "Payment/InterSwitch/PaymentDetails" ,ClearAllRightsBeforeRules =true,AvailableToPublic = true                      
            },
             new MenuView { Id =104, 
                           Name ="Generate Payment Code", 
                           Url =  "Payment/InterSwitch/GenerateVoucherCode" ,ClearAllRightsBeforeRules =true,AvailableToPublic = true                      
            },
             new MenuView { Id =105, 
                           Name ="Verify Online Payment Transaction", 
                           Url =  "Payment/InterSwitch/TransactionDetails" ,ClearAllRightsBeforeRules =true,AvailableToPublic = true                      
            },
             new MenuView { Id =106, 
                           Name ="Check Payment Balance", 
                           Url =  "Payment/InterSwitch/CheckBalance" ,ClearAllRightsBeforeRules =true,AvailableToPublic = true  },

            new MenuView { Id =36, Name ="Credit Activities", Url="Membership/Client/IndexOfCreditActivities",Roles=new string[]{"Administrator (Client)","Administrator (Owner)","Finance Officer","CRL Finance Officer","Registrar","Support"}}


            };


            var regmen4 = menuList.Where(s => s.Id == 23).Single();
            regmen4.SubMenus = new List<MenuView>(){
                new MenuView { Id =24, Name ="My Pending Tasks",  Url = "Workflow/TaskHandle/Index" }
                //new MenuView { Id =25, Name ="My Executed Task", Url = "FinancialStatement/FinancialStatementActivity/SelectAmendmentType" , LimitOwnersOrClients = 2},
                
                 
                   };


            return menuList;
        }

        public static class MenuRule
        {
            public static bool ShowMenu(MenuView c, SecurityUser _user)
            {
                bool IsSatisifed = !c.ClearAllRightsBeforeRules;


                if (_user == null)
                {
                    if (c.AvailableToPublic)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }



                //Check if we can show this menu for public users
                if (c.OverrideRolesForPaypointUser)
                {
                    if (_user.IsPaypointUser)
                        return true;
                }
                if (!c.AvailableToPublic)
                    if (_user == null)
                        return false;

                if (c.Roles != null)
                {
                    bool inRole = false;
                    foreach (string s in c.Roles)
                    {
                        if (_user.IsInRole(s))
                        {
                            inRole = true; break;
                        }
                    }
                    if (!inRole)
                        return false;
                    else
                        IsSatisifed = true;
                }

                if (c.LimitOwnersOrClients == 1)
                {
                    if (_user != null)
                    {
                        if (!_user.IsOwnerUser) return false;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (c.LimitOwnersOrClients == 2)
                {
                    if (_user != null)
                    {
                        if (_user.IsOwnerUser) return false;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (c.LimitInstitutionOrIndividual == 1)
                {
                    if (_user != null)
                    {
                        if (_user.InstitutionId == null) return false;
                    }
                    else
                    {
                        return false;
                    }
                }


                if (c.LimitInstitutionOrIndividual == 2)
                {
                    if (_user != null)
                    {
                        if (_user.InstitutionId != null) return false;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (c.LimitToInstitutionOrUnits == 1)
                {
                    if (_user != null)
                    {
                        if (_user.InstitutionUnitId != null) return false;
                    }
                    else
                    {
                        return false;
                    }
                }



                if (c.LimitToInstitutionOrUnits == 2)
                {
                    if (_user != null)
                    {
                        if (_user.InstitutionUnitId == null) return false;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (c.LimitToRegularClients == true)
                {
                    if (_user != null)
                    {
                        if (_user.AccountType != 2) return false;
                    }
                    else
                    {
                        return false;
                    }

                }


                if (c.LimitToRegularClientAndOwner == true)
                {
                    if (_user != null)
                    {
                        if (!(_user.AccountType == 2 || _user.IsOwnerUser)) return false;
                    }
                    else
                    {
                        return false;
                    }

                }


                return IsSatisifed;

            }
        }
        public static MvcHtmlString MenuLinks(this HtmlHelper html, string CurrentMenu)
        {
            string port = "";
            if (ConfigurationManager.AppSettings["ApplicationPort"] != null)
            {
                port = !String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ApplicationPort"].ToString()) ? ":" + ConfigurationManager.AppSettings["ApplicationPort"] : "";
            }


            string Root = HttpContext.Current.Request.ApplicationPath;

            if (Root.Trim() != "/")
                Root += "/";
            string urlSuffix = HttpContext.Current.Request.Url.Authority + port + Root;
            Root = HttpContext.Current.Request.Url.Scheme + @"://" + urlSuffix;

            SecurityUser _user = null;
            if (HttpContext.Current.User != null)
            {
                try
                {
                    _user = (SecurityUser)HttpContext.Current.User;
                }
                catch (Exception ex)
                {
                    _user = null;
                }
            }

            StringBuilder result = new StringBuilder();
            TagBuilder tag_ul = new TagBuilder("ul");


            tag_ul.MergeAttribute("class", "main-nav");
            var menuInfo = BuildSampleMenu();
            //(c.Submenus == null || c.submenus.count>0 ||) && c.Url="#"
            foreach (MenuView c in menuInfo)
            {
                if (!MenuRule.ShowMenu(c, _user))
                    continue;
                int ShowMenu = 0;
                if (c.Url == "#" && c.SubMenus == null)
                    continue;

                if (c.SubMenus != null)
                {
                    foreach (var sub in c.SubMenus)
                    {

                        if (MenuRule.ShowMenu(sub, _user))
                            ShowMenu++;
                    }

                    if (c.Url == "#" && ShowMenu == 0)
                        continue;
                }
                TagBuilder tag_li = new TagBuilder("li");
                TagBuilder tag_a = new TagBuilder("a");
                TagBuilder tag_span = new TagBuilder("span");
                TagBuilder tag_span_caret = null;
                if (!(String.IsNullOrEmpty(c.Url)))
                {
                    tag_a.MergeAttribute("href", Root + c.Url);

                }
                if (c.SubMenus != null && c.SubMenus.Count > 0)
                {
                    tag_a.MergeAttribute("data-toggle", "dropdown");
                    tag_a.AddCssClass("dropdown-toggle");

                    tag_span_caret = new TagBuilder("span");
                    tag_span_caret.InnerHtml = "<i class='icon-chevron-down'></i>";
                    tag_span_caret.AddCssClass("menuhelperFont");

                }

                if (c.Name == CurrentMenu)
                    tag_li.AddCssClass("active");

                tag_span.SetInnerText(c.Name);

                if (tag_span_caret != null)
                {
                    tag_a.InnerHtml = tag_span.ToString() + tag_span_caret.ToString();

                }
                else
                {
                    tag_a.InnerHtml = tag_span.ToString();

                }
                tag_li.InnerHtml = tag_a.ToString();

                if (c.SubMenus != null && c.SubMenus.Count > 0)
                {

                    TagBuilder subtag_ul = new TagBuilder("ul");
                    foreach (var submenus in c.SubMenus)
                    {
                        if (!MenuRule.ShowMenu(submenus, _user))
                            continue;
                        TagBuilder subtag_li = new TagBuilder("li");
                        TagBuilder subtag_a = new TagBuilder("a");

                        subtag_a.MergeAttribute("href", Root + submenus.Url);
                        if (submenus.Name == CurrentMenu)
                        {
                            subtag_li.AddCssClass("active");
                            tag_li.AddCssClass("active");
                        }
                        subtag_a.SetInnerText(submenus.Name);

                        subtag_li.InnerHtml = subtag_a.ToString();
                        subtag_ul.InnerHtml += subtag_li.ToString();
                        subtag_ul.AddCssClass("dropdown-menu");
                    }
                    tag_li.InnerHtml += subtag_ul.ToString();
                }



                tag_ul.InnerHtml += tag_li.ToString();




            }

            return new MvcHtmlString(tag_ul.ToString());



        }
    }
}