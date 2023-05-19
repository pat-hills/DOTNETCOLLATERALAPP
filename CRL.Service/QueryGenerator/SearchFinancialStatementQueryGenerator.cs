using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using LinqKit;
using CRL.Model.FS.IRepository;
using CRL.Model.Messaging;

namespace CRL.Service.QueryGenerator
{
    public static class SearchFinancialStatementQueryGenerator
    {
        public static string[] CreateQueryForNonLegalEffect(
           SearchRequest request, IFinancialStatementRepository _rpFS)
        {
            string BorrowerFlexibleName="";
            if (!String.IsNullOrWhiteSpace(request.SearchParam.BorrowerName))
            {
                BorrowerFlexibleName = Util.GetFlexibleBorrowerNameForSearch(request.SearchParam.BorrowerName);
            }
             var outer = PredicateBuilder.False<Model.FS.FinancialStatement>();
             DateTime? StartDate = null;
             DateTime? EndDate = null;
             if (request.SearchParam.DebtorDateOfBirth != null)
             {
                 StartDate = request.SearchParam.DebtorDateOfBirth.StartDate;
                 EndDate = request.SearchParam.DebtorDateOfBirth.EndDate;
             }
             var param = request.SearchParam;
                var inner = PredicateBuilder.True<Model.FS.FinancialStatement>();

                //IQueryable<Model.FS.FinancialStatement> query = _rpFS.GetDbSetComplete();   
                if (param.SearchType == 1)
                {
                  
                    if (!(String.IsNullOrWhiteSpace(param.BorrowerFirstName) && String.IsNullOrWhiteSpace(param.BorrowerLastName)
                        && String.IsNullOrWhiteSpace(param.BorrowerIDNo) && String.IsNullOrWhiteSpace(param.DebtorEmail) && param.DebtorDateOfBirth == null))
                    {
                        inner = inner.And(s => s.Participants.OfType<IndividualParticipant>().Any(t => t.ParticipationTypeId == ParticipationCategory.AsBorrower && ((
                         ((String.IsNullOrEmpty(param.BorrowerIDNo) || param.BorrowerIDNo == t.Identification.CardNo.TrimEnd()))
                         && ((String.IsNullOrEmpty(param.BorrowerFirstName) || param.BorrowerFirstName.Trim().ToLower() == t.Identification.FirstName.TrimEnd().ToLower()))
                         && ((String.IsNullOrEmpty(param.BorrowerMiddleName) || param.BorrowerMiddleName.Trim().ToLower() == t.Identification.MiddleName.TrimEnd().ToLower()))
                         && ((String.IsNullOrEmpty(param.BorrowerLastName) || param.BorrowerLastName.Trim().ToLower() == t.Identification.Surname.TrimEnd().ToLower())
                         )
                       
                         ) || (t.OtherPersonIdentifications.Any(u =>
                              ((String.IsNullOrEmpty(param.BorrowerIDNo) || param.BorrowerIDNo == u.Identification.CardNo.TrimEnd()))
                                && ((String.IsNullOrEmpty(param.BorrowerFirstName) || param.BorrowerFirstName.Trim().ToLower() == u.Identification.FirstName.TrimEnd().ToLower()))
                         && ((String.IsNullOrEmpty(param.BorrowerMiddleName) || param.BorrowerMiddleName.Trim().ToLower() == u.Identification.MiddleName.TrimEnd().ToLower()))
                         && ((String.IsNullOrEmpty(param.BorrowerLastName) || param.BorrowerLastName.Trim().ToLower() == u.Identification.Surname.TrimEnd().ToLower()))
                              )

                         ) 
                         )
                         && 
                        ((String.IsNullOrEmpty(param.DebtorEmail ) || param.DebtorEmail  == t.Address.Email.TrimEnd()))
                        &&
                        (StartDate  == null || (t.DOB >= StartDate && t.DOB < EndDate ))
                         
                         ));
                    }

                }
                else
                {
                    if (!(String.IsNullOrWhiteSpace(param.BorrowerName) && String.IsNullOrWhiteSpace(param.BorrowerIDNo) && String.IsNullOrWhiteSpace(param.DebtorEmail)))
                    {
                        inner = inner.And(s => s.Participants.OfType<InstitutionParticipant>().Any(t => t.ParticipationTypeId == ParticipationCategory.AsBorrower &&
                       ((String.IsNullOrEmpty(param.BorrowerIDNo) || param.BorrowerIDNo == t.CompanyNo.TrimEnd())) &&
                       ((String.IsNullOrEmpty(param.DebtorEmail ) || param.DebtorEmail  == t.Address .Email.TrimEnd()))
                       && ((String.IsNullOrEmpty(param.BorrowerName) || t.SearchableName == BorrowerFlexibleName))));
                    }
                }

                if (!(String.IsNullOrEmpty(param.CollateralSerialNo) && String.IsNullOrWhiteSpace(param.CollateralDescription )))
                {
                    inner = inner.And(s => s.Collaterals.Any(c => 
                         ((String.IsNullOrEmpty(param.CollateralSerialNo) || param.CollateralSerialNo  == c.SerialNo.TrimEnd())) &&
                     ((String.IsNullOrEmpty(param.CollateralDescription) || c.Description.ToLower().Contains(param.CollateralDescription .ToLower ().Trim ()) ))               

                        ));

                }
            //m.Address.Email.ToLower().StartsWith(param2.DebtorEmail.ToLower().Trim())
                inner = inner.And(s => s.isApprovedOrDenied ==1 && s.IsDeleted == false && s.ClonedId == null);  //for now is active is true but ti should not be
                outer = outer.Or(inner.Expand());
                var query = _rpFS.GetDbSet().AsExpandable().Where(outer);
                var query_array = query.ToArray().Select(s => s.RegistrationNo).Distinct();
                return query_array.ToArray();
        }
        public static string[] CreateQueryFor(
            SearchRequest request, IFinancialStatementRepository _rpFS)
        {

            string BorrowerFlexibleName = "";
            if (!String.IsNullOrWhiteSpace(request.SearchParam.BorrowerName))
            {
                BorrowerFlexibleName = Util.GetFlexibleBorrowerNameForSearch(request.SearchParam.BorrowerName);
            }
            //Note we only need where the collateral is active and not discharged
            //FS should be active and not discharge
            //FS should not be terminated
            //Borrower and Lender should be active

            //HAVE MANY VERSIONS OF THE VIEW CLASS AND SHOULD BE CONFIGURABLE

            //first search is for using or so that we get or between collateral and serial
            //that is both by collateral and serial
            //then we try to make sure that they have the same registration o

            var outer = PredicateBuilder.False<Model.FS.FinancialStatement>();

               var param = request.SearchParam;
                var inner = PredicateBuilder.True<Model.FS.FinancialStatement>();

                //IQueryable<Model.FS.FinancialStatement> query = _rpFS.GetDbSetComplete();   
                if (param.SearchType == 1)
                {
                    //if ((!String.IsNullOrWhiteSpace(param.BorrowerFirstName) && !String.IsNullOrWhiteSpace(param.BorrowerLastName)))
                    //{
                    //    inner = inner.And(s => s.Participants.OfType<IndividualParticipant>().Any(t => t.ParticipationTypeId == ParticipationCategory.AsBorrower &&
                    //   ((String.IsNullOrEmpty(param.BorrowerIDNo) || param.BorrowerIDNo == t.Identification.CardNo.TrimEnd()))
                    //   && ( param.BorrowerFirstName.Trim().ToLower() == t.Identification.FirstName.TrimEnd().ToLower())
                    //   && (param.BorrowerMiddleName.Trim().ToLower() == t.Identification.MiddleName.TrimEnd().ToLower())
                    //   && (param.BorrowerLastName.Trim().ToLower() == t.Identification.Surname.TrimEnd().ToLower())
                    //   ));
                    //}
                    //else 
                    if (!(String.IsNullOrWhiteSpace(param.BorrowerFirstName) && String.IsNullOrWhiteSpace(param.BorrowerLastName)
                        && String.IsNullOrWhiteSpace(param.BorrowerIDNo)))
                    {
                        inner = inner.And(s => s.Participants.OfType<IndividualParticipant>().Any(t => t.ParticipationTypeId == ParticipationCategory.AsBorrower && ((
                         ((String.IsNullOrEmpty(param.BorrowerIDNo) || param.BorrowerIDNo == t.Identification.CardNo.TrimEnd()))
                         && ((String.IsNullOrEmpty(param.BorrowerFirstName) || param.BorrowerFirstName.Trim().ToLower() == t.Identification.FirstName.TrimEnd().ToLower()))
                         && ((String.IsNullOrEmpty(param.BorrowerMiddleName) || param.BorrowerMiddleName.Trim().ToLower() == t.Identification.MiddleName.TrimEnd().ToLower()))
                         && ((String.IsNullOrEmpty(param.BorrowerLastName) || param.BorrowerLastName.Trim().ToLower() == t.Identification.Surname.TrimEnd().ToLower()))
                       
                         ) || (t.OtherPersonIdentifications.Any(u =>
                              ((String.IsNullOrEmpty(param.BorrowerIDNo) || param.BorrowerIDNo == u.Identification.CardNo.TrimEnd()))
                                && ((String.IsNullOrEmpty(param.BorrowerFirstName) || param.BorrowerFirstName.Trim().ToLower() == u.Identification.FirstName.TrimEnd().ToLower()))
                         && ((String.IsNullOrEmpty(param.BorrowerMiddleName) || param.BorrowerMiddleName.Trim().ToLower() == u.Identification.MiddleName.TrimEnd().ToLower()))
                         && ((String.IsNullOrEmpty(param.BorrowerLastName) || param.BorrowerLastName.Trim().ToLower() == u.Identification.Surname.TrimEnd().ToLower()))
                              )

                         ))));
                    }

                }
                else
                {
                    if (!(String.IsNullOrWhiteSpace(param.BorrowerName) && String.IsNullOrWhiteSpace(param.BorrowerIDNo)))
                    {
                        inner = inner.And(s => s.Participants.OfType<InstitutionParticipant>().Any(t => t.ParticipationTypeId == ParticipationCategory.AsBorrower &&
                       ((String.IsNullOrEmpty(param.BorrowerIDNo) || param.BorrowerIDNo == t.CompanyNo.TrimEnd()))

                       && ((String.IsNullOrEmpty(param.BorrowerName) || t.SearchableName == BorrowerFlexibleName))));
                    }
                }

                if (!(String.IsNullOrEmpty(param.CollateralSerialNo)))
                {
                    inner = inner.And(s => s.Collaterals.Any(c => c.SerialNo == param.CollateralSerialNo 
                       

                        ));

                }
                inner = inner.And(s => s.isApprovedOrDenied == 1 && s.IsDeleted == false && s.ClonedId == null);  //for now is active is true but ti should not be
                outer = outer.Or(inner.Expand());
            var query = _rpFS.GetDbSet().AsExpandable().Where(outer);
            SearchParam param2 = request.SearchParam;           
          

            //After we get disntinct then we need need to check that if both borrower and collateral were entered
            //then we need to make sure that we filter out those registra
            var query_array = query.ToArray().Select(s => s.RegistrationNo).Distinct().ToArray();
            return query_array;


       


        }

    }
}

//query = query.Where(s => s.Participants.OfType<IndividualParticipant>().Where(w => w.ParticipationTypeId == ParticipationCategory.AsBorrower
//    && (String.IsNullOrEmpty(request.BorrowerParam.BorrowerName) || w.FirstName + ' '+ w.MiddleName + ' ' + w.Surname == request.BorrowerParam.BorrowerName)
//    && (String.IsNullOrEmpty(request.BorrowerParam.BorrowerIDNo) ||  w.IdentificationCard.Where (sd=>sd.CardNo == request.BorrowerParam.BorrowerName).Count()>0)
//    && (request.BorrowerParam.BorrowerIDType == 0 || w.IdentificationCard.Where(sd => sd.PersonIdentificationCardTypeId == request.BorrowerParam.BorrowerIDType).Count() > 0)).Count() > 0
//    || 
//    s.Participants.OfType<InstitutionParticipant>().Where(w => w.ParticipationTypeId == ParticipationCategory.AsBorrower &&
//       (String.IsNullOrEmpty(request.BorrowerParam.BorrowerName) || w.Name  == request.BorrowerParam.BorrowerName)
//   ).Count()>0 );    
//Now we need to handle when we specify the ID type
