using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sneaker.Areas.Identity.Data.DbInit
{
    public interface IDbInitializer
    {
        void Initalize();
    }
}
