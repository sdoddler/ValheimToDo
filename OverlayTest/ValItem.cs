using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimToDo
{
    [Serializable]
    public class ValItem
    {

        public string DisplayText { get; set; }
        public int ItemHierarchy { get; set; }
        public bool IsHeader { get; set; }

        public List<ValComponent> Recipe{get;set;}

            public ValItem(string displayText, bool isHeader, int itemHierarchy = 0, List<ValComponent> recipe = null)
            {
                DisplayText = displayText;
                 IsHeader = isHeader;
                ItemHierarchy = itemHierarchy;
                if (recipe == null)
                {
                    Recipe = new List<ValComponent>();
                }else
                Recipe = recipe;
            }
        
    }
}
