using ComboBoxTreeViewSample;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverlayTest
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            var items13 = new List<SomeHierarchyViewModel>{
                                new SomeHierarchyViewModel("Item 1.3.1", null),
                                new SomeHierarchyViewModel("Item 1.3.2", null)};

            var items1 = new List<SomeHierarchyViewModel>{
                                new SomeHierarchyViewModel("Item 1.1", null),
                                new SomeHierarchyViewModel("Item 1.2", null),
                                new SomeHierarchyViewModel("Item 1.3", items13)};
            var items2 = new List<SomeHierarchyViewModel>{
                                new SomeHierarchyViewModel("Item 2.1", null),
                                new SomeHierarchyViewModel("Item 2.2", null)};

            var outerItems = new List<SomeHierarchyViewModel>{
                                new SomeHierarchyViewModel("Item 1", items1),
                                new SomeHierarchyViewModel("Item 2", items2)};

            this.Items = outerItems;
            this.SelectedItem = this.Items[0].Children[1];
        }

        public List<SomeHierarchyViewModel> Items { get; set; }

        public SomeHierarchyViewModel SelectedItem { get; set; }
    }

    public class SomeHierarchyViewModel : ITreeViewItemModel
    {
        public SomeHierarchyViewModel(string title, List<SomeHierarchyViewModel> children, SomeHierarchyViewModel parent = null)
        {
            this.Title = title;
            this.Parent = parent;
            this.Children = children;

            if (this.Children != null)
            {
                this.Children.ForEach(ch => ch.Parent = this);
            }
        }

        public string Title { get; set; }

        public SomeHierarchyViewModel Parent { get; set; }

        public List<SomeHierarchyViewModel> Children { get; set; }

        #region ITreeViewItemModel
        public string SelectedValuePath
        {
            get { return Title; }
        }

        public string DisplayValuePath
        {
            get { return Title; }
        }

        private bool isExpanded;

        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                RaisePropertyChanged("IsExpanded");
            }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public IEnumerable<ITreeViewItemModel> GetHierarchy()
        {
            return GetAscendingHierarchy().Reverse();
        }

        public IEnumerable<ITreeViewItemModel> GetChildren()
        {
            return this.Children;
        }

        #endregion

        private IEnumerable<SomeHierarchyViewModel> GetAscendingHierarchy()
        {
            var vm = this;

            yield return vm;
            while (vm.Parent != null)
            {
                yield return vm.Parent;
                vm = vm.Parent;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
