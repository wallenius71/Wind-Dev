using System;
using System.Collections.Generic;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Core;
using System.IO;

namespace ExcelApplication
{
    class Utils
    {
        private System.Object missing = System.Type.Missing;

        public int FindStringInSpreadsheet(Excel.Worksheet ws, string match)
        {
            Excel.Range cell = null;
            int row = 0;
            int col = 0;

            while (row <= 0)
            {
                try
                {
                    cell = (Excel.Range)ws.Cells.Find(match, missing, missing, missing, missing, Excel.XlSearchDirection.xlNext, missing, missing, missing).Cells;
                }
                catch
                {
                    // if not found, throws an exception!
                    break;
                }

                row = cell.Row;
                col = cell.Column;
                if (col > 1)
                {
                    // These are not the droids you are looking for
                    // Only items in the first column are valid
                    row = 0;
                }
            }

            return row;
        }

        public void FindAndReplace(UFLDetails ufl, List<String> iniTemplate)
        {
            int numberOfTags = ufl.PiTagList.Count;
            int fieldIndex = 22;


            for (int i = 0; i < iniTemplate.Count; i++)
            {

                //iniTemplate[i] = iniTemplate[i].Replace("{InputFileMask}", ufl.FileMask.ToString());
                // Sometime the extension changes (decryption, extraction) so we look for *
                // Filemask is used to identify the file format however
                iniTemplate[i] = iniTemplate[i].Replace("{InputFileMask}", "*");
                iniTemplate[i] = iniTemplate[i].Replace("{InputFolder}", ufl.Folder);
                iniTemplate[i] = iniTemplate[i].Replace("{FileFormat}", String.Format("f{0}_{1}", ufl.UFLDetailId.ToString().PadLeft(4, '0'),ufl.FileMask));  
                iniTemplate[i] = iniTemplate[i].Replace("{UFLFileFormatId}", ufl.UFLDetailId.ToString());
                iniTemplate[i] = iniTemplate[i].Replace("{DateTimeFormat}", ufl.DateTimeFormat);

                if (iniTemplate[i].Equals("{FieldsTagVariables}"))
                {
                    iniTemplate[i] = String.Empty;


                    int tagNumber = 1;
                    for (int j = 0; j < numberOfTags; j++)
                    {
                        if (ufl.DataType == "DAT")
                        {
                            // DAT only has one field
                            iniTemplate.Insert(i++, String.Format("FIELD({0}).NAME	= \"TAG{1}DAT\" ", fieldIndex++, tagNumber));
                            iniTemplate.Insert(i++, String.Format("FIELD({0}).NAME	= \"TAG{1}DCL\" ", fieldIndex++, tagNumber));
                            iniTemplate.Insert(i++, String.Format("FIELD({0}).NAME	= \"TAG{1}QCW\" ", fieldIndex++, tagNumber++));
                        }
                        else if (ufl.DataType == "DPD")
                        {
                            iniTemplate.Insert(i++, String.Format("FIELD({0}).NAME	= \"TAG{1}DPD\" ", fieldIndex++, tagNumber));
                            iniTemplate.Insert(i++, String.Format("FIELD({0}).NAME	= \"TAG{1}QCP\" ", fieldIndex++, tagNumber));
                            iniTemplate.Insert(i++, String.Format("FIELD({0}).NAME	= \"TAG{1}DCL\" ", fieldIndex++, tagNumber));
                            iniTemplate.Insert(i++, String.Format("FIELD({0}).NAME	= \"TAG{1}QCW\" ", fieldIndex++, tagNumber++));
                        }
                    }
                }
                else if (iniTemplate[i].Equals("{FieldsTagValues}"))
                {
                    iniTemplate[i] = String.Empty;

                    //int fieldIndex = 100;
                    int tagNumber = 1;
                    for (int j = 0; j < numberOfTags; j++)
                    {
                        iniTemplate.Insert(i++, String.Format("FIELD({0}).NAME	= \"Value{1}\" ", fieldIndex, tagNumber++));
                        iniTemplate.Insert(i++, String.Format("FIELD({0}).TYPE	= \"Number\" ", fieldIndex++));
                    }
                    
                }
                else if (iniTemplate[i].Equals("{HeaderFilter}"))
                {
                    StringBuilder filter = new StringBuilder();
                    string delim = "";
                    foreach (string tag in ufl.HeaderMarkerRowContents)
                    {
                        filter.Append(delim);
                        filter.Append(tag);

                        delim = ufl.DelimiterCode;
                    }
                    iniTemplate[i] = "Header.FILTER	= C1==\"" + filter.ToString() + "\"";
                }
                else if (iniTemplate[i].Equals("{DefineTagNames}"))
                {
                    iniTemplate[i] = String.Empty;

                    for (int j = 0; j < numberOfTags; j++)
                    {
                        // Don't store N/A or TIME tags
                        if ((ufl.PiTagList[j].ToString().ToUpper() != "N/A") && (ufl.PiTagList[j].ToString().ToUpper() != "DATETIME"))
                        {
                            if (ufl.DataType == "DAT")
                            {
                                iniTemplate.Insert(i++, String.Format("TAG{0}DAT=\"{1}.DAT.{2}\" ", j + 1, ufl.PiTowerName, ufl.PiTagList[j].ToString()));
                                iniTemplate.Insert(i++, String.Format("TAG{0}DCL=\"{1}.DCL.{2}\" ", j + 1, ufl.PiTowerName, ufl.PiTagList[j].ToString()));
                                iniTemplate.Insert(i++, String.Format("TAG{0}QCW=\"{1}.QCW.{2}\" ", j + 1, ufl.PiTowerName, ufl.PiTagList[j].ToString()));
                            }
                            else if (ufl.DataType == "DPD")
                            {
                                iniTemplate.Insert(i++, String.Format("TAG{0}DPD=\"{1}.DPD.{2}\" ", j + 1, ufl.PiTowerName, ufl.PiTagList[j].ToString()));
                                iniTemplate.Insert(i++, String.Format("TAG{0}QCP=\"{1}.QCP.{2}\" ", j + 1, ufl.PiTowerName, ufl.PiTagList[j].ToString()));
                                iniTemplate.Insert(i++, String.Format("TAG{0}DCL=\"{1}.DCL.{2}\" ", j + 1, ufl.PiTowerName, ufl.PiTagList[j].ToString()));
                                iniTemplate.Insert(i++, String.Format("TAG{0}QCW=\"{1}.QCW.{2}\" ", j + 1, ufl.PiTowerName, ufl.PiTagList[j].ToString()));
                            }
                        }
                    }
                }
                else if (iniTemplate[i].Equals("{DataFilter}"))
                {
                    StringBuilder filter = new StringBuilder();
                    string delim = "";

                    for (int j = 0; j < ufl.PiTagList.Count; j++)
                    {
                        filter.Append(delim + "*");
                        delim = ufl.DelimiterCode;
                    }

                    //filter.Append("*" +ufl.Delimitor, 0, ufl.PiTagList.Count);

                    iniTemplate[i++] = String.Format("Data.FILTER	= C1==\"{0}\" ",filter.ToString() );

                    for (int j = 0; j < numberOfTags; j++)
                    {
                        // Don't store N/A tags
                        if (ufl.PiTagList[j].ToString().ToUpper() != "N/A")
                        {
                            // Record time value in a special tag
                            if (ufl.PiTagList[j].ToString() == "DateTime")
                            {
                                iniTemplate.Insert(i++, string.Format("DATETIME = [\"{0}\"]", starsPick(ufl.PiTagList.Count, j+1, ufl.DelimiterCode)));
                            }
                            else
                            {
                                iniTemplate.Insert(i++, string.Format("Value{0} = [\"{1}\"]", j+1, starsPick(ufl.PiTagList.Count, j+1, ufl.DelimiterCode)));
                            }
                        }
                    }
                }
                else if (iniTemplate[i].Equals("{DataStoreInPi}"))
                {
                    iniTemplate[i] = String.Empty;

                    for (int j = 0; j < numberOfTags; j++)
                    {
                        // Don't store N/A or TIME tags
                        if ((ufl.PiTagList[j].ToString().ToUpper() != "N/A") && (ufl.PiTagList[j].ToString().ToUpper() != "DATETIME"))
                        {
                            if (ufl.DataType == "DAT")
                            {
                                iniTemplate.Insert(i++, String.Format("Result = StoreInPI(TAG{0}DAT,,DATETIME,Value{0},,) ", j + 1));
                                iniTemplate.Insert(i++, String.Format("Result = StoreInPI(TAG{0}DCL,,DATETIME,Value{0},,) ", j + 1));

                                // Only store QCW values for AVG tags
                                if(ufl.PiTagList[j].ToString().ToUpper().Contains("AVG"))
                                {
                                    iniTemplate.Insert(i++, String.Format("Result = StoreInPI(TAG{0}QCW,,DATETIME,-9999,,) ", j + 1));
                                }
                            } else if (ufl.DataType == "DPD")
                            {
                                iniTemplate.Insert(i++, String.Format("Result = StoreInPI(TAG{0}DPD,,DATETIME,Value{0},,) ", j + 1));
                                iniTemplate.Insert(i++, String.Format("Result = StoreInPI(TAG{0}DCL,,DATETIME,Value{0},,) ", j + 1));

                                // Only store QCP/QCW values for AVG tags
                                if (ufl.PiTagList[j].ToString().ToUpper().Contains("AVG"))
                                {
                                    iniTemplate.Insert(i++, String.Format("Result = StoreInPI(TAG{0}QCP,,DATETIME,-9999,,) ", j + 1));
                                    iniTemplate.Insert(i++, String.Format("Result = StoreInPI(TAG{0}QCW,,DATETIME,-9999,,) ", j + 1));
                                }
                            }
                        }
                    }
                }
            }

        }

        private string starsPick(int numberOfStars, int selectStar, string inDelim)
        {
            StringBuilder stars = new StringBuilder();
            string delim = string.Empty;

            for (int i = 1; i <= numberOfStars; i++)
            {
                if (i == selectStar)
                {
                    stars.Append(delim + "(*)");
                }
                else
                {
                    stars.Append(delim+"*");
                }
                delim = inDelim;
            }


            return stars.ToString();
        }

        public void SaveTemplate(String saveFilename, List<String> template)
        {

            StreamWriter sw = File.CreateText(saveFilename);
            List<String> contents = new List<String>();

            foreach (string templateLine in template)
            {
                sw.WriteLine(templateLine);
            }

            sw.Close();

        } 

        public List<String> LoadTemplate(String loadFile)
        {
            if (!File.Exists(loadFile))
            {
                return null;
            }

            StreamReader sr = File.OpenText(loadFile);
            List<String> contents = new List<String>();
            String inputLine;

            while ((inputLine = sr.ReadLine()) != null)
            {
                // Remove any weird characters in the line
                contents.Add(inputLine.Replace('', ' '));
            }
            sr.Close();

            return (contents);
        }


    }
}
