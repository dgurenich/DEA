<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<%@ Register src="FileUploader.ascx" tagname="FileUploader" tagprefix="uc1" %>
<%@ Register Src="CSVContent.ascx" TagPrefix="uc1" TagName="CSVContent" %>
<%@ Register Src="DEAOutput.ascx" TagPrefix="uc1" TagName="DEAOutput" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="styles.css" rel="stylesheet" />
    <div>
        <h3><strong>Multi-Objective Optimization (MOO) with Data Envelopment Analysys (DEA)</strong></h3>
        <p>&nbsp;</p>
        <p>
            <uc1:FileUploader ID="FileUploader1" runat="server" />
            <uc1:CSVContent runat="server" ID="CSVContent1" />
            <uc1:DEAOutput runat="server" ID="DEAOutput1" />
        </p>
    </div>
    
</asp:Content>

