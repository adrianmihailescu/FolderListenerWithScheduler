using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Net;
using System.IO;

public partial class EditareErori : System.Web.UI.Page
{
    #region Data members
    public static DataTable Table = new DataTable();
    ArrayList ParameterArray = new ArrayList();

    string strConnectionStringRBD = @"Server=dcsv75; Database=RBD; User ID=user_rbd; Password=!@#rbd$%^";
    string strNumeTabelaErori = "RBD_ISTORIC_VERIFICARE_DETALIU_ERORI_PT_CORECTARE";
    string strNumeServer = "dcsv75";
    string strProceduraAducereDateConformPermissions = "SP_GET_RANDURI_DUPA_NIVEL_ACCES_PE_SISTEM_RATING";

    string strUserAccessingPage = @"IT\Adrian.Mihailescu";

    #endregion

    #region file methods
    /// <summary>
    /// scrie un text in fisierul specificat
    /// </summary>
    /// <param name="strText"></param>
    /// <param name="strFileName"></param>
    protected void writeTextToFile(string strText, string strFileName)
    {
        // creaza fisierul
        FileInfo fiItem = new FileInfo(Server.MapPath(strFileName));
        if (!fiItem.Exists)
            fiItem.Create();
        
        // scrie textul in fisier
        StreamWriter StreamWriter1 = new StreamWriter(Server.MapPath(strFileName));
        StreamWriter1.Write(strText);
        StreamWriter1.Close();
    }
    #endregion file methods

    #region audit methods
    /// <summary>
    /// decodes an xml line and returns the value for a specific tag
    /// </summary>
    /// <param name="strToCheck"></param>
    /// <param name="strTagToCheck"></param>
    /// <returns></returns>
    protected string getValueBetweenTags(string strToCheck, string strTagToCheck)
    {
        int firstPosition = strToCheck.IndexOf("<" + strTagToCheck + ">");
        int lastPosition = strToCheck.IndexOf("</" + strTagToCheck + ">");
        return strToCheck.Substring(strToCheck.IndexOf("<" + strTagToCheck + ">") + ((string)("<" + strTagToCheck + ">")).Length, lastPosition - firstPosition - ((string)("</" + strTagToCheck)).Length);
    }

    /// <summary>
    /// gets the query to insert a new line in the log table
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="utilizator"></param>
    /// <param name="server"></param>
    /// <param name="tabela"></param>
    /// <param name="operatie"></param>
    /// <param name="query"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public string getLineForNewLogEntry(string ip, string utilizator, string server, string tabela, string operatie, string query, DateTime data)
    {
        //<-- start build the query
        // build the query with the parameters
        string strTempQuery = "";

        strTempQuery += "declare @tab as table ";
        strTempQuery += " (";
        strTempQuery += " ip nvarchar(max),";
        strTempQuery += " utilizator nvarchar(max),";
        strTempQuery += " server nvarchar(max),";
        strTempQuery += " tabela nvarchar(max),";
        strTempQuery += " operatie nvarchar(max),";
        strTempQuery += " query nvarchar(max),";
        strTempQuery += " data nvarchar(max)";
        strTempQuery += " )";

        strTempQuery += @" insert into @tab values ('" + ip + "', '" + utilizator + "', '" + server + "', '" + tabela + "', '" + operatie + "', '" + query + "', '" + data + "')";
        strTempQuery += " select * from @tab for xml auto, elements";
        //--> end build the query

        return strTempQuery;
    }

    /// <summary>
    /// returns the computer's ip address
    /// </summary>
    /// <returns></returns>
    public string getIPAddress()
    {
        string sHostName = Dns.GetHostName();
        IPHostEntry ipE = Dns.GetHostByName(sHostName);
        IPAddress[] IpA = ipE.AddressList;

        return IpA[0].ToString();
    }
    
