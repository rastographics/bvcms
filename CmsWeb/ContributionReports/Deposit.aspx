<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Deposit.aspx.cs" Inherits="CmsWeb.Contributions.Deposit" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        table
        {
            border: 1px solid black;
            border-collapse: collapse;
        }
        td, th
        {
            border: 1px solid black;
        }
        thead
        {
            display: table-header-group;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div style="font-size: 110%; text-align: center">
        <div style="font-weight: bold; padding-bottom: 1em">
            Contributions Bank Deposit Report<br />
            <asp:Label ID="dtlabel" runat="server">1/1/1001</asp:Label></div>
        <asp:ListView ID="ListView1" runat="server" DataSourceID="ObjectDataSource1">
            <LayoutTemplate>
                <table id="itemPlaceholderContainer" style="width: 40em; border: 1px solid black"
                    border="1" runat="server" cellspacing="0">
                    <thead>
                        <tr id="thead" runat="server" style="">
                            <th runat="server">
                                BundleId
                            </th>
                            <th runat="server">
                                Total
                            </th>
                            <th runat="server">
                                Checks
                            </th>
                            <th runat="server">
                                Cash
                            </th>
                            <th runat="server">
                                Coins
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr id="itemPlaceholder" runat="server">
                        </tr>
                    </tbody>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr style='<%# (string)Eval("BundleId") == "TOTALS" ? "font-weight: bold": "" %>'>
                    <td class="center">
                        <asp:Label ID="BundleIdLabel" runat="server" Text='<%# Eval("BundleId") %>' />
                    </td>
                    <td class="right" style="padding-right: .4em">
                        <asp:Label ID="TotalLabel" runat="server" Text='<%# Eval("Total", "{0:c}") %>' />
                    </td>
                    <td class="right" style="padding-right: .4em">
                        <asp:Label ID="ChecksLabel" runat="server" Text='<%# Eval("Checks", "{0:c}") %>' />
                    </td>
                    <td class="right" style="padding-right: .4em">
                        <asp:Label ID="CashLabel" runat="server" Text='<%# Eval("Cash", "{0:c}") %>' />
                    </td>
                    <td class="right" style="padding-right: .4em">
                        <asp:Label ID="CoinsLabel" runat="server" Text='<%# Eval("Coins", "{0:c}") %>' />
                    </td>
                </tr>
            </ItemTemplate>
            <EmptyDataTemplate>
                <table runat="server" style="">
                    <tr>
                        <td>
                            No data was returned.
                        </td>
                    </tr>
                </table>
            </EmptyDataTemplate>
        </asp:ListView>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}"
            SelectMethod="FetchDepositBundles" TypeName="CmsWeb.Models.BundleModel">
            <SelectParameters>
                <asp:QueryStringParameter Name="depositdt" QueryStringField="dt" Type="DateTime" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </div>
    </form>
</body>
</html>
