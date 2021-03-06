﻿using System;
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

        public override double Value
        {
            get
            {
				throw new NotImplementedException();
            }
            protected set
            {
                base.Value = value;
            }
        }
    }
}