    /// <summary>
    /// logs an operation in the table named vto_audit
    /// </summary>
    /// <param name="strServer"></param>
    /// <param name="strTable"></param>
    /// <param name="strOperation"></param>
    /// <param name="strQuery"></param>
    public void logOperation(string strServer, string strTable, string strOperation, string strQuery)
    {
        //<-- start set query parameters
        string strTempIP = getIPAddress(); // the computer's ip
        string strTempUtilizator = Request.ServerVariables["LOGON_USER"];
        string strTempTabela = strTable;
        string strTempServer = strServer;
        string strTempOperatie = strOperation;
        string strTempQuery = strQuery;
        DateTime dtData = DateTime.Now; // operation's date
        //--> end set query parameters

        ArrayList arrCollection = new ArrayList();

        // start run query
        try
        {
            // SqlConnection myConnection = new SqlConnection("Data Source=" + currentServer + ";Initial Catalog=" + currentDatabase + ";User ID=" + currentUser + ";Password=" + currentPass + "; Persist Security Info=True;");
            SqlConnection myConnection = new SqlConnection(strConnectionStringRBD);
            string sqlQuery = getLineForNewLogEntry(strTempIP, strTempUtilizator, strTempServer, strTempTabela, strTempOperatie, strTempQuery.Replace("'", "''"), dtData);

            SqlCommand myCommand = new SqlCommand(sqlQuery, myConnection);

            myConnection.Open();

            SqlDataReader myReader = myCommand.ExecuteReader();

            // iterate through the table fields description
            while (myReader.Read())
            {
                arrCollection.Add(myReader.GetValue(0).ToString().Replace("'", "''"));
            }

            myReader.Close();
            myConnection.Close();
            // return arrFieldValuesCollection[0].ToString();
        }

        catch (Exception ex)
        {
            msg_lbl.Text = "Conectare esuata la " + Server + " !";
            MsgPanel.Visible = true;

            // lblShowLegend.Visible = false;
        }
        // end run query

        //-----------------------------
        // start insert new row in table

        // SqlConnection Connection = new System.Data.SqlClient.SqlConnection("Data Source=" + ServerName + ";Initial Catalog=" + DatabaseName + ";User ID=" + UserName + ";Password=" + Password + "; Persist Security Info=True;");
        SqlConnection Connection = new System.Data.SqlClient.SqlConnection(strConnectionStringRBD);
        try
        {
            //<-- start stabilire parametri pentru inserate
            // stabilesc parametrii pentru insert
            string ipToInsert = getValueBetweenTags(arrCollection[0].ToString(), "ip");
            string utilizatorToInsert = getValueBetweenTags(arrCollection[0].ToString(), "utilizator");
            string serverToInsert = getValueBetweenTags(arrCollection[0].ToString(), "server");
            string tabelaToInsert = getValueBetweenTags(arrCollection[0].ToString(), "tabela");
            string operatieHToInsert = getValueBetweenTags(arrCollection[0].ToString(), "operatie");
            string queryToInsert = getValueBetweenTags(arrCollection[0].ToString(), "query");
            string dataToInsert = getValueBetweenTags(arrCollection[0].ToString(), "data"); ;

            // query pentru inserare in tabela
            string Query = "insert into [dcsv75].[RBD].[dbo].[rbd_audit_operatii_efectuate](operatie, ip, utilizator, server, tabela, operatie_h, query, data) values ('" + arrCollection[0].ToString() + "'" + ", " + "'" + ipToInsert + "'" + ", " + "'" + utilizatorToInsert + "'" + ", " + "'" + serverToInsert + "'" + ", " + "'" + tabelaToInsert + "'" + ", " + "'" + operatieHToInsert + "'" + ", " + "'" + queryToInsert + "'" + ", " + "'" + dataToInsert + "'" + ")";

            // todo here 05.02.2009

            SqlCommand Command = new System.Data.SqlClient.SqlCommand(Query.Replace("&#x07;", @"\"), Connection); // &#x07; = backslash

            if (Connection.State == ConnectionState.Closed)
                Connection.Open();

            Command.ExecuteNonQuery();
        }

        catch (SqlException se)
        {
            if (!(Connection.State == ConnectionState.Closed))
                Connection.Close();
        }
        // end insert new row in table
        //-----------------------------
    }
    #endregion audit methods

    #region Events Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        //<-- setare parametri conexiune
        //msg_lbl.Text = "";
        Session["TableSelected"] = strNumeTabelaErori;
        createTemplatedGridView();
        //--> setare parametri conexiune

    }

    public void tableGridView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        tableGridView.EditIndex = e.NewEditIndex;
        tableGridView.DataBind();
        // Session["SelecetdRowIndex"] = e.NewEditIndex;
        lblOperationStatus.Text = "";
    }

