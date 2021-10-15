using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace map_console_test
{
    class Parser
    {
        string all_gr_str = Download("https://mai.ru/education/schedule/");

        static string Download(string link)
        {
            string page;

            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                page = client.DownloadString(link);
                //client.DownloadFile("https://mai.ru/education/schedule/detail.php?group=М1О-109М-21", "E:/test1.txt");
            }

            return page;
        }
    }
}
