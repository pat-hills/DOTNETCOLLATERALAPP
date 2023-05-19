using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Infrastructure.Messaging;
using CRL.Model.Model.FS;

namespace CRL.Service.BusinessServices
{
    public static class MessageGenerator
    {
        public static AuditAction AuditActionAmendment(RequestMode requestMode, FinancialStatementActivity fsActivity, string RequestUrl, string UserIp, string Msg)
        {


            AuditAction? auditAction = null;
            if (requestMode == RequestMode.Submit)
            {
                Msg = "Request No: " + fsActivity.RequestNo;
                if (fsActivity is ActivityUpdate)
                {
                    auditAction = AuditAction.SubmitUpdateFS;
                }
                else if (fsActivity is ActivityDischarge)
                {
                    auditAction = AuditAction.SubmitFullDischargeFS;

                    //if (((ActivityDischarge)fsActivity).DischargeType == 1)
                    //{
                    //    auditAction = AuditAction.SubmitFullDischargeFS;
                    //}
                    //else
                    //{
                    //    auditAction = AuditAction.SubmitPartialDischargeFS;
                    //}

                }
                else if (fsActivity is ActivitySubordination)
                {
                    auditAction = AuditAction.SubmitSubordinationFS;
                }
                else if (fsActivity is ActivityAssignment)
                {
                    auditAction = AuditAction.SubmitFullAssignmentFS;
                    //if (((ActivityAssignment)fsActivity).AssignmentType == 1)
                    //{
                    //    auditAction = AuditAction.SubmitFullAssignmentFS;
                    //}
                    //else
                    //{
                    //    auditAction = AuditAction.SubmitPartialAssignmentFS;
                    //}

                }
                else if (fsActivity is ActivityDischargeDueToError)
                {
                    auditAction = AuditAction.SubmitFullDischargeFSDueToError;

                    //if (((ActivityAssignment)fsActivity).AssignmentType == 1)
                    //{
                    //    auditAction = AuditAction.FullAssignmentFS;
                    //}
                    //else
                    //{
                    //    auditAction = AuditAction.PartialAssignmentFS;
                    //}

                }
            }
            else if (requestMode == RequestMode.Resend)
            {
                Msg = "Request No: " + fsActivity.RequestNo;
                if (fsActivity is ActivityUpdate)
                {
                    auditAction = AuditAction.ResubmitUpdateS;
                }

            }
            else if (requestMode == RequestMode.Approval)
            {
                Msg = "Activity No: " + fsActivity.ActivityCode;
                if (fsActivity is ActivityUpdate)
                {
                    auditAction = AuditAction.AuthorizeUpdateFS;
                }
                else if (fsActivity is ActivityDischarge)
                {
                    auditAction = AuditAction.AuthorizeFullDischargeFS;

                    //if (((ActivityDischarge)fsActivity).DischargeType == 1)
                    //{
                    //    auditAction = AuditAction.AuthorizeFullDischargeFS;
                    //}
                    //else
                    //{
                    //    auditAction = AuditAction.AuthorizePartialDischargeFS;
                    //}

                }
                else if (fsActivity is ActivitySubordination)
                {
                    auditAction = AuditAction.AuthorizeSubordinationFS;
                }
                else if (fsActivity is ActivityDischargeDueToError)
                {
                    auditAction = AuditAction.AuthorizeFullDischargeFSDueToError;

                    //if (((ActivityAssignment)fsActivity).AssignmentType == 1)
                    //{
                    //    auditAction = AuditAction.FullAssignmentFS;
                    //}
                    //else
                    //{
                    //    auditAction = AuditAction.PartialAssignmentFS;
                    //}

                }
                else if (fsActivity is ActivityAssignment)
                {
                    if (((ActivityAssignment)fsActivity).AssignmentType == 1)
                    {
                        auditAction = AuditAction.AuthorizeFullAssignmentFS;
                    }
                    else
                    {
                        auditAction = AuditAction.AuthorizePartialAssignmentFS;
                    }

                }
            }
            else if (requestMode == RequestMode.Deny)
            {
                Msg = "Request No: " + fsActivity.RequestNo;
                if (fsActivity is ActivityUpdate)
                {
                    auditAction = AuditAction.DenyUpdateFS;
                }
                else if (fsActivity is ActivityDischarge)
                {
                    auditAction = AuditAction.DenyFullDischargeFS;

                    //if (((ActivityDischarge)fsActivity).DischargeType == 1)
                    //{
                    //    auditAction = AuditAction.DenyFullDischargeFS;
                    //}
                    //else
                    //{
                    //    auditAction = AuditAction.DenyPartialDischargeFS;
                    //}

                }
                else if (fsActivity is ActivitySubordination)
                {
                    auditAction = AuditAction.DenySubordinationFS;
                }
                else if (fsActivity is ActivityAssignment)
                {
                    auditAction = AuditAction.DenyFullAssignmentFS;
                    //if (((ActivityAssignment)fsActivity).AssignmentType == 1)
                    //{
                    //    auditAction = AuditAction.DenyFullAssignmentFS;
                    //}
                    //else
                    //{
                    //    auditAction = AuditAction.DenyPartialAssignmentFS;
                    //}

                }
                else if (fsActivity is ActivityDischargeDueToError)
                {
                    auditAction = AuditAction.DenyFullDischargeFSDueToError;

                    //if (((ActivityAssignment)fsActivity).AssignmentType == 1)
                    //{
                    //    auditAction = AuditAction.FullAssignmentFS;
                    //}
                    //else
                    //{
                    //    auditAction = AuditAction.PartialAssignmentFS;
                    //}

                }
            }
            else if (requestMode == RequestMode.Create)
            {
                Msg = "Activity No: " + fsActivity.ActivityCode;
                if (fsActivity is ActivityUpdate)
                {
                    auditAction = AuditAction.UpdateFS;
                }
                else if (fsActivity is ActivityDischarge)
                {
                    auditAction = AuditAction.FullDischargeFS;

                    //if (((ActivityDischarge)fsActivity).DischargeType == 1)
                    //{
                    //    auditAction = AuditAction.FullDischargeFS;
                    //}
                    //else
                    //{
                    //    auditAction = AuditAction.PartialDischargeFS;
                    //}

                }
                else if (fsActivity is ActivitySubordination)
                {
                    auditAction = AuditAction.SubordinateFS;
                }
                else if (fsActivity is ActivityAssignment)
                {
                    auditAction = AuditAction.FullAssignmentFS;

                    //if (((ActivityAssignment)fsActivity).AssignmentType == 1)
                    //{
                    //    auditAction = AuditAction.FullAssignmentFS;
                    //}
                    //else
                    //{
                    //    auditAction = AuditAction.PartialAssignmentFS;
                    //}

                }
                else if (fsActivity is ActivityDischargeDueToError)
                {
                    auditAction = AuditAction.FullDischargeFSDueToError;

                    //if (((ActivityAssignment)fsActivity).AssignmentType == 1)
                    //{
                    //    auditAction = AuditAction.FullAssignmentFS;
                    //}
                    //else
                    //{
                    //    auditAction = AuditAction.PartialAssignmentFS;
                    //}

                }
            }

            return (AuditAction)auditAction;



        }
    }
}
