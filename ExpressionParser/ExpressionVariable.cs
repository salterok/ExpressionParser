using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser
{
    class ExpressionVariable : ExpressionNode
    {
        public string Name { get; private set; }
        public ExpressionVariable(string name)
            : base(null, null)
        {
            Name = name;
        }

        public override float Value
        {
            get
            {
                return base.Value;
            }
            protected set
            {
                base.Value = value;
            }
        }
    }
}
