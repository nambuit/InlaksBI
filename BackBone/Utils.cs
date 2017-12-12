using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Web.UI;
using Newtonsoft.Json;
using System.Net.Mime;

namespace BackBone
{
    public static class Rfc4180Writer
    {
        public static void WriteDataTable(DataTable sourceTable, TextWriter writer, bool includeHeaders)
        {
            if (includeHeaders)
            {
                IEnumerable<String> headerValues = sourceTable.Columns
                    .OfType<DataColumn>()
                    .Select(column => QuoteValue(column.ColumnName));

                writer.WriteLine(String.Join(",", headerValues));
            }

            IEnumerable<String> items = null;

            foreach (DataRow row in sourceTable.Rows)
            {
                items = row.ItemArray.Select(o => QuoteValue(o.ToString()));
                writer.WriteLine(String.Join(",", items));
            }

            writer.Flush();
        }

        private static string QuoteValue(string value)
        {
            if (value.CleanUp() == "true")
            {
                value = "1";
            }
            if (value.CleanUp() == "false")
            {
                value = "0";
            }
            return String.Concat("\"",
            value.Replace("\"", "\"\""), "\"");
        }
    }
    public static class Utils
    {
        public static SqlConnection globalConn;
        public static SqlTransaction globaltrasact;
        public static SqlCommand globalcmd;

        public static string getLastDateofMonth(DateTime date)
        {
            var lastdate = DateTime.Parse(DateTime.DaysInMonth(date.Year, date.Month) + "-" + date.ToString("MMM-yyyy"));
            return lastdate.ToString("dd-MMM-yyyy");

        }

        public static string getFirstDayofMonth(DateTime date)
        {
            var firstdate = DateTime.Parse(1 + "-" + date.ToString("MMM-yyyy"));
            return firstdate.ToString("dd-MMM-yyyy");

        }

        public static string getPreviousDay()
        {
            int prev = DateTime.Now.Day - 1;
            var previousday = DateTime.Parse(prev + "-" + DateTime.Now.ToString("MMM-yyyy"));

            return previousday.ToString("dd-MMM-yyyy");

        }


        public static void checkSettings(object[] settings)
        {
            foreach (object[] setting in settings)
            {
                if (setting[1] == null)
                {
                    throw new Exception(setting[0] + " has not been set");
                }
                else
                {
                    if (setting[1].ToString().Trim() == string.Empty)
                    {

                        throw new Exception(setting[0] + " has not been set");
                    }
                }



            }
        }


        public static void Log(string message)
        {
            try {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Event_Log.txt";
                if (message.Equals("clearAll"))
                {
                    File.WriteAllText(path, String.Empty);
                }
                else
                {
                    string logtxt = DateTime.Now.ToString("dd/MM/yyyy h:mm:ss tt") + ": " + message;
                    File.AppendAllText(path, logtxt + Environment.NewLine);
                }
            }
            catch
            {

            }
        }



        public static DataTable AddExtractionColumns(DataTable source)
        {
          var myDataColumn = new DataColumn
          {
              DataType = Type.GetType("System.Int32"),
              ColumnName = "SN"
          };

            source.Columns.Add(myDataColumn);




            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.DateTime"),
                ColumnName = "PostDate"
            };

            source.Columns.Add(myDataColumn);
            

            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.DateTime"),
                ColumnName = "Valdate"
            };

