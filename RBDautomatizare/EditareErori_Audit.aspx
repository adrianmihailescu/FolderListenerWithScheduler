<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditareErori_Audit.aspx.cs" Inherits="EditareEroriAudit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>RBD Automatizare - Audit tables</title>
    <style type="text/css">
    <!--
    body {
	    margin-left: 0px;
	    margin-top: 0px;
	    margin-right: 0px;
	    margin-bottom: 0px;
    }
    -->
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="760" align="center">
            <tr>
                <td colspan="2" class="leftnav">
                    <asp:Panel ID="pnlAccessDenied" runat="server" Visible="false">
                        <asp:Label ID="lblAccessDenied" runat="server"></asp:Label>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td valign="top" class="leftnav">
                                                <!-- start left menu -->
                            <table width="144" border="1" cellpadding="0" cellspacing="0" bordercolor="#6699cc">
                                <tr>
                                  <td height="21" bgcolor="#6699cc" class="leftnavheader">Meniu</td>
                                </tr>
                                <tr>
                                  <td valign="top" bgcolor="#FFFFFF">                                   
                                        <table width="101" border="0" align="center" cellpadding="0" cellspacing="0">
                                          <tr>
                                            <td>
                                                <asp:Repeater ID="rptMeniu" runat="server">
                    		                        <ItemTemplate>
          						                        <asp:HyperLink  ID="Hyperlink1" runat="server"   CssClass="homegrau"
								                        NavigateUrl=<%# DataBinder.Eval(Container.DataItem, "link") %>
                                                        Text=<%# DataBinder.Eval(Container.DataItem, "den_meniu") %>
                                                        /><br />
          					                        </ItemTemplate>
          				                        </asp:Repeater>
                                            </td>
                                          </tr>
                                        </table>
                                  </td>
                                </tr>
                            </table>
                            <!-- end left menu -->
                </td>
                <td>
                    <asp:Panel ID="pnlShowAuditLogs" runat="server" style="width: 603px; overflow: auto;">
                        <asp:Label ID="lblGridViewDetails" runat="server" class="leftnav"></asp:Label>
                        <br />
                        <asp:Label ID="lblShowAuditLinesMessage" runat="server" Text="Arata log-urile in format: " CssClass="homegrau"></asp:Label>
                        <asp:Panel ID="pnlShowAuditLinesHtml" runat="server" Visible="false" style="display: inline">                            
                            <asp:HyperLink ID="lnkShowAuditLinesHtml" runat="server" Text="xml" NavigateUrl="~/EditareErori_Audit.aspx?type=html"></asp:HyperLink>
                        </asp:Panel>
                        <asp:Panel ID="pnlShowAuditLinesXml" runat="server" Visible="false" style="display: inline">
                            <asp:HyperLink ID="lnkShowAuditLinesXml" runat="server" Text="html" NavigateUrl="~/EditareErori_Audit.aspx?type=xml"></asp:HyperLink>
                        </asp:Panel>
                        
                        <!--gridul pentru versiunea html a liniilor de audit!-->
                        <asp:GridView ID="GridView1Html" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1"
                        AllowPaging="True" CellPadding="4" ForeColor="#333333" CssClass="textblack" Visible="false" GridLines="Both">
                            <Columns>
                                <asp:BoundField DataField="id" HeaderText="id" ReadOnly="True" SortExpression="id" />
                                <asp:BoundField DataField="operatie" HeaderText="operatie" ReadOnly="true" SortExpression="operatie" />
                            </Columns>
                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                            <RowStyle BackColor="#EFF3FB" />
                            <EditRowStyle BackColor="#2461BF" />
                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                            <AlternatingRowStyle BackColor="White" />
                            <headerstyle Font-Bold="True" ForeColor="#FFFFFF" BackColor="#6699CC" CssClass="homegrau"></headerstyle>
                        </asp:GridView>
                        <!--gridul pentru versiunea xml a liniilor de audit-->
                        <asp:GridView ID="GridView1Xml" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1"
                        AllowPaging="True" CellPadding="4" ForeColor="#333333" CssClass="textblack" Visible="False" GridLines="Both">
                            <Columns>
                                <asp:BoundField DataField="id" HeaderText="id" ReadOnly="True" SortExpression="id" ItemStyle-VerticalAlign="Top"/>
                                <asp:BoundField DataField="ip" HeaderText ="ip" ReadOnly="True" SortExpression="ip" ItemStyle-VerticalAlign="Top" />
                                <asp:BoundField DataField="utilizator" HeaderText ="utilizator" ReadOnly="True" SortExpression="utilizator" ItemStyle-VerticalAlign="Top" />
                                <asp:BoundField DataField="server" HeaderText ="server" ReadOnly="True" SortExpression="server" ItemStyle-VerticalAlign="Top" />
                                <asp:BoundField DataField="tabela" HeaderText ="tabela" ReadOnly="True" SortExpression="tabela" ItemStyle-VerticalAlign="Top" />
                                <asp:BoundField DataField="query" HeaderText="query" ReadOnly="true" SortExpression="query" ItemStyle-VerticalAlign="Top" />
                                <asp:BoundField DataField="data" HeaderText ="data (mm/dd/yyyy hh:mm:ss)" ReadOnly="True" SortExpression="data" ItemStyle-VerticalAlign="Top" />
                            </Columns>
                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                            <RowStyle BackColor="#EFF3FB" />
                            <EditRowStyle BackColor="#2461BF" />
                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                            <AlternatingRowStyle BackColor="White" />
                            <headerstyle Font-Bold="True" ForeColor="White" BackColor="#6699CC" CssClass="homegrau"></headerstyle>
                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:MM_CONNECTION_STRING_b2_diverse_audit %>"
                            ProviderName="<%$ ConnectionStrings:MM_CONNECTION_STRING_b2_diverse_audit.ProviderName %>"
                            SelectCommand="SELECT [id], [operatie], [ip], [utilizator], [server], [tabela], [operatie_h], [query], [data] FROM [manual_update_operations_audit]"></asp:SqlDataSource>
                   </asp:Panel>
                </td>
            </tr>
        </table>       
    </div>
    </form>
</body>
</html>
