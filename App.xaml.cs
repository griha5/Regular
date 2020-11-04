using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Regular.Infrastructure;

namespace Regular
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //Отображение тултипов, если приложение загруженно впервые
            if(!ConfigurationData.Data.SecondStart())
                Application.Current.Resources.Remove(typeof(System.Windows.Controls.ToolTip));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //Сохранение настроек по выходу из приложения
            ConfigurationData.Data.SaveData();
        }
    }
}
