using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace FileWatch
{
	
	public class Form1 : System.Windows.Forms.Form
	{
		[DllImport("user32.dll")]
		public static extern bool  MessageBeep(int Sound);
		[DllImport("user32.dll")]
		public static extern int   MessageBox(int hWnd, String text, 
			String caption, uint type);
		
		// public FileSystemWatcher watcher;
		public string[] m_textFilters;// Arry to hold filter supplied by txtTextFilter coltrol
        //{{{
        //FileSystemWatcher[] fsWatcherList = new FileSystemWatcher[20];
       // int fsWatcherCount = 0;
        //}}}
        
        // private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.ListBox lstCreatedFilesFiles;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.ListBox lstCreatedFiles;
		private System.Windows.Forms.NotifyIcon notifyIcon1;
        // private System.Windows.Forms.Timer timer1;
        private GroupBox groupBox3;
        private Label lblCreated;
        private TabPage tabCreatedFiles;
		private System.ComponentModel.IContainer components;

        private string strConnectionStringRBD = @"Server=adimihailescu\ADM_SQL2005; Database=RBD; User ID=user_rbd; Password=!@#rbd$%^";
        
        // mapa si filtrul pentru fisierele pe care le urmareste
        private string strMapToWatch = "Q:\\";
        private Label label1;
        private string strFilterToWatch = "*";

        #region form methods
        public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			
            // m_starting_path.Text = "C:\\Temp";
			
            // watcher = new FileSystemWatcher();
		}

		public bool isExcluded(string txt)
		{
			return true;
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabCreatedFiles = new System.Windows.Forms.TabPage();
            this.lstCreatedFiles = new System.Windows.Forms.ListBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            // this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblCreated = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabCreatedFiles.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabCreatedFiles);
            this.tabControl1.ItemSize = new System.Drawing.Size(0, 18);
            this.tabControl1.Location = new System.Drawing.Point(8, 14);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(544, 276);
            this.tabControl1.TabIndex = 38;
            // 
            // tabCreatedFiles
            // 
            this.tabCreatedFiles.Controls.Add(this.lstCreatedFiles);
            this.tabCreatedFiles.Location = new System.Drawing.Point(4, 22);
            this.tabCreatedFiles.Name = "tabCreatedFiles";
            this.tabCreatedFiles.Size = new System.Drawing.Size(536, 250);
            this.tabCreatedFiles.TabIndex = 0;
            this.tabCreatedFiles.Text = "Fisiere adaugate in mapa de la ultima scanare";
            this.tabCreatedFiles.UseVisualStyleBackColor = true;
            // 
            // lstCreatedFiles
            // 
            this.lstCreatedFiles.HorizontalScrollbar = true;
            this.lstCreatedFiles.Location = new System.Drawing.Point(0, 0);
            this.lstCreatedFiles.Name = "lstCreatedFiles";
            this.lstCreatedFiles.ScrollAlwaysVisible = true;
            this.lstCreatedFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstCreatedFiles.Size = new System.Drawing.Size(533, 238);
            this.lstCreatedFiles.TabIndex = 25;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Folder Listener Utility";
            this.notifyIcon1.Visible = true;
            // 
            // timer1
            // 
            //this.timer1.Interval = 500;
            //this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.lblCreated);
            this.groupBox3.Location = new System.Drawing.Point(10, 291);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(542, 88);
            this.groupBox3.TabIndex = 42;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Rezultate monitorizare mapa:";
            // 
            // lblCreated
            // 
            this.lblCreated.Location = new System.Drawing.Point(104, 16);
            this.lblCreated.Name = "lblCreated";
            this.lblCreated.Size = new System.Drawing.Size(72, 16);
            this.lblCreated.TabIndex = 29;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "fisiere create:";
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(564, 389);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.tabControl1);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Folder watcher";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabCreatedFiles.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}
		//-----------------------------------------------------------------------
		private void ExitButtonClick(object sender, System.EventArgs e)
		{
			Application.Exit();
		}
		//-----------------------------------------------------------------------

    //  private delegate int addDelegate(object o);

		/// <summary>
		/// apelata la deschiderea formului
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void Form1_Load(object sender, System.EventArgs e)
		{	// Disable text boxes having paths
			// m_starting_path.Enabled=false;
			////txtBasePath.Enabled=false;

            ///////////////////////////////////////
            //if (watcher.EnableRaisingEvents == true)
            //{
            //    MessageBeep(100);
            //    return;
            //}

            // m_textFilters = txtTextFilter.Text.Split(';');// Reset Text Filters


            // Set Control Status

            lstCreatedFiles.Items.Clear();
            //btnBaseFolderBrowse.Enabled=false;
            //Start Timer  to enable Forml Caption Blinking
            // timer1.Start();
            // Set Task bar Icon Text
            notifyIcon1.Text = "Listening";

            this.FindForm().Text = "Scanare director pentru fisiere noi. Inceput la: " + DateTime.Now;

            //Filter duplicate Entry

            int upperLimit = alGetFilesInWatchFolder().Count;
            ArrayList alFilesList = alGetFilesInWatchFolder();

            for (int i = 0; i < upperLimit; i++)
            {

                    // lstCreatedFiles.Items.Add(txtFileChange);
                    lstCreatedFiles.TopIndex = lstCreatedFiles.Items.Count - 1;
                    lblCreated.Text = lstCreatedFiles.Items.Count.ToString();

                    //<-- start import fisier in baza

                    // daca fisierul nu se afla in lista fisierelor deja vizionate
                    string strNumeFisier = alFilesList[i].ToString();

                    if (!isFileAlreadyInWatchedList(strNumeFisier))
                    {
                        // import fisierul
                        importFileRowsInHistoryFile(getFileDateFromDateName(strNumeFisier.ToString().Replace(strMapToWatch, "")), strNumeFisier);
                        // il adaug la lista de fisiere deja vizionate
                        ruleazaComanda("insert into [rbd_fisiere_in_mapa] values ('" + strNumeFisier + "')");

                        lstCreatedFiles.Items.Add(strMapToWatch + strNumeFisier);
                    }
                    //--> end import fisier in baza
            }
			
		}

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //watcher.EnableRaisingEvents = false;		//Stop looking

            ///////////////////////
            //watcher.EnableRaisingEvents = false;
            //// groupBox1.Enabled = true;
            ////btnBaseFolderBrowse.Enabled=true;
            ////Start Timer  to enable Forml Caption Blinking
            //timer1.Stop();
            Form1.ActiveForm.Text = "Folder Listener";
            /////////////////////
        }

		
		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Save Text Filters 
			TextWriter tw = new StreamWriter("FolderListnerSettings.txt");
			// tw.WriteLine(txtTextFilter.Text);// Text Filters
			//tw.WriteLine(txtBasePath.Text);//Base folder path
			// tw.WriteLine(m_filter.Text);// File Type filter
			tw.Close();

			//MessageBox(0,"","",0);

            notifyIcon1.Dispose();
		}

		private void groupBox3_Enter(object sender, System.EventArgs e)
		{
			
		}

		private void Form1_Resize(object sender, System.EventArgs e)
		{
			
			
			if ((this.WindowState == FormWindowState.Minimized )&& Form1.ActiveForm==this)
			
			{
				if ((this.ShowInTaskbar == true))
				{
					this.ShowInTaskbar = false;
					//timer1.Stop();
				}
				else
				{
					this.ShowInTaskbar = true;				
					//timer1.Start();
				}
			}

		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			// Handle timer evnets only when if window not minimized
		}

		private void Form1_Activated(object sender, System.EventArgs e)
		{
            // timer1.Start();
		}
        		
		/// <summary>
		/// indicates that weather form caption is changed or not  Used only in time event to show the listing Status 
		/// </summary>
		public bool FormTextStatus
		{
			get
			{
				return true;
			}

			set
			{
			}
		}
        #endregion form methods
        //--------------------------------------------------------------------

        #region database operations
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
        /// importa un fisier in istoricul din baza cu data dtDataVerificare si numele strFileName
        /// </summary>
        /// <param name="dtDataVerificare"></param>
        /// <param name="strFileName"></param>
        protected void importFileRowsInHistoryFile(DateTime dtDataVerificare, string strFileName)
        {
                // string strLine = "RLRA 0000000000000J128298000021GABRIEL-CRISTIAN                                                                                    FARCAS                                            000000000000000,00             1981-03-25M4          PA                                                                                          I0000000000000J128298                              ";
            if (!isRecordAlreadyInFile(dtDataVerificare, strFileName))
            {
                try
                {
                    ArrayList alAllItems = new ArrayList();
                    alAllItems = readMergedLinesFromFile(strMapToWatch + strFileName);

                    // pentru fiecare linie din fisierul merge-uit, o adaug atat  in istoricul de fisier detaliat cat si de istoric cu fiecare linie
                    for (int i = 0; i < alAllItems.Count; i++)
                    {
                        // daca este o linie care incepe cu "RLRA"
                        if (alAllItems[i].ToString().StartsWith("RLRA"))
                        {
                            // rulez comanda pentru importul liniei
                            ruleazaComanda(buildInsertQueryInIstoricImport(dtDataVerificare.ToString("MM/dd/yyyy"), strFileName, alAllItems[i].ToString())); // o inserez in tabela de importuri
                        }
                    }

                }

                catch (Exception ex)
                {
                    throw ex; // todo 27062009
                }
            }
        }

        /// <summary>
        /// construieste stringul pt insertul unei linii in istoricul liniilor importate
        /// </summary>
        /// <param name="strLinie"></param>
        /// <returns></returns>
        protected string buildInsertQueryInIstoricImport(string strData, string strFileName, string strLinie)
        {
            	StringBuilder strTemp = new StringBuilder();

		// apoi inserez si linia importata in fisierul de istoric
		strTemp.Append("; insert into [rbd_istoric_import_linie] values (");
		strTemp.Append("'").Append(strData).Append("'"); // data importului;
		strTemp.Append(", '").Append(strFileName).Append("'"); // numele fisierului importat;
		strTemp.Append(", '").Append(strLinie.Replace("'", "''")).Append("'"); // linia importata;
		strTemp.Append("); ");

            return strTemp;
        }

        /// <summary>
        /// returneaza lista fisierelor din mapa
        /// </summary>
        /// <returns></returns>
        protected ArrayList alGetFilesInWatchFolder()
        {
            ArrayList alTemp = new ArrayList();
            DirectoryInfo diToWatch = new DirectoryInfo(strMapToWatch);
            FileInfo[] fiToWatch = diToWatch.GetFiles();

            int upperLimit = fiToWatch.Length;

            for (int i = 0; i < upperLimit; i++)
            {
                alTemp.Add(fiToWatch[i]);
            }

            return alTemp;
        }
        
        /// <summary>
        /// verifica inregistrarile pentru o zi si un nume de fisier se afla deja in fisierul de istoric. Cautarea se va face dupa data si fisier
        /// </summary>
        /// <param name="dtToCheck"></param>
        /// <param name="strFileToCheck"></param>
        /// <returns></returns>
        protected bool isRecordAlreadyInFile(DateTime dtToCheck, string strFileToCheck)
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

        /// <summary>
        /// verifica inregistrarile pentru o zi si un nume de fisier se afla deja in fisierul de istoric. Cautarea se va face dupa data si fisier
        /// </summary>
        /// <param name="dtToCheck"></param>
        /// <param name="strFileToCheck"></param>
        /// <returns></returns>
        protected bool isFileAlreadyInWatchedList(string strFileToCheck)
        {
            bool tempRez = false;
            string strRez = String.Empty;

            //<-- start verificare in tabela rbd_istoric_import_detaliu daca mai am o data recordurile in fisier
            SqlConnection connection = new SqlConnection(strConnectionStringRBD);
            string query = "if exists(select nume_fisier from dbo.rbd_fisiere_in_mapa where (nume_fisier = '" + strFileToCheck + "')) select 'da' as [rezultat] else select 'nu'  as [rezultat]";

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

        /// <summary>
        /// citeste liniile din fisier si merge-uieste liniile din fisier in cazul in care am gasit o persoana fizica
        /// </summary>
        /// <returns></returns>
        protected ArrayList readMergedLinesFromFile(string strFileName)
        {
            // todo 05062009 aici apare o problema
            ArrayList tempAlItems = new ArrayList();
            tempAlItems = readLinesFromFile(strFileName);

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
        /// citeste linie cu line dintr-un fisier text
        /// </summary>
        protected ArrayList readLinesFromFile(string strFileName)
        {
            ArrayList alTemp = new ArrayList();

            string line = "";

            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(strFileName);
            while ((line = file.ReadLine()) != null)
            {
                alTemp.Add(line);
            }

            file.Close();
            
            return alTemp;
        }
        #endregion database operations

        #region verificari
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
        /// verifica daca o litera este cifra
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected bool isDigit(char s)
        {
            return Char.IsDigit(s);
        }

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
        #endregion verificari
    } //end class


} //end Namespace
