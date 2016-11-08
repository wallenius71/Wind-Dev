using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Configuration;
using Helper.ListsService;
using System.Threading;

namespace Helper
{
    public class DocLibHelper
    {

        ListsService.Lists m_listService;

        ICredentials m_credentials;

        ListInfoCollection m_lists;

        static string processName = "DocLibHelper";

        public DocLibHelper()
        {

            //m_credentials = CredentialCache.DefaultCredentials;
            //m_credentials = new NetworkCredential("-lsd-sheam0", "", "HE1");
            m_credentials = new NetworkCredential(ConfigurationSettings.AppSettings["ServiceAccount"], ConfigurationSettings.AppSettings["ServiceAccountPW"], ConfigurationSettings.AppSettings["ServiceAccountDomain"]);

            m_listService = new ListsService.Lists();

            m_listService.Credentials = m_credentials;

            m_lists = new ListInfoCollection(m_listService);

        }

        public class ListInfo
        {

            public string m_rootFolder;

            public string m_listName;

            public string m_version;

            public string m_webUrl;

            public ListInfo(XmlNode listResponse)
            {

                m_rootFolder = listResponse.Attributes["RootFolder"].Value + "/";

                m_listName = listResponse.Attributes["ID"].Value;

                m_version = listResponse.Attributes["Version"].Value;

            }

            public bool IsMatch(string url)
            {

                try
                {

                    url += "/";

                    return url.Substring(0, m_rootFolder.Length) == m_rootFolder;

                }

                catch { }

                return false;

            }

        }

        public class ListInfoCollection : IEnumerable<ListInfo>
        {

            ListsService.Lists m_listService;

            Dictionary<string, ListInfo> m_lists = new Dictionary<string, ListInfo>();

            public ListInfoCollection(ListsService.Lists listService)
            {

                m_listService = listService;

            }

            public IEnumerator<ListInfo> GetEnumerator()
            {

                return m_lists.Values.GetEnumerator();

            }

            IEnumerator IEnumerable.GetEnumerator()
            {

                return this.GetEnumerator();

            }

            public ListInfo Find(FileInfo fileInfo)
            {

                if (m_lists.ContainsKey(fileInfo.LookupName))

                    return m_lists[fileInfo.LookupName];

                foreach (ListInfo li in m_lists.Values)

                    if (li.IsMatch(fileInfo.LookupName)) return li;

                string webUrl = fileInfo.m_URL;

                if (fileInfo.m_listInfo != null && !string.IsNullOrEmpty(fileInfo.m_listInfo.m_listName))
                {

                    ListInfo listInfo = new ListInfo(CallService(ref webUrl, delegate { return m_listService.GetList(fileInfo.LookupName); }));

                    listInfo.m_webUrl = webUrl;

                    return listInfo;

                }

                else
                {

                    XmlNode lists = CallService(ref webUrl, delegate { return m_listService.GetListCollection(); });

                    if (lists == null) throw new Exception("Could not find web.");

                    //Find list by RootFolder (which doesn't seem to be populated in GetListCollection response so must iterate GetList response)

                    foreach (XmlNode list in lists.ChildNodes)
                    {
                        ListInfo listInfo = null;

                        bool success = false;
                        while (!success)
                        {
                            // web exception issue
                            try
                            {
                                listInfo = new ListInfo(m_listService.GetList(list.Attributes["Name"].Value));
                                success = true;
                            }
                            catch
                            {
                                success = false;
                                Thread.Sleep(1000);
                            }
                        }

                        listInfo.m_webUrl = webUrl;

                        m_lists.Add(listInfo.m_listName, listInfo);

                        if (listInfo.IsMatch(fileInfo.LookupName))

                            return listInfo;

                    }

                }

                throw new Exception("Could not find list.");

            }

            private delegate XmlNode ServiceOperation();

            private XmlNode CallService(ref string webURL, ServiceOperation serviceOperation)
            {

                try
                {

                    webURL = webURL.Substring(0, webURL.LastIndexOf("/"));

                    try
                    {

                        m_listService.Url = webURL + "/_vti_bin/Lists.asmx";

                        return serviceOperation();

                    }

                    catch
                    {

                        return CallService(ref webURL, serviceOperation);

                    }

                }

                catch
                {

                    webURL = null;

                    return null;

                }

            }

        }

        public class FileInfo
        {

            public string m_URL;

            public byte[] m_bytes;

            public Dictionary<string, object> m_properties;

