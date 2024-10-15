<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>RBD Automatizare</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        d:\adp\prod_files\spec_10.00\test_folder\20090116.010605.CDUT0435.BZ090115
        <br />
        <asp:GridView ID="GridView1" runat="server" CellPadding="4" ForeColor="#333333">
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#EFF3FB" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#2461BF" />
            <AlternatingRowStyle BackColor="White" />
        </asp:GridView>
        <br />
        <asp:GridView ID="GridView2" runat="server" CellPadding="4" ForeColor="#333333">
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#EFF3FB" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#2461BF" />
            <AlternatingRowStyle BackColor="White" />
        </asp:GridView>
        <asp:Button ID="btnBindGrid" runat="server" Text="test read file" OnClick="btnBindGrid_Click" />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="test import fisier" />
        <asp:Button ID="btnTestEditareErori" runat="server" Text="test editare erori" OnClick="btnTestEditareErori_Click" />
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="export in fisier text" />
        <asp:Label ID="lblStatusMessage" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>
