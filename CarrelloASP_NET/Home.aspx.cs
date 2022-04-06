using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace CarrelloASP_NET
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataRow account = Session["account"] as DataRow;
                if (account != null)
                {
                    if((bool)Session["appenaIscritto"])
                        lblNavBrand.Text = "Benvenuto " + account["Cognome"].ToString() + " " + account["Nome"].ToString();
                    else
                        lblNavBrand.Text = account["Cognome"].ToString() + " " +  account["Nome"].ToString();
                    lblNavBrand.NavigateUrl = "#";
                }
                else
                {
                    Response.Redirect("paginaErrore.aspx?codErr=1");
                }
            }
        }
    }
}