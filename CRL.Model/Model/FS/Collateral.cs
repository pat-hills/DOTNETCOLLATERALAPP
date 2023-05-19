using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.FS.Enums;
using CRL.Model.Infrastructure;
using CRL.Model.FS.Enums;

namespace CRL.Model.FS
{

    [Serializable]
    public partial class Collateral : AuditedEntityBaseModel<int>, IAggregateRoot
    {
       
      
        public string Description { get; set; }
        public string SerialNo { get; set; }
        public bool IsDischarged { get; set; }
        public string CollateralNo { get; set; }

        //Relationship fields
        public int? ClonedId { get; set; }
        public int CollateralSubTypeId { get; set; }
         
        public virtual LKCollateralSubTypeCategory CollateralSubTypeType { get; set; }
        public AssetCategory AssetTypeId { get; set; }
        public LKAssetCategory AssetType { get; set; }
        public int FinancialStatementId { get; set; }
        public virtual FinancialStatement FinancialStatement { get; set; }
        /// <summary>
        /// Represents the associated dischargeid if the collateral is dicharge
        /// </summary>
        public int? DischargeActivityId { get; set; }
        /// <summary>
        /// Represents the associated discharge object if the collateral is discharged
        /// </summary>
        public virtual ActivityDischarge DischargeActivity { get; set; }
        public System.Nullable<DateTime> AuthorizedDate { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }

        public Collateral Duplicate(FinancialStatement fs=null)
        {
            Collateral collateral = new Collateral
            {
                
                AssetTypeId = this.AssetTypeId,
                CollateralSubTypeId = this.CollateralSubTypeId,
                CollateralNo = this.CollateralNo ,
                DischargeActivityId = this.DischargeActivityId,
                 ClonedId = this.Id ,
                FinancialStatement =fs,                
                IsActive = this.IsActive,
                SerialNo = this.SerialNo,
                IsDischarged = this.IsDischarged,
                Description = this.Description,
                CreatedBy = this.CreatedBy,
                UpdatedBy = this.UpdatedBy,
                CreatedOn = this.CreatedOn,
                IsDeleted = this.IsDeleted,
                UpdatedOn = this.UpdatedOn

            };

            return collateral;

        }
    }
    [Serializable]
    public partial class LKCollateralCategory : EntityBase<CollateralCategory>, IAggregateRoot
    {
        [MaxLength(50)]
        public string CollateralCategoryName { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public partial class LKAssetCategory : EntityBase<AssetCategory>, IAggregateRoot
    {
        [MaxLength(50)]
        public string AssetCategoryName { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public partial class LKCollateralSubTypeCategory : EntityBase<int>, IAggregateRoot
    {
        [MaxLength(50)]
        public string CollateralSubTypeCategoryName { get; set; }
        public string SerialNumberName { get; set; }
        public ICollection<Collateral> Collaterals { get; set; }
        public int SortId { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }


    public static class CollateralExtensions
    {

        public static void MakeInActive(this ICollection<Collateral> collaterals)
        {
            foreach (Collateral c in collaterals)
            {
                c.IsActive = false;
            }
        }

        public static void AuditUpdate(this ICollection<Collateral> collaterals, int UserId, DateTime Updatedon)
        {
            foreach (Collateral c in collaterals)
            {
                c.UpdatedBy = UserId;
                c.UpdatedOn = Updatedon;
            }
        }

        public static void AuditUpdate(this ICollection<Collateral> collaterals, int UserId, DateTime Updatedon, int[] c)
        {
            
            foreach (int i in c)
            {
                Collateral cv = collaterals.Where(s => s.Id == i).Single();
                cv.UpdatedBy = UserId;
                cv.UpdatedOn = Updatedon;

            }
          
        }



     
    }
}
