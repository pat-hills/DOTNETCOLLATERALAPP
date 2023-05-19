using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CRL.Service.Messaging.Workflow.Response;
using CRL.UI.MVC.Common;

using CRL.Infrastructure.Authentication;
using CRL.Service.Messaging.Workflow.Request;
using CRL.Service.Interfaces;
using StructureMap;
using CRL.UI.MVC.Models.Report.ViewModel;
using CRL.Service.Messaging.FinancialStatements.Response;
using CRL.Service.Messaging;
using CRL.UI.MVC.Models.Dashboard.ViewModel;
using CRL.Service.Messaging.User.Response;
using CRL.Service.Messaging.User.Request;
using CRL.Service.Views.FinancialStatement;
using CRL.Infrastructure.Configuration;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using CRL.Service.Messaging.Institution.Response;
using CRL.Service.Messaging.Institution.Request;
using CRL.Infrastructure.Enums;
using CRL.Service.Messaging.Reporting.Request;
using CRL.Infrastructure.Messaging;
using CRL.Model.Messaging;
using CRL.UI.MVC.Areas.Membership.Models.ViewPageModels;
using CRL.Model.FS.Enums;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.UI.MVC.Controllers
{
    public class HomeController : Controller
    {
        
        SecurityUser SecurityUser
        {
            get
            {
                return (SecurityUser)this.HttpContext.User;

            }
        }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            ViewBag.Active = "Home";
            return View("Home");
        }

        //
        // GET: /Home/Dasboard/5
        //This is Kwasi's Comment
        [MasterEventAuthorizationAttribute]
        public ActionResult Dashboard()
        {
            RequestBase request = new RequestBase();
            request.SecurityUser = SecurityUser;
            ViewFSResponse response = new ViewFSResponse();
            ViewMyTasksResponse response2 = new ViewMyTasksResponse();
            
            ViewMyMessagesResponse response4 = new ViewMyMessagesResponse();
            ViewStatResponse response5 = new ViewStatResponse ();

            DashboardViewModel viewModel = new DashboardViewModel();

            IWidgetService WS = ObjectFactory.GetInstance<IWidgetService>();
            response = WS.ViewMy10AboutToExpireFinancingStatement(request);
            response2 = WS.ViewMy10LatestTasks(request);
            if (SecurityUser.IsAudit()) 
            {
                ViewAuditsResponse response3 = new ViewAuditsResponse();
                response3 = WS.ViewMy10Audits(request);
                viewModel.AuditViewModel = response3.AuditViews.ToList();
            }
            response4 = WS.ViewMy10Messages(request);
            response5 = WS.ViewNoOfFinancingStatement (request );
            viewModel.FSGridViewModel = response.FSGridView.ToList();
            
            viewModel.TaskGridView = response2.TaskGridView.ToList();
            viewModel.MessagesView = response4.MessagesViews.ToList();
            viewModel.TotalNoOfFS = response5.Total;
            //graph data
            ViewBag.Searches = "23,456,900,900";
            ViewBag.FinanceSt = "79,970,900";
            ViewBag.Ammends = "34,000";
            ViewBag.Revenue = "100,678";

            ViewBag.MaxSearch = "40,000";
            ViewBag.MinSearch = "1,320";
            ViewBag.AvgSearch = "2,581";

            ViewBag.MaxFin = "53,000";
            ViewBag.MinFin = "1,561";
            ViewBag.AvgFin = "3,010";
           

            return View(viewModel);

        }

        //
        // GET: /Home/Create

        public ActionResult Registration()
        {
            return RedirectToAction("Index", "Registration", new { Area = "Membership" });

        }

        //
        // POST: /Home/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Home/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Home/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Home/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Home/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Faq()
        {
            return View();
        }

        //public ActionResult Fee()
        //{
        //    return View();
        //}

        public ActionResult Request()
        {
            return View();
        }

        public ActionResult DownloadBatchFIle()
        {
            string path = "";
            string fileName = "";
            string clientCode = "";
            string ClientProfileFolder = "Client profiles";
            if (SecurityUser.IsIndividualUser)
            {

                IUserService US = ObjectFactory.GetInstance<IUserService>();
                GetUserViewResponse response = US.GetView(new GetUserViewRequest {  Id = SecurityUser.Id });
                clientCode = response.UserView.MembershipView.CreditCode;
                IdentificationView Identity = new IdentificationView()
                {
                    FirstName = response.UserView.FirstName,
                    MiddleName = response.UserView.MiddleName,
                    Surname = response.UserView.Surname,


                };

                IndividualSPView individualuser = new IndividualSPView()
                {
                    Id = response.UserView.Id,
                    Title = response.UserView.PersonTitle,
                    Address = response.UserView.Address,
                    City = response.UserView.City,
                    CountryId = response.UserView.CountryId,
                    CountyId = response.UserView.CountyId,
                    //Email = response.UserView.Email,
                    Gender = response.UserView.Gender,
                    ParticipantTypeId = Model.FS.Enums.ParticipantCategory.Individual,
                    ParticipationTypeId = ParticipationCategory.AsSecuredParty,
                    //Phone = response.UserView.Phone,
                    //NationalityId = response.UserView.NationalityId,
                    //DOB =  null,
                    Identification = Identity,
                };

                Random rand = new Random();
                string randomNumber = rand.Next(1, 10000).ToString();
                fileName = "IndividualClient_Profile_" + randomNumber + " - " + clientCode.Trim() + ".dat";
                path = Constants.GetApplicationPath + "\\docs\\" + ClientProfileFolder + "\\" + fileName;
                Stream stream = System.IO.File.Open(path, FileMode.Create);

                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, individualuser);
                stream.Close();
                //  fileName = "IndividualClient_Profile-" + clientCode + ".dat";

            }
            else
            {
                int instiutionId = (int)SecurityUser.InstitutionId;
                IInstitutionService IS = ObjectFactory.GetInstance<IInstitutionService>();
                GetViewInstitutionRequest request = new GetViewInstitutionRequest { Id = instiutionId };
                request.SecurityUser = SecurityUser;
                GetViewInstitutionResponse response = IS.GetView(new GetViewInstitutionRequest { Id = instiutionId, EditMode = EditMode.View });
                clientCode = response.InstitutionView.MembershipView.CreditCode;
                InstitutionSPView InstitutionClient = new InstitutionSPView()
                {
                    Address = response.InstitutionView.Address,
                    City = response.InstitutionView.City,
                    CompanyNo = response.InstitutionView.CompanyNo,
                    CountryId = response.InstitutionView.CountryId,
                    Name = response.InstitutionView.Name,
                    CountyId = response.InstitutionView.CountyId,
                    //Email = response.InstitutionView.Email,
                    //NationalityId = response.InstitutionView.NationalityId,
                    //Phone = response.InstitutionView.Phone,
                    SecuringPartyIndustryTypeId = response.InstitutionView.SecuringPartyTypeId,
                };

                Random rand = new Random();

                string randomNumber = rand.Next(1, 10000).ToString();
                fileName = "InstitutionClient_Profile_" + randomNumber + " - " + clientCode.Trim() + ".dat";
                path = Constants.GetApplicationPath + "\\docs\\" + ClientProfileFolder + "\\" + fileName;
                Stream stream = System.IO.File.Open(path, FileMode.Create);
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, InstitutionClient);
                stream.Close();
                //  fileName = "InstitutionClient_Profile-" + clientCode + ".dat";
            }

            string FilePath = path;
            byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

        }

        public ActionResult GetSearchData(int year)
        {
            System.Globalization.DateTimeFormatInfo mfi = new 
System.Globalization.DateTimeFormatInfo();


            RequestByDate request = new RequestByDate();
            IReportService WS = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewStatResponse response = WS.ViewNoOfSearchStatByYearMonth (request);
            List<int[]> myarray = new List<int[]>();
            List<string> xaxisArray = new List<string>();
            int[] amts = response.CountOfItemStatView.Select(s => s.CountOfItem).Skip(response.CountOfItemStatView.Count() - 12).ToArray();
            int total=amts.Sum(); ;
            int max = amts.Max();
            int min = amts.Min();
            double avg = amts.Average();
            int Index=1;
            foreach (var rows in response.CountOfItemStatView.Skip(response.CountOfItemStatView.Count() - 12))
            {

                myarray.Add(new[] {Index, rows.CountOfItem });
                xaxisArray.Add(mfi.GetAbbreviatedMonthName(rows.MonthNum) + " "+ rows.Year.ToString ().Substring (2));
                Index++;
                
            }
            var result = new
            {
                data = myarray .ToArray (),
                xlabel = xaxisArray.ToArray (),
                total = total.ToString("#,##0"),
                max = max.ToString("#,##0"),
                min = min.ToString("#,##0"),
                avg = avg.ToString("#,##0.00")
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFinancingData(int year)
        {
            System.Globalization.DateTimeFormatInfo mfi = new
System.Globalization.DateTimeFormatInfo();
            RequestByDate request = new RequestByDate();

            IReportService WS = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewStatResponse response = WS.ViewNoOfFSByYearMonthStat(request);
            List<int[]> myarray = new List<int[]>();
            List<string> xaxisArray = new List<string>();
            int[] amts = response.CountOfItemStatView.Select(s => s.CountOfItem).Skip(response.CountOfItemStatView.Count() - 12).ToArray();
            int total = amts.Sum(); ;
            int max = amts.Max();
            int min = amts.Min();
            double avg = amts.Average();
            int Index = 1;
            foreach (var rows in response.CountOfItemStatView.Skip(response.CountOfItemStatView.Count() - 12))
            {

                myarray.Add(new[] { Index, rows.CountOfItem });
                xaxisArray.Add(mfi.GetAbbreviatedMonthName(rows.MonthNum) + " " + rows.Year.ToString().Substring(2));
                Index++;

            }
            var result = new
            {
                data = myarray.ToArray(),
                xlabel = xaxisArray.ToArray(),
                total = total.ToString("#,##0"),
                max = max.ToString("#,##0"),
                min = min.ToString("#,##0"),
                avg = avg.ToString("#,##0.00")
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAmmendData(int year)
        {
            System.Globalization.DateTimeFormatInfo mfi = new
            System.Globalization.DateTimeFormatInfo();
            RequestByDateAndClient request = new RequestByDateAndClient();
            IReportService WS = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewStatResponse response = WS.ViewNoOfAmendmentStatByYearMonth (request);
            List<int[]> myarray = new List<int[]>();
            List<string> xaxisArray = new List<string>();
            int[] amts = response.CountOfItemStatView.Select(s => s.CountOfItem).Skip(response.CountOfItemStatView.Count() - 12).ToArray();
            int total = amts.Sum(); 
            int max = amts.Max();
            int min = amts.Min();
            double avg = amts.Average();
            int Index = 1;
            foreach (var rows in response.CountOfItemStatView.Skip(response.CountOfItemStatView.Count() - 12))
            {

                myarray.Add(new[] { Index, rows.CountOfItem });
                xaxisArray.Add(mfi.GetAbbreviatedMonthName(rows.MonthNum) + " " + rows.Year.ToString().Substring(2));
                Index++;

            }
            var result = new
            {
                data = myarray.ToArray(),
                xlabel = xaxisArray.ToArray(),
                total = total.ToString("#,##0"),
                max = max.ToString("#,##0"),
                min = min.ToString("#,##0"),
                avg = avg.ToString("#,##0.00")
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetRevenueData(int year)
        //{
        //    Thread.Sleep(12000);
        //    return Json(new[] { new[] { 1, 10 }, new[] { 2, 15 }, new[] { 3, 20 }, new[] { 4, 25 }, new[] { 5, 30 }, new[] { 6, 35 }, new[] { 7, 10 }, new[] { 8, 15 }, new[] { 9, 20 }, new[] { 10, 25 }, new[] { 11, 30 }, new[] { 12, 35 } }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Fees() 
        {
            return View();
        }

        public ActionResult HowTo()
        {
            return View();
        }

        public ActionResult VideoTutorials()
        {
            return View();
        }
        
    }

}
