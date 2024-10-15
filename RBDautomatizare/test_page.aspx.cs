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
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using HelperAPI;

public partial class test_page : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        //////////////////
        try
        {
            NetworkHelper networkhelper;
            networkhelper = NetworkHelper.GetInstance();

            bool result;

            /*
            result = networkhelper.MapDrive("M:", true);
            */

            // asta merge pentru alocarea mapei
            result = networkhelper.WNetAddConnection(@"Q:", @"\\10.150.0.235\cdnt\download\unprocessed", "oana", "romania", true);  // drive, cale, user, parola, force
            result = networkhelper.MapDrive("Q:", true);

            // asta merge pentru dealocarea mapei
            result = networkhelper.WNetCancelConnection("Q:", true);
        }

        catch (Exception ex)
        {
        }
        //////////////////
    }
}

namespace HelperAPI
{
    public partial class NetworkHelper
    {
        LogManager log = LogManager.GetInstance();
        XMLReader xmlreader = XMLReader.GetInstance();

        private static NetworkHelper m_object;

        private NetworkHelper()
        {
            bool success = xmlreader.ReadXML();
        }

        public static NetworkHelper GetInstance()
        {
            if (m_object == null)
            {
                m_object = new NetworkHelper();
            }
            return m_object;
        }

        public bool WNetAddConnection(string LocalDrive, string NetworkFolderPath, string User,
                        string Password, bool Force)
        {
            bool success = false;
            try
            {
                NetResource netresource = new NetResource();
                netresource.Scope = RESOURCE_GLOBALNET;
                netresource.Type = RESOURCETYPE_DISK;
                netresource.Usage = RESOURCEUSAGE_CONNECTABLE;
                netresource.DisplayType = RESOURCEDISPLAYTYPE_SHARE;
                netresource.LocalName = LocalDrive;
                netresource.RemoteName = NetworkFolderPath;
                netresource.Comment = "";
                netresource.Provider = "";

                int Flag = CONNECT_UPDATE_PROFILE;

                if (Force)
                {
                    success = WNetCancelConnection(LocalDrive, true);
                }

                int result = WNetAddConnection2A(ref netresource, Password, User, Flag);
                if (result > 0)
                {
                    throw new System.ComponentModel.Win32Exception(result);
                }
                success = true;
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
            }
            return success;
        }

        public bool WNetCancelConnection(string LocalDrive, bool Force)
        {
            bool success = false;
            try
            {
                int result = WNetCancelConnection2(LocalDrive, CONNECT_UPDATE_PROFILE, Force);
                if (result > 0)
                {
                    throw new System.ComponentModel.Win32Exception(result);
                }
                success = true;
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
            }
            return success;
        }

        public bool MapDrive(string LocalDrive, bool force)
        {
            bool success = false;
            try
            {
                NetworkInfoProperties networkinfo = (NetworkInfoProperties)xmlreader.HashList[LocalDrive];
                success = WNetAddConnection(networkinfo.LocalDrive, networkinfo.NetworkfolderPath, networkinfo.UserName,
                                      networkinfo.Password, force);

            }
            catch (Exception e)
            {
                log.Debug(e.Message);
            }
            return success;

        }
    }

    public class LogManager : IDisposable
    {
        private static LogManager m_object;
        private static TextWriter textwriter;
        static LogManager()
        {
            textwriter = new StreamWriter("Helper.log", true);
        }
        public static LogManager GetInstance()
        {
            if (m_object == null)
            {
                m_object = new LogManager();
            }
            return m_object;
        }

        public void Debug(string Message)
        {
            if (textwriter != null)
            {
                textwriter.WriteLine(Message);
            }
        }

        ~LogManager()
        {
            textwriter.Close();
        }

