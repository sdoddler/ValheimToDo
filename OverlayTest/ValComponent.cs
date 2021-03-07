using System;

namespace ValheimToDo
{
    [Serializable]
    public class ValComponent
    {

        public bool IsSelected { get; set; }
        public string Title { get; set; }
        public int Amount { get; set; }

        public ValComponent(bool isSelected, string title, int amount)
        {
            this.IsSelected = isSelected;
            this.Title = title;
            this.Amount = amount;
        }
    }
}