using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser
{
	class ExpressionNode
	{
		protected ExpressionNode _leftNode;
		protected ExpressionNode _rightNode;

		public ExpressionNode(ExpressionNode _l = null, ExpressionNode _r = null)
		{
			_leftNode = _l;
			_rightNode = _r;
		}

        public virtual double Value
        {
            get;
            protected set;
        }
	}
}
