﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

using adoNetWebSQlServer;
using System.Net.Mail;

namespace CarrelloASP_NET
{
    public partial class Home : System.Web.UI.Page
    {
        private int categoria = 0;
        private adoNet dbConnection;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["account"] is DataRow account)
            {
                if (!IsPostBack)
                {
                    if (Session["appenaIscritto"] != null && (bool)Session["appenaIscritto"])
                        lblNavBrand.Text = "Benvenuto " + account["Cognome"].ToString() + " " + account["Nome"].ToString();
                    else
                        lblNavBrand.Text = account["Cognome"].ToString() + " " + account["Nome"].ToString();
                }
                caricaCategorie();
                if (Session["page"] != null && (int)Session["page"]==1)
                    caricaCarrello();
                else if (Session["page"] != null && (int)Session["page"] == 2)
                    caricaPagamento();
                else
                    caricaHome();
            }
            else
            {
                Response.Redirect("paginaErrore.aspx?codErr=1");
            }
        }
        #region nav bar
        private void caricaCategorie()
        {
            dbConnection = new adoNet();
            DataTable categorie = dbConnection.eseguiQuery("SELECT * FROM Categorie", CommandType.Text);
            HtmlGenericControl li = new HtmlGenericControl("li");
            phElencoCategorie.Controls.Add(li);
            Button a = new Button();
            a.CssClass = "dropdown-item";
            a.ID = "categoria_0";
            a.Text = "Tutto";
            a.Click += new EventHandler(selectedCategoria);
            li.Controls.Add(a);
            foreach (DataRow categoria in categorie.Rows)
            {
                li = new HtmlGenericControl("li");
                phElencoCategorie.Controls.Add(li);
                a = new Button
                {
                    CssClass = "dropdown-item",
                    ID = "categoria_" + categoria["ID"].ToString(),
                    Text = categoria["Descrizione"].ToString()
                };
                a.Click += new EventHandler(this.selectedCategoria);
                li.Controls.Add(a);
            }
        }
        protected void selectedCategoria(object sender, EventArgs e)
        {
            Session["page"] = 0;
            Button btn = sender as Button;
            categoria = Convert.ToInt32(btn.ID.Split('_')[1]);
            if (categoria == 0)
                caricaProdotti();
            else
                caricaProdotti(categoria);
            
        }
        protected void btnHome_Click(object sender, EventArgs e)
        {
            Session["page"] = 0;
            caricaHome();
        }
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session["account"] = null;
            Session["page"] = null;
            Response.Redirect("default.aspx");
        }
        #endregion
        #region prodotti
        private void caricaHome()
        {
            if (categoria == 0)
                caricaProdotti();
            else
                caricaProdotti(categoria);
        }
        private void caricaProdotti()
        {
            dbConnection = new adoNet();
            DataTable prodotti = dbConnection.eseguiQuery("SELECT * FROM Prodotti where Valido=1", CommandType.Text);
            generaCarte(prodotti);
        }
        private void caricaProdotti(int categoria)
        {
            dbConnection = new adoNet();
            dbConnection.cmd.Parameters.AddWithValue("@categoria", categoria);
            DataTable prodotti = dbConnection.eseguiQuery("SELECT * FROM Prodotti WHERE Categoria = @categoria and Valido=1", CommandType.Text);
            generaCarte(prodotti);
        }
        private void generaCarte(DataTable prodotti)
        {
            container.Controls.Clear();
            int i = 0;
            int colonne = 4;
            HtmlGenericControl row = new HtmlGenericControl("div");
            foreach (DataRow prodotto in prodotti.Rows)
            {
                if (i % colonne == 0)
                {
                    row = new HtmlGenericControl("div");
                    row.Attributes.Add("class", "row m-2");
                    container.Controls.Add(row);
                }
                HtmlGenericControl div = new HtmlGenericControl("div");
                div.Attributes.Add("class", "col-md-3");
                row.Controls.Add(div);
                HtmlGenericControl divCard = new HtmlGenericControl("div");
                divCard.Attributes.Add("class", "card mb-3 box-shadow text-center p-2");
                divCard.Style.Add("border-radius", "10px");
                div.Controls.Add(divCard);
                HtmlGenericControl divCardImg = new HtmlGenericControl("div");
                divCardImg.Attributes.Add("class", "card-img-top");
                divCard.Controls.Add(divCardImg);
                Image img = new Image
                {
                    CssClass = "img-fluid"
                };
                img.Style.Add("height", "225px");
                img.Style.Add("border-radius", "10px");
                img.ImageUrl = prodotto["Immagine"].ToString();
                divCardImg.Controls.Add(img);
                HtmlGenericControl divCardBody = new HtmlGenericControl("div");
                divCardBody.Attributes.Add("class", "card-body");
                divCard.Controls.Add(divCardBody);
                HtmlGenericControl h5 = new HtmlGenericControl("h5");
                h5.Attributes.Add("class", "card-title");
                h5.InnerText = prodotto["NomeProdotto"].ToString();
                divCardBody.Controls.Add(h5);
                HtmlGenericControl p = new HtmlGenericControl("p");
                p.Attributes.Add("class", "card-text");
                dbConnection.cmd.Parameters.AddWithValue("@idFornitore", prodotto["Fornitore"]);
                p.InnerHtml = prodotto["Descrizione"].ToString() + 
                    "<br/>" + prodotto["Prezzo"].ToString() + "€" +
                    "<br/>" + dbConnection.eseguiScalar("select Descrizione from Fornitori where Id=@idFornitore", CommandType.Text);
                divCardBody.Controls.Add(p);
                HtmlGenericControl divCardFooter = new HtmlGenericControl("div");
                divCardFooter.Attributes.Add("class", "card-footer justify-content-center");
                divCardFooter.Style.Add("border-radius", "5px");
                divCard.Controls.Add(divCardFooter);
                Button btnCart = new Button
                {
                    CssClass = "btn btn-primary",
                    ID = "addP_" + prodotto["Id"].ToString(),
                    Text = "Aggiungi al carrello"
                };
                btnCart.Style.Add("width", "100%");
                btnCart.Style.Add("height", "100%");
                btnCart.Click += new EventHandler(this.aggiungiAlCarrello);
                divCardFooter.Controls.Add(btnCart);
                i++;
            }
        }
        protected void aggiungiAlCarrello(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int idProdotto = Convert.ToInt32(btn.ID.Split('_')[1]);
            DataRow account = Session["account"] as DataRow;
            if (account != null)
            {
                dbConnection = new adoNet();
                dbConnection.cmd.Parameters.AddWithValue("@idProdotto", idProdotto);
                dbConnection.cmd.Parameters.AddWithValue("@idAccount", account["Id"]);
                if (Convert.ToBoolean(
                        Convert.ToInt32(
                        dbConnection.eseguiScalar(
                            "SELECT COUNT(*) FROM Carrello WHERE IdAccount = @idAccount AND IdProdotto = @idProdotto",
                            CommandType.Text
                            )
                        )
                    )
                )
                {
                    dbConnection.cmd.Parameters.AddWithValue("@idProdotto", idProdotto);
                    dbConnection.cmd.Parameters.AddWithValue("@idAccount", account["Id"]);
                    dbConnection.cmd.Parameters.AddWithValue("@data", DateTime.Now);
                    dbConnection.eseguiNonQuery(
                        "UPDATE Carrello SET Quantita = Quantita + 1, Data = @data WHERE IdAccount = @idAccount AND IdProdotto = @idProdotto",
                        CommandType.Text
                        );
                }
                else
                {
                    dbConnection.cmd.Parameters.AddWithValue("@idProdotto", idProdotto);
                    dbConnection.cmd.Parameters.AddWithValue("@idAccount", account["Id"]);
                    dbConnection.cmd.Parameters.AddWithValue("@data", DateTime.Now);
                    dbConnection.cmd.Parameters.AddWithValue("@quantita", 1);
                    dbConnection.eseguiNonQuery(
                        "INSERT INTO Carrello (IdAccount, IdProdotto, Data, Quantita) VALUES (@idAccount, @idProdotto, @data, @quantita)",
                        CommandType.Text
                        );
                }
            }
            else
            {
                Response.Redirect("paginaErrore.aspx?codErr=1");
            }
        }
        #endregion
        #region carrello
        protected void btnCart_Click(object sender, EventArgs e)
        {
            container.Controls.Clear();
            Session["page"] = 1;
            caricaCarrello();
        }
        private void caricaCarrello()
        {
            DataRow account = Session["account"] as DataRow;
            if (account != null)
            {
                dbConnection = new adoNet();
                dbConnection.cmd.Parameters.AddWithValue("@idAccount", account["Id"]);
                DataTable carrello = dbConnection.eseguiQuery(
                    "SELECT * FROM Carrello WHERE IdAccount = @idAccount",
                    CommandType.Text
                    );
                if (carrello.Rows.Count > 0)
                {
                    HtmlGenericControl table = new HtmlGenericControl("table");
                    table.Attributes.Add("class", "table table-striped table-bordered");
                    container.Controls.Add(table);
                    HtmlGenericControl thead = new HtmlGenericControl("thead");
                    table.Controls.Add(thead);
                    HtmlGenericControl tr = new HtmlGenericControl("tr");
                    thead.Controls.Add(tr);
                    HtmlGenericControl th = new HtmlGenericControl("th")
                    {
                        InnerText = "Prodotto"
                    };
                    tr.Controls.Add(th);
                    th = new HtmlGenericControl("th")
                    {
                        InnerText = "Descrizione"
                    };
                    tr.Controls.Add(th);
                    th = new HtmlGenericControl("th")
                    {
                        InnerText = "Prezzo Totale Articolo(€)"
                    };
                    tr.Controls.Add(th);
                    th = new HtmlGenericControl("th")
                    {
                        InnerText = "Quantità"
                    };
                    tr.Controls.Add(th);
                    th = new HtmlGenericControl("th")
                    {
                        InnerText = "Data di Aggiunta"
                    };
                    tr.Controls.Add(th);
                    HtmlGenericControl tbody = new HtmlGenericControl("tbody");
                    table.Controls.Add(tbody);
                    foreach (DataRow carRow in carrello.Rows)
                    {
                        tr = new HtmlGenericControl("tr");
                        tbody.Controls.Add(tr);
                        dbConnection.cmd.Parameters.AddWithValue("@idProdotto", carRow["IdProdotto"]);
                        DataRow prodotto = dbConnection.eseguiQuery(
                            "SELECT * FROM Prodotti WHERE Id = @idProdotto",
                            CommandType.Text
                            ).Rows[0];
                        HtmlGenericControl td = new HtmlGenericControl("td")
                        {
                            InnerText = prodotto["NomeProdotto"].ToString()
                        };
                        tr.Controls.Add(td);
                        td = new HtmlGenericControl("td")
                        {
                            InnerText = prodotto["Descrizione"].ToString()
                        };
                        tr.Controls.Add(td);
                        td = new HtmlGenericControl("td")
                        {
                            InnerText = (decimal)prodotto["Prezzo"] * (int)carRow["Quantita"] + " €"
                        };
                        tr.Controls.Add(td);
                        td = new HtmlGenericControl("td")
                        {
                            InnerText = carRow["Quantita"].ToString()
                        };
                        tr.Controls.Add(td);
                        td = new HtmlGenericControl("td")
                        {
                            InnerText = ((DateTime)carRow["Data"]).Date.ToString("dd/MM/yyyy")
                        };
                        tr.Controls.Add(td);
                        td = new HtmlGenericControl("td");
                        Button btn = new Button
                        {
                            CssClass = "btn btn-danger",
                            ID = "delP_" + carRow["IdProdotto"].ToString(),
                            Text = "Elimina"
                        };
                        btn.Style.Add("width", "100%");
                        btn.Style.Add("height", "100%");
                        btn.Click += new EventHandler(this.eliminaProdottoDalCarrello);
                        td.Controls.Add(btn);
                        tr.Controls.Add(td);
                    }
                    HtmlGenericControl btnWrapper = new HtmlGenericControl("div");
                    btnWrapper.Attributes.Add("class", "d-flex justify-content-center");
                    container.Controls.Add(btnWrapper);
                    Button btnEffettuaOrdine = new Button
                    {
                        CssClass = "btn btn-success px-5",
                        ID = "btnEffettuaOrdine",
                        Text = "Vai alla cassa"
                    };
                    btnEffettuaOrdine.Style.Add("margin", "0 auto");
                    btnEffettuaOrdine.Click += new EventHandler(this.effettuaOrdine);
                    btnWrapper.Controls.Add(btnEffettuaOrdine);
                }
                else
                {
                    HtmlGenericControl div = new HtmlGenericControl("div");
                    div.Attributes.Add("class", "alert alert-info text-center");
                    div.InnerText = "Il carrello è vuoto";
                    container.Controls.Add(div);
                }
            }
        }
        protected void eliminaProdottoDalCarrello(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int idProdotto = Convert.ToInt32(btn.ID.Split('_')[1]);
            if (Session["account"] is DataRow account)
            {
                dbConnection = new adoNet();
                dbConnection.cmd.Parameters.AddWithValue("@idProdotto", idProdotto);
                dbConnection.cmd.Parameters.AddWithValue("@idAccount", account["Id"]);
                DataRow productCarrello = dbConnection.eseguiQuery(
                    "select * from Carrello where IdAccount = @idAccount and IdProdotto = @idProdotto",
                    CommandType.Text
                    ).Rows[0];
                dbConnection.cmd.Parameters.AddWithValue("@idProdotto", idProdotto);
                dbConnection.cmd.Parameters.AddWithValue("@idAccount", account["Id"]);
                if ((int)productCarrello["Quantita"] > 1)
                {
                    dbConnection.cmd.Parameters.AddWithValue("@data", DateTime.Now);
                    dbConnection.eseguiNonQuery(
                        "UPDATE Carrello SET Quantita = Quantita - 1, Data = @data WHERE IdAccount = @idAccount AND IdProdotto = @idProdotto",
                        CommandType.Text
                        );
                }
                else
                {
                    dbConnection.eseguiNonQuery(
                        "DELETE FROM Carrello WHERE IdAccount = @idAccount AND IdProdotto = @idProdotto",
                        CommandType.Text
                        );
                }
            }
            else
            {
                Response.Redirect("paginaErrore.aspx?codErr=1");
            }
            container.Controls.Clear();
            caricaCarrello();
        }
        #endregion
        #region pagamento
        protected void effettuaOrdine(object sender, EventArgs e)
        {
            Session["page"] = 2;
            caricaPagamento();
        }
        private void caricaPagamento()
        {
            if (Session["account"] is DataRow account)
            {
                dbConnection = new adoNet();
                dbConnection.cmd.Parameters.AddWithValue("@idAccount", account["Id"]);
                DataTable carrello = dbConnection.eseguiQuery(
                    "SELECT * FROM Carrello WHERE IdAccount = @idAccount",
                    CommandType.Text
                    );
                if (carrello.Rows.Count > 0)
                {
                    container.Controls.Clear();
                    HtmlGenericControl div = new HtmlGenericControl("div");
                    div.Attributes.Add("class", "col-lg-6 px-5 py-4 mx-auto");
                    container.Controls.Add(div);
                    HtmlGenericControl h3 = new HtmlGenericControl("h3")
                    {
                        InnerText = "Pagamento"
                    };
                    h3.Attributes.Add("class", "mb-5 pt-2 text-center fw-bold text-uppercase");
                    div.Controls.Add(h3);
                    dbConnection.cmd.Parameters.AddWithValue("@idAccount", account["Id"]);
                    HtmlGenericControl h5 = new HtmlGenericControl("h5")
                    {
                        InnerText = dbConnection.eseguiScalar(
                            "SELECT SUM(p.Prezzo * c.Quantita) FROM Carrello as c inner join Prodotti as p on c.IdProdotto=p.Id WHERE IdAccount = @idAccount",
                            CommandType.Text
                            ).ToString() + " €"
                    };
                    h5.Attributes.Add("class", "mb-5 pt-2 text-center fw-bold text-uppercase");
                    div.Controls.Add(h5);
                    HtmlGenericControl form = new HtmlGenericControl("div");
                    form.Attributes.Add("class", "mb-5");
                    div.Controls.Add(form);

                    HtmlGenericControl formOutline = new HtmlGenericControl("div");
                    formOutline.Attributes.Add("class", "form-outline mb-5");
                    form.Controls.Add(formOutline);
                    TextBox inputCardNumber = new TextBox
                    {
                        ID = "txtCardNumber",
                        CssClass = "form-control form-control-lg"
                    };
                    inputCardNumber.TextChanged += new EventHandler(this.checkInputCard);
                    inputCardNumber.AutoPostBack = true;
                    inputCardNumber.Attributes.Add("maxlength", "19");
                    inputCardNumber.Attributes.Add("minlength", "19");
                    formOutline.Controls.Add(inputCardNumber);
                    HtmlGenericControl label = new HtmlGenericControl("label");
                    label.Attributes.Add("for", "txtCardNumber");
                    label.InnerText = "Numero di carta di credito";
                    label.Attributes.Add("class", "form-label");
                    formOutline.Controls.Add(label);
                    formOutline = new HtmlGenericControl("div");
                    formOutline.Attributes.Add("class", "form-outline mb-5");
                    form.Controls.Add(formOutline);
                    TextBox nameOnCard = new TextBox
                    {
                        ID = "txtNameOnCard",
                        CssClass = "form-control form-control-lg"
                    };
                    formOutline.Controls.Add(nameOnCard);
                    label = new HtmlGenericControl("label");
                    label.Attributes.Add("for", "txtNameOnCard");
                    label.Attributes.Add("class", "form-label");
                    label.InnerText = "Nome Completo sulla carta";
                    formOutline.Controls.Add(label);

                    HtmlGenericControl row = new HtmlGenericControl("div");
                    row.Attributes.Add("class", "row");
                    form.Controls.Add(row);

                    div = new HtmlGenericControl("div");
                    div.Attributes.Add("class", "col-md-6 mb-5");
                    row.Controls.Add(div);
                    formOutline = new HtmlGenericControl("div");
                    formOutline.Attributes.Add("class", "form-outline");
                    div.Controls.Add(formOutline);
                    TextBox dataScadenza = new TextBox
                    {
                        ID = "txtDataScadenza",
                        CssClass = "form-control form-control-lg"
                    };
                    dataScadenza.TextChanged += new EventHandler(this.checkInputCard);
                    dataScadenza.AutoPostBack = true;
                    dataScadenza.Attributes.Add("maxlength", "5");
                    formOutline.Controls.Add(dataScadenza);
                    label = new HtmlGenericControl("label");
                    label.Attributes.Add("for", "txtDataScadenza");
                    label.Attributes.Add("class", "form-label");
                    label.InnerText = "Data di scadenza";
                    formOutline.Controls.Add(label);

                    div = new HtmlGenericControl("div");
                    div.Attributes.Add("class", "col-md-6 mb-5");
                    row.Controls.Add(div);
                    formOutline = new HtmlGenericControl("div");
                    formOutline.Attributes.Add("class", "form-outline");
                    div.Controls.Add(formOutline);
                    TextBox cvv = new TextBox
                    {
                        ID = "txtCvv",
                        CssClass = "form-control form-control-lg"
                    };
                    cvv.TextMode = TextBoxMode.Password;
                    cvv.TextChanged += new EventHandler(this.checkInputCard);
                    cvv.AutoPostBack = true;
                    cvv.Attributes.Add("maxlength", "4");
                    formOutline.Controls.Add(cvv);
                    label = new HtmlGenericControl("label");
                    label.Attributes.Add("for", "txtCvv");
                    label.Attributes.Add("class", "form-label");
                    label.InnerText = "Cvv";
                    formOutline.Controls.Add(label);

                    HtmlGenericControl p = new HtmlGenericControl("p")
                    {
                        InnerText = "non salveremo nessun dato, la transazione è sicura al 100%"
                    };
                    p.Attributes.Add("class", "mb-5 text-center");
                    form.Controls.Add(p);
                    HtmlGenericControl btnWrapper = new HtmlGenericControl("div");
                    btnWrapper.Attributes.Add("class", "d-flex justify-content-center");
                    form.Controls.Add(btnWrapper);
                    Button btnOrdina = new Button
                    {
                        ID = "btnOrdina",
                        Text = "Compra",
                        CssClass = "btn btn-primary btn-lg btn-block"
                    };
                    btnOrdina.Style.Add("width", "250px");
                    btnOrdina.Click += new EventHandler(this.btnOrdina_Click);
                    btnWrapper.Controls.Add(btnOrdina);
                }
                else
                    Response.Redirect("paginaErrore.aspx?codErr=1");
            }
            else
                Response.Redirect("paginaErrore.aspx?codErr=1");
        }
        protected void checkInputCard(object sender, EventArgs e)
        {
            controllaCarta();
        }
        private bool controllaCarta()
        {
            Regex rgxCarta = new Regex("^(?:4[0-9]{12}(?:[0-9]{3})?|[25][1-7][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\\d{3})\\d{11})$");
            Regex rgxData = new Regex("^(0[1-9]|1[0-2])\\/?([0-9]{4}|[0-9]{2})$");
            TextBox[] controlli = {
                (TextBox)container.FindControl("txtCardNumber"),
                (TextBox)container.FindControl("txtDataScadenza"),
                (TextBox)container.FindControl("txtCvv")
            };
            controlli[2].Attributes["value"] = controlli[2].Text;
            if (rgxCarta.IsMatch(controlli[0].Text)
            ){
                controlli[0].Attributes.CssStyle.Add("border-color", "green");
                if (
                    controlli[1].Text != string.Empty &&
                    rgxData.IsMatch(controlli[1].Text)
                )
                {
                    controlli[1].Attributes.CssStyle.Add("border-color", "green");
                    if (controlli[2].Text != string.Empty)                     
                        return true;
                }
                else
                    controlli[1].Attributes.CssStyle.Add("border-color", "red");
            }
            else
                controlli[0].Attributes.CssStyle.Add("border-color", "red");
            if (controlli[0].Text == string.Empty)
                controlli[0].Attributes.CssStyle.Remove("border-color");
            if (controlli[1].Text == string.Empty)
                controlli[1].Attributes.CssStyle.Remove("border-color");
            if (controlli[2].Text == string.Empty)
                controlli[2].Attributes.CssStyle.Remove("border-color");
            
            return false;
        }
        protected void btnOrdina_Click(object sender, EventArgs e)
        {
            if(
                controllaCarta() &&
                Session["account"] is DataRow account)
            {
                dbConnection = new adoNet();
                MailMessage mail = new MailMessage();
                try
                {
                    mail.From = new MailAddress("c.conte.1292@vallauri.edu");
                    mail.To.Add(new MailAddress(account["Mail"].ToString()));
                    mail.Subject = "Ordine effettuato";
                    mail.IsBodyHtml = true;
                    dbConnection.cmd.Parameters.AddWithValue("@idAccount", account["Id"]);
                    mail.Body = $@"
                        Grazie <strong>{account["Nome"]} {account["cognome"]}</strong> per aver scelto il nostro e-commerce
                        <br>
                        <br>
                        Il tuo ordine è stato effettuato con <span style='color: #0a0'>successo</span>.
                        <br>
                        <br>
                        Hai speso <strong>{dbConnection.eseguiScalar(
                            "SELECT SUM(p.Prezzo * c.Quantita) FROM Carrello as c inner join Prodotti as p on c.IdProdotto=p.Id WHERE IdAccount = @idAccount",
                            CommandType.Text
                            )} €</strong>";
                    mail.Priority = MailPriority.High;
                    System.Net.NetworkCredential credenziali = new System.Net.NetworkCredential();
                    credenziali.UserName = "c.conte.1292@vallauri.edu";
                    credenziali.Password = "Claudioc0@";

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Credentials = credenziali;

                    smtp.Send(mail);
                    dbConnection.cmd.Parameters.AddWithValue("@idAccount", account["Id"]);
                    dbConnection.eseguiNonQuery(@"insert into StoricoOrdini(Account, Prodotto, Quantita, DataAcquisto) 
                                                  select IdAccount as Account, IdProdotto as Prodotto, Quantita, Data as DataAcquisto
                                                  from Carrello 
                                                  where IdAccount = @idAccount",
                                                  CommandType.Text);
                    dbConnection.cmd.Parameters.AddWithValue("@idAccount", account["Id"]);
                    dbConnection.eseguiNonQuery(@"delete from Carrello where IdAccount = @idAccount", CommandType.Text);
                    Session["page"] = 1;
                }
                catch 
                {
                    Response.Redirect("paginaErrore.aspx?codErr=2");
                }
                Response.Redirect("Home.aspx");
            }
        }
        #endregion
        #region cronologiaOrdini
        protected void btnCronologiaOrdini_Click(object sender, EventArgs e)
        {
            Session["page"] = 3;
            caricaCronologiaOrdini();
        }
        private void caricaCronologiaOrdini()
        {
            if (Session["account"] is DataRow account)
            {
                dbConnection = new adoNet();
                dbConnection.cmd.Parameters.AddWithValue("@idAccount", account["Id"]);
                DataTable ordini = dbConnection.eseguiQuery(@"select p.NomeProdotto as Prodotto, c.Quantita as Quantita, c.DataAcquisto as DataAcquisto, p.Prezzo, p.Immagine
                                                            from StoricoOrdini as c inner join Prodotti as p on c.Prodotto=p.Id
                                                            where c.Account = @idAccount
                                                            order by c.DataAcquisto desc",
                                                            CommandType.Text);
                if (ordini.Rows.Count > 0)
                {
                    container.Controls.Clear();
                    DateTime dataPrec = new DateTime();
                    HtmlGenericControl rowPerDate = new HtmlGenericControl("div");
                    decimal totale = 0;
                    HtmlGenericControl row = new HtmlGenericControl("div");
                    HtmlGenericControl totLbl;
                    foreach (DataRow ordine in ordini.Rows)
                    {
                        if (((DateTime)ordine["DataAcquisto"]).Date != dataPrec)
                        {
                            if(totale>0)
                            {
                                totLbl = new HtmlGenericControl("h5");
                                totLbl.Style.Add("text-align", "center");
                                totLbl.Attributes.Add("class", "my-3");
                                totLbl.InnerText = $"Totale: {totale} €";
                                row.Controls.Add(totLbl);
                            }
                            rowPerDate = new HtmlGenericControl("div");
                            rowPerDate.Attributes.Add("class", "row m-5");
                            rowPerDate.Attributes.Add("style", "border: 1px solid #ccc; border-radius: 5px; margin: min-content;");
                            container.Controls.Add(rowPerDate);
                            HtmlGenericControl h4 = new HtmlGenericControl("h4")
                            {
                                InnerText = $"Data di acquisto: {(DateTime)ordine["DataAcquisto"]:dd/MM/yyyy}"
                            };
                            h4.Style.Add("text-align", "center");
                            h4.Attributes.Add("class", "m-5");
                            totale = 0;
                            rowPerDate.Controls.Add(h4);
                        }
                        row = new HtmlGenericControl("div");
                        row.Attributes.Add("class", "row");
                        rowPerDate.Controls.Add(row);
                        HtmlGenericControl div = new HtmlGenericControl("div");
                        div.Attributes.Add("class", "col-12");
                        row.Controls.Add(div);
                        HtmlGenericControl card = new HtmlGenericControl("div");
                        card.Attributes.Add("class", "card my-1 mx-5");
                        div.Controls.Add(card);
                        HtmlGenericControl cardBody = new HtmlGenericControl("div");
                        cardBody.Attributes.Add("class", "card-body");
                        card.Controls.Add(cardBody);
                        HtmlGenericControl image = new HtmlGenericControl("img");
                        image.Attributes.Add("class", "img mr-4");
                        image.Style.Add("width", "9rem");
                        image.Style.Add("float", "left");
                        image.Style.Add("border-radius", "1rem");
                        image.Attributes.Add("src", $"{ordine["Immagine"]}");
                        cardBody.Controls.Add(image);
                        HtmlGenericControl cardTitle = new HtmlGenericControl("h5");
                        cardTitle.Attributes.Add("class", "card-title");
                        cardTitle.InnerText = ordine["Prodotto"].ToString();
                        cardBody.Controls.Add(cardTitle);
                        HtmlGenericControl cardText = new HtmlGenericControl("p");
                        cardText.Attributes.Add("class", "card-text");
                        cardText.InnerHtml = $@"Quantità: {ordine["Quantita"]} 
                                                <br>
                                                Costo: <strong>{(int)ordine["Quantita"] * (decimal)ordine["Prezzo"]} €</strong>";
                        cardBody.Controls.Add(cardText);
                        totale += (int)ordine["Quantita"] * (decimal)ordine["Prezzo"];

                        dataPrec = ((DateTime)ordine["DataAcquisto"]).Date;
                    }
                    totLbl = new HtmlGenericControl("h5");
                    totLbl.Style.Add("text-align", "center");
                    totLbl.Attributes.Add("class", "my-3");
                    totLbl.InnerText = $"Totale: {totale} €";
                    row.Controls.Add(totLbl);
                }
                else
                {
                    container.Controls.Clear();
                    HtmlGenericControl div = new HtmlGenericControl("div");
                    div.Attributes.Add("class", "alert alert-info text-center");
                    div.InnerText = "Non hai ancora effettuato ordini";
                    container.Controls.Add(div);
                }
            }
        }
        #endregion
    }
}