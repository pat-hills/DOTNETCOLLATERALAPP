using CRL.Infrastructure.Domain;
using CRL.Model.FS;
using CRL.Model.ModelViews.FinancingStatement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.FS.Enums;

namespace CRL.Model.ModelViewMappers
{
    public static class CollateralViewExtension
    {
     public static Collateral ConvertToNewCollateral(this CollateralView view)
        {

            Collateral model = new Collateral();
            model.State = RecordState.New;
            view.ConvertToNewCollateral(model);
            return model;
        }
     public static void ConvertToNewCollateral(this CollateralView view, Collateral model)
     {
         model.SerialNo = view.SerialNo.TrimNull();
         model.Description = view.Description.TrimNull();
         model.CollateralSubTypeId = view.CollateralSubTypeId;
         model.AssetTypeId = AssetCategory.Movable;
         model.CollateralNo = view.CollateralNo;
         

     }
     public static void ConvertToUpdatedCollateral(this CollateralView view, Collateral model)
     {
         model.SerialNo = view.SerialNo.TrimNull();
         model.Description = view.Description.TrimNull();
         model.CollateralSubTypeId = view.CollateralSubTypeId;
         model.AssetTypeId = AssetCategory.Movable;
         


     }
  

     public static ICollection<Collateral> ConvertToNewCollaterals(
                                         this ICollection<CollateralView> CollateralViews)
     {
         ICollection<Collateral> collaterals = new List<Collateral>();
         foreach (CollateralView c in CollateralViews)
         {
             collaterals.Add(c.ConvertToNewCollateral());
         }

         return collaterals;
     }

     public static ICollection<Collateral> ConvertToUpdatedCollaterals(
                                    this ICollection<CollateralView> CollateralViews, ICollection<Collateral> models)
     {
         //We need to add new collaterals using the ConvertToNewCollateral, we use id==0 for an update process and collateral no for submitted updates since they will have an id already
         //We will add them to the financing statement to be modified
         //We will also set them to new
         //Note that the time of a collateral addition is when it is added
         foreach (CollateralView c in CollateralViews.Where(s=>s.Id ==0 || String.IsNullOrWhiteSpace (s.CollateralNo )))
         {
             models.Add(c.ConvertToNewCollateral());
         }

         //Updated Collaterals are handled here
         foreach (CollateralView c in CollateralViews.Where(s => !String.IsNullOrWhiteSpace(s.CollateralNo)))
         {
             c.ConvertToUpdatedCollateral(models.Where (s=>s.CollateralNo == c.CollateralNo ).Single ());
         }

         string[] submittedCollaterals = CollateralViews.Where(s => !String.IsNullOrWhiteSpace(s.CollateralNo)).Select(s => s.CollateralNo).ToArray();
         foreach (var remove in models.Where(s=>!submittedCollaterals.Contains(s.CollateralNo )))
         {
             remove.IsDeleted = true;             
         }


         return models;

       
     }


     public static CollateralView ConvertToCollateralView(this Collateral model)
     {
         CollateralView view = new CollateralView()
             {
               AssetTypeId = model.AssetTypeId ,
                CollateralNo = model.CollateralNo ,
                CollateralSubTypeId = model .CollateralSubTypeId ,
                 CollateralSubTypeName = model.CollateralSubTypeType .CollateralSubTypeCategoryName ,
                  Description = model.Description ,
                   Id= model.Id ,
                    SerialNo= model.SerialNo ,
                     RecordState = RecordState.Original
             };

         return view;


     }

     public static List<CollateralView> ConvertToCollateralViews(
                                         this IEnumerable<Collateral> models)
     {
         List<CollateralView> views = new List<CollateralView>();
         foreach (Collateral c in models)
         {
             views.Add(c.ConvertToCollateralView());
         }
         return views;    

     }


    
    }
}