            public ListInfo m_listInfo;

            public bool m_ensureFolders = true;

            private Uri m_uri;

            public bool HasProperties
            {

                get { return m_properties != null && m_properties.Count > 0; }

            }

            public string RelativeFilePath
            {

                get { return m_URL.Substring(m_URL.IndexOf(m_listInfo.m_rootFolder) + 1); }

            }

            public Uri URI
            {

                get
                {

                    if (m_uri == null) m_uri = new Uri(m_URL);

                    return m_uri;

                }

            }

            public string LookupName
            {

                get
                {

                    if (m_listInfo != null && !string.IsNullOrEmpty(m_listInfo.m_listName))

                        return m_listInfo.m_listName;

                    return URI.LocalPath;

                }

            }

            public FileInfo(string url, byte[] bytes, Dictionary<string, object> properties)
            {

                m_URL = url.Replace("%20", " ");

                m_bytes = bytes;

                m_properties = properties;

            }

        }

        public bool Upload(string destinationUrl, byte[] bytes, Dictionary<string, object> properties)
        {

            return Upload(new FileInfo(destinationUrl, bytes, properties));

        }

        public bool Upload(FileInfo fileInfo)
        {

            if (fileInfo.HasProperties)
            {
                bool success = false;

                while (!success)
                {
                    // web exception issue
                    try
                    {
                        fileInfo.m_listInfo = m_lists.Find(fileInfo);
                        success = true;
                    }
                    catch(Exception e)
                    {
                        Utils.WriteToLog(Utils.LogSeverity.Error, processName, "Error contacting sharepoint: " + e.InnerException);
                        success = false;
                        Thread.Sleep(1000);
                    }
                }
            }

            bool result = TryToUpload(fileInfo);

            if (!result && fileInfo.m_ensureFolders)
            {

                string root = fileInfo.URI.AbsoluteUri.Replace(fileInfo.URI.AbsolutePath, "");

                for (int i = 0; i < fileInfo.URI.Segments.Length - 1; i++)
                {

                    root += fileInfo.URI.Segments[i];

                    if (i > 1) CreateFolder(root);

                }

                result = TryToUpload(fileInfo);

            }

            return result;

        }

        private bool TryToUpload(FileInfo fileInfo)
        {

            try
            {

                WebRequest request = WebRequest.Create(fileInfo.m_URL);

                request.Credentials = m_credentials;

                request.Method = "PUT";

                byte[] buffer = new byte[1024];

                using (Stream stream = request.GetRequestStream())

                using (MemoryStream ms = new MemoryStream(fileInfo.m_bytes))

                    for (int i = ms.Read(buffer, 0, buffer.Length); i > 0; i = ms.Read(buffer, 0, buffer.Length))

                        stream.Write(buffer, 0, i);

                WebResponse response = request.GetResponse();

                response.Close();

                if (fileInfo.HasProperties)
                {

                    StringBuilder sb = new StringBuilder();

                    sb.Append("<Method ID='1' Cmd='Update'><Field Name='ID'/>");

                    sb.AppendFormat("<Field Name='FileRef'>{0}</Field>", fileInfo.m_URL);

                    foreach (KeyValuePair<string, object> property in fileInfo.m_properties)

                        sb.AppendFormat("<Field Name='{0}'>{1}</Field>", property.Key, property.Value);

                    sb.Append("</Method>");

                    System.Xml.XmlElement updates = (new System.Xml.XmlDocument()).CreateElement("Batch");

                    updates.SetAttribute("OnError", "Continue");

                    updates.SetAttribute("ListVersion", fileInfo.m_listInfo.m_version);

                    updates.SetAttribute("PreCalc", "TRUE");

                    updates.InnerXml = sb.ToString();

                    m_listService.Url = fileInfo.m_listInfo.m_webUrl + "/_vti_bin/Lists.asmx";

                    XmlNode updatesResponse = m_listService.UpdateListItems(fileInfo.m_listInfo.m_listName, updates);

                    if (updatesResponse.FirstChild.FirstChild.InnerText != "0x00000000")

                        throw new Exception("Could not update properties.");

                }

                return true;

            }

            catch (WebException)
            {

                return false;

            }

        }

        private bool CreateFolder(string folderURL)
        {

            try
            {

                WebRequest request = WebRequest.Create(folderURL);

                request.Credentials = m_credentials;

                request.Method = "MKCOL";

                WebResponse response = request.GetResponse();

                response.Close();

                return true;

            }

            catch (WebException)
            {

                return false;

            }

        }

