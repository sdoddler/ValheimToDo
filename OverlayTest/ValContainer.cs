using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimToDo
{
    [Serializable]
    class ValContainer
    {
        public string ContainerName { get; set; }
        public List<ValItem> ContainerItems { get; set; }

        public ValContainer(string containerName, List<ValItem> containerItems)
        {
            ContainerName = containerName;
            ContainerItems = containerItems;
        }
    }
}
