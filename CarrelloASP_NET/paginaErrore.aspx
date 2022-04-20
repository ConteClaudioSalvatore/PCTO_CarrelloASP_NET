<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="paginaErrore.aspx.cs" Inherits="CarrelloASP_NET.paginaErrore" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Errore</title>
    <link rel="stylesheet" href="css/bootstrap.css" />
    <script type="text/javascript" src="js/bootstrap.js"></script>
    <style>
       body{
            -webkit-user-select: none; /* Safari */        
            -moz-user-select: none; /* Firefox */
            -ms-user-select: none; /* IE10+/Edge */
            user-select: none; /* Standard */
       }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="h-100" style="display:flex; flex-direction:column; justify-content:center; width:400px; margin:0 auto;">
            <asp:Label style="color:#f00;" ID="lblErrore" runat="server" CssClass="text-danger text-center" Text=""></asp:Label>
            <br />
            <br />
            <asp:Button ID="btnContinua" runat="server" Text="Continua" CssClass="btn btn-primary" OnClick="btnContinua_Click" />
        </div>
    </form>
</body>
</html>