            source.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "Details"
            };

            source.Columns.Add(myDataColumn);
           

            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.Decimal"),
                ColumnName = "Debits"
            };

            source.Columns.Add(myDataColumn);


            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.Decimal"),
                ColumnName = "Credits"
            };

            source.Columns.Add(myDataColumn);


            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.Decimal"),
                ColumnName = "Amount"
            };

            source.Columns.Add(myDataColumn);
   
         
            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "CrDr"
            };

            source.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "ud1"
            };

            source.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "ud2"
            };

            source.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "ud3"
            };

            source.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "ud4"
            };

            source.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "ud5"
            };

            source.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "Id"
            };

            source.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "username"
            };

            source.Columns.Add(myDataColumn);

            return source;

        }


        public static String getDate(DateTime date)
        {
            return date.ToString("dd-MMM-yyyy");
        }

        public static String getDate(DateTime date, string format)
        {
            return date.ToString(format);
        }

        public static string padWithZeros(string field, int maxlenght)
        {
            var lenght = field.Length;
            if (lenght == maxlenght)
            {
                return field;
            }
            if (lenght > maxlenght)
            {
                throw new Exception("input string cannot be greater in lenght than the specified maximum lenght");
            }

            var padding = new StringBuilder();
            var zerosToAdd = maxlenght - lenght;
            for (int i = 0; i < zerosToAdd; i++)
            {
                padding.Append("0");
            }
            padding.Append(field);
            return padding.ToString();
        }

        public static void InitiateGlobalTransaction(string ConStr)
        {
            try
            {
                globalConn = new SqlConnection(ConStr);
                globalConn.Open();
                globalcmd = globalConn.CreateCommand();
                globaltrasact = globalConn.BeginTransaction();
                globalcmd.Connection = globalConn;
                globalcmd.Transaction = globaltrasact;
            }
            catch (Exception s)
            {
                throw (s);
            }
        }


        public static bool ExecuteSQL(String sql, string constr)
        {
            SqlConnection conn = new SqlConnection(constr);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch
            {
                conn.Close();
                return false;

            }

        }

        public static void sendMail(MailInfo info, bool secure)
        {
            try
            {
             
                SmtpClient smtpClient = new SmtpClient(info.smtpserver, info.smptpPort);
                
               
                smtpClient.Credentials = new System.Net.NetworkCredential(info.From, info.smtppass);
                smtpClient.UseDefaultCredentials = true;
               
                
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                
          
                MailMessage mail = new MailMessage();

                //Setting From , To and CC
                mail.From = new MailAddress(info.From, info.FromDisplayName,System.Text.Encoding.UTF8);

                foreach (var mailTo in info.Tos.Split(",".ToCharArray(),StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.To.Add(new MailAddress(mailTo));
                }

                if (info.CCs != null)
                {
                    foreach (var copy in info.CCs.Split(','))
                    {
                        mail.CC.Add(new MailAddress(copy));
                    }
                }

                if (info.BCCs != null)
                {
                    foreach (var BCC in info.BCCs.Split(','))
                    {
                      
                        mail.Bcc.Add(new MailAddress(BCC));
                    }
                }

                if (info.AttachmentItems != null)
                {
                    foreach (var item in info.AttachmentItems)
                    {
                        mail.Attachments.Add(item);
                    }
                }


                mail.Sender = new MailAddress(info.From);

               

                if (info.mailBody != null)
                {
                    mail.Body = info.mailBody;
                }

                if(info.alternateview != null)
                {
                    mail.AlternateViews.Add(info.alternateview);
                }

                mail.Subject = info.mailSubject;

                mail.SubjectEncoding = System.Text.Encoding.UTF8;

                mail.IsBodyHtml = true;

                smtpClient.EnableSsl = secure;

                smtpClient.Send(mail);

            }
            catch (Exception t)
            {
                try
                {
                    throw (t.InnerException);
                }
                catch
                {
                    throw new Exception (t.Message);
                }
            }


        }



        public static AlternateView getAlternateView(string signaturepath,string message)
        {
            LinkedResource logo = new LinkedResource(signaturepath);
            logo.ContentId = Guid.NewGuid().ToString();



            string bodytext = "<!DOCTYPE html>\n" +
            "<html>\n" +
            "<head>\n" +
            "    <title></title>\n" +
            "	<meta charset=\"utf-8\" />\n" +
            "</head>\n" +
            "<body>\n" +
             "<p>"+message+"</p>"+  
            "     <br>  <img style=\"margin-left:2%\" src='cid:" + logo.ContentId + "' /> \n" +
            "</body>\n" +
            "</html>";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(bodytext, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(logo);
            return alternateView;

        }

        public static string hashPassword(string pass)
        {
            var sha1 = new SHA1CryptoServiceProvider();

            var data = Encoding.ASCII.GetBytes(pass);

            var sha1data = sha1.ComputeHash(data);

            return Encoding.ASCII.GetString(sha1data);
        }


        public static string NumberToWords(long number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000000000) > 0)
            {
                words += NumberToWords(number / 1000000000000) + " Trillion ";
                number %= 1000000000000;
            }



            if ((number / 1000000000) > 0)
            {
                words += NumberToWords(number / 1000000000) + " Billion ";
                number %= 1000000000;
            }


            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
       public static string standarAmountInWords(string amount, string baseunit, string decimalunit)
        {
            if (amount.ToLower().Contains("k")&& amount.ToLower().Contains("m"))
            {
                throw new Exception("Only one symbolic character is allowed at a time");
            }

            if (amount.ToLower().Contains("k"))
            {
                decimal c;
                if (amount.Split('.').Length > 1)
                {
                    c = decimal.Parse(amount.Split('.')[0].Replace("k", ""));

                    c = c * 1000;

                    amount = c.ToString() + "." + amount.Split('.')[1];
                }
                else {

                    c = decimal.Parse(amount.Replace("k", ""));

                    c = c * 1000;

                    amount = c.ToString("#.00");
                }
                
            }


            if (amount.ToLower().Contains("m"))
            {
                decimal c;
                if (amount.Split('.').Length > 1)
                {
                    c = decimal.Parse(amount.Split('.')[0].Replace("m", ""));

                    c = c * 1000000;

                    amount = c.ToString() + "." + amount.Split('.')[1];
                }
                else
                {

                    c = decimal.Parse(amount.Replace("m", ""));

                    c = c * 1000000;

                    amount = c.ToString("#.00");
                }

            }



            long number = (long)Double.Parse(amount);
            string baseamount = NumberToWords(number);


            string decimalamount = "";
            if (amount.Split('.').Length > 1)
            {
                var dec = long.Parse(amount.Split('.')[1]);
                if (dec > 0)
                {
                    decimalamount = NumberToWords(dec);

                    decimalamount = decimalamount + " " + decimalunit;

                }
                }


            return baseamount + " " + baseunit + ((decimalamount.Equals(string.Empty)) ? "" : " " + decimalamount) + " only";
        }





        public static void LogAudit(string auditEvent, string module, HttpContext context, string constr)
        {

            string theComputerName = context.getRemoteMachineName();
            string UserFullname = (string)context.Session["UserFullName"];
            string StrStore1Period = (string)context.Session["gsSrchPeriod"];
            string AcctCode = (string)context.Session["AcctCode"];
            string APPID = "1";


            string MySql;
            MySql = "Insert into bo_audit_table(USER_ID,Event,Date_Time,DATE_TIME_UTC,MachineName,Period,Account_ID,APPID,MODULE) Values('";
            MySql += UserFullname.Trim() + "','";
            MySql += auditEvent.Trim() + "',convert(datetime,getdate()),convert(datetime,getdate()),'";
            MySql += theComputerName.Trim() + "','";
            MySql += StrStore1Period + "','";
            MySql += AcctCode.Trim() + "','";

            MySql += APPID + "','";
            MySql += module + "')";

            var db = new DBConnector("", constr);

            try
            {
                db.Execute(MySql);
            }
            catch { }
        }


        public static void LogAudit(string auditEvent, string module, Page context, string constr)
        {
            try
            {
                string theComputerName = context.getRemoteMachineName();
            string UserFullname = (string)context.Session["UserFullName"];
            string StrStore1Period = (string)context.Session["gsSrchPeriod"];
            string AcctCode = (string)context.Session["AcctCode"];
            string APPID = "1";


            string MySql;
            MySql = "Insert into bo_audit_table(USER_ID,Event,Date_Time,DATE_TIME_UTC,MachineName,Period,Account_ID,APPID,MODULE) Values('";
            MySql += UserFullname.Trim() + "','";
            MySql += auditEvent.Trim() + "',convert(datetime,getdate()),convert(datetime,getdate()),'";
            MySql += theComputerName.Trim() + "','";
            MySql += StrStore1Period + "','";
            MySql += AcctCode.Trim() + "','";

            MySql += APPID + "','";
            MySql += module + "')";

            var db = new DBConnector("", constr);

         
                db.Execute(MySql);
            }
            catch { }
        }
        public static int getAge(DateTime trandate)
        {
            var now = DateTime.Now;

            var span = now - trandate;

            return span.Days;

        }

        public static DataColumn createDataColumn(string columnName)
        {
            DataColumn cm = new DataColumn();
            cm.DataType = Type.GetType("System.String");
            cm.ColumnName = columnName;
            cm.Caption = columnName;
            return cm;
        }


        public static string[] getDateRange(int start, int end)
        {
            var now = DateTime.Now;

            var startdate = now.AddDays(-1 * start);

            var enddate   = now.AddDays(-1 * end);



            return new string[] { enddate.ToString("dd-MMM-yyyy"), startdate.ToString("dd-MMM-yyyy") };
        }


        public static JObject getLanguageObject(string path)
        {
            var filestream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var file = new StreamReader(filestream, System.Text.Encoding.UTF8, true, 128);

            return JObject.Parse(file.ReadToEnd());

        }


        public static LicenseInfo getLicenseInfo(string path)
        {
            try
            {
                var filestream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var file = new StreamReader(filestream, System.Text.Encoding.UTF8, true, 128);
            

                return JsonConvert.DeserializeObject<LicenseInfo>(file.ReadToEnd().getRawText());
            }
            catch
            {
                Utils.Log("Error Retrieving License Information");
                throw new Exception("Error Retrieving License Information");
            }
        }

        public static void generateLicenseInfo(LicenseInfo info)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\PFSLibrary.dll";

            string key = Guid.NewGuid().ToString().Substring(0, 6);
            string Text = key + AES.Encrypt(JsonConvert.SerializeObject(info), key);

            File.WriteAllText(path, Text);
        }


       

    }







    public class MailInfo
    {
        public string smtpserver { get; set; }

        public int smptpPort { get; set; }

        public string smtppass { get; set; }

        public string From { get; set; }

        public string FromDisplayName { get; set; }

        public string mailSubject { get; set; }

        public string mailBody { get; set; }

        public string Tos { get; set; }

        public string CCs { get; set; }

        public string BCCs { get; set; }

        public AlternateView alternateview { get; set; }

        public List<Attachment> AttachmentItems { get; set;}
      }
}
