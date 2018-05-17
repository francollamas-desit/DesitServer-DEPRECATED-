using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesitServer.Models
{
    public class DbAccess
    {

        private static DbAccess instance;

        public static DbAccess Instance {
            get
            {
                if (instance == null) instance = new DbAccess();
                return instance;
            }
        }

        public string ConnectionString { get; set; }

        public DbAccess()
        {

        }


    }
}
