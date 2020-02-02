using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Keystroke.API;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Json.Net;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;

namespace FLash
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            Flash flash = new Flash(new KeystrokeAPI());
        }

        




    }
}
