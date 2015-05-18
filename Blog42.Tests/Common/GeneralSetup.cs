using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog42.Tests.Common
{
    public class GeneralSetup
    {
        public GeneralSetup() {
            // Define o diretório do repositório
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test_Data"));
        }
    }
}
