using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Xml.Serialization;

namespace BackBone
{
    public static class Exensions
    {
        /// <summary>
        /// Trims and and convert input string to lowercase
        /// </summary>
        /// <param name="input"></param>
        /// <returns>string</returns>
        public static String CleanUp(this String input)
        {
            try {
                return input.Trim().ToLower();
            }
            catch
            {
                return string.Empty;
            }

        }



        public static string getAgeGroup(this DateTime date)
        {
            var age = Utils.getAge(date);

            if (age <= 30)
            {
                return "0-30";
            }


            if (age > 30 && age <=60)
            {
                return "31-60";
            }

            if (age > 60 && age <= 90)
            {
                return "61-90";
            }

            if (age > 60 && age <= 90)
            {
                return "61-90";
            }

            if (age > 90 && age <= 180)
            {
                return "91-180";
            }

            if (age > 180 && age <= 360)
            {
                return "181-360";
            }

     
            return "OVER360";
            

        }



        public static void AddTableColumns(this DataTable dt, string [] columns)
        {
           foreach(var column in columns)
            {
                var myDataColumn = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = column
                };

                dt.Columns.Add(myDataColumn);
            }
          

        }


        public static string  DataTableToJson(this DataTable dataTable)
        {
            var result = new StringBuilder();
            result.Append("[");
            var records = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {


                var fields = new List<string>();

                StringBuilder data = new StringBuilder();
                data.Append("{");
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {

                    fields.Add(string.Format("\"{0}\":\"{1}\"", new object[] { dataTable.Columns[i].ColumnName, row[i].ToString() }));
                }

                data.Append(string.Join(",", fields));

                data.Append("}");

                records.Add(data.ToString());




            }


            result.Append(string.Join(",", records));

            result.Append("]");

            return result.ToString();

        }

        public static string getValidFormat(this DateTime input)
        {
            try
            {
                return input.ToString("yyyy-MM-dd h:mm:ss.000");
            }
            catch(Exception u)
            {
                throw (u);
            }

        }


        /// <summary>
        /// Decrypts a specialized AES Encrypted text to raw text
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string getRawText(this string input)
        {
            try
            {

                var value = input.Substring(6, input.Length - 6);

                var key = input.Substring(0, 6);
                
                return AES.Decrypt(value, key);
            }
            catch
            {
                throw;
            }

        }


        public static string getValidPostllonFormat(this DateTime input)
        {
            try
            {
                return input.ToString("dd-MMM-yy h.mm.ss tt");
            }
            catch (Exception u)
            {
                throw (u);
            }

        }

        public static string getRemoteMachineName(this HttpContext context)
        {
            string strIpAddress = "";
            try
            {
                strIpAddress = System.Net.Dns.GetHostEntry(context.Request.UserHostName).HostName;
            }
            catch { }

            if (strIpAddress.isNull())
            {
                strIpAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (strIpAddress.isNull())
                {
                    strIpAddress = context.Request.ServerVariables["REMOTE_ADDR"];
                }
            }

            return strIpAddress;
        }

        public static string getRemoteMachineName(this Page context)
        {
            string strIpAddress = "";
            try
            {
                strIpAddress = System.Net.Dns.GetHostEntry(context.Request.UserHostName).HostName;
            }
            catch { }

            if (strIpAddress.isNull())
            {
                strIpAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (strIpAddress.isNull())
                {
                    strIpAddress = context.Request.ServerVariables["REMOTE_ADDR"];
                }
            }

            return strIpAddress;
        }


        /// <summary>
        /// Attempts to convert an object to Decimal
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static decimal toDecimal(this Object input)
        {
            if (input == null) return decimal.Zero;
            return string.IsNullOrEmpty(input.ToString())? decimal.Zero:Convert.ToDecimal(input.ToString());

        }

        /// <summary>
        /// Attempts to convert an object to DateTime
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime toDateTime(this Object input)
        {
            if(System.Globalization.CultureInfo.CurrentCulture.Equals(new System.Globalization.CultureInfo("en-us")))
            {
                return Convert.ToDateTime(input.ToString(), new System.Globalization.CultureInfo("en-us"));
            }
            else
           
            {
                return Convert.ToDateTime(input.ToString(), new System.Globalization.CultureInfo("en-GB"));
            }
           

        }


