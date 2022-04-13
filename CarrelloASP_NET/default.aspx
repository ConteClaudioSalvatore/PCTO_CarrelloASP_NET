<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="CarrelloASP_NET._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <script type="text/javascript" src="js/jquery.js"></script>
        <link rel="stylesheet" href="css/bootstrap.css" />
        <link rel="stylesheet" href="css/style.css" />
        <title>Carrello ASP</title>
        <style>
           body{
                -webkit-user-select: none; /* Safari */        
                -moz-user-select: none; /* Firefox */
                -ms-user-select: none; /* IE10+/Edge */
                user-select: none; /* Standard */
           }
        </style>
    </head>
    <body style="background-color: #508bfc;">
        <form id="form1" runat="server">
            <section class="vh-100">
                <div class="container py-5 h-100">
                    <div class="row d-flex justify-content-center align-items-center h-100">
                        <div class="col-12 col-md-8 col-lg-6 col-xl-5">
                            <div class="card shadow-2-strong" style="border-radius: 1rem;">
                                <div class="card-body p-5 text-center">
                                    <h3 class="mb-5">Accedi</h3>
                                    <div class="form-outline mb-4">
                                        <label class="form-label" for="txtUserMail">Email o Username</label>
                                        <asp:TextBox CssClass="form-control form-control-lg" ID="txtUserMail" runat="server"></asp:TextBox>
                                    </div>
                                    <div class="form-outline mb-4">
                                        <label class="form-label" for="txtPassword">Password</label>
                                        <asp:TextBox CssClass="form-control form-control-lg" ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
                                    </div>
                                    <asp:Button CssClass="btn btn-primary btn-lg btn-block" ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
                                    <asp:Label ID="lblErrore" runat="server" Text="" ForeColor="Red"></asp:Label>
                                    <hr class="my-4" />
                                    <span>Non hai un account? <a href="iscrizione.aspx">Iscriviti</a></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </form>
    </body>
</html>
