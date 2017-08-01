using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS2.BusinessModel.AlgorithmModel
{
    //算法处理
    public interface IAlgorithmOperation
    {
       decimal? GetAlgorithmOperationValue(decimal operand1, decimal operand2, string operationMethod);
        
    }
}
