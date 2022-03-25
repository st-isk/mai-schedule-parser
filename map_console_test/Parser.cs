using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using LiteDB;
using System.IO;
using System.Threading;

namespace map_console_test
{
    public static class Parser
    {
        private const string pattern_inst = "<option value=\"Институт №(\\d{1,2})\" >";
        private const string pattern_std_year = "<option value=\"(\\d)\" >";
        //private const string pattern_gr = @"href=""detail\.php\?group=(.{3,5}-.{3,7}-\d{2})"; //для старой верстки сайта
        private const string pattern_gr = "href=\"index\\.php\\?group=(.{3,5}-.{3,7}-\\d{2})\"";
        //private const string pattern_day = "<div class=\"sc-table-col sc-day-header sc-blue.*?<div class=\"sc-table-col sc-day-header sc-gray"; //для старой верстки сайта
        private const string pattern_day = "step-icon me-0 me-sm-3 step-icon-soft-primary.*?step-icon me-0 me-sm-3 step-icon-soft-dark";
        //private const string pattern_time = "<div class=\"sc-table-col sc-item-time\">(\\d{2}:\\d{2}) &ndash; (\\d{2}:\\d{2})</div>";
        private const string pattern_time = "<li class=\"list-inline-item\">(\\d{2}:\\d{2}) &ndash; (\\d{2}:\\d{2})</li>";
        //private const string pattern_class = "<span class=\"glyphicon glyphicon-map-marker\">&nbsp;</span>(.*?)</div>";
        private const string pattern_class = "fad fa-map-marker-alt me-2\"></i>(.*?)</li>";
        //private const string pattern_subj = "<span class=\"sc-title\">(.*?)</span>";
        private const string pattern_subj = "<p class=\"mb-2 fw-semi-bold text-dark\">(.*?)<span class=\"text-nowrap\">(.*?)<span";

        public static string Download(string link) 
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

        private static void Get_groups()
        {
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            var sw = new StreamWriter("groups_list.txt");

            try
            {
                Random rndm = new Random();

                string main_shdl_page = client.DownloadString("https://mai.ru/education/studies/schedule/groups.php"); //Качаем главную страницу с расписанием (на ней есть options со всеми институтами и курсами)

                var inst_collection = Regex.Matches(main_shdl_page, Parser.pattern_inst, RegexOptions.Singleline); //Парсим со странички данные об институтах
                var year_collection = Regex.Matches(main_shdl_page, Parser.pattern_std_year, RegexOptions.Singleline); //Парсим со странички данные о курсах
                foreach (Match m_inst in inst_collection)
                {
                    foreach (Match m_year in year_collection)
                    {
                        try
                        {
                            string inst_year_page = client.DownloadString("https://mai.ru/education/studies/schedule/groups.php?department=Институт+№" + m_inst.Groups[1].Value + "&course=" + m_year.Groups[1].Value); //Бежим по каждому курсу каждого института

                            var gr_collection = Regex.Matches(inst_year_page, Parser.pattern_gr, RegexOptions.Singleline); //Парсим данные о группах на конкретном курсе конкретного института
                            foreach (Match m_gr in gr_collection)
                            {
                                sw.WriteLine(m_gr.Groups[1].Value); //В файл записываем каждую полученную группу 
                                Console.WriteLine("записал группу в файл");
                            }

                            Thread.Sleep(rndm.Next(450, 700));
                        }
                        catch (Exception inner_ex)
                        {
                            Console.WriteLine(inner_ex.Message); 
                            continue; //Хорошо бы устроить здесь нормальный логгер
                            // и предусмотреть возможность моментального автоматического перезапуска парсера в случае выдачи ошибки от сервера,
                            // связанной с количеством и быстротой запросов
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (client != null) client.Dispose();
                if (sw != null) sw.Dispose();
            }
        }
        
        public static void Get_info(/*ILiteCollection<SchedulePos> col*/)
        {
            Random rndm = new Random();
            //Parser.Get_groups();
            var sr= new StreamReader("groups_list.txt");
            string group_str;

            while (!sr.EndOfStream) //бежим по файлу с названиями групп (иначе говоря, берем название каждой группы и далее вставляем в ссылку для скачивания)
            {
                try
                {
                    string actl_grp = sr.ReadLine();
                    if (actl_grp == "") continue;

                    group_str = Parser.Download("https://mai.ru/education/studies/schedule/index.php?group=" + actl_grp); //скачиваем страницу конкретной группы
                    group_str = Regex.Matches(group_str, Parser.pattern_day, RegexOptions.Singleline)[0].Value; //обрезаем полученную строку (удаляем все данные, не относящиеся к текущему дню)

                    MatchCollection matches_time = Regex.Matches(group_str, Parser.pattern_time, RegexOptions.Singleline); //находим на странице совпадения по паттерну времени
                    MatchCollection matches_class = Regex.Matches(group_str, Parser.pattern_class, RegexOptions.Singleline); //находим на странице совпадения по паттерну локации
                    MatchCollection matches_subj = Regex.Matches(group_str, Parser.pattern_subj, RegexOptions.Singleline); //находим на странице совпадения по паттерну названия предмета
                    int mtch_cnt = matches_time.Count;
                    
                    for (int i = 0; i < mtch_cnt; i++) //цикл, в котором записываем данные о времени
                    {
                        //var pos = new SchedulePos(); //экземпляр класса, в который кидаем данные о парах текущей группы 
                        Console.WriteLine(matches_time[i].Groups[1].Value + "-" + matches_time[i].Groups[2].Value);
                        Console.WriteLine(matches_class[i].Groups[1].Value);
                        Console.WriteLine(matches_subj[i].Groups[1].Value + matches_subj[i].Groups[2].Value);
                        //pos.Time_start = matches_time[i].Groups[1].Value;
                        //pos.Time_finish = matches_time[i].Groups[2].Value;
                        //pos.Location = matches_class[i].Groups[1].Value;
                        //pos.Subject = matches_subj[i].Groups[1].Value + matches_subj[i].Groups[2].Value;
                        //pos.Group = m.Groups[1].Value;
                        //if (pos.Group == "") continue;
                        //col.Insert(pos); //Добавляем данные в коллекцию (информацию из экземпляра класса в БД)
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
