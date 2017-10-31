using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA
{
    [Serializable]
    public class DragAndDropCanvas
    {
        public bool IsPipe;
        public bool IsPipe90;
        public bool IsText;
        public bool IsEthernet;
        public bool IsCom;
        public bool IsDisplay;
        public bool IsImage;
        public bool IsModbus;
    }
    
}
