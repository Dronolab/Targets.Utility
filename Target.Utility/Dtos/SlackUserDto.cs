using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Target.Utility.Dtos
{
    public class SlackUserDto
    {
        public string Id { get; set; }
        public string RealName { get; set; }
        public string Image32 { get; set; }
        public int Points { get; set; }
        public int NewPoints { get; set; }

        public override string ToString()
        {
            return this.RealName;
        }
    }
}
