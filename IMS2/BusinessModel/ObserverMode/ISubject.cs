using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS2.BusinessModel.ObserverMode
{
    public interface ISubject
    {
        List<IObserver> Observers { get; set; }





        void Attach(IObserver observer);

        void Detach(IObserver observer);

        void Notify();
    }
}
