using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS2.BusinessModel.ObserverMode
{
    public interface IObserver
    {
        ISubject Subject { get; set; }





        void Update();
    }
}
