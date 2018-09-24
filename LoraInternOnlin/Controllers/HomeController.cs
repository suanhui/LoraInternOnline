using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Diagnostics;

namespace LoraInternOnlin.Controllers
{
    public class HomeController : Controller
    {
        DateTime date = DateTime.Now.AddDays(-7);

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public WebImage dustChart(DateTime date)
        {
            var tuple = ConnectSQL(date);
            var hankrecords = tuple.Item1;
            var lorarecords = tuple.Item2;

            var dusttimelist = hankrecords.Select(i => i.Time).ToArray();
            var dustvaluelist = hankrecords.Select(i => i.Dust).ToArray();

            var dusttimelist1 = lorarecords.Select(i => i.Time).ToArray();
            var dustvaluelist1 = lorarecords.Select(i => i.Dust).ToArray();

            var dustchart = CreateChart(dusttimelist, dustvaluelist, dusttimelist1, dustvaluelist1);

            return dustchart.ToWebImage();
        }

        public WebImage uvChart(DateTime date)
        {
            var tuple = ConnectSQL(date);
            var hankrecords = tuple.Item1;
            var lorarecords = tuple.Item2;

            var uvtimelist = hankrecords.Select(i => i.Time).ToArray();
            var uvvaluelist = hankrecords.Select(i => i.UV).ToArray();

            var uvtimelist1 = lorarecords.Select(i => i.Time).ToArray();
            var uvvaluelist1 = lorarecords.Select(i => i.UV).ToArray();

            var uvchart = CreateChart(uvtimelist, uvvaluelist, uvtimelist1, uvvaluelist1);

            return uvchart.ToWebImage();
        }

        public WebImage tempChart(DateTime date)
        {
            var tuple = ConnectSQL(date);
            var hankrecords = tuple.Item1;
            var lorarecords = tuple.Item2;

            var temptimelist = hankrecords.Select(i => i.Time).ToArray();
            var tempvaluelist = hankrecords.Select(i => i.Temp).ToArray();

            var temptimelist1 = lorarecords.Select(i => i.Time).ToArray();
            var tempvaluelist1 = lorarecords.Select(i => i.Temp).ToArray();

            var tempchart = CreateChart(temptimelist, tempvaluelist, temptimelist1, tempvaluelist1);

            return null;
        }

        public WebImage presChart(DateTime date)
        {
            var tuple = ConnectSQL(date);
            var hankrecords = tuple.Item1;
            var lorarecords = tuple.Item2;

            var prestimelist = hankrecords.Select(i => i.Time).ToArray();
            var presvaluelist = hankrecords.Select(i => i.Pressure).ToArray();

            var prestimelist1 = lorarecords.Select(i => i.Time).ToArray();
            var presvaluelist1 = lorarecords.Select(i => i.Pressure).ToArray();

            var preschart = CreateChart(prestimelist, presvaluelist, prestimelist1, presvaluelist1);

            return null;
        }

        public WebImage humChart(DateTime date)
        {
            var tuple = ConnectSQL(date);
            var hankrecords = tuple.Item1;
            var lorarecords = tuple.Item2;

            var humtimelist = hankrecords.Select(i => i.Time).ToArray();
            var humvaluelist = hankrecords.Select(i => i.Humidity).ToArray();

            var humtimelist1 = lorarecords.Select(i => i.Time).ToArray();
            var humvaluelist1 = lorarecords.Select(i => i.Humidity).ToArray();

            var humchart = CreateChart(humtimelist, humvaluelist, humtimelist1, humvaluelist1);

            return null;
        }

        public WebImage RSSIChart(DateTime date)
        {
            var tuple = ConnectSQL(date);
            var hankrecords = tuple.Item1;
            var lorarecords = tuple.Item2;

            var rssitimelist = hankrecords.Select(i => i.Time).ToArray();
            var rssivaluelist = hankrecords.Select(i => i.RSSI).ToArray();

            var rssitimelist1 = lorarecords.Select(i => i.Time).ToArray();
            var rssivaluelist1 = lorarecords.Select(i => i.RSSI).ToArray();

            var rssichart = CreateChart(rssitimelist, rssivaluelist, rssitimelist1, rssivaluelist1);

            return null;
        }
        
        public Tuple<List<SensorData>,List<SensorData>> ConnectSQL(DateTime date)
        {
            SqlConnectionStringBuilder sql = new SqlConnectionStringBuilder();

            DateTime date2 = DateTime.Now;

            Debug.WriteLine(date, "date a week ago");
            Debug.WriteLine(date2, "today");

            string retrieve = string.Format("select * from(select Row_Number() over (order by TIMESUBMIT) as RowIndex, * from LORA_TABLE) " +
                "as Sub where TimeSubmit >= '{0}' and TimeSubmit <='{1}';", date.ToString("MM-dd-yyyy HH:mm:ss"), date2.ToString("MM-dd-yyyy HH:mm:ss"));

            Debug.WriteLine(retrieve, "sql test");

            //list for client "HANK"
            List<SensorData> hankrecords = new List<SensorData>();

            //list for client "LORA"
            List<SensorData> lorarecords = new List<SensorData>();

            //build conenction string
            sql.DataSource = "lorashp.database.windows.net";
            sql.UserID = "shp";
            sql.Password = "Loraintern1234";
            sql.InitialCatalog = "lorashp"; // database name

            using (SqlConnection sqlConn = new SqlConnection(sql.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(retrieve, sqlConn);
                try
                {
                    sqlConn.Open();
                    sqlCommand.ExecuteNonQuery();
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DateTime time = reader.GetDateTime(3);
                            if (reader.GetString(1) == "HANK")
                            {
                                hankrecords.Add(new SensorData()
                                {
                                    Time = time,
                                    Dust = reader.GetValue(4),
                                    UV = reader.GetValue(5),
                                    Temp = reader.GetValue(6),
                                    Pressure = reader.GetValue(7),
                                    Humidity = reader.GetValue(8),
                                    RSSI = reader.GetValue(9)
                                });
                            }
                            if (reader.GetString(1) == "LORA")
                            {
                                lorarecords.Add(new SensorData()
                                {
                                    Time = time,
                                    Dust = reader.GetValue(4),
                                    UV = reader.GetValue(5),
                                    Temp = reader.GetValue(6),
                                    Pressure = reader.GetValue(7),
                                    Humidity = reader.GetValue(8),
                                    RSSI = reader.GetValue(9)
                                });
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Debug.WriteLine(ex);
                }
                sqlConn.Close();
            }
            return Tuple.Create(hankrecords, lorarecords);
        }

        public Chart CreateChart(DateTime[] hanktimelist,object[] hankvaluelist,DateTime[] loratimelist,object[] loravaluelist)
        {
            var chart = new Chart(width: 1000, height: 300)
                .AddLegend("Lora Clients")
                .AddSeries(
                name: "Hank",
                chartType: "line",
                xValue: hanktimelist,
                yValues: hankvaluelist)
                .AddSeries(
                name: "Lora",
                chartType: "line",
                xValue: loratimelist,
                yValues: loravaluelist)
                .Write("png");

            return chart;
        }

        public class SensorData
        {
            public DateTime Time
            {
                get;
                set;
            }

            public object Dust
            {
                get;
                set;
            }

            public object UV
            {
                get;
                set;
            }

            public object Temp
            {
                get;
                set;
            }

            public object Pressure
            {
                get;
                set;
            }

            public object Humidity
            {
                get;
                set;
            }

            public object RSSI
            {
                get;
                set;
            }
        }
    }   
}