using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MonitoringSystem.ViewModels
{
    class SubMenuViewModel
    {
        //to call resource dictionary in our code behind
        ResourceDictionary dict = Application.LoadComponent(new Uri("/MonitoringSystem;component/Hong/img/IconDictonary.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;


        //Our Source List for Menu Items
        public List<MenuItemsData> MenuList
        {
            get
            {
                return new List<MenuItemsData>
                {
                    //MainMenu without SubMenu Button 
                    new MenuItemsData(){ PathData= (PathGeometry)dict["icon_home"], MenuText="Mornitoring"},

                    new MenuItemsData(){ PathData= (PathGeometry)dict["icon_contact"], MenuText="Tank"},

                    new MenuItemsData(){ PathData= (PathGeometry)dict["icon_message"], MenuText="Containers"},

                    new MenuItemsData(){ PathData= (PathGeometry)dict["icon_map"], MenuText="Logs"},

                    new MenuItemsData(){ PathData= (PathGeometry)dict["icon_setting"], MenuText="Settings"}

                };
            }
        }
    }

    public class MenuItemsData
    {
        //Icon Data
        public PathGeometry PathData { get; set; }
        public string MenuText { get; set; }

        //To Add click event to our Buttons we will use ICommand here to switch pages when specific button is clicked
        public MenuItemsData()
        {
            Command = new CommandViewModel(Execute);
        }

        public ICommand Command { get; }

        private void Execute()
        {
            //our logic comes here
            string MT = MenuText.Replace(" ", string.Empty);
            if (!string.IsNullOrEmpty(MT))
                navigateToPage(MT);
        }

        private void navigateToPage(string Menu)
        {
            //We will search for our Main Window in open windows and then will access the frame inside it to set the navigation to desired page..
            //lets see how... ;)
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    (window as MainWindow).MainWindowFrame.Navigate(new Uri(string.Format("{0}{1}{2}", "Pages/", Menu, ".xaml"), UriKind.RelativeOrAbsolute));
                }
            }
        }
    }
}

