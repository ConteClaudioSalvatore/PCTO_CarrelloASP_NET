<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="paginaErrore.aspx.cs" Inherits="CarrelloASP_NET.paginaErrore" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="display:flex; flex-direction:column; justify-content:center; width:400px; margin:0 auto;">
            <asp:Label style="color:#f00;" ID="lblErrore" runat="server" Text=""></asp:Label>
            <br />
            <br />
            <asp:Button ID="btnContinua" runat="server" Text="Continua" OnClick="btnContinua_Click" />
        </div>
    </form>
</body>
</html>
