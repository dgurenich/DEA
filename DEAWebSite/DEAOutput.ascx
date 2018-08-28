<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DEAOutput.ascx.cs" Inherits="DEAOutput" %>
<style type="text/css">
    .auto-style1 {
        width: 100%;
    }

    .auto-style4 {
        height: 8px;
    }
    .auto-style5 {
        width: 288px;
        font-size: medium;
        height: 8px;
    }
    .auto-style6 {
        width: 288px;
    }
</style>

<table class="auto-style1">
    <tr>
        <td class="auto-style4">
            <asp:Label ID="LabelProj" runat="server" Text="Projects (ranked)" style="font-size: medium; font-weight: 700;"></asp:Label>
        </td>
    </tr>
    <tr>
        <td style="vertical-align: top">
            <asp:Panel runat="server" ID="PanelProjects" Height="600px" Width="1210px" ScrollBars="Auto">
                <asp:GridView ID="GridViewProjects" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Height="461px"  Width="1207px" AutoGenerateColumns="False">
                    <AlternatingRowStyle BackColor="White" />
                    <EditRowStyle BackColor="#7C6F57" />
                    <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" HorizontalAlign="Right" />
                    <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                    <RowStyle BackColor="#E3EAEB" />
                    <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                    <SortedAscendingCellStyle BackColor="#F8FAFA" />
                    <SortedAscendingHeaderStyle BackColor="#246B61" />
                    <SortedDescendingCellStyle BackColor="#D4DFE1" />
                    <SortedDescendingHeaderStyle BackColor="#15524A" />
                </asp:GridView>
            </asp:Panel>
            <asp:Label runat="server" ID ="LabelError" ForeColor="Red" style="font-size: small" />
        </td>
    </tr>
    <tr>
        <td>

            <table class="auto-style1">
                <tr>
                    <td colspan="2">

                        <hr />

                    </td>
                </tr>
                <tr>
                    <td>

                        <asp:HyperLink ID="hlinkExcel" runat="server" NavigateUrl="~/DEAOutputExcel.aspx" Target="_blank" ToolTip="Click to convert to Microsoft Excel format.">Export to Microsoft Excel</asp:HyperLink>

                    </td>
                    <td style="text-align: right">
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click" ToolTip="Click to go back to CSV input files selection.">Start Over</asp:LinkButton>
                    </td>
                </tr>
            </table>

        </td>
    </tr>
</table>

