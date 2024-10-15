using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.IO;
using System.Text;
using System.Net;

public partial class _Default : System.Web.UI.Page
{
    string strProceduraCitireDinFisierLog = "SP_CITESTE_FISIER_TEXT";
    string strConnectionStringFisierText = "Server=dcsv75; Database=RBD; User ID=user_rbd; Password=!@#rbd$%^";
    string strConnectionStringRBD = "Server=dcsv75; Database=RBD; User ID=user_rbd; Password=!@#rbd$%^";

    DateTime dtDataVerificare = DateTime.Now;

    #region page methods
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    #endregion page methods

    #region export in flat file
    
    /// <summary>
    /// exporta un datareader in fisier text
    /// </summary>
    /// <param name="fileOut"></param>
    /// <param name="dr"></param>
    private void exportToFlatFile(string fileOut)
    {
        //<- start creare datareader cu rezultatele pt export
        SqlDataReader rdr = null;

        // start citire din fisier
        SqlConnection connReader = new SqlConnection(strConnectionStringRBD);
        try
            {

                // Instantiate a new command with a query and connection
                SqlCommand cmdReader = new SqlCommand("select cod_client, rating, data_rating from [RBD_ISTORIC_VERIFICARE_DETALIU_CORECT]", connReader);
                cmdReader.CommandType = CommandType.Text;

                // Open the connection
                connReader.Open();

                // Call Execute reader to get query results
                rdr = cmdReader.ExecuteReader();
            }

            catch (FileNotFoundException ex)
            {
                throw ex;
            }

        // end citire din fisier
        //-> end creare datareader cu rezultatele pt export
        
        // Creates the flat file as a stream, using the given encoding.
        // StreamWriter sw = new StreamWriter(fileOut, false, "Unicode");
        StreamWriter sw = new StreamWriter(fileOut);
        string strRow; // represents a full row

        // Reads the rows one by one from the SqlDataReader
        // transfers them to a string with the given separator character and
        // writes it to the file.
        while (rdr.Read())
        {
            strRow = "";
            for (int i = 0; i < rdr.FieldCount; i++)
            {
                if (rdr.GetValue(i) != null)
                {
                    strRow += rdr.GetValue(i).ToString();
                    if (i < rdr.FieldCount - 1)
                    {
                        strRow += "\t"; // separator tab
                        // strRow += this.separator;
                    }
                }
            }
            
            //scriu linia in fisier
            sw.WriteLine(strRow);
        }

        sw.Close();

        // start inchidere data reader
        if (rdr != null)
        {
            rdr.Close();
        }
        
        // Close the connection
        if (connReader != null)
        {
            connReader.Close();
        }
        // end inchidere data reader
    }


    #endregion export in flat file

    #region validation methods
    /// <summary>
    /// intoarce data fisierului dupa numele fisierului
    /// </summary>
    /// <param name="strFileName"></param>
    /// <returns></returns>
    protected DateTime getFileDateFromDateName(string strFileName)
    {
        // data fisierului este data de primele 6 caractere din numele fisierului
        string strData = strFileName.Substring(0, 8);

        int tempAn = Convert.ToInt32(strData.Substring(0, 4)); // yyyy
        int tempLuna = Convert.ToInt32(strData.Substring(4, 2)); // mm
        int tempZi = Convert.ToInt32(strData.Substring(6, 2)); // dd

        int an = tempAn;
        int luna = ((tempLuna.ToString()[0] == '0') ? tempLuna.ToString()[1] : tempLuna);
        int zi = ((tempZi.ToString()[0] == '0') ? tempZi.ToString()[1] : tempZi);

        return new DateTime(an, luna, zi); // returneaza numele datei
    }
    
    /// <summary>
    /// verifica daca o litera este cifra
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    protected bool isDigit(char s)
    {
        return Char.IsDigit(s);
    }

    /// <summary>
    /// verifica daca un cnum contine J, C, M
    /// </summary>
    /// <param name="strCnum"></param>
    /// <returns></returns>
    protected bool isValidCNUM(string strCnum)
    {
        return strCnum.IndexOfAny(new char[] { 'J', 'j', 'C', 'c', 'M', 'm' }) > -1;
    }

