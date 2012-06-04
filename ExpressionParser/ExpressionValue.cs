using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser
{
    class ExpressionValue : ExpressionNode
    {
        private float _value;

        public ExpressionValue(float value)
            : base(null, null)
        {
            //_value = value;
            Value = value;
        }

        public override float Value
        {
            get
            {
                //return _value;
                return base.Value;
            }
            protected set
            {
                //_value = value;
                base.Value = value;
            }
        }
    }
}
