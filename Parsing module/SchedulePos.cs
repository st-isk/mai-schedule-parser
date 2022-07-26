using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsing_module
{
    /// <summary>
    /// POCO-класс для использования LiteDB
    /// </summary>
    public class SchedulePos
    {
        public int Id { get; set; }
        public string Time_start { get; set; }
        public string Time_finish { get; set; }
        public string Subject { get; set; }
        public string Location { get; set; }
        public string Group { get; set; }

        public SchedulePos()
        {
            this.Time_start = "";
            this.Time_finish = "";
            this.Subject = "";
            this.Location = "";
            this.Group = "";
        }
    }
}