        /// <summary>
        /// Attempts to convert an object to Float
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static float toFloat(this Object input)
        {

            return float.Parse(input.ToString());

        }

        /// <summary>
        /// Attempts to convert an object to Int
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Int32 toInt(this Object input)
        {
            if (input.ToString().isNull()) return 0;
                return Convert.ToInt32(input.ToString());
        

        }

        /// <summary>
        /// Attempts to convert an object to int, returns zero when an empty string is detected
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Int32 getIntorZero(this Object input)
        {
            try {
                return Convert.ToInt32(input.ToString());
            }
            catch
            {
                if (input.ToString().isNull())
                {
                    return 0;
                }
                else
                {
                    throw new Exception(input.ToString()+" is not a valid integer input");
                }
            }

        }

        /// <summary>
        /// Attempts to convert an object to Long
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static long toLong(this Object input)
        {

            return long.Parse(input.ToString());

        }

        /// <summary>
        /// Attempts to convert an object to string, returns an empty string if object is null
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string getString( this Object input)
        {
            return input.isNull() ? string.Empty : input.ToString().Trim();
        }


        /// <summary>
        /// tests if an object is null
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool isNull(this Object input)
        {

            return input == null;

        }

        /// <summary>
        /// tests if a string is empty or null
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool isNull(this string input)
        {

            return string.IsNullOrEmpty(input);

        }

        public static bool toBoolean(this Object input)
        {

            return Convert.ToBoolean(input.ToString());

        }

        /// <summary>
        /// attempts to get a string collection of all numeric values in the string that represents an input object
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<string> getNumerics(this Object input)
        {
            var numerics = new List<string>();

            var regex = new Regex(@"(?<=)\d+");

            var r = regex.Match(input.ToString());

            var b = string.Empty;

            while (!(b = r.Value).isNull())
            {
                try {
                    numerics.Add(b.ToString().Length > 20 ? b.ToString() : b.toLong().ToString());
                }
                catch
                {
                    try {
                        numerics.Add(b.toLong().ToString());
                    }
                    catch
                    {
                        numerics.Add(b.ToString());
                    }
                }
                r = r.NextMatch();
               }


            return numerics;
        }

        public static List<string> getNumerics(this Object input, int count)
        {
            var numerics = new List<string>();

            var regex = new Regex(@"(?<=)\d+");

            var r = regex.Match(input.ToString());

            var b = string.Empty;

            while (!(b = r.Value).isNull())
            {
                try
                {
                    numerics.Add(b.ToString().Length > 20 ? b.ToString() : b.toLong().ToString());
                }
                catch
                {
                 
                        numerics.Add(b.ToString());
                 
                }
                r = r.NextMatch();
                if (numerics.Count == count) break;
            }


            return numerics;
        }

        /// <summary>
        /// gets all numerics as a single string seperated by space from an input string 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string getAllNumerics(this Object input)
        {
            var numerics = input.getNumerics();

            var groups = new StringBuilder();

            foreach(var group in numerics)
            {
                groups.Append(" ").Append(group).Append(" ");
            }

            return groups.ToString();
        }





        /// <summary>
        /// gets a string range defined by a key and delimeter
        /// </summary>
        /// <param name="input"></param>
        /// <param name="keyword"></param>
        /// <param name="delimeter"></param>
        /// <returns></returns>
        public static string getDynamicKey(this Object input, string keyword, string delimeter)
        {
            var bodyRegex = new Regex(keyword + @"[\s\S]*" + delimeter);

            return keyword.isNull() ? input.ToString():bodyRegex.Match(input.ToString()).Value.Replace(keyword, "").CleanUp();
      
        }



