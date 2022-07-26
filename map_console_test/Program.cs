using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;
using LiteDB;

namespace map_console_test
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime time = DateTime.Parse("10:25"); //вводим нужное/текущее время
            using (var db = new LiteDatabase(@"MAI.db"))
            {
                //var col = db.GetCollection<SchedulePos>("Schedule");
              
                //var result = col.Find(x => time > DateTime.Parse(x.Time_start) && time < DateTime.Parse(x.Time_finish));  
            }
        }

    }
}
