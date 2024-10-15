using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

public partial class EditareEroriAudit : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // start verificare drepturi access

        if (Session["USERNAME"] == null) Response.Redirect("login.aspx");
        if (Request.ServerVariables["REMOTE_USER"] == null) Response.Redirect("login.aspx");

        if (Session["USERNAME"].ToString().ToLower() != Request.ServerVariables["REMOTE_USER"].ToLower())
        {
            Response.Redirect("login2.aspx");
        }

        if ((Session["USERNAME"].ToString().ToLower() != @"it\adrian.mihailescu") && (Session["USERNAME"].ToString().ToLower() != @"it\mihai-valeriu.popescu"))
        {
            pnlAccessDenied.Visible = true;
            pnlShowAuditLogs.Visible = false;
            lblAccessDenied.Text = "Nu aveti acces pe aceasta pagina!<br />Click <a href=\"default_rbd.aspx\">aici</a> pentru a reveni la prima pagina.";
        }
        // end verificare drepturi access

        GridView1Html.Visible = ((Request.QueryString["type"] == "html") || (Request.QueryString["type"] == null));

        pnlShowAuditLinesHtml.Visible = !((Request.QueryString["type"] == "html") || (Request.QueryString["type"] == null));
        pnlShowAuditLinesXml.Visible = !(Request.QueryString["type"] == "xml");

        GridView1Xml.Visible = (Request.QueryString["type"] == "xml");

        if ((Request.QueryString["type"] == "html") || (Request.QueryString["type"] == null))
        {
            if (GridView1Html.Rows.Count == 0)
            {
                lblGridViewDetails.Text = "[nu exista linii in tabela rbd_istoric_erori_linie]";
            }
                
            else
            {
                lblGridViewDetails.Text = "rbd_istoric_erori_linie: " + GridView1Html.Rows.Count + " linii in aceasta pagina.";
            }
        }

        else
        {
            if (GridView1Xml.Rows.Count == 0)
            {
                lblGridViewDetails.Text = "[nu exista linii in tabela rbd_istoric_erori_linie]";
            }

            else
            {
                lblGridViewDetails.Text = "rbd_istoric_erori_linie: " + GridView1Xml.Rows.Count + " linii in aceasta pagina.";
            }
        }

        //repeater pe pagina
        DataSet ds3 = new DataSet();
        SqlCommand cmd = new SqlCommand();
        SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["MM_CONNECTION_STRING_b2_diverse"]);
        cmd.Connection = con;
        cmd.CommandText = "select den_meniu,link from Meniu_b2admin";
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(ds3);
        con.Open();
        cmd.ExecuteNonQuery();
        rptMeniu.DataSource = ds3;
        rptMeniu.DataBind();
        con.Close();
        //repeater pe pagina
    }
}
