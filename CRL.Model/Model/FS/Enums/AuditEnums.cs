﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.FS.Enums
{
    public enum AuditCategory { FinancingStatement = 1, Amendment = 2, Search = 3, Workflow=4, Membership=5, Configuration=6,Payment=7, Security=8 };
    
    public enum AuditAction { 
        SubmitFS=1, 
        ResubmitFS=2, DenyFS=3,
        AuthorizeFS=4, 
        RegisterFS=5,
        ViewFSs=6,
        ViewFSDetail=7
            , DownloadFSAttachment=8,
    DownloadFSVerification=9,
    SubmitUpdateFS=10, 
        ResubmitUpdateS=11,
        DenyUpdateFS=12, 
        AuthorizeUpdateFS=13, 
        UpdateFS=14,
    SubmitFullDischargeFS=15,
        DenyFullDischargeFS=16,
        AuthorizeFullDischargeFS=17,
        FullDischargeFS=18, 
        SubmitPartialDischargeFS=19, 
        DenyPartialDischargeFS=20, 
        AuthorizePartialDischargeFS=21
            ,PartialDischargeFS=22,
    SubmitSubordinationFS = 23, 
        DenySubordinationFS = 24, 
        AuthorizeSubordinationFS = 25, 
        SubordinateFS = 26,
        SubmitFullAssignmentFS=27, 
        DenyFullAssignmentFS=28, 
        AuthorizeFullAssignmentFS=29,
        FullAssignmentFS=30, 
        SubmitPartialAssignmentFS=31, 
        DenyPartialAssignmentFS=32, 
        AuthorizePartialAssignmentFS=33          ,
        PartialAssignmentFS=34,
        ReceivedRegisteredClientPayment = 36,
        ReceivedUnRegisteredClientPayment =37, 
        DownloadPaymentReceipt=38,
       SubmittedClientRegistrationIndi =39,
SubmittedClientRegistrationLegal=40,
        VerifiedClientRegistrationIndi=41,
VerifiedClientRegistrationLegal=42,
CreatedClientLegal=43,
ModifiedClientLegal=44,
ActivatedClientLegal=45,
DeactivatedClientLegal=46,
DeletedClientLegal=47,
CreatedClientIndi=48,
ModifiedClientIndi=49,
ActivatedClientIndi=50,
DeactivatedIndi=51,
DeletedClientIndi=52,
CreatedUnit=53,
ModifiedUnit=54,
ActivatedUnit=55,
DeactivatedUnit=56,
DeletedUnit=57,
CreatedUser = 58,
ModifiedUser= 59,
ModifiedUserRoles = 60,
ActivatedUser = 61,
DeactivatedUser = 62,
DeletedUser = 63,
DeactivateUserPaypoint=64,
ActivateUserPaypoint = 65,
SubmitPasswordReset=66,
ChangePasswordFromReset=67,
        SubmitPostpaidRequest=68,
        DenyPostpaidRequest=69,
        AuthorizePostpaidRequest=70,
        RevokePostpaidRequest = 71,//----------------- reserve for login,  
        LoginFailure=72,
        LoginPasswordFailure=73,
        LoginDeactivatedUser=74,
        LoginFailureDeletedUser=75,
        LoginFailureUnApprovedUser = 76,
        LoginFailureUnApprovedMembership=77,
        LoginSuccess=78,
        LogoutSuccess=79,
        LegalEffectiveSearch=80,
        FlexibleEffectiveSearch = 81,
        GenerateCertifiedSearchResult =82,
        GenerateUnCertifiedSearchResult=83,
        HandledTask=84,
        ObtainedExistingCertifiedSearch=85,
        DownloadExistingCertifiedSearchResult = 86,
        DownloadExistingUnCertifiedSearchResult = 87,
        ForcedChangePassword=89 ,
        ReversedClientPayment=90,
        ReversedUnregisteredClientPayment=91,
        GeneratedBatch=92,
        ReconciledBatch=93,
        UploadFSBatch=94,
        SubmitOrCreatedFSBatch=95,
        DeleteBatch=96,      
        AuthorizeLegalEntity=97,
        DenyLegalEntity = 98,
        CreateMessage = 100,
        SaveDraft=99,
        SubmitPayPointUsers = 101,
        LoginUserLockedOut=102,
        GenerateConfirmationReport=103,
        GenerateActivityReport=104,
        SubmitFullDischargeFSDueToError = 105,
        DenyFullDischargeFSDueToError = 106,
        AuthorizeFullDischargeFSDueToError = 107,
        FullDischargeFSDueToError = 108, 
        SearchUsingCACLink = 109
    };
    
}
