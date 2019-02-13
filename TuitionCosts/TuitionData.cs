using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using ExcelDataReader;
namespace TuitionCosts
{
    public class TuitionData
    {
        private static Dictionary<string, College> collegeMap = new Dictionary<string, College>();
        private static bool csv = false;
        public struct College
        {
            public string name { get; set; }
            public double? inStateCost { get; set; }
            public double? outStateCost { get; set; }
            public double? roomAndBoard { get; set; }
        }
        public struct Response
        {
            public bool success { get; set; }
            public double? totalCost { get; set; }
            public string errorMessage { get; set; }
        }
        public TuitionData(string path)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelReader;
                if (path.EndsWith(".xls") || path.EndsWith(".xlsx"))
                {
                    excelReader = ExcelReaderFactory.CreateReader(fs);
                }
                else if (path.EndsWith(".csv"))
                {
                    excelReader = ExcelReaderFactory.CreateCsvReader(fs);
                    csv = true;
                }
                else
                {
                    throw new Exception();
                }
                CreateCollegeMap(excelReader.AsDataSet().Tables[0]);
            }
        }
       
        public Response getTuitionInState(string collegeName, bool roomAndBoard = false)
        {
            Response res = new Response();
            string key = MakeKeyFromName(collegeName);
            if (collegeName.Equals("") || collegeName == null)
            {
                res.success = false;
                res.errorMessage = "Error: College name is required";
                return res;
            }

            if (collegeMap.TryGetValue(key, out College college))
            {
                res.success = true;
                if (roomAndBoard)
                {
                    res.totalCost = college.inStateCost + college.roomAndBoard;
                }
                else
                {
                    res.totalCost = college.inStateCost;
                }
                return res;
            }
            else
            {
                res.success = false;
                res.errorMessage = "Error: College not found";
                return res;
            }
        }

        public Response getTuitionOutState(string collegeName, bool roomAndBoard = false)
        {
            Response res = new Response();
            string key = MakeKeyFromName(collegeName);
            if (collegeName.Equals("") || collegeName == null)
            {
                res.success = false;
                res.errorMessage = "Error: College name is required";
                return res;
            }

            if (collegeMap.TryGetValue(key, out College college))
            {
                res.success = true;
                if (roomAndBoard)
                { 
                    res.totalCost = college.outStateCost + college.roomAndBoard;
                }
                else
                {
                    res.totalCost = college.outStateCost;
                }

                return res;
            }
            else
            {
                res.success = false;
                res.errorMessage = "Error: College not found";
                return res;
            }
        }

        private static void CreateCollegeMap(DataTable dataTable)
        {
            for (var i = 1; i < dataTable.Rows.Count; i++)
            {
                College temp = new College();

                if (dataTable.Rows[i][0] != DBNull.Value && !dataTable.Rows[i][0].Equals(""))
                {
                    temp.name = (string)dataTable.Rows[i][0];

                    if (dataTable.Rows[i][1] != DBNull.Value && !dataTable.Rows[i][1].Equals(""))
                    {
                        temp.inStateCost = csv ? Double.Parse((string)dataTable.Rows[i][1]) : (double)dataTable.Rows[i][1];
                    }

                    if (dataTable.Rows[i][2] != DBNull.Value && !dataTable.Rows[i][2].Equals(""))
                    {
                        temp.outStateCost = csv ? Double.Parse((string)dataTable.Rows[i][2]) : (double)dataTable.Rows[i][2];
                    }

                    if (dataTable.Rows[i][3] != DBNull.Value && !dataTable.Rows[i][3].Equals(""))
                    {
                        temp.roomAndBoard = csv ? Double.Parse((string)dataTable.Rows[i][3]) : (double)dataTable.Rows[i][3];
                    }

                    string key = MakeKeyFromName(temp.name);
                    if (!collegeMap.TryGetValue(key, out College value))
                    {
                        collegeMap.Add(key, temp);
                    }
                }
            }
        }

        private static string MakeKeyFromName(string name)
        {
            string[] splitName = name.ToLower().Split(" ");
            return string.Join("", splitName);
        }
    }
}
