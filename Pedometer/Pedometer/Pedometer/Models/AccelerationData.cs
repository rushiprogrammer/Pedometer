using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pedometer.Models
{
    public class AccelerationData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Ax { get; set; }
        public string Ay { get; set; }
        public string Az { get; set; }
        public string Aall { get; set; }
        public string Atime { get; set; }
        public int StepsCount { get; set; }
        public bool Checked { get; set; }
    }
}
