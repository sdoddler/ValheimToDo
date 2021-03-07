using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ValheimToDo
{
    [Serializable]
    public class ToDo
    {
       
            public bool IsSelected { get; set; }
            public string Title { get; set; }
         //   public string Note { get; set; }

        public string Location { get; set; }

        public int Amount { get; set; }


        public System.Drawing.Color O_Color { get; set; }

        public List<ValComponent> Components { get; set; }

        public ToDo() { }

            public ToDo(bool isSelected, int amount, string title,  string location, System.Drawing.Color color, List<ValComponent> components = null)
            {
                this.IsSelected = isSelected;
            this.Amount = amount;
                this.Title = title;
            //    this.Note = note;
            this.Location = location;
            this.O_Color = color;
            if (components == null)
                 this.Components = new List<ValComponent>();
            else
            this.Components = components;

            }
    }
}