    /// <summary>
    /// verifica daca o linie este a unei persoane fizice sau juridice 
    /// </summary>
    /// <param name="strLine"></param>
    /// <returns></returns>
    protected bool isPhysicalPerson(string strLine)
    {
        // verifica daca sirul "PA" este in linie si este dupa prima aparitie a unei variabile de tip data
        return (strLine.IndexOf("PA") > getDatePositionInLine(strLine));
    }

    /// <summary>
    /// verifica daca in sir este cuprins un subsir de forma YYYY-MM-DD si intoarce prima pozitie de aparitie
    /// </summary>
    /// <param name="strLine"></param>
    /// <returns></returns>
    protected int getDatePositionInLine(string strLine)
    {
        if (strLine.StartsWith("RLRA")) // daca sunt pe o linie care incepe cu RLRA
        {
            for (int i = 0; i < strLine.Length; i++)
            {
                bool isYYYY = (isDigit(strLine[i]) && isDigit(strLine[i + 1]) && isDigit(strLine[i + 2]) && isDigit(strLine[i + 3]));
                bool isDDorMM = (isDigit(strLine[i + 5]) && isDigit(strLine[i + 6]) && isDigit(strLine[i + 8]) && isDigit(strLine[i + 9]));

                if (isYYYY && (strLine[i + 4] == '-') && (strLine[i + 7] == '-'))
                    return (strLine.IndexOf(strLine[i].ToString() + strLine[i + 1].ToString() + strLine[i + 2].ToString() + strLine[i + 3].ToString() + "-" + strLine[i + 5].ToString() + strLine[i + 6].ToString() + "-" + strLine[i + 8].ToString() + strLine[i + 9].ToString()));
            }
        }

        return -1;
    }

        /// <summary>
        /// verifica inregistrarile pentru o zi si un nume de fisier se afla deja in fisierul de istoric. Cautarea se va face dupa data si fisier
        /// </summary>
        /// <param name="dtToCheck"></param>
        /// <param name="strFileToCheck"></param>
        /// <returns></returns>
        protected bool isRecordAlreadyInFile(string strFileToCheck)
        {
            bool tempRez = false;
            string strRez = String.Empty;

            //<-- start verificare in tabela rbd_istoric_import_detaliu daca mai am o data recordurile in fisier
            SqlConnection connection = new SqlConnection(strConnectionStringRBD);
            string query = "if exists(select data_fisier from dbo.rbd_istoric_import_detaliu where ((nume_fisier = '" + strFileToCheck + "') and convert(varchar(10), data_fisier, 101) = convert(varchar(10), '" + dtToCheck.ToString("MM/dd/yyyy") + "', 101))) select 'da' as [rezultat] else select 'nu'  as [rezultat]";

            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                strRez = command.ExecuteScalar().ToString();
            }

            catch (SqlException ex)
            {
                throw ex;
            }

            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            //--> verificare in tabela rbd_istoric_import_detaliu daca mai am o data recordurile in fisier

