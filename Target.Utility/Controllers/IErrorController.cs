using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Target.Utility.Controllers
{
    public interface IErrorController
    {
        void ShowExceptionDetailed(Exception ex);
        void ShowException(Exception ex);
    }
}
