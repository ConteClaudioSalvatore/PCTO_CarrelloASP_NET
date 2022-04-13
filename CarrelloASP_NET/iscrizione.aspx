<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="iscrizione.aspx.cs" Inherits="CarrelloASP_NET.iscrizione" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Iscrizione CarrelloASP</title>
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
        <section style="background-color: #508bfc;">
            <div class="container py-5 h-100">
                <div class="row d-flex justify-content-center align-items-center h-100">
                    <div class="col-12 col-md-8 col-lg-6 col-xl-5">
                        <div class="card shadow-2-strong" style="border-radius: 1rem;">
                            <div class="card-body p-5 text-center">
                                <h3 class="mb-5">Iscriviti</h3>
                                <div class="form-outline mb-4">
                                    <label class="form-label" for="txtUsername">Username</label>
                                    <asp:TextBox CssClass="form-control form-control-lg" ID="txtUsername" runat="server" AutoPostBack="True" OnTextChanged="txtUsername_TextChanged"></asp:TextBox>
                                </div>
                                <label class="form-label">Nominativo</label>
                                <div class="form-outline input-group mb-4">
                                    <asp:TextBox CssClass="form-control form-control-lg" ID="txtCognome" runat="server" placeholder="Cognome"></asp:TextBox>
                                    <span></span>                                    
                                    <asp:TextBox CssClass="form-control form-control-lg" ID="txtNome" runat="server" placeholder="Nome"></asp:TextBox>
                                </div>
                                <div class="form-outline mb-4">
                                    <label class="form-label" for="txtIndirizzo">Indirizzo</label>
                                    <asp:TextBox CssClass="form-control form-control-lg" ID="txtIndirizzo" runat="server" placeholder="Via Roma 1, 12034, Fossano(CN)"></asp:TextBox>
                                </div>
                                <div class="form-outline mb-4">
                                    <label class="form-label" for="txtMail">E-Mail</label>
                                    <asp:TextBox CssClass="form-control form-control-lg" ID="txtMail" runat="server" TextMode="Email" placeholder="mail@domain.com" AutoPostBack="True" OnTextChanged="txtMail_TextChanged"></asp:TextBox>
                                </div>
                                <div class="form-outline mb-4">
                                    <label class="form-label" for="txtTelefono">Telefono</label>
                                    <asp:TextBox CssClass="form-control form-control-lg" ID="txtTelefono" runat="server" TextMode="Phone" placeholder="0123456789"></asp:TextBox>
                                </div>
                                <label class="form-label">Password</label>
                                <div class="form-outline input-group mb-4">
                                    <asp:TextBox CssClass="form-control form-control-lg" ID="txtPassword" runat="server" TextMode="Password" placeholder="Password"></asp:TextBox>
                                    <span class="input-group-text"> </span>
                                    <asp:TextBox CssClass="form-control form-control-lg" ID="txtConfermaPassword" runat="server" TextMode="Password" placeholder="Conferma Password"></asp:TextBox>
                                </div>
                                <asp:Button CssClass="btn btn-primary btn-lg btn-block" ID="btnLogin" runat="server" Text="Iscriviti" OnClick="btnLogin_Click" />
                                <br />
                                <asp:Label ID="lblErrore" runat="server" Text="" ForeColor="Red"></asp:Label>
                                <hr class="my-4" />
                                <span>Hai già un account? <a href="default.aspx">Accedi</a></span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </form>
</body>
</html>