        public static List<string> getAllDynamicKey(this Object input, string keyword, string delimeter)
        {
            var bodyRegex = new Regex(keyword + @"[\s\S]*" + delimeter);

            var matches = new List<string>();

            var match = bodyRegex.Match(input.ToString());

           

            Match a = match.NextMatch();

            matches.Add(match.Value);

            string b;
            while ((b = match.NextMatch().Value) != null)
            {
                if (b == "") break;
                matches.Add(b);
            }

            return matches;
        }

        public static string serializeToXML(this object o)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(o.GetType());
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, o);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }
            return sw.ToString();
        }

        public static Object fromXMLtoObject(this string xml, Type objectType)
        {
            StringReader strReader = null;
            XmlSerializer serializer = null;
            XmlTextReader xmlReader = null;
            Object obj = null;
            try
            {
                strReader = new StringReader(xml);
                serializer = new XmlSerializer(objectType);
                xmlReader = new XmlTextReader(strReader);
                obj = serializer.Deserialize(xmlReader);
            }
            catch (Exception exp)
            {
                //Handle Exception Code
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
                if (strReader != null)
                {
                    strReader.Close();
                }
            }
            return obj;
        }


        public static byte[] StringToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static bool IsParseableAs<TInput>(this string value)
        {
            var type = typeof(TInput);

            var tryParseMethod = type.GetMethod("TryParse", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder,
                new[] { typeof(string), type.MakeByRefType() }, null);
            if (tryParseMethod == null) return false;

            var arguments = new[] { value, Activator.CreateInstance(type) };
            return (bool)tryParseMethod.Invoke(null, arguments);
        }


        public static void WriteDataTableToCSV(this DataTable sourceTable, TextWriter writer, bool includeHeaders)
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
            return String.Concat("\"",
            value.Replace("\"", "\"\""), "\"");
        }

        //public static object CastToModel<T>(this DataTable table)
        //{
        //    var type = typeof(T);

        //    var properties = type.GetProperties();

        //    var outputobject = ty

        //    int pos = 0;  

        //    for (int i = 0; i < table.Rows.Count; i++) {

        //        foreach (var property in properties)
        //        {

        //            if (property.CanWrite)
        //            {
        //                //var ptype = property.GetType();

        //                var value = table.Rows[i][property.Name].ToString();



        //                property.SetValue(outputobject, value, null);

        //            }
        //        }

        //        if (type.IsArray)
        //        {
        //           // var addMethod = type.("Parse", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder,
        //           //new[] { typeof(string), type.MakeByRefType() }, null);
        //        }
        //    }

        //    return null;
        //}

        public static DataTable BsonToDataTable(this List<BsonDocument> data)
        {
            DataTable txn = new DataTable();

            var first = data[0];

            foreach (BsonElement element in first.Elements.Where(e => e.Name.CleanUp() != "_id"))
            {
                txn.Columns.Add(new DataColumn() { ColumnName = element.Name, DataType = Type.GetType("System.String") });
            }


            foreach (BsonDocument document in data)
            {
                var row = txn.NewRow();

                foreach (DataColumn column in txn.Columns)
                {
                    try
                    {
                        row[column.ColumnName] = document.GetElement(column.ColumnName).Value.ToString();
                    }
                    catch (Exception d)
                    {

                    }
                }

                txn.Rows.Add(row);
               
            }

            return txn;

        }

        public static void SaveDataTableToCollection(this DataTable dt, string Constr, string CollectionName, string DatabaseName)
        {

            var client = new MongoClient(Constr);
            var database = client.GetDatabase(DatabaseName);



            var collection = database.GetCollection<BsonDocument>(CollectionName);

            List<BsonDocument> batch = new List<BsonDocument>();
            foreach (DataRow dr in dt.Rows)
            {
                var dictionary = dr.Table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => dr[col.ColumnName]);
                batch.Add(new BsonDocument(dictionary));
            }

            var task = collection.InsertManyAsync(batch.AsEnumerable());

            task.Wait(10);

        }





        public static string getOfsString(this DataTable dt, string appname)
        {
            var outstring = new StringBuilder();


            for(int i =0;i<dt.Columns.Count; i++)
            {
                
            }

            return outstring.ToString();
        }

    }
}