        public void UpdateFileStatus(int processFileId, Utils.FileStatus fileStatus, int fileFormatId, int siteId)
        {
            if (ConfigurationSettings.AppSettings["SkipSharePoint"] != "True")
            {
                Lists listService = connectToListsWebService();

                int rowId = getRowIdForProcessFileId(processFileId, listService);

                updateFileStatusInDocumentLibrary(rowId, fileStatus, listService, fileFormatId, siteId);
            }
        }

        // new code
        public System.Xml.XmlNode GetList(string url, string listName)
        {
            if (ConfigurationSettings.AppSettings["SkipSharePoint"] != "True")
            {
                Lists listService = connectToListsWebService(url);

                // Get the information about the given list ID/Version  
                try
                {
                    // MS 6 Jan 2010
                    //listService.UseDefaultCredentials = true;
                    //listService.PreAuthenticate = true;

                    XmlNode listResponse = listService.GetList(listName);

                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();

                    /* Assign values to the string parameters of the GetListItems method, using GUIDs for the listName and viewName variables. For listName, using the list display name will also work, but using the list GUID is recommended. For viewName, only the view GUID can be used. Using an empty string for viewName causes the default view 
                    to be used.*/
                    //string listName = "PipelineLoader";
                    string viewName = "";
                    string rowLimit = "150";

                    /*Use the CreateElement method of the document object to create elements for the parameters that use XML.*/
                    System.Xml.XmlElement query = xmlDoc.CreateElement("Query");
                    System.Xml.XmlElement viewFields =
                        xmlDoc.CreateElement("ViewFields");
                    System.Xml.XmlElement queryOptions =
                        xmlDoc.CreateElement("QueryOptions");

                    /*To specify values for the parameter elements (optional), assign CAML fragments to the InnerXml property of each element.*/
                    //query.InnerXml = String.Format("<Where><Eq><FieldRef Name=\"ProcessFileId\" />" +
                    //    "<Value Type=\"Text\">{0}</Value></Eq></Where>", 99999);

                    // Appears to be optional: viewFields.InnerXml = "<FieldRef Name=\"Name\" />";
                    queryOptions.InnerXml = "";

                    /* Declare an XmlNode object and initialize it with the XML response from the GetListItems method. The last parameter specifies the GUID of the Web site containing the list. Setting it to null causes the Web site specified by the Url property to be used.*/
                    System.Xml.XmlNode nodeListItems =
                        listService.GetListItems
                        (listName, viewName, query, viewFields, rowLimit, queryOptions, null);

                    return (nodeListItems);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private void updateFileStatusInDocumentLibrary(int rowId, Utils.FileStatus fileStatus, Lists listService, int fileFormatId, int siteId)
        {
            // Get the information about the given list ID/Version  
            //XmlNode listResponse = listService.GetList("PipelineLoader");

            XmlNode listResponse = null;

            bool success = false;
            while (!success)
            {
                // web exception issue
                try
                {
                    listResponse = listService.GetList("PipelineLoader"); 
                    success = true;
                }
                catch
                {
                    success = false;
                    Thread.Sleep(1000);
                }
            }


            string listID = listResponse.Attributes["ID"].Value;          // Provides the GUID id for the actual list  
            string listVersion = listResponse.Attributes["Version"].Value;  // Provides the version number for the actual ist  

            // Build the CAML statement  
            string updateCAML = String.Format("<Method ID='1' Cmd='Update'><Field Name='ID'>{0}</Field><Field Name='FileStatus'>{1}</Field><Field Name='FileFormatId'>{2}</Field><Field Name='SiteId'>{3}</Field></Method>", rowId.ToString(), fileStatus, fileFormatId, siteId);

            // Build the XMLdocument object that contains the update request  
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement elBatch = xmlDoc.CreateElement("Batch");

            elBatch.SetAttribute("OnError", "Continue");
            elBatch.SetAttribute("ListVersion", listVersion);

            elBatch.InnerXml = updateCAML;

            //XmlNode ndReturn = listService.UpdateListItems(listID, elBatch);
            XmlNode ndReturn = null;

            success = false;
            while (!success)
            {
                // web exception issue
                try
                {
                    ndReturn = listService.UpdateListItems(listID, elBatch); 
                    success = true;
                }
                catch
                {
                    success = false;
                    Thread.Sleep(1000);
                }
            }

        }

        private int getRowIdForProcessFileId(int processFileId, Lists listService)
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();

            /* Assign values to the string parameters of the GetListItems method, using GUIDs for the listName and viewName variables. For listName, using the list display name will also work, but using the list GUID is recommended. For viewName, only the view GUID can be used. Using an empty string for viewName causes the default view 
            to be used.*/
            string listName = "PipelineLoader";
            string viewName = "";
            string rowLimit = "150";

            /*Use the CreateElement method of the document object to create elements for the parameters that use XML.*/
            System.Xml.XmlElement query = xmlDoc.CreateElement("Query");
            System.Xml.XmlElement viewFields =
                xmlDoc.CreateElement("ViewFields");
            System.Xml.XmlElement queryOptions =
                xmlDoc.CreateElement("QueryOptions");

            /*To specify values for the parameter elements (optional), assign CAML fragments to the InnerXml property of each element.*/
            query.InnerXml = String.Format("<Where><Eq><FieldRef Name=\"ProcessFileId\" />" +
                "<Value Type=\"Text\">{0}</Value></Eq></Where>", processFileId);
            // Appears to be optional: viewFields.InnerXml = "<FieldRef Name=\"Name\" />";
            queryOptions.InnerXml = "";

            /* Declare an XmlNode object and initialize it with the XML response from the GetListItems method. The last parameter specifies the GUID of the Web site containing the list. Setting it to null causes the Web site specified by the Url property to be used.*/
            //System.Xml.XmlNode nodeListItems =
            //    listService.GetListItems
            //    (listName, viewName, query, viewFields, rowLimit, queryOptions, null);

            System.Xml.XmlNode nodeListItems = null;

            bool success = false;
            while (!success)
            {
                // web exception issue
                try
                {
                    nodeListItems =
                        listService.GetListItems
                        (listName, viewName, query, viewFields, rowLimit, queryOptions, null);
                    success = true;
                }
                catch
                {
                    success = false;
                    Thread.Sleep(1000);
                }
            }


            return (getIdFromXmlRS(nodeListItems));
        }

        // TO DO: would be nicer to use a proper XML parse, rather than substring it
        private int getIdFromXmlRS(System.Xml.XmlNode nodeListItems)
        {
            /*Loop through each node in the XML response and display each item.*/
            string resultXml = "";
            int listColumnId = 0;

            foreach (System.Xml.XmlNode listItem in nodeListItems)
            {
                resultXml += listItem.OuterXml;
            }

            int listColumnIdPosStart = resultXml.IndexOf("ows_ID=");
            if (listColumnIdPosStart > 0)
            {
                listColumnIdPosStart += 8; // skip pas the ows_ID=\" part
                int listColumnIdPosEnd = resultXml.IndexOf("\"", listColumnIdPosStart);

                listColumnId = int.Parse(resultXml.Substring(listColumnIdPosStart, listColumnIdPosEnd - listColumnIdPosStart));
            }

            return listColumnId;
        }

        private Lists connectToListsWebService()
        {
            Lists listService = new Lists();

            System.Net.ICredentials credential = new NetworkCredential(ConfigurationSettings.AppSettings["ServiceAccount"], ConfigurationSettings.AppSettings["ServiceAccountPW"], ConfigurationSettings.AppSettings["ServiceAccountDomain"]);
            listService.Credentials = credential;
            listService.Url = string.Format("{0}/{1}", "http://alternativeenergyportal/applications/WindART/", "_vti_bin/lists.asmx");
            // if only this worked....returnService.Url = string.Format("{0}/{1}", "http://alternativeenergy.bpglobal.com/applications/WindART/", "_vti_bin/lists.asmx");
            return listService;
        }

        private Lists connectToListsWebService(string url)
        {
            Lists listService = new Lists();

            System.Net.ICredentials credential = new NetworkCredential(ConfigurationSettings.AppSettings["ServiceAccount"], ConfigurationSettings.AppSettings["ServiceAccountPW"], ConfigurationSettings.AppSettings["ServiceAccountDomain"]);
            listService.Credentials = credential;
            listService.Url = string.Format("{0}/{1}", url, "_vti_bin/lists.asmx");
            // if only this worked....returnService.Url = string.Format("{0}/{1}", "http://alternativeenergy.bpglobal.com/applications/WindART/", "_vti_bin/lists.asmx");
            return listService;
        }

    }
}