    public void tableGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        tableGridView.EditIndex = -1;
        tableGridView.DataBind();
        // Session["SelecetdRowIndex"] = -1;

    }

    public void tableGridView_RowUpdating(Object sender, GridViewUpdateEventArgs e)
    {
        SqlConnection Connection = new SqlConnection(strConnectionStringRBD);

        // adaug valorile din fiecare linie
        GridViewRow row = tableGridView.Rows[e.RowIndex];
        for (int i = 0; i < Table.Columns.Count; i++)
        {
            string field_value = ((TextBox)row.FindControl(Table.Columns[i].ColumnName)).Text;
            ParameterArray.Add(field_value);
        }

        string Query = "";

        Query = generateUpdateQuery();

        SqlCommand Command = new SqlCommand(Query, Connection);

        try
        {
            if (Connection.State == ConnectionState.Closed)
                Connection.Open();

            Command.ExecuteNonQuery();
            logOperation(strNumeServer, strNumeTabelaErori, "Modificare", Query);
            // Session["InsertFlag"] = (int)Session["InsertFlag"] == 1 ? 0 : 1;
        }
        catch (SqlException se)
        {
            msg_lbl.Text = se.ToString();
            MsgPanel.Visible = true;
            Connection.Close();
        }
        tableGridView.EditIndex = -1;
        createTemplatedGridView();

        lblOperationStatus.Text = "linia a fost modificata cu succes !";


    }

    protected void tableGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //createTemplatedGridView();
        tableGridView.PageIndex = e.NewPageIndex;
        tableGridView.DataBind();
    }

    protected void msg_button_Click(object sender, EventArgs e)
    {
        MsgPanel.Visible = false;
    }

    #endregion

    #region data table methods
    /// <summary>
    /// selecteaza datele pt dataset fara a lua in considerare tipul de rating
    /// </summary>
    protected void populateDataTable()
    {
        Table = new DataTable();
        tableGridView.Columns.Clear();

        SqlConnection connection = new SqlConnection(strConnectionStringRBD);
        SqlCommand command = new SqlCommand();

        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = strProceduraAducereDateConformPermissions;
        command.Connection = connection;
        // parametru pentru procedura
        command.Parameters.AddWithValue("UTILIZATOR", strUserAccessingPage); // click here

        // SqlDataAdapter adapter = new SqlDataAdapter(strProceduraAducereDateConformPermissions, connection);
        SqlDataAdapter adapter = new SqlDataAdapter();
        adapter.SelectCommand = command;

        try
        {
            adapter.Fill(Table); // TODO 23062009
            // bind and display the data
            DataReaderAdapter dar = new DataReaderAdapter();
            dar.FillFromReader(Table, readErrorLinesDependingOnPermissions(strUserAccessingPage));
        }
        catch (Exception ex)
        {
            msg_lbl.Text = ex.ToString();
            MsgPanel.Visible = true;
            connection.Close();
        }


    }

    /// <summary>
    /// intoarce liniile din fisierul de erori in functie de utilizator si de nivelul de acces
    /// </summary>
    protected SqlDataReader readErrorLinesDependingOnPermissions(string strUser)
    {
        SqlDataReader rdr = null;

        // start citire din fisier
        using (SqlConnection conn = new SqlConnection(strConnectionStringRBD))
        {
            try
            {

                // Instantiate a new command with a query and connection
                SqlCommand cmd = new SqlCommand(strProceduraAducereDateConformPermissions, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UTILIZATOR", strUser);

                // Open the connection
                conn.Open();

                // Call Execute reader to get query results
                rdr = cmd.ExecuteReader();
            }

            catch (FileNotFoundException ex)
            {
                throw ex;
            }

            finally
            {
                // close the reader
                if (rdr != null)
                {
                    rdr.Close();
                }

                // Close the connection
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
        // end citire din fisier

        return rdr;
    }

    /// <summary>
    /// creaza gridul pe baza datatable-ului primit
    /// </summary>
    protected void createTemplatedGridView()
    {
        // fill the table which is to bound to the GridView
        populateDataTable();
        // add templated fields to the GridView
        TemplateField BtnTmpField = new TemplateField();
        BtnTmpField.ItemTemplate =
            new DynamicallyTemplatedGridViewHandler(ListItemType.Item, "...", "Command");
        BtnTmpField.HeaderTemplate =
            new DynamicallyTemplatedGridViewHandler(ListItemType.Header, "...", "Command");
        BtnTmpField.EditItemTemplate =
            new DynamicallyTemplatedGridViewHandler(ListItemType.EditItem, "...", "Command");
        tableGridView.Columns.Add(BtnTmpField);

        for (int i = 0; i < Table.Columns.Count; i++)
        {
            // daca nu e coloana de id
            TemplateField ItemTmpField = new TemplateField();
            // create HeaderTemplate
            ItemTmpField.HeaderTemplate = new DynamicallyTemplatedGridViewHandler(ListItemType.Header,
                                                          Table.Columns[i].ColumnName,
                                                          Table.Columns[i].DataType.Name);
            // create ItemTemplate
            ItemTmpField.ItemTemplate = new DynamicallyTemplatedGridViewHandler(ListItemType.Item,
                                                          Table.Columns[i].ColumnName,
                                                          Table.Columns[i].DataType.Name);
            //create EditItemTemplate
            ItemTmpField.EditItemTemplate = new DynamicallyTemplatedGridViewHandler(ListItemType.EditItem,
                                                          Table.Columns[i].ColumnName,
                                                          Table.Columns[i].DataType.Name);
            // then add to the GridView
            tableGridView.Columns.Add(ItemTmpField);

        }

        /*
        tableGridView.DataSource = Table;
        tableGridView.DataBind();
        */
      
        tableGridView.DataSource = Table;
        tableGridView.DataBind();

        tableGridView.Columns[1].Visible = false;
    }

    /// <summary>
    /// genereaza query-ul pt update in tabela
    /// </summary>
    /// <returns></returns>
    protected string generateUpdateQuery()
    {
        int i = 0;
        string tempstr = "";
        int temp_index = -1;

        string Query = "";
        Query = "update  [" + strNumeTabelaErori + "] set ";

        for (i = 0; i < Table.Columns.Count; i++)
        {
            if (Table.Columns[i].ToString() != "id")
            {
                switch (Table.Columns[i].DataType.Name)
                {

                    case "Boolean":
                    case "Int32":
                    case "Byte":
                    case "Decimal":
                        if ((string)ParameterArray[i] == "True")
                            ParameterArray[1] = "1";
                        else if ((string)ParameterArray[i] == "False")
                            ParameterArray[i] = "0";

                        if (i == Table.Columns.Count - 1)
                            Query = Query + "[" + Table.Columns[i].ColumnName + "]=" + ParameterArray[i];
                        else
                            Query = Query + "[" + Table.Columns[i].ColumnName + "]=" + ParameterArray[i] + ", ";

                        break;
                    case "String":
                    case "Char":
                    case "DateTime":
                        if (((string)ParameterArray[i]).Contains("'"))
                        {
                            tempstr = ((string)ParameterArray[i - 1]);
                            ParameterArray[i] = ((string)ParameterArray[i]).Replace("'", "''");
                            temp_index = i;
                        }

                        if (i == Table.Columns.Count - 1)
                            Query = Query + "[" + Table.Columns[i].ColumnName + "]" + "='" + ParameterArray[i] + "' ";
                        else
                            Query = Query + "[" + Table.Columns[i].ColumnName + "]" + "='" + ParameterArray[i] + "', ";
                        break;

                }
            }
        }
        if (temp_index > -1)
            ParameterArray[temp_index] = tempstr;

        // conditia de where
        if (Table.Columns[0].DataType.Name == "String" || Table.Columns[0].DataType.Name == "DateTime" || Table.Columns[0].DataType.Name == "Char")
            Query += " where " + "[id]" + " = '" + ParameterArray[0] + "'";
        else
            Query += " where " + "[id]" + " = " + ParameterArray[0];

            // scriu in tabela de statusuri erori
        Query += "; update [RBD_ISTORIC_VERIFICARE_DETALIU_ERORI] set [status] = 'S'" + ", [data_solutionare] = '" + DateTime.Now + "' where [id] = " + ParameterArray[0];

        return Query;
    }
    #endregion data table methods

    #region grid main class
    /// <summary>
    /// Article: Dynamically Templated GridView with Edit,Insert and Delete Options
    /// Author: G. Mohyuddin
    /// Brief Notes: This class implements ITemplate which is resposible to create template fields of 
    /// the GridView dynamically and also to add buttons for Edit,Delete and Insert.
    /// </summary>
    public class DynamicallyTemplatedGridViewHandler : ITemplate
    {
        #region data memebers

        ListItemType ItemType;
        string FieldName;
        string InfoType;

        #endregion

        #region constructor

        public DynamicallyTemplatedGridViewHandler(ListItemType item_type, string field_name, string info_type)
        {
            ItemType = item_type;
            FieldName = field_name;
            InfoType = info_type;
        }

        #endregion

        #region Methods

        public void InstantiateIn(System.Web.UI.Control Container)
        {
            switch (ItemType)
            {
                case ListItemType.Header:
                    Literal header_ltrl = new Literal();
                    header_ltrl.Text = "<b>" + FieldName + "</b>";
                    Container.Controls.Add(header_ltrl);
                    break;
                case ListItemType.Item:
                    switch (InfoType)
                    {
                        case "Command":
                            LinkButton edit_button = new LinkButton();
                            edit_button.ID = "edit_button";
                            edit_button.Text = " [editare] ";
                            edit_button.CommandName = "Edit";
                            edit_button.Click += delegate { new Page().Session["InsertFlag"] = 0; };
                            edit_button.ToolTip = "editare";
                            Container.Controls.Add(edit_button);

                            /* Similarly add button for insert.
                             * It is important to know when 'insert' button is added 
                             * its CommandName is set to "Edit"  like that of 'edit' button 
                             * only because we want the GridView enter into Edit mode, 
                             * and this time we also want the text boxes for corresponding fields empty*/

                            break;

                        default:
                            Label field_lbl = new Label();
                            field_lbl.ID = FieldName;
                            field_lbl.Text = String.Empty; //we will bind it later through 'OnDataBinding' event
                            field_lbl.DataBinding += new EventHandler(OnDataBinding);
                            Container.Controls.Add(field_lbl);
                            break;

                    }
                    break;
                case ListItemType.EditItem:
                    if (InfoType == "Command")
                    {
                        LinkButton update_button = new LinkButton();
                        update_button.ID = "update_button";
                        update_button.CommandName = "Update";
                        update_button.Text = " [ok] ";

                        update_button.ToolTip = "Update";

                        update_button.OnClientClick = "return confirm('Are you sure to update this record?')";
                        Container.Controls.Add(update_button);

                        LinkButton cancel_button = new LinkButton();
                        cancel_button.Text = " [anulare] ";
                        cancel_button.ID = "cancel_button";
                        cancel_button.CommandName = "Cancel";
                        cancel_button.ToolTip = "anulare";
                        Container.Controls.Add(cancel_button);

                    }
                    else// for other 'non-command' i.e. the key and non key fields, bind textboxes with corresponding field values
                    {
                        TextBox field_txtbox = new TextBox();
                        field_txtbox.ID = FieldName;
                        field_txtbox.Text = String.Empty;
                        // if Inert is intended no need to bind it with text..keep them empty
                        if ((int)new Page().Session["InsertFlag"] == 0)
                            field_txtbox.DataBinding += new EventHandler(OnDataBinding);
                        Container.Controls.Add(field_txtbox);

                    }
                    break;

            }

        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// sets the insert flag OFF so that we ll be able to decide in OnRowUpdating event whether to insert or update 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void edit_button_Click(Object sender, EventArgs e)
        {
            new Page().Session["InsertFlag"] = 0;
        }

        private void OnDataBinding(object sender, EventArgs e)
        {

            object bound_value_obj = null;
            Control ctrl = (Control)sender;
            IDataItemContainer data_item_container = (IDataItemContainer)ctrl.NamingContainer;
            bound_value_obj = DataBinder.Eval(data_item_container.DataItem, FieldName);

            switch (ItemType)
            {
                case ListItemType.Item:
                    Label field_ltrl = (Label)sender;
                    field_ltrl.Text = bound_value_obj.ToString();

                    break;
                case ListItemType.EditItem:
                    TextBox field_txtbox = (TextBox)sender;
                    field_txtbox.Text = bound_value_obj.ToString();

                    break;
            }


        }

        #endregion
    }
    #endregion grid main class
}

#region export
/// <summary>
/// realizeaza conversia intre un datareader si un datatable. Converteste datareader in datatable
/// </summary>
public class DataReaderAdapter : System.Data.Common.DataAdapter
{
    /// <summary>
    /// realizeaza conversia intre un datareader si un datatable. Converteste datareader in datatable
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="dataReader"></param>
    /// <returns></returns>
    public int FillFromReader(DataTable dataTable, IDataReader dataReader)
    {
        return this.Fill(dataTable, dataReader);
    }
}

#endregion export