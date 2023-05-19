using CRL.Infrastructure.Domain;
using CRL.Model.Configuration;
using CRL.Model.Payments;
using CRL.Service.Views.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Mappings.Configuration
{
    public static class FeeConfigurationMapper
    {
        //public static PerTransactionConfigurationFeeView ConvertToPerTransactionConfigurationFeeView(this PerTransactionConfigurationFee model)
        // {
        //     PerTransactionConfigurationFeeView iview = new PerTransactionConfigurationFeeView();
        //     model.ConvertToPerTransactionConfigurationFeeView(iview);
        //     return iview;
        // }

        public static PerTransactionConfigurationFeeView ConvertToPerTransactionConfigurationFeeView(this PerTransactionConfigurationFee model)
        {
            PerTransactionConfigurationFeeView iview = new PerTransactionConfigurationFeeView();
            model.ConvertToPerTransactionConfigurationFeeView(iview);
            return iview;
        }

        public static void ConvertToPerTransactionConfigurationFeeView(this PerTransactionConfigurationFee model, PerTransactionConfigurationFeeView iview)
        {
            iview.Id = model.Id;
            iview.Amount = model.FlatAmount;
            iview.AllowPostpaidIfClientIsPostpaid = model.AllowPostpaidIfClientIsPostpaid;
            iview.Name = model.Name;
            iview.PerTransactionOrReoccurence = model.PerTransactionOrReoccurence;
            //iview.UseLoanAmountFilter = model.UseLoanAmountFilter;
            iview.ServiceFeeCategory = model.ServiceFeeType.Select(s => (int)s.Id).ToArray();



            if (model.ServiceFeeType.Count > 0)
            {
                iview.FeeLoanSetupsView = new List<FeeLoanSetupView>();
                foreach (var feeSetup in model.FeeLoanSetups.Where(s => s.IsDeleted == false))
                {
                    FeeLoanSetupView _FeeLoanSetup = new FeeLoanSetupView()
                    {
                        Id = (int)feeSetup.Id,
                        PerTransactionConfigurationFeeId = feeSetup.PerTransactionConfigurationFeeId,
                        LoanAmountLimit = feeSetup.LoanAmountLimit,
                        Fee = feeSetup.Fee,
                        IsLSDorUSD = feeSetup.isLSDorUSD

                    };

                    iview.FeeLoanSetupsView.Add(_FeeLoanSetup);
                }
            }
        }


        public static PerTransactionConfigurationFee ConvertToPerTransactionConfigurationFee(this PerTransactionConfigurationFeeView iview, IEnumerable<LKServiceFeeCategory> serviceFees)
        {

            PerTransactionConfigurationFee model = new PerTransactionConfigurationFee();
            iview.ConvertToPerTransactionConfigurationFee(model, serviceFees);
            return model;
        }
        public static void ConvertToPerTransactionConfigurationFee(this PerTransactionConfigurationFeeView iview, PerTransactionConfigurationFee model, IEnumerable<LKServiceFeeCategory> serviceFees)
        {
            model.Name = iview.Name;
            model.AllowPostpaidIfClientIsPostpaid = iview.AllowPostpaidIfClientIsPostpaid;
            model.FlatAmount = iview.Amount;
            model.PerTransactionOrReoccurence = iview.PerTransactionOrReoccurence;
            model.UseFlatAmountForAllCurrencies = iview.UseFlatAmountForAllCurrencies;
        
          
            //Add new servicefeeCategory
            if (iview.ServiceFeeCategory != null && iview.ServiceFeeCategory.Length > 0)
            {
                foreach (var intSubtypes in iview.ServiceFeeCategory)
                {
                    if (model.ServiceFeeType.Where(s => (int)s.Id == intSubtypes).SingleOrDefault() == null)
                    {
                        model.ServiceFeeType.Add(serviceFees.Where(s => (int)s.Id == intSubtypes).Single());

                    }
                }

            }

            //Remove Service Fee Categories that have been unchecked
            if (model.ServiceFeeType.Count > 0)
            {

                ICollection<LKServiceFeeCategory> removeServiceFee = new List<LKServiceFeeCategory>();

                foreach (var _item in model.ServiceFeeType)
                {
                    if (!(iview.ServiceFeeCategory.Contains((int)_item.Id)))
                    {
                        removeServiceFee.Add(_item);
                    }
                }

                foreach (var _item2 in removeServiceFee)
                {
                    model.ServiceFeeType.Remove(_item2);
                }

            }
        }



        public static FeeLoanSetup ConvertToFeeLoanSetup(this FeeLoanSetupView FeeLoanSetupView)
        {
            FeeLoanSetup model = new FeeLoanSetup();
            FeeLoanSetupView.ConvertToFeeLoanSetup(model);

            return model;
        }


        public static void ConvertToFeeLoanSetup(this FeeLoanSetupView FeeLoanSetupView, FeeLoanSetup _feeLoanSetupView)
        {
            _feeLoanSetupView.Fee = FeeLoanSetupView.Fee;
            _feeLoanSetupView.LoanAmountLimit = FeeLoanSetupView.LoanAmountLimit;
            _feeLoanSetupView.isLSDorUSD = (short)FeeLoanSetupView.IsLSDorUSD;
        }

        public static ICollection<FeeLoanSetup> ConvertToFeeLoanSetups(
                                          this ICollection<FeeLoanSetupView> FeeLoanSetupViews)
        {
            ICollection<FeeLoanSetup> feeLoans = new List<FeeLoanSetup>();
            foreach (FeeLoanSetupView fee in FeeLoanSetupViews)
            {
                feeLoans.Add(fee.ConvertToFeeLoanSetup());
            }

            return feeLoans;
        }



        public static ICollection<FeeLoanSetup> ModifyFeeLoanSetups(
                                         this ICollection<FeeLoanSetupView> FeeLoanSetupViews, ICollection<FeeLoanSetup> FeeLoanSetups, AuditingTracker auditTracker = null)
        {

            foreach (FeeLoanSetupView c in FeeLoanSetupViews)
            {

                if (c.Id == 0)
                {
                    FeeLoanSetup _addedFeeLoan = c.ConvertToFeeLoanSetup();
                    FeeLoanSetups.Add(_addedFeeLoan);
                    auditTracker.Created.Add(_addedFeeLoan);
                }
                else
                {

                    FeeLoanSetup modifiedFeeLoanSetup = FeeLoanSetups.Where(s => s.Id == ((ServiceFees)c.Id)).Single();
                    c.ConvertToFeeLoanSetup(modifiedFeeLoanSetup);
                }
                }



            

            //Get the ids of the collaterals
            int[] retainedFeeSetups = FeeLoanSetupViews.Where(s => s.Id > 0).Select(s => s.Id).ToArray();

            //Now some collaterals will no longer be in the view because they were deleted but they are in the submitted so we need to take them off
            foreach (FeeLoanSetup fee in FeeLoanSetups.Where(s => s.Id > 0 && !retainedFeeSetups.Contains((int)s.Id)))  //**All existing particpants will be updated, we need to change this behaviour
            {
                fee.IsDeleted = true; //Will not be included when authorised
                auditTracker.Updated.Add(fee);

            }

            return FeeLoanSetups;
        }


        public static PeriodicConfigurationFeeView ConvertToPeriodicConfigurationFeeView(this PeriodicConfigurationFee model)
        {
            PeriodicConfigurationFeeView iview = new PeriodicConfigurationFeeView();
            model.ConvertToPeriodicConfigurationFeeView(iview);
            return iview;
        }


        public static void ConvertToPeriodicConfigurationFeeView(this PeriodicConfigurationFee model, PeriodicConfigurationFeeView iview)
        {
            iview.Id = model.Id;
            iview.Amount = model.FlatAmount;
            iview.AllowPostpaidIfClientIsPostpaid = model.AllowPostpaidIfClientIsPostpaid;
            iview.Name = model.Name;
            iview.PerTransactionOrReoccurence = model.PerTransactionOrReoccurence;
            iview.ReoccurencePeriod = model.ReoccurencePeriod;
            iview.IsActive = model.IsActive;
            iview.ServiceFeeCategory = model.ServiceFeeType.Select(s => (int)s.Id).ToArray();
        }

        public static PeriodicConfigurationFee ConvertToPeriodicFeeConfiguration(this PeriodicConfigurationFeeView iview, IEnumerable<LKServiceFeeCategory> serviceFees)
        {

            PeriodicConfigurationFee model = new PeriodicConfigurationFee();
            iview.ConvertToPeriodicFeeConfiguration(model, serviceFees);
            return model;
        }

        public static void ConvertToPeriodicFeeConfiguration(this PeriodicConfigurationFeeView iview, PeriodicConfigurationFee model, IEnumerable<LKServiceFeeCategory> serviceFees)
        {
            model.Name = iview.Name;
            model.AllowPostpaidIfClientIsPostpaid = iview.AllowPostpaidIfClientIsPostpaid;
            model.FlatAmount = iview.Amount;
            model.PerTransactionOrReoccurence = iview.PerTransactionOrReoccurence;
            model.ReoccurencePeriod = model.ReoccurencePeriod;
  


            //Add new servicefeeCategory
            if (iview.ServiceFeeCategory != null && iview.ServiceFeeCategory.Length > 0)
            {
                foreach (var intSubtypes in iview.ServiceFeeCategory)
                {
                    if (model.ServiceFeeType.Where(s => (int)s.Id == intSubtypes).SingleOrDefault() == null)
                    {
                        model.ServiceFeeType.Add(serviceFees.Where(s => (int)s.Id == intSubtypes).Single());

                    }
                }

            }



            //Remove Service Fee Categories that have been unchecked
            if (model.ServiceFeeType.Count > 0)
            {

                ICollection<LKServiceFeeCategory> removeServiceFee = new List<LKServiceFeeCategory>();

                foreach (var _item in model.ServiceFeeType)
                {
                    if (!(iview.ServiceFeeCategory.Contains((int)_item.Id)))
                    {
                        removeServiceFee.Add(_item);
                    }
                }

                foreach (var _item2 in removeServiceFee)
                {
                    model.ServiceFeeType.Remove(_item2);
                }

            }

            

            
        }


        public static void ConvertToConfigurationView(this ConfigurationFee model, ConfigurationFeeView iview)
        {
            iview.Id = model.Id;
            iview.Amount = model.FlatAmount;
            iview.AllowPostpaidIfClientIsPostpaid = model.AllowPostpaidIfClientIsPostpaid;
            iview.Name = model.Name;
            iview.PerTransactionOrReoccurence = model.PerTransactionOrReoccurence;
            iview.ServiceFeeCategory = model.ServiceFeeType.Select(s => (int)s.Id).ToArray();
            iview.Id = model.Id;
            iview.IsActive = model.IsActive;
            iview.UseFlatAmountForAllCurrencies = model.UseFlatAmountForAllCurrencies;

        }



        public static ICollection<ConfigurationFeeView> ConvertToConfigurationFeesView(this IEnumerable<ConfigurationFee> ConfigurationFee)
        {
            ICollection<ConfigurationFeeView> ConfigurationFeeView = new List<ConfigurationFeeView>();
            foreach (var configfee in ConfigurationFee)
            {
                ConfigurationFeeView _configurationFeeView;
                if (configfee is PerTransactionConfigurationFee)
                {

                    _configurationFeeView = new PerTransactionConfigurationFeeView();
                    configfee.ConvertToConfigurationView(_configurationFeeView);
                    ((PerTransactionConfigurationFee)configfee).ConvertToPerTransactionConfigurationFeeView((PerTransactionConfigurationFeeView)_configurationFeeView);


                }
                else
                {
                    _configurationFeeView = new PeriodicConfigurationFeeView();
                    configfee.ConvertToConfigurationView(_configurationFeeView);
                    ((PeriodicConfigurationFee)configfee).ConvertToPeriodicConfigurationFeeView((PeriodicConfigurationFeeView)_configurationFeeView);
                }
                ConfigurationFeeView.Add(_configurationFeeView);
            }

            return ConfigurationFeeView;
        }



        public static void ConvertToConfigurationFee(this ConfigurationFeeView iview, ConfigurationFee model, IEnumerable<LKServiceFeeCategory> serviceFees, AuditingTracker auditTracker = null)
        {
            model.Name = iview.Name;
            model.AllowPostpaidIfClientIsPostpaid = iview.AllowPostpaidIfClientIsPostpaid;
            model.FlatAmount = iview.Amount;
            model.PerTransactionOrReoccurence = iview.PerTransactionOrReoccurence;
            model.UseFlatAmountForAllCurrencies = iview.UseFlatAmountForAllCurrencies;

            if (iview is PerTransactionConfigurationFeeView)
            {

                ((PerTransactionConfigurationFeeView)iview).FeeLoanSetupsView.ModifyFeeLoanSetups(((PerTransactionConfigurationFee)model).FeeLoanSetups, auditTracker);
            
            }



            //Add new servicefeeCategory
            if (iview.ServiceFeeCategory != null && iview.ServiceFeeCategory.Length > 0)
            {
                foreach (var intSubtypes in iview.ServiceFeeCategory)
                {
                    if (model.ServiceFeeType.Where(s => (int)s.Id == intSubtypes).SingleOrDefault() == null)
                    {
                        model.ServiceFeeType.Add(serviceFees.Where(s => (int)s.Id == intSubtypes).Single());

                    }
                }

            }



            //Remove Service Fee Categories that have been unchecked
            if (model.ServiceFeeType.Count > 0)
            {

                ICollection<LKServiceFeeCategory> removeServiceFee = new List<LKServiceFeeCategory>();

                foreach (var _item in model.ServiceFeeType)
                {
                    if (!(iview.ServiceFeeCategory.Contains((int)_item.Id)))
                    {
                        removeServiceFee.Add(_item);
                    }
                }

                foreach (var _item2 in removeServiceFee)
                {
                    model.ServiceFeeType.Remove(_item2);
                }

            }
        }

        public static PerTransactionConfigurationFee ConvertToConfigurationFee(this ConfigurationFeeView iview, IEnumerable<LKServiceFeeCategory> serviceFees, AuditingTracker auditTracker = null)
        {

            PerTransactionConfigurationFee model = new PerTransactionConfigurationFee();
            iview.ConvertToConfigurationFee(model, serviceFees);
            return model;
        }

        public static ICollection<ConfigurationFee> ConvertToConfigurationFees(
                                             this ICollection<ConfigurationFeeView> ConfigurationFeeViews, IEnumerable<LKServiceFeeCategory> cd, AuditingTracker audittracker = null)
        {
            ICollection<ConfigurationFee> ConfigurationFees = new List<ConfigurationFee>();
            foreach (var config in ConfigurationFeeViews)
            {
                //**Do the internal conversion here
                ConfigurationFee _configurationFee;


                if (config is PerTransactionConfigurationFeeView)
                {

                    _configurationFee = new PerTransactionConfigurationFee();
                    config.ConvertToConfigurationFee(_configurationFee, cd);
                    ((PerTransactionConfigurationFeeView)config).ConvertToPerTransactionConfigurationFee((PerTransactionConfigurationFee)_configurationFee, cd);
                    ConfigurationFees.Add(_configurationFee);
                }
                else if (config is PerTransactionConfigurationFeeView)
                {
                    _configurationFee = new PeriodicConfigurationFee();
                    config.ConvertToConfigurationFee(_configurationFee, cd);
                    ((PeriodicConfigurationFeeView)config).ConvertToPeriodicFeeConfiguration((PeriodicConfigurationFee)_configurationFee, cd);
                    ConfigurationFees.Add(_configurationFee);
                    // collaterals.Add(c.ConvertToCollateral(cd));

                }


            }
            return ConfigurationFees;
        }

        public static TransactionPaymentSetup ConvertToTransactionConfiguration(this ConfigurationTransactionFeesView iView, TransactionPaymentSetup model, int LenderType = 0, bool IsUpdateMode = false)
        {
            if (!IsUpdateMode)
            {
                model.ServiceTypeId = iView.ServiceTypeId;
            }
            model.LenderType = LenderType;
            model.Fee = iView.Fee;
            return model;
        }



























        //public static PerTransactionConfigurationFee ConvertToInstitutionPerTransactionConfigurationFee(this PerTransactionConfigurationFeeView iview, IEnumerable<LKServiceFeeCategory> ServiceFeeCategories)
        //{

        //    PerTransactionConfigurationFee model = new PerTransactionConfigurationFee();
        //    iview.ConvertToInstitutionPerTransactionConfigurationFee(model, ServiceFeeCategories);
        //    return model;
        //}

        //public static void ConvertToInstitutionPerTransactionConfigurationFee(this PerTransactionConfigurationFeeView iview, PerTransactionConfigurationFee model, IEnumerable<LKServiceFeeCategory> ServiceFeeCategories, AuditingTracker auditTracker = null)
        //{
        //    //Determine the type of MembershipRegistration this is
        //    model.Name = iview.Name;
        //    model.Amount = iview.Amount;
        //    model.AllowPostpaidIfClientIsPostpaid = iview.AllowPostpaidIfClientIsPostpaid;
        //    model.PerTransactionOrReoccurence = iview.PerTransactionOrReoccurence;
        //    model.UseLoanAmountFilter = iview.UseLoanAmountFilter;



        //    //Add new items
        //    if (iview.OtherIdentifications != null)
        //    {
        //        foreach (var _item in iview.OtherIdentifications.Where(s => s.OtherIdentificationId == 0))
        //        {
        //            PersonIdentification _identity = new PersonIdentification();
        //            _identity.Identification.CardNo = _item.CardNo;
        //            _identity.Identification.OtherDocumentDescription = _item.OtherDocumentDescription;
        //            _identity.Identification.FirstName = _item.FirstName;
        //            _identity.Identification.MiddleName = _item.MiddleName;
        //            _identity.Identification.Surname = _item.Surname;
        //            _identity.PersonIdentificationTypeId = _item.PersonIdentificationTypeId;
        //            _identity.UniqueCode = _item.UniqueCode;

        //            model.OtherPersonIdentifications.Add(_identity);
        //            if (auditTracker != null)
        //            {
        //                auditTracker.Created.Add(_identity);
        //            }
        //        }
        //    }


        //    ICollection<PersonIdentification> removeIdentifications = new List<PersonIdentification>();
        //    //We have some personal identifications
        //    foreach (var _item in model.OtherPersonIdentifications.Where(s => s.Id > 0))
        //    {
        //        if (iview.OtherIdentifications.Where(m => m.OtherIdentificationId == _item.Id).SingleOrDefault() == null)
        //        {

        //            removeIdentifications.Add(_item);
        //        }

        //    }

        //    foreach (var _item2 in removeIdentifications)
        //    {
        //        model.OtherPersonIdentifications.Remove(_item2);
        //    }



        //    //Add new sectors
        //    if (iview.SectorOfOperationTypes != null && iview.SectorOfOperationTypes.Length > 0)
        //    {
        //        foreach (var intSubtypes in iview.SectorOfOperationTypes)
        //        {
        //            if (model.SectorOfOperationTypes.Where(s => s.Id == intSubtypes).SingleOrDefault() == null)
        //            {
        //                model.SectorOfOperationTypes.Add(sectors.Where(s => s.Id == intSubtypes).Single());

        //            }
        //        }

        //    }

        //    //Remove sectors that have been kicked out
        //    if (model.SectorOfOperationTypes.Count > 0)
        //    {
        //        ICollection<LKSectorOfOperationCategory> removeSectors = new List<LKSectorOfOperationCategory>();
        //        //We have some personal identifications
        //        foreach (var _item in model.SectorOfOperationTypes)
        //        {
        //            if (!(iview.SectorOfOperationTypes.Contains(_item.Id)))
        //            {
        //                removeSectors.Add(_item);
        //            }
        //        }

        //        foreach (var _item2 in removeSectors)
        //        {
        //            model.SectorOfOperationTypes.Remove(_item2);
        //        }

        //    }

        //    model.Gender = iview.Gender;
        //    model.Title = iview.Title;


        //}



    }
}
