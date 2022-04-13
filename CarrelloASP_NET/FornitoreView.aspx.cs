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
    public partial class FornitoreView : System.Web.UI.Page
    {
        private adoNet dbConnection;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["account"] is DataRow account)
            {
                dbConnection = new adoNet();
                if (!IsPostBack)
                {
                    if (Session["appenaIscritto"] != null && (bool)Session["appenaIscritto"])
                        lblNavBrand.Text = "Benvenuto " + account["Cognome"].ToString() + " " + account["Nome"].ToString();
                    else
                        lblNavBrand.Text = account["Cognome"].ToString() + " " + account["Nome"].ToString();
                    caricaHome();
                }
            }
        }

        private void caricaHome()
        {
            container.Controls.Clear();
            dbConnection = new adoNet();
            if (Session["account"] is DataRow account)
            {
                dbConnection.cmd.Parameters.AddWithValue("@account", account["Id"].ToString());
                DataTable prodottiFornitore = dbConnection.eseguiQuery(
                        "select * from Prodotti where Account = @account",
                        CommandType.Text
                    );
                if (prodottiFornitore.Rows.Count > 0)
                {
                    DataGrid prodotti = new DataGrid
                    {
                        DataSource = prodottiFornitore
                    };
                    container.Controls.Add(prodotti);
                    prodotti.DataBind();
                }
                else
                {
                    HtmlGenericControl div = new HtmlGenericControl("div");
                    div.Attributes.Add("class", "alert alert-info");
                    div.InnerHtml = "Non hai ancora inserito nessun prodotto";
                    container.Controls.Add(div);
                }
            }
        }

        protected void btnAggiungiProdotto_Click(object sender, EventArgs e)
        {

        }
    }
}