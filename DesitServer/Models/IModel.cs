using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesitServer.Models
{
    interface IModel
    {
        void Save();

        void Update();

        void Delete();
    }
}
