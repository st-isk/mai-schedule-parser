using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace map_console_test
{
    public class Classroom 
    {
        /*
         * По каждому индексу в соответствующем массиве находим нужную информацию, например:
         * для i=0 смотрим локацию, название предмета и время начала и конца пары
         */
        public int Id { get; set; }
        public string[] Time_start { get; set; } 
        public string[] Time_finish { get; set; }
        public string[] Subject { get; set; }
        public string[] Location { get; set; }

        public Classroom(int n)
        {
            Time_start = new string[n];
            Time_finish = new string[n];
            Subject = new string[n];
            Location = new string[n];
        }

    }
}