            return (strRez == "da");
        }
    #endregion validation methods

    #region file methods
    
    /// <summary>
    /// intoarce numele fisierului
    /// </summary>
    /// <returns></returns>
    protected string buildFileName()
    {
        // hardcodat pt teste
        // return @"d:\adp\prod_files\spec_10.00\test_folder\20090116.010605.CDUT0435.BZ090115";
        return @"\\Dcsv75\spec_10.00\test_folder\20090116.010605.CDUT0435.BZ090115";
    }

    /// <summary>
    /// citeste linie cu line dintr-un fisier
    /// </summary>
    protected ArrayList readLinesFromFile()
    {
        ArrayList alTemp = new ArrayList();

        // start citire din fisier
        using (SqlConnection conn = new SqlConnection(strConnectionStringFisierText))
        {
            SqlDataReader rdr = null;

            try
            {

                // Instantiate a new command with a query and connection
                SqlCommand cmd = new SqlCommand(strProceduraCitireDinFisierLog, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("nume_fisier", buildFileName()); // TODO 04052009

                // Open the connection
                conn.Open();

                // Call Execute reader to get query results
                rdr = cmd.ExecuteReader(); // todo 17062009

                // print the CategoryName of each record
                int rowCounter = 0;
                while (rdr.Read())
                {
                    rowCounter++;
                    if (rowCounter > 1)
                        alTemp.Add(rdr[0].ToString().Replace("\n", ""));
                }
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

        return alTemp;
    }

    /// <summary>
    /// citeste liniile din fisier si merge-uieste liniile din fisier in cazul in care am gasit o persoana fizica
    /// </summary>
    /// <returns></returns>
    protected ArrayList readMergedLinesFromFile()
    {
        // todo 05062009 aici apare o problema
        ArrayList tempAlItems = new ArrayList();
        tempAlItems = readLinesFromFile();

        ArrayList tempMergedAlItems = new ArrayList();
        int i = 0;

        while (i < tempAlItems.Count)
        {
            // if (tempAlItems[i].ToString().IndexOf("PA") >= 0)
            if (isPhysicalPerson(tempAlItems[i].ToString()))
            {
                tempMergedAlItems.Add(mergeFileLinesWhenCnumEqual(tempAlItems[i].ToString(), tempAlItems[i + 1].ToString()));
                i += 2; // daca e persoana fizica, PA, sar un rand din fisier
            }

            else
            {
                tempMergedAlItems.Add(tempAlItems[i]);
                i++; // daca nu e persoana fizica, (diferit de PA), nu sar nici un rand
            }
        }

        return tempMergedAlItems;
    }

    /// <summary>
    /// converteste JXXXXXX, CXXXXXX, MXXXXXX in cnum
    /// </summary>
    /// <param name="strCnum"></param>
    /// <returns></returns>
    protected string getCnumFromCustomerNumber(string strCnum)
    {
        string strTemp = String.Empty;

        if (strCnum.IndexOf("C") >= 0)
            strTemp = strCnum.Substring(strCnum.IndexOf("C"), 8);

        else if (strCnum.IndexOf("J") >= 0)
            strTemp = strCnum.Substring(strCnum.IndexOf("J"), 7);

        else if (strCnum.IndexOf("M") >= 0)
            strTemp = strCnum.Substring(strCnum.IndexOf("M"), 7);

        return strTemp;
    }

    /// <summary>
    /// intoarce un string cu linia 1 suprapusa peste linia 2, doar in cazul in care linia 2 contine un caracter care nu e in linia 1
    /// </summary>
    /// <param name="strLine1"></param>
    /// <param name="strLine2"></param>
    /// <returns></returns>
    protected StringBuilder mergeFileLinesWhenCnumEqual(string strLine1, string strLine2)
    {
        StringBuilder strTemp = new StringBuilder(strLine1);

        // parcurg fiecare caracter din string 1
        for (int i = 0; i < strLine1.Length; i++)
        {
            if ((strLine2[i] == ' ') && (strLine1[i] != ' '))
                strTemp[i] = strLine1[i];

            else if ((strLine1[i] == ' ') && (strLine2[i] == ' '))
                strTemp[i] = ' ';

            else if ((strLine1[i] != ' ') && (strLine2[i] != ' '))
                strTemp[i] = strLine2[i];

            else
                strTemp[i] = strLine2[i];
        }

        return strTemp;
    }

    /// <summary>
    /// verifica inregistrarile pentru o zi si un nume de fisier se afla deja in fisierul de istoric. Cautarea se va face dupa data si fisier
    /// </summary>
    /// <param name="dtToCheck"></param>
    /// <param name="strFileToCheck"></param>
    /// <returns></returns>
    protected ArrayList getLiniiPersoaneFizice()
    {
        bool tempRez = false;
        string strRez = String.Empty;
        ArrayList alListaLinii = new ArrayList();

        //<-- start verificare in tabela rbd_istoric_import_detaliu daca mai am o data recordurile in fisier
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(strConnectionStringRBD);
        string query = "select * from [rbd_istoric_import_detaliu] where sistem_rating = 'PA'";

        SqlCommand command = new System.Data.SqlClient.SqlCommand(query, connection);

        try
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            strRez = command.ExecuteScalar().ToString();
        }

        catch (SqlException ex)
        {
            connection.Close();
            throw ex;
        }
        //--> verificare in tabela rbd_istoric_import_detaliu daca mai am o data recordurile in fisier

        return alListaLinii;

        // return (strRez == "da");
    }

    /// <summary>
    /// verifica inregistrarile pentru o zi si un nume de fisier se afla deja in fisierul de istoric. Cautarea se va face dupa data si fisier
    /// </summary>
    /// <param name="dtToCheck"></param>
    /// <param name="strFileToCheck"></param>
    /// <returns></returns>
    protected ArrayList getLiniiPersoaneJuridice()
    {
        bool tempRez = false;
        string strRez = String.Empty;
        ArrayList alListaLinii = new ArrayList();

        //<-- start verificare in tabela rbd_istoric_import_detaliu daca mai am o data recordurile in fisier
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(strConnectionStringRBD);
        string query = "select * from [rbd_istoric_import_detaliu] where sistem_rating <> 'PA'";

        SqlCommand command = new System.Data.SqlClient.SqlCommand(query, connection);

        try
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            strRez = command.ExecuteScalar().ToString();
        }

        catch (SqlException ex)
        {
            connection.Close();
            throw ex;
        }
        //--> verificare in tabela rbd_istoric_import_detaliu daca mai am o data recordurile in fisier

        return alListaLinii;

        // return (strRez == "da");
    }
    
    /// <summary>
    /// creaza un datatable cu valorile returnate de multime
    /// </summary>
    /// <returns></returns>
    protected DataTable buildDataTableFromArrayLists()
    {
        ArrayList alAllItems = new ArrayList();
        alAllItems = readMergedLinesFromFile();

        DataTable dtAllValues = new DataTable();
        //------------------------------------
        // definire coloane pentru DataTable
        DataColumn dcSatzart = new DataColumn();
        dcSatzart.ColumnName = "SATZART";

        DataColumn dcCnum1 = new DataColumn();
        dcCnum1.ColumnName = "cnum1";

        DataColumn dcNume1 = new DataColumn();
        dcNume1.ColumnName = "nume1";

        DataColumn dcNume2 = new DataColumn();
        dcNume2.ColumnName = "nume2";

        DataColumn dcSuma1 = new DataColumn();
        dcSuma1.ColumnName = "suma1";

        DataColumn dcData1 = new DataColumn();
        dcData1.ColumnName = "data1";

        DataColumn dcData2 = new DataColumn();
        dcData2.ColumnName = "data2";

        DataColumn dcCar1 = new DataColumn();
        dcCar1.ColumnName = "car1";

        DataColumn dcData3 = new DataColumn();
        dcData3.ColumnName = "data3";

        DataColumn dcSistemRating = new DataColumn();
        dcSistemRating.ColumnName = "sistem_rating";

        DataColumn dcRating = new DataColumn();
        dcRating.ColumnName = "rating";

        DataColumn dcUser1 = new DataColumn();
        dcUser1.ColumnName = "user1";

        DataColumn dcUser2 = new DataColumn();
        dcUser2.ColumnName = "user2";

        DataColumn dcCnum2 = new DataColumn();
        dcCnum2.ColumnName = "cnum2";

        // adaugare coloane la DataTable
        dtAllValues.Columns.Add(dcSatzart); //rlra
        dtAllValues.Columns.Add(dcCnum1); // cnum
        dtAllValues.Columns.Add(dcNume1); // nume1
        dtAllValues.Columns.Add(dcNume2); // nume2
        dtAllValues.Columns.Add(dcSuma1); //suma1
        dtAllValues.Columns.Add(dcData1); //data1
        dtAllValues.Columns.Add(dcData2); //data2
        dtAllValues.Columns.Add(dcCar1); //doua caractere
        dtAllValues.Columns.Add(dcData3); //data3
        dtAllValues.Columns.Add(dcSistemRating); //sistem rating
        dtAllValues.Columns.Add(dcRating); //rating
        dtAllValues.Columns.Add(dcUser1); //user1
        dtAllValues.Columns.Add(dcUser2); //user2
        dtAllValues.Columns.Add(dcCnum2); //cnum2
        //------------------------------------
        //<-- start adaugare linii
        for (int i = 0; i < alAllItems.Count; i++)
        {
            string strLiniaCurenta = alAllItems[i].ToString();
           
            // pentru fiecare linie gasita
            if (strLiniaCurenta.IndexOf("RLRA") >= 0)
            {
                DataRow drCurrentRow = dtAllValues.NewRow();
                // adaugare valori pt coloane
                drCurrentRow["SATZART"] = "RLRA";
                drCurrentRow["cnum1"] = strLiniaCurenta.Substring(5, 26); // cnum, start 5, end 31, 26 caractere
                drCurrentRow["nume1"] = strLiniaCurenta.Substring(31, 100); // nume1
                drCurrentRow["nume2"] = strLiniaCurenta.Substring(131, 50); // nume2
                drCurrentRow["suma1"] = strLiniaCurenta.Substring(181, 21); // suma1
                drCurrentRow["data1"] = strLiniaCurenta.Substring(202, 10); // data1
                drCurrentRow["data2"] = strLiniaCurenta.Substring(212, 10); // data2
                drCurrentRow["car1"] = strLiniaCurenta.Substring(222, 2); // doua caractere
                drCurrentRow["data3"] = strLiniaCurenta.Substring(224, 10); // data3
                drCurrentRow["sistem_rating"] = strLiniaCurenta.Substring(234, 2); // sistem rating
                drCurrentRow["rating"] = strLiniaCurenta.Substring(241, 2); // rating
                drCurrentRow["user1"] = strLiniaCurenta.Substring(246, 40); // user1
                drCurrentRow["user2"] = strLiniaCurenta.Substring(286, 40); // user2
                drCurrentRow["cnum2"] = strLiniaCurenta.Substring(326, 50); // cnum2

                dtAllValues.Rows.Add(drCurrentRow);
            }
        }
        //--> end adaugare linii

        return dtAllValues;
    }

  
    /// <summary>
    /// construieste stringul pt insertul unei linii in istoricul liniilor importate
    /// </summary>
    /// <param name="strLinie"></param>
    /// <returns></returns>
    protected string buildInsertQueryInIstoricImport(string strLinie)
    {
        string strTemp = String.Empty;

        // citesc doar liniile care incep cu RLRA
            // date despre data si fisier
            strTemp += " insert into [rbd_istoric_import_detaliu] values (";
            strTemp += "'" + DateTime.Now.ToString("MM/dd/yyyy") + "'"; // data importului
            strTemp += ", '" + buildFileName() + "'"; // numele fisierului
            // start coloane fisier
            strTemp += ", 'RLRA'";
            strTemp += ", '" + strLinie.Substring(5, 26) + "'"; // cnum, start 5, end 31, 26 caractere;
            strTemp += ", '" + strLinie.Substring(31, 100).Replace("'", "''") + "'"; // nume1
            strTemp += ", '" + strLinie.Substring(131, 50).Replace("'", "''") + "'"; // nume2
            strTemp += ", '" + strLinie.Substring(181, 21) + "'"; // suma1
            strTemp += ", '" + strLinie.Substring(202, 10) + "'"; // data1
            strTemp += ", '" + strLinie.Substring(212, 10) + "'"; // data2;
            strTemp += ", '" + strLinie.Substring(222, 2) + "'"; // doua caractere;
            strTemp += ", '" + strLinie.Substring(224, 10) + "'"; // data3;
            strTemp += ", '" + strLinie.Substring(234, 2) + "'"; // sistem_rating;
            strTemp += ", '" + strLinie.Substring(241, 2) + "'"; // rating;
            strTemp += ", '" + strLinie.Substring(246, 40) + "'"; // user1;
            strTemp += ", '" + strLinie.Substring(286, 40) + "'"; // user2;
            strTemp += ", '" + strLinie.Substring(326, 50) + "'"; // cnum2;
            // end coloane fisier
            strTemp += ")";

            // apoi inserez si linia importata in fisierul de istoric
            strTemp += "; insert into [rbd_istoric_import_linie] values (";
            strTemp += "'" + DateTime.Now.ToString("MM/dd/yyyy") + "'"; // data importului;
            strTemp += ", '" + buildFileName() + "'"; // numele fisierului importat;
            strTemp += ", '" + strLinie.Replace("'", "''") + "'"; // linia importata;
            strTemp += "); ";

        return strTemp;
    }

        /// <summary>
        /// ruleaza o comanda de update pe tabela de istoric pentru fisierele importate
        /// </summary>
        /// <param name="strComanda"></param>
        protected void ruleazaComanda(string strComanda)
        {
            // daca exista deja inregistrari pt aceasta zi in fisierul importat in rbd..rbd_istoric_import_detaliu, nu il mai import
            SqlConnection connection = new SqlConnection(strConnectionStringRBD);
            string query = strComanda;

            SqlCommand command = new SqlCommand(query, connection);
            command.CommandType = CommandType.Text;
            command.CommandText = strComanda;

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                command.ExecuteNonQuery();
            }

            catch (SqlException ex)
            {
                throw ex;
            }

            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

    /// <summary>
    /// updateaza in baza datele despre erorile intalnite
    /// </summary>
    /// <param name="dtDataFisier"></param>
    /// <param name="strNumeFisier"></param>
    /// <param name="strStatus"></param>
    protected void updateEroareInBaza(DateTime dtDataFisier, string strNumeFisier, string strStatus)
    {
        // daca exista deja inregistrari pt aceasta zi in fisierul importat in rbd..rbd_istoric_import_detaliu, nu il mai import
        string query = "SP_UPDATE_EROARE_IN_BAZA";
        SqlConnection connection = new System.Data.SqlClient.SqlConnection(strConnectionStringRBD);
        SqlCommand command = new System.Data.SqlClient.SqlCommand(query, connection);

        command.CommandType = CommandType.StoredProcedure;
        // adaugare parametri procedura
        command.Parameters.AddWithValue("DATA_FISIER", dtDataFisier); // TODO 04052009
        command.Parameters.AddWithValue("NUME_FISIER", strNumeFisier);
        command.Parameters.AddWithValue("STATUS", strStatus);

        try
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            command.ExecuteNonQuery();
        }

        catch (SqlException ex)
        {
            connection.Close();
            throw ex;
        }
    }

    /// <summary>
    /// creaza un fisier cu numele specificat
    /// </summary>
    /// <param name="strNume"></param>
    protected void createFile(string strNume)
    {
        if (!(File.Exists(Server.MapPath(strNume))))
        {
            File.Create(strNume);
        }  
    }
    #endregion file methods

    #region events
    protected void btnBindGrid_Click(object sender, EventArgs e)
    {
        // GridView1.DataSource = readMergedLinesFromFile();
        GridView1.DataSource = buildDataTableFromArrayLists();
        GridView1.DataBind();

        GridView2.DataSource = readMergedLinesFromFile();
        GridView2.DataBind();
    }

    /// <summary>
    /// handles testBulk
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button1_Click(object sender, EventArgs e)
    {
        // string strLine = "RLRA 0000000000000J128298000021GABRIEL-CRISTIAN                                                                                    FARCAS                                            000000000000000,00             1981-03-25M4          PA                                                                                          I0000000000000J128298                              ";
        if (!isRecordAlreadyInFile(dtDataVerificare, buildFileName()))
        {
            try
            {
                ArrayList alAllItems = new ArrayList();
                alAllItems = readMergedLinesFromFile();

                // pentru fiecare linie din fisierul merge-uit, o adaug atat  in istoricul de fisier detaliat cat si de istoric cu fiecare linie
                for (int i = 0; i < alAllItems.Count; i++)
                {
                    // daca este o linie care incepe cu "RLRA"
                    if (alAllItems[i].ToString().StartsWith("RLRA"))
                        ruleazaComanda(buildInsertQueryInIstoricImport(alAllItems[i].ToString())); // o inserez in tabela de importuri
                }

                Response.Write("fisier importat pentru " + dtDataVerificare.ToString("MM/dd/yyyy"));
            }

            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        else
        {
            Response.Write("fisierul pentru " + dtDataVerificare.ToString("MM/dd/yyyy") + " este importat deja !");
        }
    }

    protected void btnTestEditareErori_Click(object sender, EventArgs e)
    {
        Response.Redirect("EditareErori.aspx");
    }
    #endregion events
    /// <summary>
    /// export in fisier text
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button2_Click(object sender, EventArgs e)
    {
        exportToFlatFile(Server.MapPath("~/corecte_18062009.txt"));
        lblStatusMessage.Text = "export facut";
    }
}
