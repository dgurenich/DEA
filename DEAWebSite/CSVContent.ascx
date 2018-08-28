<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CSVContent.ascx.cs" Inherits="CSVContent" %>
<style type="text/css">
    .auto-style1 {
        width: 100%;
    }

    .auto-style2 {
        width: 266px;
    }
    .auto-style3 {
        width: 266px;
        font-size: medium;
        height: 8px;
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
        <td class="auto-style3"><strong>Parameters</strong></td>
        <td class="auto-style5" style="width: 16px">&nbsp;</td>
        <td class="auto-style4">
            <asp:Label ID="LabelProj" runat="server" Text="Projects (un-ranked)" style="font-size: medium; font-weight: 700;"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="auto-style2" style="vertical-align: top">
            <asp:GridView ID="GridViewParams" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" GridLines="None" Width="241px">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="Name" />
                    <asp:BoundField DataField="Value" HeaderText="Value" >
                    <HeaderStyle CssClass="text-right" HorizontalAlign="Right" />
                    <ItemStyle CssClass="text-right" HorizontalAlign="Right" />
                    </asp:BoundField>
                </Columns>
                <EditRowStyle BackColor="#7C6F57" />
                <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#E3EAEB" />
                <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#F8FAFA" />
                <SortedAscendingHeaderStyle BackColor="#246B61" />
                <SortedDescendingCellStyle BackColor="#D4DFE1" />
                <SortedDescendingHeaderStyle BackColor="#15524A" />
            </asp:GridView>
        </td>
        <td class="auto-style6" style="width: 16px;">
            &nbsp;</td>
        <td rowspan="5" style="vertical-align: top">
            <asp:Panel runat="server" ID="PanelProjects" Height="600px" Width="1010px" ScrollBars="Auto">
                <asp:GridView ID="GridViewProjects" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Height="461px"  Width="1007px" AutoGenerateColumns="False">
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
        <td class="auto-style2">
            <asp:Label ID="LabelVars" runat="server" Text="Variables" style="font-size: medium; font-weight: 700;"></asp:Label>
        </td>
        <td class="auto-style6" style="width: 16px">
            &nbsp;</td>
    </tr>
    <tr>
        <td class="auto-style2" style="vertical-align: top">
            <asp:GridView ID="GridViewVars" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" GridLines="None" Width="238px">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="Name" ReadOnly="True" />
                    <asp:BoundField DataField="Type" HeaderText="Input/Output" >
                    <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Scale" HeaderText="Scale" >
                    <HeaderStyle CssClass="text-right" HorizontalAlign="Right" />
                    <ItemStyle CssClass="text-right" HorizontalAlign="Right" />
                    </asp:BoundField>
                </Columns>
                <EditRowStyle BackColor="#7C6F57" />
                <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#E3EAEB" />
                <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#F8FAFA" />
                <SortedAscendingHeaderStyle BackColor="#246B61" />
                <SortedDescendingCellStyle BackColor="#D4DFE1" />
                <SortedDescendingHeaderStyle BackColor="#15524A" />
            </asp:GridView>
        </td>
        <td class="auto-style6" style="width: 16px;">
            &nbsp;</td>
    </tr>
    <tr>
        <td class="auto-style2">
            <asp:Label ID="LabelConst" runat="server" Text="Constraints" style="font-size: medium; font-weight: 700;"></asp:Label>
        </td>
        <td class="auto-style6" style="width: 16px">
            &nbsp;</td>
    </tr>
    <tr>
        <td class="auto-style2" style="vertical-align: top">
            <asp:GridView ID="GridViewConst" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" GridLines="None" Width="236px">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="Variable" />
                    <asp:BoundField DataField="Bound" HeaderText="Upper Limit (scaled)" DataFormatString="{0:N0}" >
                    <HeaderStyle CssClass="text-right" />
                    <ItemStyle CssClass="text-right" HorizontalAlign="Right" />
                    </asp:BoundField>
                </Columns>
                <EditRowStyle BackColor="#7C6F57" />
                <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#E3EAEB" />
                <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#F8FAFA" />
                <SortedAscendingHeaderStyle BackColor="#246B61" />
                <SortedDescendingCellStyle BackColor="#D4DFE1" />
                <SortedDescendingHeaderStyle BackColor="#15524A" />
            </asp:GridView>
        </td>
        <td class="auto-style6" style="width: 16px;">
            &nbsp;</td>
    </tr>
    <tr>
        <td>

        </td>
        <td class="auto-style6" style="width: 16px">

            &nbsp;</td>
        <td>

            <table class="auto-style1">
                <tr>
                    <td colspan="2">

                        <hr />

                    </td>
                </tr>
                <tr>
                    <td>

            <asp:Button ID="ButtonRun" runat="server" Text="Optimize" Width="300px" Height="35px" OnClick="ButtonRun_Click" />

                    </td>
                    <td style="text-align: right">
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click" ToolTip="Click to go back to CSV input files selection.">Start Over</asp:LinkButton>
                    </td>
                </tr>
            </table>

        </td>
    </tr>
</table>

