using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Math
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    internal class IgnoreMemberAttribute : Attribute
    {

        public IgnoreMemberAttribute()
        {
            
        }

    }
}
