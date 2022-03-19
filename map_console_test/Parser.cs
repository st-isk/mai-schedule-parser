using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using LiteDB;
using System.Threading;

namespace map_console_test
{
    public static class Parser
    {
        private const string pattern_inst = "<option value=\"Институт №(\\d{1,2})\" >";
        private const string pattern_std_year = "<option value=\"(\\d)\" >";
        //private const string pattern_gr = @"href=""detail\.php\?group=(.{3,5}-.{3,7}-\d{2})";
        private const string pattern_gr = "href=\"index.php?group=(.{3,5}-.{3,7}-\\d{2})\"";
        private const string pattern_day = "<div class=\"sc-table-col sc-day-header sc-blue.*?<div class=\"sc-table-col sc-day-header sc-gray";
        private const string pattern_time = "<div class=\"sc-table-col sc-item-time\">(\\d{2}:\\d{2}) &ndash; (\\d{2}:\\d{2})</div>";
        private const string pattern_class = "<span class=\"glyphicon glyphicon-map-marker\">&nbsp;</span>(.*?)</div>";
        private const string pattern_subj = "<span class=\"sc-title\">(.*?)</span>";

        public static string Download_old(string link)
        {
            string page;

            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                page = client.DownloadString(link); 
                //client.DownloadFile(link, "pisos.txt");
            }

            return page;
        }

        /*public static void Get_test_info()
        {
            var all_groups = Parser.Download("https://mai.ru/education/studies/schedule/groups.php");
            foreach (Match m in Regex.Matches(all_groups, Parser.pattern_inst, RegexOptions.Singleline))
            {
                Console.WriteLine(m.Groups[1].Value);
            }
            foreach (Match m in Regex.Matches(all_groups, Parser.pattern_std_year, RegexOptions.Singleline))
            {
                Console.WriteLine(m.Groups[1].Value);
            }
        }*/

        private static async void Get_groups()
        {
            var main_shdl_page = await new WebClient().DownloadStringTaskAsync("https://mai.ru/education/studies/schedule/groups.php");
            var inst_collection = Regex.Matches(main_shdl_page, Parser.pattern_inst, RegexOptions.Singleline);
            var year_collection = Regex.Matches(main_shdl_page, Parser.pattern_std_year, RegexOptions.Singleline);
            foreach (Match m_inst in inst_collection)
            {
                foreach (Match m_year in year_collection)
                {
                    var inst_year_page = await new WebClient().DownloadStringTaskAsync("https://mai.ru/education/studies/schedule/groups.php?department=Институт+№" + m_inst + "&course=" + m_year);
                    var gr_collection = Regex.Matches(inst_year_page, Parser.pattern_gr, RegexOptions.Singleline);
                    foreach (Match m_gr in gr_collection)
                    {
                        //Мб сделать список и бахать туда все группы, но чет такой себе варик, скорее просто записывать все это дело в файл
                        //запись в файл сделать асинхронной
                    }
                }
            }
        }

        public static void Get_info(ILiteCollection<SchedulePos> col)
        {
            Random rndm = new Random();
            
            foreach (Match m in Regex.Matches(all_gr_str, Parser.pattern_gr, RegexOptions.Multiline)) //бежим по всем совпадениям по паттерну групп (иначе говоря, берем название каждой группы и далее вставляем в ссылку для скачивания)
            {
                try
                {
                    string group_str = Parser.Download("https://mai.ru/education/schedule/detail.php?group=" + m.Groups[1].Value); //скачиваем страницу конкретной группы
                    group_str = Regex.Matches(group_str, Parser.pattern_day, RegexOptions.Singleline)[0].Value; //обрезаем полученную строку (удаляем все данные, не относящиеся к текущему дню)
                    Console.WriteLine(m.Groups[1].Value);

                    MatchCollection matches_time = Regex.Matches(group_str, Parser.pattern_time, RegexOptions.Singleline); //находим на странице совпадения по паттерну времени
                    MatchCollection matches_class = Regex.Matches(group_str, Parser.pattern_class, RegexOptions.Singleline); //находим на странице совпадения по паттерну локации
                    MatchCollection matches_subj = Regex.Matches(group_str, Parser.pattern_subj, RegexOptions.Singleline); //находим на странице совпадения по паттерну названия предмета
                    int mtch_cnt = matches_time.Count;
                    
                    for (int i = 0; i < mtch_cnt; i++) //цикл, в котором записываем данные о времени
                    {
                        var pos = new SchedulePos(); //экземпляр класса, в который кидаем данные о парах текущей группы 
                        Console.WriteLine(matches_time[i].Groups[1].Value + "-" + matches_time[i].Groups[2].Value);
                        Console.WriteLine(matches_class[i].Groups[1].Value.Trim());
                        Console.WriteLine(matches_subj[i].Groups[1].Value);
                        pos.Time_start = matches_time[i].Groups[1].Value;
                        pos.Time_finish = matches_time[i].Groups[2].Value;
                        pos.Location = matches_class[i].Groups[1].Value.Trim();
                        pos.Subject = matches_subj[i].Groups[1].Value;
                        pos.Group = m.Groups[1].Value;
                        if (pos.Group == "") continue;
                        col.Insert(pos); //Добавляем данные в коллекцию (информацию из экземпляра класса в БД)
                    }

                    Console.WriteLine("---");
                    Thread.Sleep(rndm.Next(450, 700));
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //continue;
                }
            }
        }
    }
}
