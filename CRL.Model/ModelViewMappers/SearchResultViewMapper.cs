using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.Common;
using CRL.Model.FS;

using CRL.Model.FS.IRepository;
using CRL.Model.ModelViews;
using CRL.Model.Messaging;
using CRL.Model.FS.Enums;
using CRL.Model.Search;
using CRL.Model.ModelViews.Search;

namespace CRL.Model.ModelViewMappers
{
    public static class SearchResultViewMapper
    {
        public static ICollection<SearchResultView> ConvertToSearchResultView(this IEnumerable<FinancialStatement> model,
            SearchRequest searchrequest,IParticipantRepository _participantRepository, ICollateralRepository _collateralrepository)
        {
            //foreach financing statement
            //Check the search request
            //if any borrower request parameter is iset then for each borrower
            //check against request and if is true then tag the borrower
            //if borrower is not tagged, 
            List<SearchResultView> _searchResult = new List<SearchResultView>();
            //Map the participants
           

            foreach (FinancialStatement fs in model)
            {
                //if (searchrequest.SearchParam.BorrowerFirstName != null || searchrequest.SearchParam.BorrowerIDType != null || searchrequest.SearchParam.BorrowerName != null)
                //{

                //Get all the participants
                ICollection<Participant> debtors = fs.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsBorrower).ToList();
                ICollection<Collateral> collaterals = fs.Collaterals;
                List<SearchResultDebtor> _debtors = new List<SearchResultDebtor>();
                List<SearchResultCollateral> _collaterals = new List<SearchResultCollateral>();
                foreach (Participant p in debtors)
                {
                    SearchResultDebtor debtor = new SearchResultDebtor();
                    debtor.Id = p.Id;
                    debtor.Email = p.Address.Email;
                    if (p is IndividualParticipant)
                    {
                        //check if this participant is tagged
                        debtor.FullName = NameHelper.GetFullName(((IndividualParticipant)p).Identification.FirstName, ((IndividualParticipant)p).Identification.MiddleName, ((IndividualParticipant)p).Identification.Surname);
                        debtor.DateOfBirth = ((IndividualParticipant)p).DOB;
                        debtor.isIndividual = true;
                        debtor.IDNo = ((IndividualParticipant)p).Identification.CardNo;
                        debtor.Email = p.Address.Email;
                        if (p.IsDeleted == false)
                        {
                            debtor.SearchItemState = SearchItemState.Current;
                        }
                        else
                        {
                            debtor .SearchItemState = SearchItemState.Deleted  ;
                        
                        }
                        
                        //Check for tagging
                        if (!String.IsNullOrWhiteSpace (searchrequest.SearchParam.BorrowerFirstName ) || !String.IsNullOrWhiteSpace (searchrequest.SearchParam.BorrowerIDNo)
                            || !String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerMiddleName ) || !String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerLastName  )
                            || !String.IsNullOrWhiteSpace(searchrequest.SearchParam.DebtorEmail )||  searchrequest.SearchParam .DebtorDateOfBirth !=null )
                        {
                            CheckIndividualIdentification(((IndividualParticipant)p), searchrequest.SearchParam, debtor, debtor.SearchItemState);
                            //if (CheckIndividualIdentification(((IndividualParticipant)p), searchrequest.SearchParam, debtor) )
                            //{
                            //    //Mark as tagged
                            //    debtor.isTagged = true;
                            //    if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerFirstName) || !String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerMiddleName )
                            //        || !String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerLastName ))
                            //        debtor.isTaggedName = true;
                            //    if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerIDNo))
                            //        debtor.isTaggedId = true;
                            //    if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.DebtorEmail ))
                            //        debtor.isTaggedEmail  = true;
                            //    //if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.DebtorDateOfBirth ))
                            //    //    debtor.isTaggedId = true;
                            //    debtor.SearchItemState = SearchItemState.Current;

                            //}
                            if (!debtor .isTagged )
                            {
                                CheckIndividualOtherIdentification(((IndividualParticipant)p), searchrequest.SearchParam, debtor);

                                //foreach (PersonIdentification  info in ((IndividualParticipant)p).OtherPersonIdentifications)
                                //{
                                //    if (CheckIndividualIdentification(info.Identification , searchrequest.SearchParam)&&
                                //       ((IndividualParticipant)p).Address.Email  )
                                //    {
                                //        //Mark as tagged
                                //        debtor.isTagged = true;
                                //        if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerFirstName) || !String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerMiddleName)
                                //    || !String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerLastName))
                                //            debtor.isTaggedName = true;
                                //        if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerIDNo))
                                //            debtor.isTaggedId = true;
                                //        if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.DebtorEmail))
                                //            debtor.isTaggedEmail   = true;
                                //        debtor.SearchItemState = SearchItemState.Current;
                                //        break;

                                //    }
                                //}
                                if (!debtor.isTagged)
                                {
                                    //**we should be able to load all participants at a go from the start before doing this
                                    List<Participant> debtorhistory = _participantRepository.GetDbSet().Where(s => s.ParticipantNo == p.ParticipantNo && s.Id != p.Id && s.FinancialStatement.isApprovedOrDenied ==1 &&  s.FinancialStatement.IsActive == true  && s.IsDeleted ==false && s.FinancialStatement .IsDeleted ==false).ToList();
                                    foreach (var item in debtorhistory )
                                    {
                                        CheckIndividualIdentification(((IndividualParticipant)item), searchrequest.SearchParam, debtor, SearchItemState.Updated);
                                    //    {
                                    //        //Mark as tagged
                                    //        debtor.isTagged = true;
                                    //        if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerFirstName) || !String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerMiddleName)
                                    //|| !String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerLastName))
                                    //            debtor.isTaggedName = true;
                                    //        if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerIDNo))
                                    //            debtor.isTaggedId = true;
                                    //        if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.DebtorEmail))
                                    //            debtor.isTaggedEmail = true;
                                    //        debtor.SearchItemState = SearchItemState.Updated ;

                                    //    }
                                        if (!debtor.isTagged )
                                        {

                                            CheckIndividualOtherIdentification(((IndividualParticipant)item), searchrequest.SearchParam, debtor, SearchItemState.Updated);
                                        }
                                    
                                    }

                                }
                            }
                        }

                        //Check if there were other debtors deleted with the same name in the history or maybe changed name
                    }
                    else
                    {
                        debtor.FullName = ((InstitutionParticipant)p).Name;
                        debtor.IDNo = ((InstitutionParticipant)p).BusinessRegistrationPrefix .Name +  ((InstitutionParticipant)p).CompanyNo;
                        debtor.isIndividual = false;
                        if (p.IsDeleted == false)
                        {
                            debtor.SearchItemState = SearchItemState.Current;
                        }
                        else
                        {
                            debtor.SearchItemState = SearchItemState.Deleted;

                        }
                        if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerName) || !String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerIDNo)
                            || !String.IsNullOrWhiteSpace(searchrequest.SearchParam.DebtorEmail ))
                        {
                            if ((String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerName) || ((InstitutionParticipant)p).SearchableName .Trim().ToLower() == searchrequest.SearchParam.BorrowerName.Trim().ToLower())
                                  &&
                                        
                                (String.IsNullOrWhiteSpace(searchrequest.SearchParam.DebtorEmail) || ((InstitutionParticipant)p).Address .Email.Trim().ToLower() == searchrequest.SearchParam.DebtorEmail.Trim().ToLower())
                                        
                                         &&
                                (String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerIDNo) || ((InstitutionParticipant)p).CompanyNo.Trim().ToLower() == searchrequest.SearchParam.BorrowerIDNo.Trim().ToLower()))
                            {
                                debtor.isTagged = true;
                                if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerName))
                                    debtor.isTaggedName = true;
                                if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerIDNo))
                                    debtor.isTaggedId = true;
                                if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.DebtorEmail))
                                    debtor.isTaggedEmail  = true;
                                
                            }
                            else
                            {
                                //Check history
                                List<Participant> debtorhistory = _participantRepository.GetDbSet().Where(s => s.ParticipantNo == p.ParticipantNo && s.Id != p.Id && s.FinancialStatement.isApprovedOrDenied == 1 && s.FinancialStatement.IsActive == true && s.IsDeleted == false && s.FinancialStatement.IsDeleted == false).ToList();
                                foreach (var item in debtorhistory)
                                {
                                    if ((String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerName) || ((InstitutionParticipant)item).SearchableName.Trim().ToLower() == searchrequest.SearchParam.BorrowerName.Trim().ToLower())
                                  &&
                                (String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerIDNo) || ((InstitutionParticipant)item).CompanyNo.Trim().ToLower() == searchrequest.SearchParam.BorrowerIDNo.Trim().ToLower())
                                          &&
                                (String.IsNullOrWhiteSpace(searchrequest.SearchParam.DebtorEmail) || ((InstitutionParticipant)item).Address .Email.Trim().ToLower() == searchrequest.SearchParam.DebtorEmail.Trim().ToLower())
                                        
                                        )
                                    {
                                        debtor.isTagged = true;
                                        if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerName))
                                            debtor.isTaggedName = true;
                                        if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.BorrowerIDNo))
                                            debtor.isTaggedId = true;
                                        if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.DebtorEmail))
                                            debtor.isTaggedEmail = true;
                                        debtor.SearchItemState = SearchItemState.Updated;
                                    }

                                }

                            }
                        }
                    }
                    if (debtor.isTagged || p.IsDeleted == false)
                    {
                        _debtors.Add(debtor);
                    }

                    //Check if this is in any current debtor
                    //Check for in other names
                    //Check for each history and it's other names


                }

                foreach (Collateral c in collaterals)
                {
                    SearchResultCollateral collateral = new SearchResultCollateral();
                    collateral.Id = c.Id;
                    collateral.SerialNo = c.SerialNo;
                    collateral.SubTypeName = c.CollateralSubTypeType.CollateralSubTypeCategoryName;
                    collateral.Description = c.Description;
                    
                    if (c.IsDeleted == false)
                    {
                        collateral.SearchItemState = SearchItemState.Current;
                    }
                    else
                    {
                        collateral.SearchItemState = SearchItemState.Deleted;

                    }
                        
                    if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.CollateralSerialNo) || !String.IsNullOrWhiteSpace(searchrequest.SearchParam.CollateralDescription ))
                    {
                        if ((String.IsNullOrWhiteSpace(searchrequest.SearchParam.CollateralSerialNo ) || c.SerialNo  == searchrequest.SearchParam.CollateralSerialNo )
                            &&
                            (String.IsNullOrWhiteSpace(searchrequest.SearchParam.CollateralDescription ) || c.Description.ToLower().Contains (searchrequest.SearchParam.CollateralDescription .ToLower().Trim ())))
                        {
                            collateral.isTagged = true;
                            if (!String.IsNullOrWhiteSpace(searchrequest .SearchParam .CollateralSerialNo ))
                                collateral.isTaggedSerialNo  = true;
                            if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.CollateralDescription))
                                collateral.istaggedDescription  = true;
                          //  collateral.SearchItemState = SearchItemState.Current ;

                                 //c.Description.ToLower().Contains(param.CollateralDescription .ToLower ().Trim ())
                        }
                        else
                        {
                            //Check history
                            List<Collateral> collateralhistory = _collateralrepository.GetDbSet().Where(s => s.CollateralNo == c.CollateralNo && s.Id != c.Id && s.FinancialStatement.isApprovedOrDenied == 1 && s.FinancialStatement.IsActive == true && s.IsDeleted == false && s.FinancialStatement.IsDeleted == false).ToList();
                            foreach (var item in collateralhistory)
                            {
                                if ((String.IsNullOrWhiteSpace(searchrequest.SearchParam.CollateralSerialNo) || item.SerialNo == searchrequest.SearchParam.CollateralSerialNo)
                                     && (String.IsNullOrWhiteSpace(searchrequest.SearchParam.CollateralDescription) || c.Description.ToLower().Contains(searchrequest.SearchParam.CollateralDescription.ToLower().Trim())))
                                {
                                    collateral.isTagged = true;
                                    if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.CollateralSerialNo))
                                        collateral.isTaggedSerialNo = true;
                                    if (!String.IsNullOrWhiteSpace(searchrequest.SearchParam.CollateralDescription))
                                        collateral.istaggedDescription = true;
                                    collateral.SearchItemState = SearchItemState.Updated;
                                }

                            }

                        }
                    }
                    if (collateral.isTagged || ( c.IsDeleted == false && c.IsDischarged ==false ))
                    {
                        _collaterals.Add(collateral);
                    }
                   
                }


                SearchResultView resultView = new SearchResultView();
                resultView.Id = fs.Id;
                resultView.RegistrationNo = fs.RegistrationNo;
                resultView.Collaterals = _collaterals.ToArray();
                resultView.Debtors = _debtors.ToArray();
                resultView.Status = fs.IsDischarged == true ? "Cancelled" : fs.SystemExpired != null || fs.ExpiryDate < DateTime.Now.Date ? "Expired" : "Active";
                
                _searchResult.Add(resultView);

            }

            return _searchResult;
        }

        public static void CheckIndividualOtherIdentification(IndividualParticipant info, SearchParam param, SearchResultDebtor debtor, SearchItemState itemState = SearchItemState.Current)
        {

            foreach (var p in info.OtherPersonIdentifications)
            {
                bool result = ((String.IsNullOrWhiteSpace(param.BorrowerFirstName) || p.Identification.FirstName.Trim().ToLower() == param.BorrowerFirstName.Trim().ToLower())
                                      &&
                                      (String.IsNullOrWhiteSpace(param.BorrowerMiddleName) || p.Identification.MiddleName.Trim().ToLower() == param.BorrowerMiddleName.Trim().ToLower())
                                      &&
                                      (String.IsNullOrWhiteSpace(param.BorrowerLastName) || p.Identification.Surname.Trim().ToLower() == param.BorrowerLastName.Trim().ToLower())
                                      &&
                                      (String.IsNullOrWhiteSpace(param.BorrowerIDNo) || p.Identification.CardNo.Trim().ToLower() == param.BorrowerIDNo.Trim().ToLower()
                                      ) &&
                                     (String.IsNullOrWhiteSpace(param.DebtorEmail) || info.Address.Email.Trim().ToLower() == param.DebtorEmail .Trim().ToLower())
                                     && ((param.DebtorDateOfBirth == null) || (info.DOB >= param.DebtorDateOfBirth.StartDate && info.DOB < param.DebtorDateOfBirth.EndDate)));
                if (result)
                {
                    //Mark as tagged
                    debtor.isTagged = true;
                    if (!String.IsNullOrWhiteSpace(param.BorrowerFirstName) || !String.IsNullOrWhiteSpace(param.BorrowerMiddleName)
                || !String.IsNullOrWhiteSpace(param.BorrowerLastName))
                        debtor.isTaggedName = true;
                    if (!String.IsNullOrWhiteSpace(param.BorrowerIDNo))
                        debtor.isTaggedId = true;
                    if (!String.IsNullOrWhiteSpace(param.DebtorEmail))
                        debtor.isTaggedEmail = true;
                    if (param.DebtorDateOfBirth != null)
                        debtor.isTaggedDOB = true;
                    debtor.SearchItemState = itemState ;
              

                }

            }
           
                                  
        }

        public static void CheckIndividualIdentification(IndividualParticipant info, SearchParam param, SearchResultDebtor debtor, SearchItemState itemState = SearchItemState.Current)
        {
            bool result = ((String.IsNullOrWhiteSpace(param.BorrowerFirstName) || info.Identification .FirstName.Trim().ToLower() == param.BorrowerFirstName.Trim().ToLower())
                                  &&
                                  (String.IsNullOrWhiteSpace(param.BorrowerMiddleName) || info.Identification.MiddleName.Trim().ToLower() == param.BorrowerMiddleName.Trim().ToLower())
                                  &&
                                  (String.IsNullOrWhiteSpace(param.BorrowerLastName) || info.Identification.Surname.Trim().ToLower() == param.BorrowerLastName.Trim().ToLower())
                                  &&
                                  (String.IsNullOrWhiteSpace(param.BorrowerIDNo) || info.Identification.CardNo.Trim().ToLower() == param.BorrowerIDNo.Trim().ToLower()
                                  )&&
                                  (String.IsNullOrWhiteSpace(param.DebtorEmail) || info.Address.Email.Trim().ToLower() == param.DebtorEmail.Trim().ToLower()) 
                                  && ((param.DebtorDateOfBirth == null) || (info.DOB >= param.DebtorDateOfBirth.StartDate && info.DOB < param.DebtorDateOfBirth.EndDate)));
            if (result)
            {
                //Mark as tagged
                debtor.isTagged = true;
                if (!String.IsNullOrWhiteSpace(param.BorrowerFirstName) || !String.IsNullOrWhiteSpace(param.BorrowerMiddleName)
                    || !String.IsNullOrWhiteSpace(param.BorrowerLastName))
                    debtor.isTaggedName = true;
                if (!String.IsNullOrWhiteSpace(param.BorrowerIDNo))
                    debtor.isTaggedId = true;
                if (!String.IsNullOrWhiteSpace(param.DebtorEmail))
                    debtor.isTaggedEmail = true;
                if (param.DebtorDateOfBirth != null)
                    debtor.isTaggedDOB  = true;
                debtor.SearchItemState = itemState;
            }
        }

        public static bool CheckIndividualIdentification(PersonIdentificationInfo info, SearchParam param)
        {
            return (String.IsNullOrWhiteSpace(param.BorrowerFirstName) || info.FirstName.Trim().ToLower() == param.BorrowerFirstName.Trim().ToLower())
                                   &&
                                   (String.IsNullOrWhiteSpace(param.BorrowerMiddleName) || info.MiddleName.Trim().ToLower() == param.BorrowerMiddleName.Trim().ToLower())
                                   &&
                                   (String.IsNullOrWhiteSpace(param.BorrowerLastName) || info.Surname.Trim().ToLower() == param.BorrowerLastName.Trim().ToLower())
                                   &&
                                   (String.IsNullOrWhiteSpace(param.BorrowerIDNo) || info.CardNo.Trim().ToLower() == param.BorrowerIDNo.Trim().ToLower()
                                   );
           
        }




    }
}
