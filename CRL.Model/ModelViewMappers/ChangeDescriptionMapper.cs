using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.FS;
using CRL.Model.ModelViews.Amendment;


namespace CRL.Model.ModelViewMappers
{
    public static class ChangeDescriptionMapper
    {
        public static IEnumerable<ChangeDescriptionView> ConvertToChangeDescriptionView(
                                               this IEnumerable<ChangeDescription> ChangeDescription)
        {
            ICollection <ChangeDescriptionView> descriptions = new List<ChangeDescriptionView>();
            foreach (var description in ChangeDescription)
            {
                ChangeDescriptionView view = new ChangeDescriptionView
                {
                    Operation = description.Operation,
                    Object = description.Object,
                    AfterUpdate = description.AfterUpdate,
                    BeforeUpdate = description.BeforeUpdate
                };
                descriptions.Add(view);

            }
            return descriptions;
        }

       
    }
}
