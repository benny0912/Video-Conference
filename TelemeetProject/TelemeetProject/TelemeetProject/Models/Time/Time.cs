using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TelemeetProject.Models
{
    public class Time
    {
        public partial class Request
        {
            public string Time { get; set; }
        }
        public BigInteger capture_time { get; set; }
    }
}
