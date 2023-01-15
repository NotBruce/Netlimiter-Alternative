using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Netlimiter_Alternative.Limit
{
    internal class FilterModel
    {
        public ushort port;
        public uint bytes;
        public string hotkeyName;
        public bool isOutbound;

        public FilterModel(ushort port, uint bytes, string hotkeyName, bool isOutbound)
        {
            this.port = port;
            this.bytes = bytes;
            this.hotkeyName = hotkeyName;
            this.isOutbound = isOutbound;
        }

        public Keys getKeyFromString()
        {
            // Casting Key
            return (Keys)new KeysConverter().ConvertFromString(this.hotkeyName);
        }
    }
}
