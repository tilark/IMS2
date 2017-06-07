using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using IMS2.PublicOperations.Interfaces;
using IMS2.ViewModels;

namespace IMS2.BusinessModel.AlgorithmModel
{
    public class AlgorithmOperationImpl : IAlgorithmOperation
    {
        public decimal GetAlgorithmOperationValue(decimal operand1, decimal operand2, string operationMethod)
        {
            decimal result = 0M;
            switch ((OperationMethod)Enum.Parse(typeof(OperationMethod), operationMethod))
            {
                case OperationMethod.addition:
                    result = operand1 + operand2;
                    break;
                case OperationMethod.division:
                    result = operand1 / operand2;
                    break;
                case OperationMethod.multiplication:
                    result = operand1 * operand2;
                    break;
                case OperationMethod.subtraction:
                    result = operand1 - operand2;
                    break;
            }
            return result;
        }

    }
}