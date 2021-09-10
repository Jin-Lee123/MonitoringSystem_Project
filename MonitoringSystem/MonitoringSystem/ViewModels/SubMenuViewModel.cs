﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MonitoringSystem.Views;
using MonitoringSystem.ViewModels;
using Caliburn.Micro;
using System.Reflection;

namespace MonitoringSystem.ViewModels
{
    public class SubMenuViewModel : Conductor<object>
    {
        //to call resource dictionary in our code behind
        ResourceDictionary dict = Application.LoadComponent(new Uri("/MonitoringSystem;component/Hong/img/IconDictonary.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;


        // Source 추가
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

                    new MenuItemsData(){ PathData= (PathGeometry)dict["icon_map"], MenuText="Log"},

                    new MenuItemsData(){ PathData= (PathGeometry)dict["icon_setting"], MenuText="SettingView"}

                };
            }
        }
    }

    public class MenuItemsData : Conductor<object>
    {
        //Icon Data
        public PathGeometry PathData { get; set; }
        public string MenuText { get; set; }

        // 클릭 이벤트 구현
        public MenuItemsData()
        {
            Command = new CommandViewModel(Execute);
        }

        public ICommand Command { get; }

        public void Execute()
        {
            //our logic comes here
            string MT = MenuText.Replace(" ", string.Empty);
            if (!string.IsNullOrEmpty(MT))
                navigateToPage(MT);
        }

        public void navigateToPage(string Menu)
        {
            // Frame 안에 들어갈 Page를 찾기
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainView))
                {
                    Assembly creator = Assembly.GetExecutingAssembly();
                    object obj = creator.CreateInstance("MonitoringSystem.ViewModels." + Menu + "Model");
                    ActivateItemAsync(obj);
/*                    (window as MainView).ActiveItem.Navigate(new Uri(string.Format("{0}{1}{2}", "Views/", Menu, ".xaml"), UriKind.RelativeOrAbsolute));
*/
                    //ActivateItemAsync(
                    //ActivateItemAsync( .MainWindowFrame.Navigate(new Uri(string.Format("{0}{1}{2}", "Views/", Menu, ".xaml"), UriKind.RelativeOrAbsolute));
                }
            }
        }
    }
}

