using ValheimToDo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ValheimToDo
{
    public class SharedData
    {
        public ObservableCollection<ToDo> toDos = new ObservableCollection<ToDo>();
        public Color? bkColor = Color.FromArgb(130, 0x33, 0x36, 0x3F);
        public int monitorOffsetX = 0;
        public int monitorOffsetY = 0;
        public int userOffsetX = 0;
        public int userOffsetY = 0;
    }
}
