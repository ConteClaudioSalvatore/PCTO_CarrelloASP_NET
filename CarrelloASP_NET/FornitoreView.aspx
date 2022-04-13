﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FornitoreView.aspx.cs" Inherits="CarrelloASP_NET.FornitoreView" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Home Fornitore</title>
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
        <nav class="navbar navbar-expand-lg navbar-light bg-light">
            <div class="container-fluid">
                <asp:HyperLink CssClass="navbar-brand" ID="lblNavBrand" runat="server"></asp:HyperLink>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="collapse navbar-collapse" id="navbarSupportedContent">
                    <ul class="navbar-nav me-auto mb-2 mb-lg-0">
                        <li class="nav-item">
                            <asp:Button ID="btnHome" runat="server" class="nav-link active btn btn-outline" BorderStyle="None" aria-current="page" href="#" Text="Home" OnClick="btnHome_Click" />
                        </li>
                    </ul>
                    <div class="d-flex">
                        <asp:Button ID="btnAggiungiProdotto" CssClass="btn btn-outline-success" Text="Aggiungi Prodotto" runat="server" OnClick="btnAggiungiProdotto_Click" />
                    </div>
                </div>
            </div>
        </nav>
        <div class="container-fluid">
            <asp:PlaceHolder ID="container" runat="server">
                
            </asp:PlaceHolder>
        </div>
    </form>
</body>
</html>