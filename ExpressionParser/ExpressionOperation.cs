using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser
{
    class ExpressionOperation : ExpressionNode
    {
        private Operation _operation;
        public ExpressionOperation(Operation operation, ExpressionNode left, ExpressionNode right)
            : base(left, right)
        {
            _operation = operation;
        }
        public override float Value
        {
            get
            {
                switch (_operation)
                {
                    case Operation.Addition:
                        return _leftNode.Value + _rightNode.Value;
                    case Operation.Substraction:
                        return _leftNode.Value - _rightNode.Value;
                    case Operation.Multiply:
                        return _leftNode.Value * _rightNode.Value;
                    case Operation.Divide:
                        return _leftNode.Value / _rightNode.Value;
                    default:
                        return float.NaN;
                }
            }
            protected set
            {

            }
        }
    }
}
