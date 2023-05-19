using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Domain;
using CRL.Model.FS.Enums;
using CRL.Infrastructure.Configuration;

namespace CRL.Model.FS
{
    [Serializable]
     public struct ChangeDescription
    {
        
        public string Operation;
        public string Object;
        public string BeforeUpdate;
        public string AfterUpdate;
    }
    
    [Serializable]
    public partial class ActivityUpdate:FinancialStatementActivity
    {
        //Reference to the finacialstatement before and after amendement
        public string UpdateXMLDescription { get; set; }
     


        public new ActivityUpdate Duplicate()
        {
            ActivityUpdate activity = new ActivityUpdate();
            activity.UpdateXMLDescription = this.UpdateXMLDescription ;
            activity.PreviousFinancialStatementId = this.PreviousFinancialStatementId;
            return activity;
        }

        //Operations

        string GenerateXML(List<ChangeDescription> changeStruct)  //Again this should be in the service class and we have to use an interface for the cloning and also for the serialising
        {
            var serializer = new XmlSerializer(typeof(List<ChangeDescription>));
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, changeStruct);
                stringWriter.Flush();
                return stringWriter.ToString();
            }

           
            
        }

         public List<ChangeDescription>   GetOperationDescription(string xmlString)  //Again this should be in the service class and we have to use an interface for the cloning and also for the serialising
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<ChangeDescription>));
            StringReader reader = new StringReader(xmlString);
            object obj = deserializer.Deserialize(reader);
            List<ChangeDescription> XmlData = (List<ChangeDescription>)obj;
            reader.Close();
            return XmlData;



        }
         public string[] GetChangeDescriptionAsArray()
         {
             List<string> changeDescription= new List<string> ();
             List<ChangeDescription> description = GetOperationDescription(UpdateXMLDescription);
             foreach (var s in description)
             {
                 string d = "";
                 d = d + s.Operation + " " + s.Object;
                 if (!String .IsNullOrWhiteSpace (s.BeforeUpdate))
                 {
                     d = d  + " from " + s.BeforeUpdate;
                 }
                 if (!String.IsNullOrWhiteSpace(s.AfterUpdate))
                 {
                     d = d + " to " + s.AfterUpdate;
                 }


                 changeDescription.Add (d);
             }

             return changeDescription .ToArray ();
         }

         public void CreateOperationDescription(AuditingTracker trakcer, LookUpForFS lookUp, bool isSubmission = false)
        {
            //Loop through the changed and current and find the difference
            //All the others will have the active set to false except the current one
            //We will later put this in a new class
            //Make sure that the two are set properly
            FinancialStatement newFs =null;
            
            FinancialStatement oldFs = null;
            if (isSubmission )
            {
                oldFs = this.FinancialStatement  ;
                newFs = this.PreviousFinancialStatement;
            }
            else
            {
                newFs = this.FinancialStatement;
                oldFs = this.PreviousFinancialStatement;
            }
           
            
            List<ChangeDescription> lc = new List<ChangeDescription>();
            string ObjectName;
            
          


            ObjectName = "Loan Details";
            string oldCurrency = lookUp.Currencies.FirstOrDefault(s => s.LkValue == oldFs.MaximumAmountSecuredCurrencyId).LkName;
            string newCurrency = lookUp.Currencies.FirstOrDefault(s => s.LkValue == newFs.MaximumAmountSecuredCurrencyId).LkName;
            //Checking for newFs change operations
            if (newFs.MaximumAmountSecuredCurrencyId != oldFs.MaximumAmountSecuredCurrencyId)
                lc.Add(new ChangeDescription { Operation = "Changed maximum amount currency for " + ObjectName .ToLower(), Object = ObjectName, BeforeUpdate = oldCurrency, AfterUpdate = newCurrency});
            //We need to check if there was a change at all in any information and we need to also check if effective date was changed too
            if (newFs.MaximumAmountSecured != oldFs.MaximumAmountSecured)
            {
                lc.Add(new ChangeDescription { Operation = "Changed maximum amount for " + ObjectName.ToLower(), Object = ObjectName, BeforeUpdate = oldFs.MaximumAmountSecured.ToString(), AfterUpdate = newFs.MaximumAmountSecured.ToString() });
            }
            
            if (newFs.SecurityAgreementDate != oldFs.SecurityAgreementDate)
            {
                lc.Add(new ChangeDescription { Operation = "Changed loan due date for " + ObjectName.ToLower(), Object = ObjectName, BeforeUpdate = oldFs.SecurityAgreementDate.ToString(), AfterUpdate = newFs.SecurityAgreementDate.ToString() });
            }
            if (newFs.CollateralTypeId  != oldFs.CollateralTypeId)
            {
                string newValue = lookUp.CollateralTypes.Where(s => s.LkValue == (int)newFs.CollateralTypeId).Single().LkName;
                string oldValue = lookUp.CollateralTypes.Where(s => s.LkValue == (int)oldFs .CollateralTypeId ).Single().LkName;
                lc.Add(new ChangeDescription { Operation = "Changed collateral type for " + ObjectName.ToLower(), Object = ObjectName, BeforeUpdate = oldValue, AfterUpdate = newValue });
            }

            if (newFs.ExpiryDate  != oldFs.ExpiryDate )
            {
                lc.Add(new ChangeDescription { Operation = "Changed expiry date for " + ObjectName.ToLower(), Object = ObjectName, BeforeUpdate = oldFs.ExpiryDate.ToString(), AfterUpdate = newFs.ExpiryDate.ToString() });
            }
            //if (newFs.FinancialStatementAttachmentId != oldFs.FinancialStatementAttachmentId)
            //{
            //    if (oldFs.FinancialStatementAttachmentId== null && (newFs.FinancialStatementAttachmentId !=null || newFs .FinancialStatementAttachment !=null))
            //    {
            //        lc.Add(new ChangeDescription { Operation = "Added file attachment", Object = ObjectName });
            //    }
            //    else if (oldFs.FinancialStatementAttachmentId != null && (newFs.FinancialStatementAttachmentId != null || newFs.FinancialStatementAttachment != null))
            //    {
            //        lc.Add(new ChangeDescription { Operation = "Changed file attachment", Object = ObjectName });
            //    }
            //    else if (oldFs.FinancialStatementAttachmentId != null && (newFs.FinancialStatementAttachmentId == null || newFs.FinancialStatementAttachment == null))
            //    {
            //        lc.Add(new ChangeDescription { Operation = "Removed file attachment", Object = ObjectName });
            //    }
            //    else
            //    {
            //        throw new Exception("We should never get here");
            //    }
               
            //}

            if (newFs.FinancialStatementLoanTypeId != oldFs.FinancialStatementLoanTypeId)
            {
                string newValue = lookUp.FinancialStatementLoanType.Where(s => s.LkValue == (int)newFs.FinancialStatementLoanTypeId).Single().LkName;
                string oldValue = lookUp.FinancialStatementLoanType.Where(s => s.LkValue == (int)oldFs.FinancialStatementLoanTypeId).Single().LkName;
                lc.Add(new ChangeDescription { Operation = "Changed loan type " + ObjectName.ToLower(), Object = ObjectName, BeforeUpdate = oldValue, AfterUpdate = newValue });
            }

            //Now we need to check for newly added collaterals:  These items have a null collateral no or their collateral no not included with the other list
            ObjectName = "File Attachment";
            int[] existingFileAttachmentId = oldFs.FileAttachments.Where(s => s.IsDeleted == false).Select(s => s.Id ).ToArray();
            int[] newFileAttachmentId =      newFs.FileAttachments.Where(qq=>qq.IsDeleted == false).Select(s => s.Id).ToArray();
            foreach (var attachment in newFs.FileAttachments.Where(qq => !existingFileAttachmentId.Contains(qq.Id ))) //Where all collno has been generated then we check where the no is not in the old one
            {
                //We will use a collateral number
                lc.Add(new ChangeDescription { Object = "File Attachment", Operation = "Added file attachment "  +attachment.AttachedFileName, BeforeUpdate = "", AfterUpdate = "" });
            }

            //Now we need to check for removed collaterals
            foreach (var attachment in oldFs.FileAttachments.Where(qq => !newFileAttachmentId.Contains(qq.Id))) //Here the isdeleted is set to true 
            {
                //We will use a collateral number
                if (newFs.FileAttachments.Where(qq => qq.Id == attachment.Id).Count() == 1)
                    lc.Add(new ChangeDescription { Object = "File Attachment", Operation = "Removed file attachment " + attachment.AttachedFileName, BeforeUpdate = "", AfterUpdate = "" });
            }


        
            //Now we need to check for newly added collaterals:  These items have a null collateral no or their collateral no not included with the other list
            ObjectName = "Collateral";
            string[] existingCollateralNo = oldFs.Collaterals.Where (s=>s.IsDeleted ==false).Select(s => s.CollateralNo ).ToArray();
            string[] newCollateralNo = newFs.Collaterals.Where(qq => !String.IsNullOrWhiteSpace(qq.CollateralNo) & qq.IsDeleted ==false).Select(s => s.CollateralNo).ToArray();
            foreach (var collateral in newFs.Collaterals.Where(qq => String.IsNullOrWhiteSpace(qq.CollateralNo) || !existingCollateralNo.Contains(qq.CollateralNo))) //Where all collno has been generated then we check where the no is not in the old one
            {
                //We will use a collateral number
                lc.Add(new ChangeDescription { Operation = "Added collateral", BeforeUpdate = "", AfterUpdate = "" });
            }

            //Now we need to check for removed collaterals
            foreach (var collateral in oldFs.Collaterals.Where(qq => !newCollateralNo.Contains(qq.CollateralNo))) //Here the isdeleted is set to true 
            {
                //We will use a collateral number
                if (newFs.Collaterals.Where(qq => qq.CollateralNo == collateral.CollateralNo).Count() == 1)
                    lc.Add(new ChangeDescription { Operation = "Removed collateral", BeforeUpdate = "", AfterUpdate = "" });
            }

            //Modified collateral values
            foreach (var newCollateral in newFs.Collaterals.Where(qq => existingCollateralNo.Contains(qq.CollateralNo) && qq.IsDeleted ==false))
            {

                Collateral oldCollateral = oldFs.Collaterals.Where(s => s.CollateralNo == newCollateral.CollateralNo).Single();
                newCollateral.CreatedOn = oldCollateral.CreatedOn;
                newCollateral.CreatedBy = oldCollateral.CreatedBy;
                newCollateral.IsActive = oldCollateral.IsActive;
                newCollateral.IsDeleted = oldCollateral.IsDeleted;
                newCollateral.IsDischarged = oldCollateral.IsDischarged;
                newCollateral.AuthorizedDate = oldCollateral.AuthorizedDate;
                newCollateral.DischargeActivityId = oldCollateral.DischargeActivityId;
                //We need to check if there was a change at all in any information and we need to also check if effective date was changed too
                if (newCollateral.Description != oldCollateral.Description)
                {
                    lc.Add(new ChangeDescription { Operation = "Changed collateral description", BeforeUpdate = oldCollateral.Description, AfterUpdate = newCollateral.Description });
                    trakcer.Updated.Add(newCollateral);
                }
                //We need to check if there was a change at all in any information and we need to also check if effective date was changed too
                if (newCollateral.SerialNo != oldCollateral.SerialNo)
                {
                    lc.Add(new ChangeDescription { Operation = "Changed collateral serial no", BeforeUpdate = oldCollateral.SerialNo, AfterUpdate = newCollateral.SerialNo });
                    trakcer.Updated.Add(newCollateral);
                }

                //We need to check if there was a change at all in any information and we need to also check if effective date was changed too
                if (newCollateral.CollateralSubTypeId != oldCollateral.CollateralSubTypeId)
                {
                    string newValue = lookUp.CollateralSubTypes.Where(s => s.LkValue == newCollateral.CollateralSubTypeId).Single().LkName;
                    string oldValue = lookUp.CollateralSubTypes.Where(s => s.LkValue == oldCollateral.CollateralSubTypeId).Single().LkName;
                    lc.Add(new ChangeDescription { Operation = "Changed collateral subtype", BeforeUpdate = oldValue, AfterUpdate = newValue });
                    trakcer.Updated.Add(newCollateral);
                }



            }

            int indx_borrower=0;
            int indx_lender = 0;
            int indx = 0;

            string[] existingParticipantNo = oldFs.Participants.Where (s=>s.IsDeleted == false).Select(s => s.ParticipantNo).ToArray();
            string[] newParticipantNo = newFs.Participants.Where(qq => !String.IsNullOrWhiteSpace(qq.ParticipantNo)&& qq.IsDeleted ==false).Select(s => s.ParticipantNo).ToArray();

            //Checking for modified participants
            foreach (var newParticipant in newFs.Participants.Where(qq => existingParticipantNo.Contains(qq.ParticipantNo)&& qq.IsDeleted ==false))
            {



                if (newParticipant.ParticipationTypeId == Enums.ParticipationCategory.AsBorrower)
                { ObjectName = "Debtor"; indx_borrower++; indx = indx_borrower; }
                else
                {

                    ObjectName = "Secured party"; indx_lender++; indx = indx_lender;
                }
                string newNameOfParticipant = "";

                if (newParticipant is IndividualParticipant)
                {
                    IndividualParticipant oldIndividuaParticipant = (IndividualParticipant)oldFs.Participants.Where(s => s.ParticipantNo == newParticipant.ParticipantNo).Single();

                    IndividualParticipant newIndividualParticipant = (IndividualParticipant)newParticipant;
                    newNameOfParticipant = NameHelper.GetFullName(newIndividualParticipant.Identification.FirstName, newIndividualParticipant.Identification.MiddleName, newIndividualParticipant.Identification.Surname);

                    if (newIndividualParticipant.Identification.FirstName != oldIndividuaParticipant.Identification.FirstName)
                    {
                        lc.Add(new ChangeDescription { Operation = "Changed firstname for " + ObjectName .ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldIndividuaParticipant.Identification.FirstName, AfterUpdate = newIndividualParticipant.Identification.FirstName });
                        trakcer.Updated.Add(newParticipant);
                    }
                    if (newIndividualParticipant.Identification.MiddleName != oldIndividuaParticipant.Identification.MiddleName)
                    {
                        lc.Add(new ChangeDescription { Operation = "Changed middle name for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldIndividuaParticipant.Identification.MiddleName, AfterUpdate = newIndividualParticipant.Identification.MiddleName });
                        trakcer.Updated.Add(newParticipant);
                    }
                    if (newIndividualParticipant.Identification.Surname != oldIndividuaParticipant.Identification.Surname)
                    {
                        lc.Add(new ChangeDescription { Operation = "Changed surname for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldIndividuaParticipant.Identification.Surname, AfterUpdate = newIndividualParticipant.Identification.Surname });
                        trakcer.Updated.Add(newParticipant);
                    }
                    if (newIndividualParticipant.Identification.CardNo != oldIndividuaParticipant.Identification.CardNo)
                    {
                        lc.Add(new ChangeDescription { Operation = "Changed BVN no for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldIndividuaParticipant.Identification.CardNo, AfterUpdate = newIndividualParticipant.Identification.CardNo });
                        trakcer.Updated.Add(newParticipant);
                    }
                    if (newIndividualParticipant.Identification.CardNo2 != oldIndividuaParticipant.Identification.CardNo2)
                    {
                        lc.Add(new ChangeDescription { Operation = "Changed passport no for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldIndividuaParticipant.Identification.CardNo2, AfterUpdate = newIndividualParticipant.Identification.CardNo2 });
                        trakcer.Updated.Add(newParticipant);
                    }
                    if (newIndividualParticipant.PersonIdentificationTypeId != oldIndividuaParticipant.PersonIdentificationTypeId)
                    {
                        string newValue = lookUp.IdentificationCardTypes.Where(s => s.LkValue == newIndividualParticipant.PersonIdentificationTypeId).Single().LkName;
                        string oldValue = lookUp.IdentificationCardTypes.Where(s => s.LkValue == oldIndividuaParticipant.PersonIdentificationTypeId).Single().LkName;
                        lc.Add(new ChangeDescription { Operation = "Changed identification card type for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldValue, AfterUpdate = newValue });
                        trakcer.Updated.Add(newParticipant);
                    }
                    if (newIndividualParticipant.OtherDocumentDescription != oldIndividuaParticipant.OtherDocumentDescription)
                    {
                        lc.Add(new ChangeDescription { Operation = "Changed other document type for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldIndividuaParticipant.OtherDocumentDescription, AfterUpdate = newIndividualParticipant.OtherDocumentDescription });
                        trakcer.Updated.Add(newParticipant);
                    }
                    if (newIndividualParticipant.DOB != oldIndividuaParticipant.DOB)
                    {
                        lc.Add(new ChangeDescription { Operation = "Changed date of birth for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldIndividuaParticipant.DOB.ToString(), AfterUpdate = newIndividualParticipant.DOB.ToString() });
                        trakcer.Updated.Add(newParticipant);
                    }
                    if (newIndividualParticipant.Gender != oldIndividuaParticipant.Gender)
                    {
                        lc.Add(new ChangeDescription { Operation = "Changed gender for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldIndividuaParticipant.Gender, AfterUpdate = newIndividualParticipant.Gender });
                        trakcer.Updated.Add(newParticipant);
                    }

                    if (newIndividualParticipant.Title != oldIndividuaParticipant.Title)
                    {
                        lc.Add(new ChangeDescription { Operation = "Changed title for for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldIndividuaParticipant.Title, AfterUpdate = newIndividualParticipant.Title });
                        trakcer.Updated.Add(newParticipant);
                    }

                    if (newIndividualParticipant.OtherPersonIdentifications != null)
                    {

                        foreach (var item in newIndividualParticipant.OtherPersonIdentifications)
                        {
                            if (item.Id == 0)
                            {
                                lc.Add(new ChangeDescription { Operation = "New identification information added", Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = "", AfterUpdate = "" });
                            }
                            else if (item.IsDeleted)
                            {

                                lc.Add(new ChangeDescription { Operation = "Identification information removed", Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = "", AfterUpdate = "" });

                            }
                            else
                            {
                                PersonIdentification olditem = oldIndividuaParticipant.OtherPersonIdentifications.Where(s => s.Id == item.Id).Single();
                                string newItemName = NameHelper.GetFullName(item.Identification.FirstName, item.Identification.MiddleName, item.Identification.Surname);
                                string oldItemName = NameHelper.GetFullName(olditem.Identification.FirstName, olditem.Identification.MiddleName, olditem.Identification.Surname);
                                if (item.Identification.FirstName != olditem.Identification.FirstName)
                                {
                                    lc.Add(new ChangeDescription { Operation = "Changed other identification firstname", Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = olditem.Identification.FirstName, AfterUpdate = item.Identification.FirstName });
                                }
                                if (item.Identification.MiddleName != olditem.Identification.MiddleName)
                                {
                                    lc.Add(new ChangeDescription { Operation = "Changed other identification middle name", Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = olditem.Identification.MiddleName, AfterUpdate = item.Identification.MiddleName });
                                }
                                if (item.Identification.Surname != olditem.Identification.Surname)
                                {
                                    lc.Add(new ChangeDescription { Operation = "Changed other identification surname", Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = olditem.Identification.Surname, AfterUpdate = item.Identification.Surname });
                                }
                                if (item.Identification.CardNo != olditem.Identification.CardNo)
                                {
                                    lc.Add(new ChangeDescription { Operation = "Changed other identification identification no", Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = olditem.Identification.CardNo, AfterUpdate = item.Identification.CardNo });
                                }
                                if (item.PersonIdentificationTypeId != olditem.PersonIdentificationTypeId)
                                {
                                    string newValue = lookUp.IdentificationCardTypes.Where(s => s.LkValue == item.PersonIdentificationTypeId).Single().LkName;
                                    string oldValue = lookUp.IdentificationCardTypes.Where(s => s.LkValue == olditem.PersonIdentificationTypeId).Single().LkName;
                                    lc.Add(new ChangeDescription { Operation = "Changed other identification identification card type", Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldValue, AfterUpdate = newValue });
                                }
                                if (item.Identification.OtherDocumentDescription != olditem.Identification.OtherDocumentDescription)
                                {
                                    lc.Add(new ChangeDescription { Operation = "Other other identification document type", Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = olditem.Identification.OtherDocumentDescription, AfterUpdate = item.Identification.OtherDocumentDescription });
                                }
                            }


                        }
                    }

                }
                else if (newParticipant is InstitutionParticipant)
                {
                    InstitutionParticipant newInstitutionParticipant = (InstitutionParticipant)newParticipant;
                    newNameOfParticipant = newInstitutionParticipant.Name;

                    InstitutionParticipant oldInstitutionParticipant = (InstitutionParticipant)oldFs.Participants.Where(s => s.ParticipantNo == newParticipant.ParticipantNo).Single();

                    if (((InstitutionParticipant)newParticipant).Name != oldInstitutionParticipant.Name)
                    {
                        lc.Add(new ChangeDescription { Operation = "Changed name for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldInstitutionParticipant.Name, AfterUpdate = ((InstitutionParticipant)newParticipant).Name });
                        trakcer.Updated.Add(newParticipant);
                    }

                    if (((InstitutionParticipant)newParticipant).CompanyNo != oldInstitutionParticipant.CompanyNo)
                    {
                        lc.Add(new ChangeDescription { Operation = "Changed business registration no for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldInstitutionParticipant.CompanyNo, AfterUpdate = ((InstitutionParticipant)newParticipant).CompanyNo });
                        trakcer.Updated.Add(newParticipant);
                    }


                    if (newParticipant.ParticipationTypeId == ParticipationCategory.AsBorrower)
                    {




                        if (((InstitutionParticipant)newParticipant).DebtorTypeId != oldInstitutionParticipant.DebtorTypeId)
                        {
                            string newValue = lookUp.DebtorTypes.Where(s => s.LkValue == (int)((InstitutionParticipant)newParticipant).DebtorTypeId).Single().LkName;
                            string oldValue = lookUp.DebtorTypes.Where(s => s.LkValue == (int)oldInstitutionParticipant.DebtorTypeId).Single().LkName;
                            lc.Add(new ChangeDescription { Operation = "Changed debtor type for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldInstitutionParticipant.DebtorTypeId.ToString(), AfterUpdate = ((InstitutionParticipant)newParticipant).DebtorTypeId.ToString() });
                            trakcer.Updated.Add(newParticipant);
                        }
                    }

                    if (newParticipant.ParticipationTypeId == ParticipationCategory.AsSecuredParty)
                    {
                        if (((InstitutionParticipant)newParticipant).SecuringPartyIndustryTypeId != oldInstitutionParticipant.SecuringPartyIndustryTypeId)
                        {
                            string newValue = lookUp.SecuringPartyIndustryTypes.Where(s => s.LkValue == (int)((InstitutionParticipant)newParticipant).SecuringPartyIndustryTypeId).Single().LkName;
                            string oldValue = lookUp.SecuringPartyIndustryTypes.Where(s => s.LkValue == (int)oldInstitutionParticipant.SecuringPartyIndustryTypeId).Single().LkName;
                            lc.Add(new ChangeDescription { Operation = "Changed secured party type for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldValue, AfterUpdate = newValue });
                            trakcer.Updated.Add(newParticipant);
                        }
                    }

                }

                Participant oldParticipant = oldFs.Participants.Where(s => s.ParticipantNo == newParticipant.ParticipantNo).Single();

                newParticipant.CreatedBy = oldParticipant.CreatedBy;
                newParticipant.CreatedOn = oldParticipant.CreatedOn;
                newParticipant.AuthorizedDate = oldParticipant.AuthorizedDate;
                newParticipant.IsActive = oldParticipant.IsActive;
                newParticipant.IsDeleted = oldParticipant.IsDeleted;
                if (newParticipant.Address.Phone != oldParticipant.Address.Phone)
                {
                    lc.Add(new ChangeDescription { Operation = "Changed phone for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldParticipant.Address.Phone, AfterUpdate = newParticipant.Address.Phone });
                    trakcer.Updated.Add(newParticipant);
                }
                if (newParticipant.Address.Email != oldParticipant.Address.Email)
                {
                    lc.Add(new ChangeDescription { Operation = "Changed email for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldParticipant.Address.Email, AfterUpdate = (newParticipant).Address.Email });
                    trakcer.Updated.Add(newParticipant);
                }
                if (newParticipant.CountryId != oldParticipant.CountryId)
                {
                    string newValue = lookUp.Countries.Where(s => s.LkValue == newParticipant.CountryId).Single().LkName;
                    string oldValue = lookUp.Countries.Where(s => s.LkValue == oldParticipant.CountryId).Single().LkName;
                    lc.Add(new ChangeDescription { Operation = "Changed country for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = newValue });
                    trakcer.Updated.Add(newParticipant);
                }
                if (newParticipant.CountyId != oldParticipant.CountyId) //** can be null
                {
                    string newValue = newParticipant.CountyId == Constants.CountyNotApplicable || newParticipant.CountyId == null? "'blank'" : lookUp.Countys.Where(s => s.LkValue == newParticipant.CountyId).Single().LkName;
                    string oldValue = oldParticipant.CountyId == Constants.CountyNotApplicable || oldParticipant.CountyId == null ? "'blank'" : lookUp.Countys.Where(s => s.LkValue == oldParticipant.CountyId).Single().LkName; 
                    
                    lc.Add(new ChangeDescription { Operation = "Changed state for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldValue, AfterUpdate = newValue });
                    trakcer.Updated.Add(newParticipant);
                }
                if (newParticipant.LGAId  != oldParticipant.LGAId ) //** can be null
                {
                    string newValue = newParticipant.LGAId == Constants.LGANotApplicable || newParticipant.LGAId == null ? "'blank'" : lookUp.LGAs.Where(s => s.LkValue == newParticipant.LGAId).Single().LkName;
                    string oldValue = oldParticipant.LGAId == Constants.LGANotApplicable || oldParticipant.LGAId == null ? "'blank'" : lookUp.LGAs.Where(s => s.LkValue == oldParticipant.LGAId).Single().LkName; 
                    
                    lc.Add(new ChangeDescription { Operation = "Changed local government area for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldValue, AfterUpdate = newValue });
                    trakcer.Updated.Add(newParticipant);
                }
                if (newParticipant.NationalityId != oldParticipant.NationalityId)
                {
                    string newValue = lookUp.Nationalities.Where(s => s.LkValue == newParticipant.NationalityId).Single().LkName;
                    string oldValue = lookUp.Nationalities.Where(s => s.LkValue == oldParticipant.NationalityId).Single().LkName;
                    lc.Add(new ChangeDescription { Operation = "Changed nationality for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldValue, AfterUpdate = newValue });
                    trakcer.Updated.Add(newParticipant);
                }

                if (newParticipant.Address.Address != oldParticipant.Address.Address)
                {
                    lc.Add(new ChangeDescription { Operation = "Changed address for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldParticipant.Address.Address, AfterUpdate = (newParticipant).Address.Address });
                    trakcer.Updated.Add(newParticipant);
                }
                if (newParticipant.Address.Address2 != oldParticipant.Address.Address2)
                {
                    lc.Add(new ChangeDescription { Operation = "Changed address2 for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldParticipant.Address.Address2, AfterUpdate = (newParticipant).Address.Address2 });
                    trakcer.Updated.Add(newParticipant);
                }
                if (newParticipant.Address.City != oldParticipant.Address.City)
                {
                    lc.Add(new ChangeDescription { Operation = "Changed city for " + ObjectName.ToLower() + " " + newNameOfParticipant, Object = ObjectName + ": " + newNameOfParticipant, BeforeUpdate = oldParticipant.Address.City, AfterUpdate = (newParticipant).Address.City });
                    trakcer.Updated.Add(newParticipant);
                }



            }

            //Now we need to check for newly added Participants
            foreach (var participant in newFs.Participants.Where(qq => String.IsNullOrWhiteSpace(qq.ParticipantNo) || !existingParticipantNo.Contains(qq.ParticipantNo)))
            {


                if (participant.ParticipationTypeId == Enums.ParticipationCategory.AsBorrower)
                { ObjectName = "Debtor"; }
                else
                {

                    ObjectName = "Secured creditor";
                }
                string newNameOfParticipant;
                if (participant is IndividualParticipant)
                {
                    newNameOfParticipant = NameHelper.GetFullName(((IndividualParticipant)participant).Identification.FirstName, ((IndividualParticipant)participant).Identification.MiddleName, ((IndividualParticipant)participant).Identification.Surname);
                }
                else
                {
                    newNameOfParticipant = ((InstitutionParticipant)participant).Name;
                }
                //We will use a collateral number
                lc.Add(new ChangeDescription { Operation = "Added " + ObjectName.ToLower(), Object = ObjectName + ": " + newNameOfParticipant });
            }

            //Now we need to check for newly added collaterals
            foreach (var participant in oldFs.Participants.Where(qq => !newParticipantNo.Contains(qq.ParticipantNo)))
            {


                if (participant.ParticipationTypeId == Enums.ParticipationCategory.AsBorrower)
                { ObjectName = "Debtor"; }
                else
                {

                    ObjectName = "Secured Creditor";
                }
                string newNameOfParticipant;
                if (participant is IndividualParticipant)
                {
                    newNameOfParticipant = NameHelper.GetFullName(((IndividualParticipant)participant).Identification.FirstName, ((IndividualParticipant)participant).Identification.MiddleName, ((IndividualParticipant)participant).Identification.Surname);
                }
                else
                {
                    newNameOfParticipant = ((InstitutionParticipant)participant).Name;
                }
                //We will use a collateral number
                lc.Add(new ChangeDescription { Operation = "Removed " +  ObjectName.ToLower(), Object = ObjectName + ": " + newNameOfParticipant });
            }



            this.UpdateXMLDescription = GenerateXML(lc);



        }

      
        

    }

 
}
