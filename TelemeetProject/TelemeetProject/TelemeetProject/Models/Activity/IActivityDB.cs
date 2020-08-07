using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelemeetProject.Models
{
    public interface IActivityDB
    {
        void logActivity(Activity activity);
        List<Activity> GetActivities();
    }
}
