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
        public const string pattern_gr = @"href=""detail\.php\?group=(.{3,5}-.{3,7}-\d{2})";
        public const string pattern_day = "<div class=\"sc-table-col sc-day-header sc-blue.*?<div class=\"sc-table-col sc-day-header sc-gray";
        public const string pattern_time = "<div class=\"sc-table-col sc-item-time\">(\\d{2}:\\d{2}) &ndash; (\\d{2}:\\d{2})</div>";
        public const string pattern_class = "<span class=\"glyphicon glyphicon-map-marker\">&nbsp;</span>(.*?)</div>";
        public const string pattern_subj = "<span class=\"sc-title\">(.*?)</span>";

        public static string Download(string link)
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

        public static void Get_info(ILiteCollection<SchedulePos> col)
        {
            Random rndm = new Random();
            var all_gr_str = Parser.Download("https://mai.ru/education/schedule/");
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
