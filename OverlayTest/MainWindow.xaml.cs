using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ValheimToDo.Properties;
using ValheimToDo;

namespace ValheimToDo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool overlayRunning = false;
        private bool overlayCreated = false;

        private ValheimOverlay example;

        //private ObservableCollection<ToDo> toDos { get; set; } = new ObservableCollection<ToDo>();

        //public ObservableCollection<ToDo> SharedData => toDos;

        private readonly SharedData _sharedData = new SharedData();

        private ObservableCollection<ValItem> ValItems { get; set; } = new ObservableCollection<ValItem>();
        private ObservableCollection<ValContainer> ValContainers { get; set; } = new ObservableCollection<ValContainer>();

        private Screen curScreen = Screen.PrimaryScreen;

        public MainWindow()
        {
            //   _sharedData.toDos.Add(new ToDo(false, "yeet", "yote","c204"));

            foreach (ValItem v in ReadCSV(Properties.Resources.ValheimData))
            {
                ValItems.Add(v);
            }


            foreach (Screen s in Screen.AllScreens)
            {
                Debug.WriteLine(s.DeviceName + " - " + s.WorkingArea.ToString());

            }



            InitializeComponent();

            cbo_Test.ItemsSource = ValItems;
            //cbo_Test.SelectedItem = "Club";

            LoadSettings();

            dataGrid.ItemsSource = _sharedData.toDos;
        }

        public IEnumerable<ValItem> ReadCSV(string fileName, char del = ',')
        {
            // TODO: Error checking.
            string[] lines   = Properties.Resources.ValheimData.Split('\n');//= File.ReadAllLines(System.IO.Path.ChangeExtension(fileName, ".csv"));


            return lines.Select(line =>
            {
                string[] hierarchy = line.Split('>');
                string[] data = hierarchy[hierarchy.Length - 1].Split(del);
                bool header = false;
                if (data.Length >= 2)
                {
                    if (data[1] == "Header") header = true;
                }

                ValItem curItem = new ValItem(data[0], header, hierarchy.Length - 1);
                if (header == false)
                {
                    for (int i = 1; i < data.Length; i++)
                    {
                        if (data[i] != string.Empty)
                        {
                            string[] split = data[i].Split(null, 2);

                            if (split.Length > 1)
                            {
                                Int32.TryParse(split[0], out int a);

                                if (a != 0)
                                curItem.Recipe.Add(new ValComponent(false, split[1], a));
                            }
                            else
                                curItem.Recipe.Add(new ValComponent(false, split[0], 1));
                        }

                    }
                }
                return curItem; //(data[0], data[1], Convert.ToInt32(data[2]), data[3]);
            });
        }



        private void btn_Overlay_Click(object sender, RoutedEventArgs e)
        {
            if (!overlayCreated)
            {
                overlayCreated = true;
                InitializeOverlay();
            }
            if (!overlayRunning)
            {

                overlayRunning = true;

                example.Window.Show();

            }
            else
            {
                overlayRunning = false;
                example.Window.Hide();
                //   example.Window.Dispose();
            }

        }

        private void InitializeOverlay()
        {
            example = new ValheimOverlay(_sharedData);
            example.Window.Create();

            example.Window.Hide();

        }

        private void btn_AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (cbo_Test.Text == string.Empty) return;
            ToDo x;

            Debug.WriteLine(cbo_Test.Text);
            Int32.TryParse(txt_Amount.Text, out int amount);
            ValItem v = ValItems.FirstOrDefault(f => f.DisplayText == cbo_Test.Text);
            if (v == null)
            {
                x = new ToDo(false, amount, cbo_Test.Text, txt_Note.Text, ColorExtension.ToDrawingColor(colorPicker.SelectedColor.Value));
            }
            else
            {
                x = new ToDo(false, amount, v.DisplayText, txt_Note.Text, ColorExtension.ToDrawingColor(colorPicker.SelectedColor.Value));
                if (v.Recipe.Count > 0)
                    foreach (ValComponent vc in v.Recipe)
                    {
                        Debug.WriteLine(vc.Title);
                        ValComponent vx = new ValComponent(false, vc.Title, vc.Amount * amount);
                        x.Components.Add(vx);
                    }


            }
            _sharedData.toDos.Add(x);



            foreach (ToDo t in _sharedData.toDos)
                Debug.WriteLine(t.IsSelected + t.Title);

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
            if (overlayRunning)
            {
                example.Window.Hide();
                example.Window.Dispose();
            }
           
        }

        private void cp_SelectedColorChanged_1(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void Background_Color_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            _sharedData.bkColor = background_Color.SelectedColor;
        }

        private void Btn_Screens_Click(object sender, RoutedEventArgs e)
        {
            if (example == null) return;
            if (Screen.AllScreens.Length == 1) return;

            Debug.WriteLine(curScreen.DeviceName);

            if (curScreen == Screen.AllScreens[Screen.AllScreens.Length - 1])
            {
                curScreen = Screen.AllScreens[0];
            }
            else
            {
                bool skip = true;
                foreach (Screen s in Screen.AllScreens)
                {
                    if (!skip)
                    {
                        curScreen = s;
                        break;
                    }
                    if (curScreen == s)
                    {
                        skip = false;
                    }
                }
            }

            _sharedData.monitorHeight = curScreen.WorkingArea.Height;
            _sharedData.monitorOffsetX = curScreen.WorkingArea.X;
            _sharedData.monitorOffsetY = curScreen.WorkingArea.Y;

            example.RefreshWindowLocation();
            Debug.WriteLine(curScreen.DeviceName);


        }

        private void SaveSettings()
        {
            Int32.TryParse(txt_Amount.Text, out int a);
            ValheimToDo.Properties.Settings.Default.amount = a;
            ValheimToDo.Properties.Settings.Default.backgroundcolor = ColorExtension.ToDrawingColor(background_Color.SelectedColor.Value);
            
                ValheimToDo.Properties.Settings.Default.item = cbo_Test.Text;
            
            ValheimToDo.Properties.Settings.Default.note = txt_Note.Text;
            ValheimToDo.Properties.Settings.Default.screen = curScreen.DeviceName;

            ValheimToDo.Properties.Settings.Default.textcolor =  ColorExtension.ToDrawingColor(colorPicker.SelectedColor.Value);


            ValheimToDo.Properties.Settings.Default.SavedToDos = _sharedData.toDos;

            ValheimToDo.Properties.Settings.Default.Save();
        }

        private void LoadSettings()
        {

            string defAmount = ValheimToDo.Properties.Settings.Default.amount.ToString();
            string defNote = ValheimToDo.Properties.Settings.Default.note;
            string defTest = ValheimToDo.Properties.Settings.Default.item;
            Color defColor = ColorExtension.ToMediaColor(ValheimToDo.Properties.Settings.Default.textcolor);
            Color defBkColor = ColorExtension.ToMediaColor(ValheimToDo.Properties.Settings.Default.backgroundcolor);

            if (defAmount == null) defAmount = "1";
            if (defNote == null) defNote = "";
            if (defTest == null) defTest = "Club";
            if (defColor == null) defColor = Colors.LimeGreen;
            if (defBkColor == null) defBkColor = Color.FromArgb(130, 0x33, 0x36, 0x3F);

            txt_Amount.Text = defAmount;

            txt_Note.Text = defNote;
            
                cbo_Test.Text = defTest;
            

            if (Screen.AllScreens.FirstOrDefault(x => x.DeviceName == ValheimToDo.Properties.Settings.Default.screen) != null)
            curScreen = Screen.AllScreens.FirstOrDefault(x => x.DeviceName == ValheimToDo.Properties.Settings.Default.screen);

            _sharedData.monitorHeight = curScreen.WorkingArea.Height;
            _sharedData.monitorOffsetX = curScreen.WorkingArea.X;
            _sharedData.monitorOffsetY = curScreen.WorkingArea.Y;

            colorPicker.SelectedColor = defColor; 
            background_Color.SelectedColor = defBkColor;

            if (ValheimToDo.Properties.Settings.Default.SavedToDos != null)
                _sharedData.toDos = ValheimToDo.Properties.Settings.Default.SavedToDos;
        }

        private void LoadDefaults()
        {
            txt_Amount.Text = "1";
            txt_Note.Text = "";
            cbo_Test.Text = "Club";
            curScreen = Screen.PrimaryScreen;

            _sharedData.monitorHeight = curScreen.WorkingArea.Height;
            _sharedData.monitorOffsetX = curScreen.WorkingArea.X;
            _sharedData.monitorOffsetY = curScreen.WorkingArea.Y;

            background_Color.SelectedColor = Color.FromArgb(130, 0x33, 0x36, 0x3F);
            _sharedData.bkColor = background_Color.SelectedColor;
            colorPicker.SelectedColor =  Colors.LimeGreen;

            if(example!=null)
            example.RefreshWindowLocation();

            SaveSettings();
        }

        private void Men_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_Twitter1_Click(object sender, RoutedEventArgs e)
        {
            WebsiteOpen("https://twitter.com/sdoddler");
        }

        private void WebsiteOpen(string url)
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void Btn_Github1_Click(object sender, RoutedEventArgs e)
        {
            WebsiteOpen("https://github.com/sdoddler/ValheimToDo");
        }

        private void Btn_Dogecoin1_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText("DNxLSfMBRfwRKgJHktQ881i4AZK1Guztm8");
        }

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            _sharedData.toDos.Clear();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoadDefaults();
        }
    }
    
}

