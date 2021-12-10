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
            using (var db = new LiteDatabase(@"test1.db"))
            {
                var col = db.GetCollection<Classroom>();
                Parser.Get_info(col);
            }

        }

    }
}
