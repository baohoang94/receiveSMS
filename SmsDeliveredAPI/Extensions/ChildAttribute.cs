using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmsDeliveredAPI.Extensions
{
    public class ChildAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
