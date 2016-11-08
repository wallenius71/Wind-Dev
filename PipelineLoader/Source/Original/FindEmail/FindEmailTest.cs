using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Network.Exchange;
using Aspose.Network;

using System.Net;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;



namespace FindEmail
{
    class FindEmailTest
    {
        private HttpWebRequest WebReq;

        public void ExchangeTest()
        {
            /*
            string serverAddress = "HY1MAAEX001.HE1.LOCAL";
            string title = "Test";
            string emailAccount = "WindMIData";
            string destinationFolder = @"C:\PipelineData\0.EmailFolder";
            */
            //string emailPassword = "";
            string emailServerType = "EXCHANGE";

            //SSL = "Yes";
            //port = 993;


            ExchangeClient client = null;

            if (emailServerType.ToUpper() == "EXCHANGE")
            {
                //client = new ExchangeClient("http://hy1maaex001.he1.local/owa/winmi0", "winmi0", emailPassword, "HE1");
                //client = new ExchangeClient("http://hy1maaex001.he1.local/exchange/goldt1", "goldt1", "Hannah321", "HE1");
                //client = new ExchangeClient("http://hy1maaex001.he1.local/owa/goldt1", "goldt1", "Hannah321", "HE1");
                //https://imail.bpglobal.com/exchange/europeasiarawindata/inbox/
//                client = new ExchangeClient("https://imail.bpglobal.com/exchange/europeasiarawindata/inbox", "sheam6", "p", "BP1");


                // start
                String strServerName   = "imail.bpglobal.com";
                String strDomain   = "bp1";
                String strUserID   = "sheam6";
                String strPassword = "pizza1970";
                //String strUserName = "midata";
                String strUserName = "mike.sheard";
                String term = "BP";
                /*
                Console.Write("Server (eg imail.bpglobal.com):");
                strServerName = Console.ReadLine();

                Console.Write("Domain (eg bp1):");
                strDomain = Console.ReadLine(); 

                Console.Write("UserID (eg sheam6):");
                strUserID = Console.ReadLine(); 

                Console.Write("Password:");
                strPassword = Console.ReadLine();

                Console.Write("Mailbox (eg mike.sheard or europeasiarawindata):");
                strUserName = Console.ReadLine();

                */
                // Create our destination URL.
                String strURL = "https://" + strServerName + "/exchange/" + strUserName + "/InBox"; // was WindData

                //strURL = "https://imail.bpglobal.com/exchange/europeasiarawindata/InBox/";


                // find unread emails

                string QUERY = "<?xml version=\"1.0\"?>" 
                       + "<g:searchrequest xmlns:g=\"DAV:\">" 
                       + "<g:sql>SELECT \"urn:schemas:httpmail:subject\", " 
                       + "\"urn:schemas:httpmail:from\", \"DAV:displayname\", " 
                       + "\"urn:schemas:httpmail:textdescription\" " 
                       + "FROM SCOPE('deep traversal of \"" + strURL + "\"') " 
                       + "WHERE \"DAV:ishidden\" = False AND \"DAV:isfolder\" = False "
                       + "AND \"urn:schemas:httpmail:read\" = False "
                       //+ "AND \"urn:schemas:httpmail:hasattachment\" = False "
                        //+ "\"DAV:hasattachment\"=True "
                       //////////+ "AND \"urn:schemas:httpmail:subject\" LIKE '%" + term + "%' " 
                       + "ORDER BY \"urn:schemas:httpmail:date\" DESC" 
                       + "</g:sql></g:searchrequest>";

                System.Net.HttpWebResponse Response = null;
                System.IO.Stream RequestStream ;
                System.IO.Stream ResponseStream;
                System.Xml.XmlDocument ResponseXmlDoc;
                System.Xml.XmlNodeList SubjectNodeList;
                System.Xml.XmlNodeList SenderNodeList;
                System.Xml.XmlNodeList BodyNodeList;
                System.Xml.XmlNodeList URLNodeList;

                Console.WriteLine("Mailbox URL: " + strURL);

                HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(strURL);
                Request.CookieContainer = new CookieContainer();

                Request.CookieContainer.Add(AuthenticateSecureOWA(strServerName, strDomain, strUserID, strPassword));

                // Ori 
                //Request.Credentials = new System.Net.NetworkCredential( strUserName, strPassword, strDomain);
                Request.Credentials = new System.Net.NetworkCredential(strUserID, strPassword, strDomain);

                //Console.WriteLine(String.Format("Got cookies for: {0}\\{1}", strDomain, strUserName));
                Console.WriteLine(String.Format("Got cookies for: {0}\\{1}", strDomain, strUserID));

                Request.Method = "SEARCH";
                Request.ContentType = "text/xml";
                Request.KeepAlive = true;
                Request.AllowAutoRedirect = false;

                Byte[] bytes  = System.Text.Encoding.UTF8.GetBytes(QUERY);
                Request.Timeout = 30000;
                Request.ContentLength = bytes.Length;
                RequestStream = Request.GetRequestStream();
                RequestStream.Write(bytes, 0, bytes.Length);
                RequestStream.Close();
                Request.Headers.Add("Translate", "F");
                try
                {
                    Response = (System.Net.HttpWebResponse)Request.GetResponse();
                    ResponseStream = Response.GetResponseStream();
                    //' Create the XmlDocument object from the XML response stream.
                    ResponseXmlDoc = new System.Xml.XmlDocument();
                    ResponseXmlDoc.Load(ResponseStream);
                    SubjectNodeList = ResponseXmlDoc.GetElementsByTagName("d:subject");
                    SenderNodeList = ResponseXmlDoc.GetElementsByTagName("d:from");
                    URLNodeList = ResponseXmlDoc.GetElementsByTagName("a:href");
                    BodyNodeList = ResponseXmlDoc.GetElementsByTagName("d:textdescription");

                    int i = 1;
                    foreach (XmlElement subject in SubjectNodeList)
                    {
                        Console.WriteLine("WebDav result - Subject: " + subject.InnerText);
                        if (i++ >= 3)
                        {
                            break; // proved our point
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                // read attachment
                enumAttachments("https://imail.bpglobal.com/exchange/mike.sheard/Inbox/FW:%20Emailing:%20Release.zip.EML", strUserName, strPassword, strDomain);
            

                // end

                // try to read a list of attachments
                // Working on this bit: GetAttachmentsListXML(strServerName, "https://imail.bpglobal.com/exchange/mike.sheard/WindData/Fuxin%20Data-32.EML", "sheam6", "p", "bp1");


                Console.WriteLine("\n\nAspose Test\n\nUsing URL:" + strURL);

                //client = new ExchangeClient(strURL, strUserName, strPassword, strDomain);
                // new code goes here
                client = new ExchangeClient(strURL, strUserID, strPassword, strDomain);

                client.CookieContainer = new CookieContainer();
                client.CookieContainer.Add(AuthenticateSecureOWA(strServerName, strDomain, strUserID, strPassword));

                // Aspose client now
                //client = new ExchangeClient("https://imail.bpglobal.com/exchange/mike.sheard/inbox/", "sheam6", "p", "BP1");


                //client = new ExchangeClient("https://imail.bpglobal.com/exchange/mike.sheard/inbox/", Request.Credentials);

               
            }

            try
            {
                //query mailbox
                ExchangeMailboxInfo mailbox = client.GetMailboxInfo();
                //list messages in the Inbox
                ExchangeMessageInfoCollection messages = client.ListMessages(mailbox.InboxUri, false);

                int i = 1;
                foreach (ExchangeMessageInfo info in messages)
                {
                    //save the message locally
                    //client.SaveMessage(info.UniqueUri, info.Subject + ".eml");
                    Console.WriteLine("Aspose result - subject: " + info.Subject);
                    if (i++ >= 10)
                    {
                        break; // proved our point
                    }

                }
            }
            catch (ExchangeException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }

        private CookieCollection AuthenticateSecureOWA(string strServerName, string strDomain, string strUserName, string strPassword)
        {
            System.Uri AuthURL = null;

            try
            {
                // Construct our destination URI.
                AuthURL = new System.Uri("https://" + strServerName + "/exchweb/bin/auth/owaauth.dll");
                Console.WriteLine("Authenticated against: " + AuthURL);
            }
            catch(Exception ex)
            {
            throw new Exception("Error occurred while you are creating the URI for OWA authentication!\n\n" + ex.Message);
            }

            CookieContainer CookieJar = new CookieContainer();

            // Create our request object for the constructed URI.
            WebReq = (HttpWebRequest)WebRequest.Create(AuthURL);
            WebReq.CookieContainer = CookieJar;

            //' Create our post data string that is required by OWA (owaauth.dll).
            String strPostFields = "destination=https%3A%2F%2F" + strServerName + "%2Fexchange%2F" + strUserName + "%2F&username=" + strDomain + "%5C" + strUserName + "&password=" + strPassword + "&SubmitCreds=Log+On&forcedownlevel=0&trusted=0";

            WebReq.KeepAlive = true;
            WebReq.AllowAutoRedirect = false;
            WebReq.Method = "POST";

            //' Store the post data into a byte array.
            Byte[]  PostData = System.Text.Encoding.ASCII.GetBytes(strPostFields);

            //' Set the content length.
            WebReq.ContentLength = PostData.Length;

            Stream tmpStream;

            try
            {
                //' Create a request stream. Write the post data to the stream.
                tmpStream = WebReq.GetRequestStream();
                tmpStream.Write(PostData, 0, PostData.Length);
                tmpStream.Close();
            }
            catch(Exception ex)
            {
                throw new Exception("Error occurred while trying OWA authentication!\n\n" + ex.Message);
            }
            
                //' Get the response from the request.
            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            WebResp.Close();
            return WebResp.Cookies;
        }

        private void GetAttachmentsListXML(String strServerName, String strURL, String strUserName, String strPassword, String strDomain)
        {
            String strPropReq;
            String strOutPutFile;
            System.Xml.XmlDocument ResponseXmlDoc;
            System.Net.HttpWebResponse Response;
            System.IO.Stream RequestStream;
            System.IO.Stream ResponseStream;
            System.Xml.XmlNodeList URLNodeList;

            //HttpWebRequest  Request = new HttpWebRequest();

            HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(strURL);
    
    //Request..  
    //    .Open "X-MS-ENUMATTS", sHREF, False, sUserName, sPassword
    //    .setRequestHeader "Content-type:", "text/xml"
    //    .setRequestHeader "Depth", "1,noroot"
    //    .Send
    
        //
            Request.CookieContainer = new CookieContainer();

            Request.CookieContainer.Add(AuthenticateSecureOWA(strServerName, strDomain, strUserName, strPassword));

            string QUERY = "<?xml version='1.0'?>" +
        "<g:searchrequest xmlns:g='DAV:'>" +
        "<g:sql>SELECT \"DAV:displayname\" " +
        "FROM SCOPE('SHALLOW TRAVERSAL OF \"" + strURL + "\"') " +
        "WHERE \"DAV:isfolder\" = false" +
        "</g:sql>" +
        "</g:searchrequest>";


            Request.Credentials = new System.Net.NetworkCredential( strUserName, strPassword, strDomain);
            //Request.Credentials = new System.Net.NetworkCredential(strUserID, strPassword, strDomain);

            Request.Method = "X-MS-ENUMATTS";
            Request.ContentType = "text/xml";
            Request.KeepAlive = true;
            Request.AllowAutoRedirect = false;

            //Byte[] bytes  = System.Text.Encoding.UTF8.GetBytes(QUERY);
            Request.Timeout = 30000;
            //Request.ContentLength = bytes.Length;
            //RequestStream = Request.GetRequestStream();
            //RequestStream.Write(bytes, 0, bytes.Length);
            //RequestStream.Close();
            Request.Headers.Add("Depth", "1");
            Response = (System.Net.HttpWebResponse)Request.GetResponse();
            ResponseStream = Response.GetResponseStream();

            //' Create the XmlDocument object from the XML response stream.
            ResponseXmlDoc = new System.Xml.XmlDocument();
            ResponseXmlDoc.Load(ResponseStream);
            //SubjectNodeList = ResponseXmlDoc.GetElementsByTagName("d:subject");
            //SenderNodeList = ResponseXmlDoc.GetElementsByTagName("d:from");
            URLNodeList = ResponseXmlDoc.GetElementsByTagName("a:href");
            //BodyNodeList = ResponseXmlDoc.GetElementsByTagName("d:textdescription");

            //

            return ; // HttpWebRequest.ResponseText
        
        }



        //// start
        private void enumAttachments(string strMessageURI, string strUserName, string strPassword, string strDomain)
      {
         // Variables.
         System.Net.HttpWebRequest Request;
         System.Net.WebResponse Response;
         System.Net.CredentialCache MyCredentialCache;
         //string strMessageURI = "http://server/exchange/username/inbox/TestMessage.eml/";
         //string strUserName = "username";
         //string strPassword = "!Password";
         //string strDomain = "Domain";
         System.IO.Stream ResponseStream = null;
         System.Xml.XmlDocument ResponseXmlDoc = null;
         System.Xml.XmlNode root = null;
         System.Xml.XmlNamespaceManager nsmgr = null;
         System.Xml.XmlNodeList PropstatNodes = null;
         System.Xml.XmlNodeList HrefNodes = null;
         System.Xml.XmlNode StatusNode = null;
         System.Xml.XmlNode PropNode = null;

         try
         {
            // Create a new CredentialCache object and fill it with the network
            // credentials required to access the server.
            MyCredentialCache = new System.Net.CredentialCache();
            MyCredentialCache.Add( new System.Uri(strMessageURI),
               "NTLM",
               new System.Net.NetworkCredential(strUserName, strPassword, strDomain)
               );

            // Create the HttpWebRequest object.
            Request = (System.Net.HttpWebRequest)HttpWebRequest.Create(strMessageURI);

            // Add the network credentials to the request.
            Request.Credentials = MyCredentialCache;

            // Specify the method.
            Request.Method = "X-MS-ENUMATTS";

            // Send the X-MS-ENUMATTS method request and get the
            // response from the server.
            Response = (HttpWebResponse)Request.GetResponse();

            // Get the XML response stream.
            ResponseStream = Response.GetResponseStream();

            // Create the XmlDocument object from the XML response stream.
            ResponseXmlDoc = new System.Xml.XmlDocument();

            // Load the XML response stream.
            ResponseXmlDoc.Load(ResponseStream);

            // Get the root node.
            root = ResponseXmlDoc.DocumentElement;

            // Create a new XmlNamespaceManager.
            nsmgr = new System.Xml.XmlNamespaceManager(ResponseXmlDoc.NameTable);

            // Add the DAV: namespace, which is typically assigned the a: prefix
            // in the XML response body.  The namespaceses and their associated
            // prefixes are listed in the attributes of the DAV:multistatus node
            // of the XML response.
            nsmgr.AddNamespace("a", "DAV:");

            // Add the http://schemas.microsoft.com/mapi/proptag/ namespace, which
            // is typically assigned the d: prefix in the XML response body.
            nsmgr.AddNamespace("d", "http://schemas.microsoft.com/mapi/proptag/");

            // Use an XPath query to build a list of the DAV:propstat XML nodes,
            // corresponding to the returned status and properties of
            // the file attachment(s).
            PropstatNodes = root.SelectNodes("//a:propstat", nsmgr);

            // Use an XPath query to build a list of the DAV:href nodes,
            // corresponding to the URIs of the attachement(s) on the message.
            // For each DAV:href node in the XML response, there is an
            // associated DAV:propstat node.
            HrefNodes = root.SelectNodes("//a:href", nsmgr);

            // Attachments found?
            if(HrefNodes.Count > 0)
            {
               // Display the number of attachments on the message.
               Console.WriteLine(HrefNodes.Count + " attachments found...");

               // Iterate through the attachment properties.
               for(int i=0;i<HrefNodes.Count;i++)
               {
                  // Use an XPath query to get the DAV:status node from the DAV:propstat node.
                  StatusNode = PropstatNodes[i].SelectSingleNode("a:status", nsmgr);

                  // Check the status of the attachment properties.
                  if(StatusNode.InnerText != "HTTP/1.1 200 OK")
                  {
                     Console.WriteLine("Attachment: " + HrefNodes[i].InnerText);
                     Console.WriteLine("Status: " + StatusNode.InnerText);
                     Console.WriteLine("");
                  }
                  else
                  {
                     Console.WriteLine("Attachment: " + HrefNodes[i].InnerText);
                     Console.WriteLine("Status: " + StatusNode.InnerText);

                     // Get the CdoPR_ATTACH_FILENAME_W MAPI property tag,
                     // corresponding to the attachment file name.  The
                     // http://schemas.microsoft.com/mapi/proptag/ namespace is typically
                     // assigned the d: prefix in the XML response body.
                     PropNode = PropstatNodes[i].SelectSingleNode("a:prop/d:x3704001f", nsmgr);
                     Console.WriteLine("Attachment name: " + PropNode.InnerText);

                     // Get the CdoPR_ATTACH_EXTENSION_W MAPI property tag,
                     // corresponding to the attachment file extension.
                     PropNode = PropstatNodes[i].SelectSingleNode("a:prop/d:x3703001f", nsmgr);
                     Console.WriteLine("File extension: " + PropNode.InnerText);

                     // Get the CdoPR_ATTACH_SIZE MAPI property tag,
                     // corresponding to the attachment file size.
                     PropNode = PropstatNodes[i].SelectSingleNode("a:prop/d:x0e200003", nsmgr);
                     Console.WriteLine("Attachment size: " + PropNode.InnerText);

                     Console.WriteLine("");
                  }
               }
            }
            else
            {
               Console.WriteLine("No attachments found.");
            }

            // Clean up.
            ResponseStream.Close();
            Response.Close();

         }
         catch(Exception ex)
         {
            // Catch any exceptions. Any error codes from the X-MS-ENUMATTS
            // method request on the server will be caught here, also.
            Console.WriteLine(ex.Message);
         }
      }
   
        ////



    }
}