        #region IDisposable Members
        public void Dispose()
        {
            textwriter.Close();
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    /// <summary>
    /// XMLReader is used to read the properties.xml file 
    /// </summary>
    public class XMLReader
    {
        private LogManager log = LogManager.GetInstance();
        private const string FILE_NAME = "properties.xml";
        private static XMLReader m_object;

        private Hashtable hashlist;
        public static XMLReader GetInstance()
        {
            if (m_object == null)
            {
                m_object = new XMLReader();
            }
            return m_object;
        }

        public Hashtable HashList
        {
            get
            {
                return hashlist;
            }
        }

        /// <summary>
        /// This function will deserialize the xml and put into Hashtable
        /// </summary>
        /// <returns></returns>
        public bool ReadXML()
        {
            bool success = false;
            try
            {
                Type[] arrType = new Type[1];
                arrType[0] = typeof(NetworkInfoProperties);

                XmlSerializer xml = new XmlSerializer(typeof(ArrayList), arrType);
                FileStream fStream = new FileStream(FILE_NAME, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite);
                ArrayList arrlist = (ArrayList)xml.Deserialize(fStream);
                ConvertToHash(arrlist);
                success = true;
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
            }
            return success;
        }


        /// <summary>
        /// This function will convert the arraylist to hashtable
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        private void ConvertToHash(ArrayList arr)
        {
            hashlist = new Hashtable();
            lock (hashlist)
            {
                NetworkInfoProperties networkinfoproperties;
                for (int iCount = 0; iCount < arr.Count; iCount++)
                {
                    networkinfoproperties = (NetworkInfoProperties)arr[iCount];
                    hashlist.Add(networkinfoproperties.LocalDrive, networkinfoproperties);
                }
            }
        }
    }

    public partial class NetworkHelper
    {

        #region "Constants"

        //NetResource Scope
        private const int RESOURCE_CONNECTED = 0x00000001;
        private const int RESOURCE_GLOBALNET = 0x00000002;
        private const int RESOURCE_REMEMBERED = 0x00000003;

        //NetResource Type
        private const int RESOURCETYPE_ANY = 0x00000000;
        private const int RESOURCETYPE_DISK = 0x00000001;
        private const int RESOURCETYPE_PRINT = 0x00000002;

        //NetResource Usage
        private const int RESOURCEUSAGE_CONNECTABLE = 0x00000001;
        private const int RESOURCEUSAGE_CONTAINER = 0x00000002;


        //NetResource Display Type
        private const int RESOURCEDISPLAYTYPE_GENERIC = 0x00000000;
        private const int RESOURCEDISPLAYTYPE_DOMAIN = 0x00000001;
        private const int RESOURCEDISPLAYTYPE_SERVER = 0x00000002;
        private const int RESOURCEDISPLAYTYPE_SHARE = 0x00000003;
        private const int RESOURCEDISPLAYTYPE_FILE = 0x00000004;
        private const int RESOURCEDISPLAYTYPE_GROUP = 0x00000005;

        //Flags
        private const int CONNECT_UPDATE_PROFILE = 0x00000001;
        private const int CONNECT_UPDATE_RECENT = 0x00000002;
        private const int CONNECT_TEMPORARY = 0x00000004;
        private const int CONNECT_INTERACTIVE = 0x00000008;
        private const int CONNECT_PROMPT = 0x00000010;
        private const int CONNECT_NEED_DRIVE = 0x00000020;

        #endregion

        #region "Structures"

        /// <summary>
        /// NetResource contains information about a network resource
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct NetResource
        {
            public int Scope;
            public int Type;
            public int DisplayType;
            public int Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }
        #endregion


        #region "Win32 functions"

        [DllImport("mpr.dll", EntryPoint = "WNetAddConnection2A", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int WNetAddConnection2A(ref NetResource netresource, string password, string username, int flags);

        [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int WNetCancelConnection2(string drivename, int flag, bool force);

        #endregion

    }

    public class NetworkInfoProperties
    {

        private string username;
        private string password;
        private string localdrive;
        private string networkfolderpath;

        public string UserName
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
            }
        }

        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }

        public string LocalDrive
        {
            get
            {
                return localdrive;
            }
            set
            {
                localdrive = value;
            }
        }

        public string NetworkfolderPath
        {
            get
            {
                return networkfolderpath;
            }
            set
            {
                networkfolderpath = value;
            }
        }

    }
}

