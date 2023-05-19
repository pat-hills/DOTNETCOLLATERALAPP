<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer.ascx.cs" Inherits="CRL.UI.MVC.Views.Shared.ReportViewer" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>


<form id="form1" runat="server">
    
<div>
    <asp:ScriptManager ID="scriptManager" runat="server" EnablePartialRendering="false" />
  

<rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="" SizeToReportContent="false" PageCountMode="Actual" AsyncRendering="false" InteractivityPostBackMode="AlwaysAsynchronous">
</rsweb:ReportViewer>
</div>
</form>