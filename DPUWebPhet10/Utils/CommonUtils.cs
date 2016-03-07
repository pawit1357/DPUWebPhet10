using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using DPUWebPhet10.Models;
using System.IO;
using System.Web.Hosting;

namespace DPUWebPhet10.Utils
{
    public class CommonUtils
    {

        public static String ranromFileName()
        {
            return DateTime.Now.ToString("ddMMyyyHHmmss");
        }
        public static String getCurrentDate()
        {
            return DateTime.Now.ToString("ddMMMyyyyhhmmsstt");
        }

        public static int getCurrentDateInt()
        {
            return Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
        }
        public static String toDate_yyyyMMdd(String _date)
        {
            if (_date == null)
                return String.Empty;
            String[] date = _date.Split('/');

            int dateSize = date.Length;
            if(dateSize!=3)
            {
                return String.Empty;
            }

            return date[2] + date[1] + date[0];
        }
        public static DateTime toDate(String _date)
        {
            //if (_date == null)
            //    return null;
            String[] date = _date.Split('/');

            int dateSize = date.Length;
            if (dateSize != 3)
            {
                //return null;
            }
            //MyString = "1999-09-01 21:34 PM";
            //ddMMyyyy--datetime
            return Convert.ToDateTime(date[2] +"-"+ date[1] +"-"+ date[0]);
        }
        //public static DataSet ToDataSet<T>(this IEnumerable<T> collection, string dataTableName)
        //{
        //    if (collection == null)
        //    {
        //        throw new ArgumentNullException("collection");
        //    }

        //    if (string.IsNullOrEmpty(dataTableName))
        //    {
        //        throw new ArgumentNullException("dataTableName");
        //    }

        //    DataSet data = new DataSet("NewDataSet");
            
        //    data.Tables.Add(FillDataTable(dataTableName, collection));
        //    return data;
        //}

        public String getStaffFullName(TB_APPLICATION_STAFF school)
        {

            return school.TB_M_TITLE.TITLE_NAME_TH+""+school.STAFF_NAME+"  "+school.STAFF_SURNAME;
        }
        public String getStudentFullName(TB_APPLICATION_STUDENT student)
        {

             return student.TB_M_TITLE.TITLE_NAME_TH+""+student.STD_NAME+"  "+student.STD_SURNAME;
        }


        public static Boolean isNumber(String _text)
        {
            try
            {
                int stdCode = Convert.ToInt32(_text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public static Boolean isDouble(String _text)
        {
            try
            {
                double stdCode = Convert.ToDouble(_text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public static String getMonthName(int index)
        {
            String[] monthList = { "", "มกราคม", "กุมภาพันธ์ ", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน", "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม" };
            return monthList[index];
        }


        public static void EncodeQueryString(ref string queryString)
        {
            var array = queryString.Split('&', '=');
            for (int i = 0; i < array.Length; i++)
            {
                string part = array[i];
                if (i % 2 == 1)
                {
                    part = System.Web.HttpUtility.UrlEncode(array[i]);
                    queryString = queryString.Replace(array[i], part);
                }
            }
        }

        public static byte[] getByteImage(int studentLevel)
        {

            FileStream fs = new FileStream(HostingEnvironment.MapPath("~/images/maps/map_" + studentLevel + ".jpg"), FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            int length = (int)br.BaseStream.Length;
            byte[] m_byte = new byte[length];
            m_byte = br.ReadBytes(length);
            br.Close();
            fs.Close();
            return m_byte;

        }

        /*

            ' Format a negative integer or floating-point number in various ways.
  Console.WriteLine("Standard Numeric Format Specifiers")
  s = String.Format("(C) Currency: . . . . . . . . {0:C}" & vbCrLf & _
                    "(D) Decimal:. . . . . . . . . {0:D}" & vbCrLf & _
                    "(E) Scientific: . . . . . . . {1:E}" & vbCrLf & _
                    "(F) Fixed point:. . . . . . . {1:F}" & vbCrLf & _
                    "(G) General:. . . . . . . . . {0:G}" & vbCrLf & _
                    "    (default):. . . . . . . . {0} (default = 'G')" & vbCrLf & _
                    "(N) Number: . . . . . . . . . {0:N}" & vbCrLf & _
                    "(P) Percent:. . . . . . . . . {1:P}" & vbCrLf & _
                    "(R) Round-trip: . . . . . . . {1:R}" & vbCrLf & _
                    "(X) Hexadecimal:. . . . . . . {0:X}" & vbCrLf, _
                    - 123, - 123.45F)
  Console.WriteLine(s)
        */

    }
}