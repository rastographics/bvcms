<%@ Page Title="" Language="C#" MasterPageFile="~/Contributions/Reports/Reports.Master"
    AutoEventWireup="true" CodeBehind="JournalDetails.aspx.cs" Inherits="CmsWeb.Contributions.Reports.JournalDetails" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="text-align: center; font-size: 110%">
        <div>
            <h1>
                Journal Details for Fund</h1>
            <h2>
                <asp:Label ID="FundName" runat="server" Text="Label"></asp:Label></h2>
            From:
            <asp:Label ID="FromDt" runat="server" Text="Label"></asp:Label>
            -- To:
            <asp:Label ID="ToDt" runat="server" Text="Label"></asp:Label>
        </div>
        <hr />
        <asp:ListView ID="ListView1" runat="server" DataSourceID="ObjectDataSource1" OnDataBound="ListView1_DataBound">
            <ItemTemplate>
                <tr style="">
                    <td class="left">
                        <asp:Label ID="HeaderIdLabel" runat="server" Text='<%# Eval("HeaderId") %>' />
                    </td>
                    <td class="left">
                        <asp:Label ID="DateLabel" runat="server" Text='<%# Eval("Date", "{0:d}") %>' />
                    </td>
                    <td class="right">
                        <asp:Label ID="TotalLabel" runat="server" Text='<%# Eval("Total", "{0:#,###.00}") %>' />
                    </td>
                    <td class="right">
                        <asp:Label ID="CountLabel" runat="server" Text='<%# Eval("Count", "{0:n0}") %>' />
                    </td>
                </tr>
            </ItemTemplate>
            <EmptyDataTemplate>
                <table id="Table1" runat="server" style="">
                    <tr>
                        <td>
                            No data was returned.
                        </td>
                    </tr>
                </table>
            </EmptyDataTemplate>
            <LayoutTemplate>
                <table id="itemPlaceholderContainer" runat="server" border="0" style="">
                    <tr id="Tr1" runat="server" style="">
                        <th id="Th1" runat="server">
                            HeaderId
                        </th>
                        <th id="Th4" runat="server">
                            Date
                        </th>
                        <th id="Th2" runat="server">
                            Total
                        </th>
                        <th id="Th3" runat="server">
                            Count
                        </th>
                    </tr>
                    <tr id="itemPlaceholder" runat="server">
                    </tr>
                    <tr style="border-top: 2px solid black">
                        <th class="left">
                            Total
                        </th>
                        <th>
                        </th>
                        <th class="right">
                            <asp:Label ID="Total" runat="server" />
                        </th>
                        <th class="right">
                            <asp:Label ID="Count" runat="server" />
                        </th>
                    </tr>
                </table>
            </LayoutTemplate>
        </asp:ListView>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}"
            SelectMethod="JournalDetails" TypeName="CmsWeb.Models.BundleModel" OnObjectCreated="ObjectDataSource1_ObjectCreated">
            <SelectParameters>
                <asp:ControlParameter Name="dt1" ControlID="FromDt" Type="DateTime" />
                <asp:ControlParameter Name="dt2" ControlID="ToDt" Type="DateTime" />
                <asp:QueryStringParameter Name="FundId" Type="Int32" QueryStringField="fundid" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </div>
</asp:Content>
