<h1 align="center">Mai schedule parser</h1>

<h2 align="center">

[<img src="https://img.shields.io/badge/-.NET%20Core-purple?style=for-the-badge">](https://dotnet.microsoft.com/en-us/)
[<img src="https://img.shields.io/badge/LiteDB-5.0.11%2B-lightgrey?style=for-the-badge">](https://www.litedb.org/)

</h2>

Custom parser built on C# regular expressions which extracts schedule from the official website of MAI.

### **Important note**!
Data packing is based on POCO-class *SchedulePos*:
```C#
public class SchedulePos
    {
        public int Id { get; set; }
        public string Time_start { get; set; }
        public string Time_finish { get; set; }
        public string Subject { get; set; }
        public string Location { get; set; }
        public string Group { get; set; }
    }
```
