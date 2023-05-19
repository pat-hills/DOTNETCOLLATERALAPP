using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.FS;
using CRL.Service.Views.FinancialStatement;
using CRL.Model.FS;

namespace CRL.Service.Mappings.FinancialStatement
{
    public static class ParticipantSummaryViewMapper
    {
         public static IEnumerable<ParticipantSummaryView> ConvertToParticipantsSummaryView(this IEnumerable<Participant> Participants)
        {
            ICollection<ParticipantSummaryView> fsviews = new List<ParticipantSummaryView>();
            foreach (var s in Participants)
            {
                ParticipantSummaryView fsview;
              

                if (s is IndividualParticipant)
                {
                    fsview = new IndividualParticipantSummaryView();
                    fsviews.Add(((IndividualParticipant)s).ConvertToIndividualParticipantSummaryView((IndividualParticipantSummaryView)fsview));

                }
                else if (s is InstitutionParticipant)
                {
                    fsview = new InstitutionParticipantSummaryView();
                    fsviews.Add(((InstitutionParticipant)s).ConvertToInstitutionParticipantSummaryView((InstitutionParticipantSummaryView)fsview));
                }
                else
                {
                    fsview = new ParticipantSummaryView();
                    fsviews.Add(fsview);
                }

                fsview.Id = s.Id;
             
                fsview.ParticipantTypeId = (int)s.ParticipantTypeId;
                fsview.ParticipantTypeName = s.ParticipationType.ParticipationCategoryName;
                fsview.ParticipationTypeId = (int)s.ParticipationTypeId;
            }

            return fsviews;

        }
    }
}
