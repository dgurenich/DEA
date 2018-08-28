<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FileUploader.ascx.cs" Inherits="VarUploader" %>
<style type="text/css">
    .auto-style2 {
        font-size: small;
        width: 200px;
    }
    .auto-style5 {
        font-size: x-small;
    }
    .auto-style6 {
        text-align: center;
    }
    .auto-style8 {
        height: 35px;
    }
</style>
<link href="styles.css" rel="stylesheet" />
<table style="width: 100%">
    <tr>
        <td class="auto-style2" aria-checked="undefined">
            <asp:Label ID="LabelVar" runat="server" Text="Vraiables' CSV file:"></asp:Label>
        </td>
        <td>
            <asp:FileUpload ID="FileUploadVar" runat="server" Width="950px" Height="24px" ToolTip="Please select a local CSV file that contains specifications (names, types, scaling factors) for the variables." />
        </td>
        <td rowspan="6" style="vertical-align: top; width: 200px">
            <asp:Label ID="LabelError" runat="server" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="auto-style2" aria-checked="undefined" >
            <asp:Label ID="LabelProj" runat="server" Text="Projects' CSV file:"></asp:Label>
        </td>
        <td >
            <asp:FileUpload ID="FileUploadProj" runat="server" Width="950px" Height="24px" ToolTip="Please select a local CSV file that contains project specifications (project names, costs and benefits)." />
        </td>
    </tr>
    <tr>
        <td class="auto-style2" aria-checked="undefined" >
            <asp:Label ID="LabelConst" runat="server" Text="Constraints CSV file*:"></asp:Label>
        </td>
        <td>
            <asp:FileUpload ID="FileUploadConstraints" runat="server" Width="950px" Height="24px" ToolTip="If you want to not only rank the projects, but also apply constrainted selection to them please upload the CSV file with constraints here." />
        </td>
    </tr>
    <tr>
         <td class="auto-style8">
        </td>
        <td class="auto-style8">
            <asp:LinkButton ID="LinkButton1" runat="server" OnCommand="LinkButton1_Command" ToolTip="Click to upload the selected file.">Upload CSV Files</asp:LinkButton>
         </td>
       
    </tr>
     <tr>
        <td>
             <asp:Label ID="LabelFootNote" runat="server" CssClass="auto-style5" Text="* Leave blank if you do not want to apply constraints, but just want to rank the projects by their relative efficiency."></asp:Label>
        </td>
        <td style="text-align: left; vertical-align: top;" class="auto-style6">
            
            <asp:Label ID="LabelContextError" runat="server" ForeColor="Red"></asp:Label>
            
        </td>
    </tr>
  
   
</table>

