using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using adoNetWebSQlServer;

namespace CarrelloASP_NET
{
    public partial class AdminView : System.Web.UI.Page
    {
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
                if (Session["page"] != null)
                {
                    if((int)Session["page"] == 1)
                    {
                        caricaAggiungiFornitore();
                        return;
                    }
                    if ((int)Session["page"] == 2)
                    {
                        caricaGestioneCategorie();
                        return;
                    }
                    if ((int)Session["page"] == 3)
                    {
                        caricaInserimentoCategorie();
                        return;
                    } 
                }
                caricaHome();
                return;
            }
            else
            {
                Response.Redirect("paginaErrore.aspx?codErr=1");
            }
        }
        #region home
        private void caricaHome()
        {
            Session["page"] = 0;
            container.Controls.Clear();
            dbConnection = new adoNet();
            //genero la tabella con i fornitori
            DataTable fornitori = dbConnection.eseguiQuery(
                @"select 
                Fornitori.Id as Id,
                Nome + ' ' + Cognome as Responsabile, 
                Descrizione as Nome,
                Fornitori.Mail as Email,
                Fornitori.Telefono as Telefono,
                Fornitori.PIva as PIva,
                Fornitori.Sede as Sede 
                from Fornitori inner join Accounts 
                on Fornitori.Account = Accounts.Id",
                CommandType.Text
                );
            if (fornitori.Rows.Count > 0)
            {
                HtmlGenericControl table = new HtmlGenericControl("table");
                table.Attributes.Add("class", "table table-striped table-bordered table-hover");
                container.Controls.Add(table);
                HtmlGenericControl thead = new HtmlGenericControl("thead");
                table.Controls.Add(thead);
                HtmlGenericControl tr = new HtmlGenericControl("tr");
                thead.Controls.Add(tr);
                HtmlGenericControl th = new HtmlGenericControl("th");
                th.InnerText = "Nome";
                tr.Controls.Add(th);
                th = new HtmlGenericControl("th");
                th.InnerText = "Sede";
                tr.Controls.Add(th);
                th = new HtmlGenericControl("th");
                th.InnerText = "Telefono";
                tr.Controls.Add(th);
                th = new HtmlGenericControl("th");
                th.InnerText = "Email";
                tr.Controls.Add(th);
                th = new HtmlGenericControl("th");
                th.InnerText = "P.Iva";
                tr.Controls.Add(th);
                th = new HtmlGenericControl("th");
                th.InnerText = "Responsabile";
                tr.Controls.Add(th);
                th = new HtmlGenericControl("th");
                th.InnerText = "Azioni";
                tr.Controls.Add(th);
                HtmlGenericControl tbody = new HtmlGenericControl("tbody");
                table.Controls.Add(tbody);
                HtmlGenericControl td;
                foreach (DataRow fornitore in fornitori.Rows)
                {
                    tr = new HtmlGenericControl("tr");
                    tbody.Controls.Add(tr);
                    td = new HtmlGenericControl("td");
                    td.InnerText = fornitore["Nome"].ToString();
                    tr.Controls.Add(td);
                    td = new HtmlGenericControl("td");
                    td.InnerText = fornitore["Sede"].ToString();
                    tr.Controls.Add(td);
                    td = new HtmlGenericControl("td");
                    td.InnerText = fornitore["Telefono"].ToString();
                    tr.Controls.Add(td);
                    td = new HtmlGenericControl("td");
                    td.InnerText = fornitore["Email"].ToString();
                    tr.Controls.Add(td);
                    td = new HtmlGenericControl("td");
                    td.InnerText = fornitore["PIva"].ToString();
                    tr.Controls.Add(td);
                    td = new HtmlGenericControl("td");
                    td.InnerText = fornitore["Responsabile"].ToString();
                    tr.Controls.Add(td);
                    td = new HtmlGenericControl("td");
                    tr.Controls.Add(td);
                    Button btnModifica = new Button
                    {
                        Text = "Modifica",
                        ID = "btnModifica_" + fornitore["Id"].ToString(),
                        CssClass = "btn btn-outline-warning"
                    };
                    btnModifica.Click += new EventHandler(this.btnModificaFornitore_Click);
                    td.Controls.Add(btnModifica);
                }
            }
            else
            {
                HtmlGenericControl div = new HtmlGenericControl("div");
                div.Attributes.Add("class", "alert alert-info text-center");
                div.InnerText = "Non ci sono fornitori registrati";
                container.Controls.Add(div);
            }
        }
        protected void btnModificaFornitore_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string id = btn.ID.Split('_')[1];
            Session["page"] = 1;
            Session["inModFornitore"] = id;
            caricaAggiungiFornitore();

        }
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session["account"] = null;
            Session["page"] = null;
            Session["inModFornitore"] = null;
            Session["inModCategoria"] = null;
            Response.Redirect("default.aspx");
        }
        protected void btnHome_Click(object sender, EventArgs e)
        {
            Session["inModFornitore"] = null;
            Session["inModCategoria"] = null;
            caricaHome();
        }
        #endregion
        #region fornitore
        protected void btnAggiungiFornitore_Click(object sender, EventArgs e)
        {
            Session["inModFornitore"] = null;
            Session["inModCategoria"] = null;
            caricaAggiungiFornitore();
        }
        private void caricaAggiungiFornitore()
        {
            Session["page"] = 1;
            container.Controls.Clear();
            dbConnection = new adoNet();
            DataRow modFornitore = new DataTable().NewRow();
            if (Session["inModFornitore"] != null)
            {
                 modFornitore = dbConnection.eseguiQuery("select * from Fornitori where Id = " + Session["inModFornitore"], CommandType.Text).Rows[0];
            }
            HtmlGenericControl formContainer = new HtmlGenericControl("div");
            formContainer.Attributes.Add("class", "container py-5 h-100");
            container.Controls.Add(formContainer);
            HtmlGenericControl form = new HtmlGenericControl("div");
            form.Attributes.Add("class", "row h-100 d-flex justify-content-center align-items-center");
            formContainer.Controls.Add(form);
            HtmlGenericControl formDiv = new HtmlGenericControl("div");
            formDiv.Attributes.Add("class", "col-12 col-md-8 col-lg-6 col-xl-5");
            form.Controls.Add(formDiv);
            HtmlGenericControl formCard = new HtmlGenericControl("div");
            formCard.Attributes.Add("class", "card shadow-2-strong");
            formCard.Attributes.Add("style", "border-radius: 1rem");
            formDiv.Controls.Add(formCard);
            HtmlGenericControl formCardBody = new HtmlGenericControl("div");
            formCardBody.Attributes.Add("class", "card-body px-4");
            formCard.Controls.Add(formCardBody);
            HtmlGenericControl formTitle = new HtmlGenericControl("h3");
            formTitle.Attributes.Add("class", "card-title text-center my-4");
            formTitle.InnerText = "Aggiungi Fornitore";
            formCardBody.Controls.Add(formTitle);
            //nome fornitore
            HtmlGenericControl formOutline = new HtmlGenericControl("div");
            formOutline.Attributes.Add("class", "form-outline mb-4");
            formCardBody.Controls.Add(formOutline);
            HtmlGenericControl formLabel = new HtmlGenericControl("label");
            formLabel.Attributes.Add("class", "form-label");
            formLabel.InnerText = "Nome Fornitore";
            formOutline.Controls.Add(formLabel);
            TextBox formControl = new TextBox
            {
                ID = "txtNomeFornitore"
            };
            if (Session["inModFornitore"] != null)
            {
                formControl.Text = modFornitore["Descrizione"].ToString();
            }
            formControl.Attributes.Add("class", "form-control");
            formOutline.Controls.Add(formControl);
            //sede fornitore
            formOutline = new HtmlGenericControl("div");
            formOutline.Attributes.Add("class", "form-outline mb-4");
            formCardBody.Controls.Add(formOutline);
            formLabel = new HtmlGenericControl("label");
            formLabel.Attributes.Add("class", "form-label");
            formLabel.InnerText = "Sede Fornitore";
            formOutline.Controls.Add(formLabel);
            formControl = new TextBox
            {
                ID = "txtSedeFornitore"
            };
            if (Session["inModFornitore"] != null)
            {
                formControl.Text = modFornitore["Sede"].ToString();
            }
            formControl.Attributes.Add("placeholder", "es. via dei salici, 2 Alba, CN");
            formControl.Attributes.Add("class", "form-control");
            formOutline.Controls.Add(formControl);
            //telefono fornitore
            formOutline = new HtmlGenericControl("div");
            formOutline.Attributes.Add("class", "form-outline mb-4");
            formCardBody.Controls.Add(formOutline);
            formLabel = new HtmlGenericControl("label");
            formLabel.Attributes.Add("class", "form-label");
            formLabel.InnerText = "Telefono Fornitore";
            formOutline.Controls.Add(formLabel);
            formControl = new TextBox
            {
                ID = "txtTelefonoFornitore",
                TextMode = TextBoxMode.Number
            };
            if (Session["inModFornitore"] != null)
            {
                formControl.Text = modFornitore["Telefono"].ToString();
            }
            formControl.Attributes.Add("placeholder", "es. 01730123456");
            formControl.Attributes.Add("class", "form-control");
            formOutline.Controls.Add(formControl);
            //email fornitore
            formOutline = new HtmlGenericControl("div");
            formOutline.Attributes.Add("class", "form-outline mb-4");
            formCardBody.Controls.Add(formOutline);
            formLabel = new HtmlGenericControl("label");
            formLabel.Attributes.Add("class", "form-label");
            formLabel.InnerText = "Email Fornitore";
            formOutline.Controls.Add(formLabel);
            formControl = new TextBox
            {
                ID = "txtEmailFornitore",
                TextMode = TextBoxMode.Email
            };
            if (Session["inModFornitore"] != null)
            {
                formControl.Text = modFornitore["Mail"].ToString();
            }
            formControl.Attributes.Add("placeholder", "es. mail@domain.com");
            formControl.Attributes.Add("class", "form-control");
            formOutline.Controls.Add(formControl);
            //p.iva fornitore
            formOutline = new HtmlGenericControl("div");
            formOutline.Attributes.Add("class", "form-outline mb-4");
            formCardBody.Controls.Add(formOutline);
            formLabel = new HtmlGenericControl("label");
            formLabel.Attributes.Add("class", "form-label");
            formLabel.InnerText = "P.IVA Fornitore";
            formOutline.Controls.Add(formLabel);
            formControl = new TextBox
            {
                ID = "txtPivaFornitore"
            };
            if (Session["inModFornitore"] != null)
            {
                formControl.Text = modFornitore["PIva"].ToString();
            }
            formControl.Attributes.Add("placeholder", "es. 01234567891");
            formControl.Attributes.Add("class", "form-control");
            formOutline.Controls.Add(formControl);
            //responsabile fornitore
            formOutline = new HtmlGenericControl("div");
            formOutline.Attributes.Add("class", "form-outline mb-4");
            formCardBody.Controls.Add(formOutline);
            formLabel = new HtmlGenericControl("label");
            formLabel.Attributes.Add("class", "form-label");
            formLabel.InnerText = "Responsabile Fornitore";
            formOutline.Controls.Add(formLabel);
            DropDownList selectAccount = new DropDownList();
            selectAccount.ID = "selectAccount";
            selectAccount.Attributes.Add("class", "form-control");
            selectAccount.Items.Add(new ListItem("", "0"));
            string sql = "select * from Accounts where Privilegi=1";
            if (Session["inModFornitore"] != null)
            {
                
                sql += $" or Id = {modFornitore["Account"]}";
            }
            foreach (
                DataRow account in
                dbConnection.eseguiQuery(
                    sql,
                    CommandType.Text
                ).Rows
            )
            {
                selectAccount.Items.Add(
                    new ListItem(
                        account["Nome"].ToString() + " " + account["Cognome"].ToString(),
                        account["Id"].ToString()
                        )
                    );
            }
            if (Session["inModFornitore"] != null)
            {
                selectAccount.SelectedValue = modFornitore["Account"].ToString();
            }
            formOutline.Controls.Add(selectAccount);
            //button
            formOutline = new HtmlGenericControl("div");
            formOutline.Attributes.Add("class", "form-button py-3");
            formCardBody.Controls.Add(formOutline);
            Button conferma = new Button
            {
                ID = "btnConferma",
                Text = "Conferma",
                CssClass = "btn btn-primary btn-lg btn-block px-5"
            };
            conferma.Attributes.Add("style", "margin-left:50%; transform: translate(-50%,0)");
            conferma.Click += new EventHandler(this.btnConfermaFornitore_Click);
            formOutline.Controls.Add(conferma);
            Label lblErrore = new Label
            {
                ID = "lblErrore",
                CssClass = "text-danger"
            };
            formOutline.Controls.Add(lblErrore);
        }
        private bool controllaCampi()
        {
            Label lblErrore = (Label)this.FindControl("lblErrore");
            if (((TextBox)this.FindControl("txtNomeFornitore")).Text != string.Empty)
            {
                if (((TextBox)this.FindControl("txtSedeFornitore")).Text != string.Empty)
                {
                    if (((TextBox)this.FindControl("txtTelefonoFornitore")).Text != string.Empty)
                    {
                        if (((TextBox)this.FindControl("txtEmailFornitore")).Text != string.Empty)
                        {
                            long pIva = 0;
                            if (Int64.TryParse(((TextBox)this.FindControl("txtPivaFornitore")).Text, out pIva))
                            {
                                if (((TextBox)this.FindControl("txtPivaFornitore")).Text.Trim().Length == 11)
                                {
                                    if (((DropDownList)this.FindControl("selectAccount")).SelectedValue != "0")
                                    {
                                        return true;
                                    }
                                    else
                                        lblErrore.Text = "Seleziona un account";
                                }
                                else
                                    lblErrore.Text = "La partita iva deve essere di 11 cifre";
                            }
                            else
                                lblErrore.Text = "Inserisci una partita iva valida";
                        }
                        else
                            lblErrore.Text = "Inserisci una email";
                    }
                    else
                        lblErrore.Text = "Inserisci un numero di telefono";
                }
                else
                    lblErrore.Text = "Inserisci la sede";
            }
            else
                lblErrore.Text = "Inserire il nome del fornitore";
            return false;
        }
        protected void btnConfermaFornitore_Click(object sender, EventArgs e)
        {
            if (controllaCampi())
            {
                dbConnection = new adoNet();
                System.Data.SqlClient.SqlParameterCollection parameters = dbConnection.cmd.Parameters;
                parameters.AddWithValue("@descrizione", ((TextBox)this.FindControl("txtNomeFornitore")).Text.Trim());
                parameters.AddWithValue("@sede", ((TextBox)this.FindControl("txtSedeFornitore")).Text.Trim());
                parameters.AddWithValue("@telefono", ((TextBox)this.FindControl("txtTelefonoFornitore")).Text.Trim());
                parameters.AddWithValue("@email", ((TextBox)this.FindControl("txtEmailFornitore")).Text.Trim());
                parameters.AddWithValue("@piva", ((TextBox)this.FindControl("txtPivaFornitore")).Text.Trim());
                parameters.AddWithValue("@idAccount", Convert.ToInt32(((DropDownList)this.FindControl("selectAccount")).SelectedValue));
                if (Session["inModFornitore"] == null)
                {
                    if (!Convert.ToBoolean(
                        dbConnection.eseguiNonQuery(
                            @"insert into Fornitori (Descrizione, Sede, Telefono, Mail, Piva, Account) 
                              values (@descrizione, @sede, @telefono, @email, @piva, @idAccount);
                              update Accounts set Privilegi=2 where Id=@idAccount",
                            CommandType.Text)
                        )
                    )
                        ((Label)this.FindControl("lblErrore")).Text = "Errore nell'inserimento del fornitore";
                        
                }
                else
                {
                    parameters.AddWithValue("@id", Convert.ToInt32(Session["inModFornitore"]));
                    if (!Convert.ToBoolean(
                        dbConnection.eseguiNonQuery(
                            @"
                            update Accounts set Privilegi=1 where Id in (select Account from Fornitori where Id=@id);
                            update Fornitori set 
                            Descrizione=@descrizione, 
                            Sede=@sede, 
                            Telefono=@telefono, 
                            Mail=@email, 
                            Piva=@piva, 
                            Account=@idAccount 
                            where Id=@id;
                            update Accounts set Privilegi=2 where Id=@idAccount",
                            CommandType.Text)
                        )
                    )
                        ((Label)this.FindControl("lblErrore")).Text = "Errore nell'inserimento del fornitore";   
                }
                Session["page"] = 0;
                Session["inModFornitore"] = null;
                caricaHome();

            }
        }
        #endregion
        #region categorie
        protected void btnCategorie_Click(object sender, EventArgs e)
        {
            Session["inModCategoria"] = null;
            Session["page"] = 2;
            caricaGestioneCategorie();
        }
        private void caricaGestioneCategorie()
        {
            container.Controls.Clear();
            dbConnection = new adoNet();
            HtmlGenericControl row = new HtmlGenericControl("div");
            row.Attributes.Add("class", "row d-flex justify-content-center");
            container.Controls.Add(row);
            HtmlGenericControl col = new HtmlGenericControl("div");
            col.Attributes.Add("class", "col-12 col-md-8 col-lg-6 col-xl-5");
            row.Controls.Add(col);
            HtmlGenericControl table = new HtmlGenericControl("table");
            table.Attributes.Add("class", "table table-striped table-bordered table-hover");
            col.Controls.Add(table);
            HtmlGenericControl thead = new HtmlGenericControl("thead");
            table.Controls.Add(thead);
            HtmlGenericControl tr = new HtmlGenericControl("tr");
            thead.Controls.Add(tr);
            HtmlGenericControl th = new HtmlGenericControl("th");
            th.InnerText = "Nome";
            tr.Controls.Add(th);
            th = new HtmlGenericControl("th")
            {
                InnerText = "Azioni"
            };
            tr.Controls.Add(th);
            HtmlGenericControl tbody = new HtmlGenericControl("tbody");
            HtmlGenericControl td;
            table.Controls.Add(tbody);
            foreach (DataRow categoria in
                dbConnection.eseguiQuery(
                    @"select * from Categorie",
                    CommandType.Text
                ).Rows
            )
            {
                tr = new HtmlGenericControl("tr");
                if (!(bool)categoria["valido"])
                {
                    tr.Attributes.Add("style", "opacity:0.5");
                }
                tbody.Controls.Add(tr);
                td = new HtmlGenericControl("td");
                td.InnerText = categoria["Descrizione"].ToString();
                tr.Controls.Add(td);
                td = new HtmlGenericControl("td");
                tr.Controls.Add(td);
                HtmlGenericControl div = new HtmlGenericControl("div");
                div.Attributes.Add("class", "btn-group");
                div.Attributes.Add("style", "width:100%");
                td.Attributes.Add("class", "d-flex justify-content-center");
                td.Controls.Add(div);
                Button buttonModifica = new Button();
                buttonModifica.Text = "Modifica";
                buttonModifica.CssClass = "btn btn-outline-primary";
                buttonModifica.Click += new EventHandler(this.btnModificaCategoria_Click);
                buttonModifica.ID = "btnModifica_" + categoria["Id"].ToString();
                div.Controls.Add(buttonModifica);
                Button buttonElimina = new Button();
                
                if ((bool)categoria["Valido"])
                {
                    buttonElimina.CssClass = "btn btn-outline-danger";
                    buttonElimina.Text = "Elimina";
                }
                else
                {
                    buttonElimina.CssClass = "btn btn-outline-warning";
                    buttonElimina.Text = "Ripristina";
                }
                buttonElimina.Click += new EventHandler(this.btnEliminaCategoria_Click);
                buttonElimina.ID = "btnElimina_" + categoria["Id"].ToString();
                div.Controls.Add(buttonElimina);
            }
            td = new HtmlGenericControl("td");
            td.Attributes.Add("colspan", "2");
            tbody.Controls.Add(td);
            Button btnAggiungi = new Button();
            btnAggiungi.Text = "+";
            btnAggiungi.CssClass = "btn btn-outline-success";
            btnAggiungi.Attributes.CssStyle.Add("width", "100%");
            btnAggiungi.Click += new EventHandler(this.btnAggiungiCategoria_Click);
            td.Controls.Add(btnAggiungi);
        }
        protected void btnAggiungiCategoria_Click(object sender, EventArgs e)
        {
            Session["page"] = 3;
            caricaInserimentoCategorie();
        }
        protected void btnModificaCategoria_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string id = btn.ID.Split('_')[1];
            Session["inModCategoria"] = id;
            caricaInserimentoCategorie();
        }
        private void caricaInserimentoCategorie()
        {
            container.Controls.Clear();
            HtmlGenericControl formContainer = new HtmlGenericControl("div");
            formContainer.Attributes.Add("class", "container py-5 h-100");
            container.Controls.Add(formContainer);
            HtmlGenericControl form = new HtmlGenericControl("div");
            form.Attributes.Add("class", "row h-100 d-flex justify-content-center align-items-center");
            formContainer.Controls.Add(form);
            HtmlGenericControl formDiv = new HtmlGenericControl("div");
            formDiv.Attributes.Add("class", "col-12 col-md-8 col-lg-6 col-xl-5");
            form.Controls.Add(formDiv);
            HtmlGenericControl formCard = new HtmlGenericControl("div");
            formCard.Attributes.Add("class", "card shadow-2-strong rounded-2");
            formDiv.Controls.Add(formCard);
            HtmlGenericControl formCardBody = new HtmlGenericControl("div");
            formCardBody.Attributes.Add("class", "card-body");
            formCard.Controls.Add(formCardBody);
            HtmlGenericControl formTitle = new HtmlGenericControl("h3");
            formTitle.Attributes.Add("class", "card-title text-center");
            formTitle.InnerText = "Categoria";
            formCardBody.Controls.Add(formTitle);
            HtmlGenericControl formOutline = new HtmlGenericControl("div");
            formOutline.Attributes.Add("class", "form-outline");
            formCardBody.Controls.Add(formOutline);
            HtmlGenericControl formLabel = new HtmlGenericControl("label")
            {
                InnerText = "Nome"
            };
            formOutline.Controls.Add(formLabel);
            TextBox txtNome = new TextBox
            {
                ID = "txtNome",
                CssClass = "form-control"
            };
            if (Session["inModCategoria"] != null)
            {
                dbConnection.cmd.Parameters.AddWithValue("@id", Session["inModCategoria"]);
                txtNome.Text = dbConnection.eseguiScalar(
                    @"select Descrizione from Categorie where Id=@id",
                    CommandType.Text
                );
            }
            formOutline.Controls.Add(txtNome);
            HtmlGenericControl formButton = new HtmlGenericControl("div");
            formButton.Attributes.Add("class", "form-button");
            formCardBody.Controls.Add(formButton);
            Button btnInserisci = new Button
            {
                Text = "Salva",
                CssClass = "btn btn-primary mt-4"
            };
            btnInserisci.Attributes.Add("style", "margin-left:50%; transform: translate(-50%,0)");
            btnInserisci.Click += new EventHandler(this.btnInserisciCategoria_Click);
            formButton.Controls.Add(btnInserisci);
        }
        protected void btnInserisciCategoria_Click(object sender, EventArgs e)
        {
            TextBox txtNome = (TextBox)container.FindControl("txtNome");
            if (txtNome.Text.Length > 0)
            {
                dbConnection = new adoNet();
                dbConnection.cmd.Parameters.AddWithValue("@descrizione", txtNome.Text);
                if (Session["inModCategoria"] == null)
                {
                    dbConnection.eseguiNonQuery(
                        @"insert into Categorie (Descrizione) values (@descrizione)",
                        CommandType.Text
                    );
                }   
                else
                {
                    dbConnection.cmd.Parameters.AddWithValue("@id", (int)Session["inModCategoria"]);
                    dbConnection.eseguiNonQuery(
                        @"update Categorie set Descrizione = @descrizione where Id = @id",
                        CommandType.Text
                    );
                }
                Session["page"] = 2;
                Session["inModCategoria"] = null;
                caricaGestioneCategorie();
            }
        }
        protected void btnEliminaCategoria_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string id = btn.ID.Split('_')[1];
            dbConnection.cmd.Parameters.AddWithValue("@id", id);
            dbConnection.eseguiNonQuery(
                @"update Categorie set Valido = 1 - Valido where Id = @id",
                CommandType.Text
            );
            Session["page"] = 2;
            Session["inModCategoria"] = null;
            caricaGestioneCategorie();
        }
        #endregion
    }
}