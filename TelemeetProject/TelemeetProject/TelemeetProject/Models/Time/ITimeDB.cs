using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TelemeetProject.Models
{
    public interface ITimeDB
    {
        BigInteger getTime();
        void updateTime(string time);
    }
}
