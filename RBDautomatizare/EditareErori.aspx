<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditareErori.aspx.cs" Inherits="EditareErori" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>RBD Automatizare - Modificare erori</title>
</head> 
<body>
    <form id="form1" runat="server">
    <div>
    <table>
        <tr>
            <td>
                <asp:GridView ID="tableGridView"  
                  OnRowEditing ="tableGridView_RowEditing" 
                  OnRowCancelingEdit="tableGridView_RowCancelingEdit" 
                  OnRowUpdating="tableGridView_RowUpdating"
                  OnPageIndexChanging="tableGridView_PageIndexChanging"      
                  runat="server"  AutoGenerateColumns="False" AllowPaging="True" AllowSorting="True">    
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                 <asp:Label ID="lblOperationStatus" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                 <asp:Panel ID="MsgPanel" runat="server" Visible="False">
                    <div>
                        <asp:Label ID="msg_lbl" runat="server" Text="" ></asp:Label>   
                    </div>
                    <asp:LinkButton ID="msg_button" runat="server" OnClick="msg_button_Click">Hide Message</asp:LinkButton>
                </asp:Panel>
            </td>
        </tr>
    </table>       
    </div>
    </form>
</body>
</html>