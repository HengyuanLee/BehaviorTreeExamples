using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework{
    public interface IAppModule
    {
        void OnInit();
        void OnUpdate();
    }
}
