using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.FS;
using CRL.Model.ModelViews.FinancingStatement;


namespace CRL.Model.ModelViewMappers
{
    public static class CollateralReportViewMapper
    {
        public static CollateralReportView ConvertToCollateralReportView(this Collateral Collateral, LookUpForFS LookUps = null)
        {

            CollateralReportView iview = new CollateralReportView();
            iview.Description = Collateral.Description;
            iview.SerialNo = Collateral.SerialNo;
            iview.FinancialStatementId = Collateral.FinancialStatementId;
            iview.DischargedId = Collateral.DischargeActivityId;

            if (LookUps != null)
            {

                
                iview.CollateralSubTypeName = LookUps.CollateralSubTypes .Where(s => s.LkValue == (int)Collateral .CollateralSubTypeId ).SingleOrDefault().LkName;
            }
            else
            {
                iview.CollateralSubTypeName = Collateral.CollateralSubTypeType.CollateralSubTypeCategoryName;
            }
            return iview;


        }

        public static ICollection<CollateralReportView> ConvertToCollateralReportViews(
                                             this IEnumerable<Collateral> Collaterals, LookUpForFS LookUps = null)
        {
            ICollection<CollateralReportView> collaterals = new List<CollateralReportView>();
            foreach (Collateral c in Collaterals)
            {
                collaterals.Add(c.ConvertToCollateralReportView(LookUps));
            }

            return collaterals;

            //Now how do we map the ints id

        }

    }
}
