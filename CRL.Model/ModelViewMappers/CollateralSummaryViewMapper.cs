using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Model.FS;
using CRL.Model.ModelViews.FinancingStatement;


namespace CRL.Model.ModelViewMappers
{
    public static class CollateralSummaryViewMapper
    {
        public static CollateralSummaryView ConvertToCollateralSummaryView(this Collateral Collateral)
        {
            CollateralSummaryView cv = new CollateralSummaryView();
            //cv.CollateralTypeName = Collateral.CollateralType.CollateralCategoryName;
            cv.SerialNo  = Collateral.SerialNo ;
            cv.Description  = Collateral.Description ;
            cv.CollateralSubTypeName = Collateral.CollateralSubTypeType.CollateralSubTypeCategoryName ;
            //cv.CollateralTypeId =(int)Collateral.CollateralTypeId;
            cv.Id = (int)Collateral.Id ;
            
            return cv; 


        }

        public static IEnumerable<CollateralSummaryView> ConvertToCollateralsSummaryView(this IEnumerable<Collateral> Collaterals)
        {
            ICollection<CollateralSummaryView> fsviews = new List<CollateralSummaryView>();
            foreach (var s in Collaterals)
            {
                fsviews.Add(s.ConvertToCollateralSummaryView());
            }

            return fsviews;

        }
    }
}
