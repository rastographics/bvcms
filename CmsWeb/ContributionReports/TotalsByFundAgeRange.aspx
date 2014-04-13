<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TotalsByFundAgeRange.aspx.cs"
    MasterPageFile="~/Contributions/Reports/Reports.Master" Inherits="CmsWeb.Reports.TotalsByFundAgeRange" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="text-align: center; font-size: 110%">
        <h1>
            <asp:Label ID="Label1" runat="server" Text="Totals by Age Range for Fund"></asp:Label></h1>
        <table style="font-size: 110%; text-align: left">
                <tr>
                    <td style="text-align: right">
                        CampusId:
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="CampusId" DataSourceID="CampusIds"  DataTextField="Value"
                        DataValueField="Id"></asp:DropDownList>
                    </td>
                        <td>
                            &nbsp;
                        </td>
                </tr>
            <tr>
                <td style="text-align: right">
                    Fund:
                </td>
                <td>
                    <asp:DropDownList ID="Fund" runat="server" DataSourceID="ODSFunds" DataTextField="Value"
                        DataValueField="Id">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="text-align: right">
                    From Date:
                </td>
                <td>
                    <asp:TextBox ID="FromDate" runat="server" AutoPostBack="false" Style="font-size: 110%"
                        Width="100"></asp:TextBox>
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td style="text-align: right">
                    To Date:
                </td>
                <td>
                    <asp:TextBox ID="ToDate" runat="server" AutoPostBack="false" Style="font-size: 110%"
                        Width="100"></asp:TextBox>
                </td>
                <td>
                    <asp:Button ID="btnSubmit" ToolTip="Press to run report" CausesValidation="true"
                        runat="server" Text="Run" CssClass="noPrint" ValidationGroup="vgDates" OnClick="btnSubmit_Click" />
                </td>
            </tr>
        </table>
        <asp:CompareValidator ID="FromDateValidator" runat="server" ErrorMessage="CompareValidator"
            ControlToValidate="FromDate" ControlToCompare="ToDate" Operator="LessThanEqual"
            SetFocusOnError="True" ValidationGroup="vgDates" Type="Date" Text="From Date must be before To Date."
            CssClass="noPrint"></asp:CompareValidator><br />
        <hr />
        <asp:ListView ID="ListView1" runat="server" DataSourceID="ODSRanges" OnDataBound="ListView1_DataBound"
            Visible="False">
            <ItemTemplate>
                <tr style="">
                    <td class="left">
                        <asp:Label ID="RangeLabel" runat="server" Text='<%# Eval("Range") %>' />
                    </td>
                    <td class="right">
                        <asp:Label ID="CountLabel" runat="server" Text='<%# Eval("DonorCount", "{0:n0}") %>' />
                    </td>
                    <td class="right">
                        <asp:Label ID="Label3" runat="server" Text='<%# Eval("Count", "{0:n0}") %>' />
                    </td>
                    <td class="right">
                        <asp:Label ID="PctCount" runat="server" Text='<%# Eval("PctCount", "{0:n1}") %>' />
                    </td>
                    <td class="right">
                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("Total", "{0:n0}") %>' />
                    </td>
                    <td class="right">
                        <asp:Label ID="Label2" runat="server" Text='<%# Eval("PctTotal", "{0:n1}") %>' />
                    </td>
                    <td class="right">
                        <asp:Label ID="Label4" runat="server" Text='<%# Eval("Average", "{0:n0}") %>' />
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
                <table id="itemPlaceholderContainer" runat="server" border="0" cellpadding="4" style="">
                    <tr id="Tr1" runat="server" style="">
                        <th id="Th1" class="left" runat="server">
                            Range
                        </th>
                        <th id="Th2" class="right" runat="server">
                            Donors
                        </th>
                        <th id="Th6" class="right" runat="server">
                            Count
                        </th>
                        <th id="Th3" class="right" runat="server">
                            %
                        </th>
                        <th id="Th4" class="right" runat="server">
                            Amount
                        </th>
                        <th id="Th5" class="right" runat="server">
                            %
                        </th>
                        <th id="Th7" class="right" runat="server">
                            Average
                        </th>
                    </tr>
                    <tr id="itemPlaceholder" runat="server">
                    </tr>
                    <tr style="border-top: 2px solid black">
                        <th class="left">
                            <asp:Label ID="RangeLabel" runat="server" Text='Total' />
                        </th>
                        <th class="right">
                            <asp:Label ID="DonorCount" runat="server" />
                        </th>
                        <th class="right">
                            <asp:Label ID="Count" runat="server" />
                        </th>
                        <th class="right">
                            <asp:Label ID="PctCount" runat="server" Text='100.0' />
                        </th>
                        <th class="right">
                            <asp:Label ID="Total" runat="server" />
                        </th>
                        <th class="right">
                            <asp:Label ID="Label2" runat="server" Text='100.0' />
                        </th>
                    </tr>
                </table>
            </LayoutTemplate>
        </asp:ListView>
    </div>
    <asp:ObjectDataSource ID="ODSRanges" runat="server" SelectMethod="TotalsByFundAgeRange"
        TypeName="CmsWeb.Models.BundleModel" OnObjectCreated="ObjectDataSource1_ObjectCreated"
        OldValuesParameterFormatString="original_{0}">
        <SelectParameters>
            <asp:ControlParameter Name="fundid" ControlID="Fund" PropertyName="SelectedValue"
                Type="Int32" />
            <asp:ControlParameter Name="dt1" ControlID="FromDate" Type="DateTime" />
            <asp:ControlParameter Name="dt2" ControlID="ToDate" Type="DateTime" />
            <asp:QueryStringParameter Name="Pledges" Type="string" QueryStringField="pledged"
                DefaultValue="false" />
            <asp:ControlParameter Name="CampusId" ControlID="CampusId" Type="Int32" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="ODSFunds" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="Funds" TypeName="CmsWeb.Code.CodeValueModel"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="CampusIds" runat="server" SelectMethod="AllCampuses0" TypeName="CmsWeb.Code.CodeValueModel"></asp:ObjectDataSource>
</asp:Content>
