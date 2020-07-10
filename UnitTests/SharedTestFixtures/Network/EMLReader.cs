// https://www.codeproject.com/Articles/29439/Easily-Retrieve-Email-Information-from-EML-Files
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace SharedTestFixtures.Network
{
    public class EMLReader
    {
        public string X_Sender { get; set; }

        public string[] X_Receivers { get; set; }

        public string Received { get; set; }

        public string Mime_Version { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string CC { get; set; }

        public DateTime Date { get; set; } = DateTime.MinValue;

        public string Subject { get; set; }

        public string Content_Type { get; set; }

        public string Content_Transfer_Encoding { get; set; }

        public string Return_Path { get; set; }

        public string Message_ID { get; set; }

        public DateTime X_OriginalArrivalTime { get; set; } = DateTime.MinValue;

        public string Body { get; set; }

        public string HTMLBody { get; set; }

        public Dictionary<string, string> UnsupportedHeaders { get; private set; } = null;

        public EMLReader(FileStream fsEML)
        {
            ParseEML(fsEML);
        }

        private void ParseEML(FileStream fsEML)
        {
            StreamReader sr = new StreamReader(fsEML);
            string sLine;
            List<string> listAll = new List<string>();
            while ((sLine = sr.ReadLine()) != null)
            {
                listAll.Add(sLine);
            }

            List<string> list = new List<string>();
            int nStartBody = -1;
            string[] saAll = new string[listAll.Count];
            listAll.CopyTo(saAll);

            for (int i = 0; i < saAll.Length; i++)
            {
                if (saAll[i] == string.Empty)
                {
                    nStartBody = i;
                    break;
                }

                string sFullValue = saAll[i];
                GetFullValue(saAll, ref i, ref sFullValue);
                list.Add(sFullValue);


                Debug.WriteLine(sFullValue);
            }

            SetFields(list.ToArray());

            if (nStartBody == -1)   // no body ?
                return;

            // Get the body info out of saAll and set the Body and/or HTMLBody properties
            if (Content_Type != null && Content_Type.ToLower().Contains("multipart/alternative"))   // set for HTMLBody messages
            {
                int ix = Content_Type.ToLower().IndexOf("boundary");        // boundary is used to separate the different body types
                if (ix == -1)
                    return;

                string sBoundaryMarker = Content_Type.Substring(ix + 8).Trim(new char[] { '=', '"', ' ', '\t' });

                // save this boundaries elements into a list of strings
                list = new List<string>();
                for (int n = nStartBody + 1; n < saAll.Length; n++)
                {
                    if (saAll[n].Contains(sBoundaryMarker))
                    {
                        if (list.Count > 0)
                        {
                            SetBody(list);
                            list = new List<string>();
                        }
                        continue;
                    }

                    list.Add(saAll[n]);
                }
            }
            else    // plain text body type only
            {
                Body = string.Empty;
                for (int n = nStartBody + 1; n < saAll.Length; n++)
                    Body += saAll[n] + "\r\n";
            }
        }

        private void SetBody(List<string> list)
        {
            bool bIsHTML = false;
            bool bIsBodyStart = false;
            List<string> listBody = new List<string>();

            foreach (string s in list)
            {
                // use to determine type of body
                if (s.ToLower().StartsWith("content-type"))
                {
                    if (s.ToLower().Contains("text/html"))
                        bIsHTML = true;
                    else if (!s.ToLower().Contains("text/plain"))
                        return;
                }
                else if (s == string.Empty && !bIsBodyStart)
                {
                    bIsBodyStart = true;
                }
                else if (bIsBodyStart)
                {
                    listBody.Add(s);
                }
            }

            string[] sa = new string[listBody.Count];
            listBody.CopyTo(sa);

            if (bIsHTML)
                HTMLBody = string.Join("\r\n", sa);
            else
                Body = string.Join("\r\n", sa);
        }

        private void GetFullValue(string[] sa, ref int i, ref string sValue)
        {
            if (i + 1 < sa.Length && sa[i + 1] != string.Empty && char.IsWhiteSpace(sa[i + 1], 0))   // spec says line's that begin with white space are continuation lines
            {
                i++;
                sValue += " " + sa[i].Trim();

                GetFullValue(sa, ref i, ref sValue);
            }
        }

        private void SetFields(string[] saLines)
        {
            //List<string> listUnsupported = new List<string>();
            UnsupportedHeaders = new Dictionary<string, string>();
            List<string> listX_Receiver = new List<string>();
            foreach (string sHdr in saLines)
            {
                string[] saHdr = Split(sHdr);
                if (saHdr == null)  // not a valid header
                    continue;

                switch (saHdr[0].ToLower())
                {
                    case "x-sender":
                        X_Sender = saHdr[1];
                        break;
                    case "x-receiver":
                        listX_Receiver.Add(saHdr[1]);
                        break;
                    case "received":
                        Received = saHdr[1];
                        break;
                    case "mime-version":
                        Mime_Version = saHdr[1];
                        break;
                    case "from":
                        From = saHdr[1];
                        break;
                    case "to":
                        To = saHdr[1];
                        break;
                    case "cc":
                        CC = saHdr[1];
                        break;
                    case "date":
                        Date = DateTime.Parse(saHdr[1]);
                        break;
                    case "subject":
                        Subject = saHdr[1];
                        break;
                    case "content-type":
                        Content_Type = saHdr[1];
                        break;
                    case "content-transfer-encoding":
                        Content_Transfer_Encoding = saHdr[1];
                        break;
                    case "return-path":
                        Return_Path = saHdr[1];
                        break;
                    case "message-id":
                        Message_ID = saHdr[1];
                        break;
                    case "x-originalarrivaltime":
                        int ix = saHdr[1].IndexOf("FILETIME");
                        if (ix != -1)
                        {
                            string sOAT = saHdr[1].Substring(0, ix);
                            sOAT = sOAT.Replace("(UTC)", "-0000");
                            X_OriginalArrivalTime = DateTime.Parse(sOAT);
                        }
                        break;
                    //case "body":
                    //    Body = saHdr[1];
                    //    break;
                    default:
                        UnsupportedHeaders.Add(saHdr[0], saHdr[1]);
                        break;
                }
            }

            X_Receivers = new string[listX_Receiver.Count];
            listX_Receiver.CopyTo(X_Receivers);
        }

        private string[] Split(string sHeader)  // because string.Split won't work here...
        {
            int ix;
            if ((ix = sHeader.IndexOf(':')) == -1)
                return null;

            return new string[] { sHeader.Substring(0, ix).Trim(), sHeader.Substring(ix + 1).Trim() };
        }
    }
}
