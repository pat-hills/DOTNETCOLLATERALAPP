using CRL.Model.FS;
using CRL.Model.ModelViews.FinancingStatement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Domain;
using CRL.Model.FS.Enums;
using CRL.Infrastructure.Configuration;

namespace CRL.Model.ModelViewMappers
{
    public static class ParticipantViewExtension
    {
        public static Participant ConvertToNewParticipant(this ParticipantView view)
        {

            Participant model = new Participant();
            model.State = RecordState.New;
            view.ConvertToNewParticipant(model);
            return model;
        }
        public static void ConvertToNewParticipant(this ParticipantView view, Participant model)
        {
           
            model.Address.Address = view.Address.TrimNull();
            model.LGAId = view.LGAId != Constants.LGANotApplicable && view.LGAId != null ? view.LGAId : null;
            model.Address.Address2 = view.Address2.TrimNull();
            model.Address.City = view.City.TrimNull();
            model.CountryId = view.CountryId;
            model.CountyId = view.CountyId != Constants.CountyNotApplicable && view.CountyId != null ? view.CountyId : null;
            model.NationalityId = Constants.DefaultNationality;
            model.ParticipationTypeId = view.ParticipationTypeId;
            model.ParticipantTypeId = view.ParticipantTypeId;
            model.ParticipantNo = view.ParticipantNo;
            model.RegistrantMembershipId = view.RegistrantMembershipId;
          
        
            
        }
        public static void ConvertToUpdateParticipant(this ParticipantView view, Participant model)
        {
            model.LGAId = view.LGAId != Constants.LGANotApplicable && view.LGAId != null ? view.LGAId : null;
            model.Address.Address = view.Address.TrimNull();
            model.Address.Address2 = view.Address2.TrimNull();
            model.Address.City = view.City.TrimNull();
            model.CountryId = view.CountryId;
            model.CountyId = view.CountyId != Constants.CountyNotApplicable && view.CountyId != null ? view.CountyId : null;
            model.ParticipationTypeId = view.ParticipationTypeId;
            model.ParticipantTypeId = view.ParticipantTypeId;
            model.ParticipantNo = view.ParticipantNo;
           

        }

        public static ICollection<Participant> ConvertToNewParticipants(
                                           this ICollection<ParticipantView> views, IEnumerable<LKSectorOfOperationCategory> sectors = null)
        {

            List<Participant> models = new List<Participant>();
            foreach (var view in views)
            {
                Participant newModel = null;
                if (view is IndividualDebtorView)
                {
                    newModel = ((IndividualDebtorView)view).ConvertToNewIndividualParticipant(sectors);
                   
                }
                else if (view is IndividualSPView)
                {
                    newModel = ((IndividualSPView)view).ConvertToNewIndividualParticipant();
                    
                }
                else if (view is InstitutionDebtorView )
                {
                    newModel = ((InstitutionDebtorView)view).ConvertToNewInstitutionParticipant(sectors);
                  
                }
                else if (view is InstitutionSPView )
                {
                    newModel = ((InstitutionSPView)view).ConvertToNewInstitutionParticipant();
                }
                else
                {
                    throw new Exception("Unknown participant");
                }
                
                models.Add(newModel);
            }

            return models;

        }

        public static ICollection<Participant> ConvertToUpdatedParticipants(
                                         this ICollection<ParticipantView> views,  ICollection<Participant> models, IEnumerable<LKSectorOfOperationCategory> sectors = null)
        {

           
            foreach (var view in views)
            {
                Participant newModel = null;
                if (view is IndividualDebtorView)
                {
                    newModel = ((IndividualDebtorView)view).ConvertToNewIndividualParticipant(sectors);

                }
                else if (view is IndividualSPView)
                {
                    newModel = ((IndividualSPView)view).ConvertToNewIndividualParticipant();

                }
                else if (view is InstitutionDebtorView)
                {
                    newModel = ((InstitutionDebtorView)view).ConvertToNewInstitutionParticipant(sectors);

                }
                else if (view is InstitutionSPView)
                {
                    newModel = ((InstitutionSPView)view).ConvertToNewInstitutionParticipant();
                }
                else
                {
                    throw new Exception("Unknown participant");
                }

                models.Add(newModel);
            }

            return models;

        }

        public static void ConvertToParticipantView(this Participant model, ParticipantView iview)
        {
            iview.Address = model.Address.Address;
            iview.Address2 = model.Address.Address2;
            iview.LGAId = model.LGAId;
            if (iview.LGAId.HasValue)
            {
                iview.LGA = model.LGA.Name;
                iview.CountyId = model.LGA.CountyId;
                iview.County = model.County.Name;
            }
            else
            {
                iview.LGA = "N/A";
                iview.CountyId = model.CountyId;
                iview.County = "N/A";

            }
            iview.City = model.Address.City;
            iview.CountryId = model.CountryId;
            
            
           
            iview.CountryView = model.Country.Name;
        
            //iview.Nationality = model.Nationality.Name; //**Nigeria
            iview.ParticipantTypeId = model.ParticipantTypeId;
            iview.ParticipationTypeId = model.ParticipationTypeId;
            iview.ParticipantNo = model.ParticipantNo;
            iview.Id = model.Id;
            iview.RegistrantMembershipId = model.RegistrantMembershipId;

        }


        public static List<ParticipantView> ConvertToParticipantsView(this IEnumerable<Participant> models)
        {
            List<ParticipantView> views = new List<ParticipantView>();
            foreach (var p in models)
            {
                ParticipantView _participantView;
                if (p is IndividualParticipant)
                {
                    if (p.ParticipationTypeId == ParticipationCategory.AsBorrower)
                    {
                        _participantView = new IndividualDebtorView();
                       
                        ((IndividualParticipant)p).ConvertToIndividualDebtorParticipantView((IndividualDebtorView)_participantView);



                    }
                    else
                    {
                        _participantView = new IndividualSPView();
                       
                        ((IndividualParticipant)p).ConvertToIndividualSPParticipantView((IndividualSPView)_participantView);

                    }
                }
                else
                {


                    if (p.ParticipationTypeId == ParticipationCategory.AsBorrower)
                    {
                        _participantView = new InstitutionDebtorView();
                      
                        ((InstitutionParticipant)p).ConvertToInstitutionDebtorParticipantView((InstitutionDebtorView)_participantView);


                    }
                    else
                    {
                        _participantView = new InstitutionSPView();
                      
                        ((InstitutionParticipant)p).ConvertToInstitutionSPParticipantView((InstitutionSPView)_participantView);


                    }
                }
                views.Add(_participantView);
            }

            return views;
        }
     

        

       
     
   
    }
}
