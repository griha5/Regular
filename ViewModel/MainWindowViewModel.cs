using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Regular.Infrastructure;
using Regular.Model;
using Microsoft.Win32;
using System.IO;

namespace Regular.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Отображение строки регулярного выражения подготовленного для испольсования в коде C#
        /// </summary>
        public List<Run> CodeString
        {
            get
            {
                Run run(string text, Brush foreground)
                {
                    return new Run(text)
                    {
                        Style = (Style)System.Windows.Application.Current.Resources["BasicStyle"],
                        Foreground = foreground
                    };
                }

                List<Run> t = new List<Run>();
                t.Add(run("new ", Brushes.Blue));
                t.Add(run("Regex(", Brushes.Black));
                t.Add(run($"@\"{ RegularString.Replace("\"", "\"\"")}\"", Brushes.Red));
                t.Add(run(", ", Brushes.Black));
                List<RegexOptions> ro = EnumIO<RegexOptions>.ToOr(this.RegexOptionsValue);
                for(int i = 0; i < ro.Count; i++)
                {

                    t.Add(run(typeof(RegexOptions).Name, new SolidColorBrush(Color.FromRgb(43, 145, 175))));
                    t.Add(run("." + ro[i].ToString(), Brushes.Black));
                    if(i != ro.Count - 1)
                        t.Add(run(" | ", Brushes.Black));
                }
                t.Add(run(")", Brushes.Black));
                return t;
            }
        }

        private List<CheckBox> _regexOptionsCheckBoxes;
        /// <summary>
        /// Чек-боксы для настроек опций вычисления к данному регулярному выражению
        /// </summary>
        public List<CheckBox> RegexOptionsCheckBoxes
        {
            get
            {
                return _regexOptionsCheckBoxes ?? (_regexOptionsCheckBoxes = EnumIO<RegexOptions>.CheckBoxes(this, nameof(this.RegexOptions)));
            }
        }

        /// <summary>
        /// Регулярное выражение
        /// </summary>
        public string RegularString
        {
            get => RegularStore.Store.RegularString;
            set
            {
                RegularStore.Store.RegularString = value;
                OnPropertyChanged(nameof(RegularString));
                OnPropertyChanged(nameof(CodeString));
                OnPropertyChanged(nameof(MatchString));
            }
        }
        /// <summary>
        /// Входной текст
        /// </summary>
        public string SourceString
        {
            get => RegularStore.Store.SourceString;
            set
            {
                RegularStore.Store.SourceString = value;
                OnPropertyChanged(nameof(SourceString));
                OnPropertyChanged(nameof(MatchString));
            }
        }

        /// <summary>
        /// Результат применения регулярного выражения к входному тексту при данных опциях
        /// </summary>
        public string MatchString { get => RegularStore.Store.GetMatchString(); }

        EnumIO<RegexOptions>.ObservableCollection regexOptions;
        RegexOptions RegexOptionsValue = System.Text.RegularExpressions.RegexOptions.None;
        /// <summary>
        /// Опции к регулярному выражению
        /// </summary>
        public EnumIO<RegexOptions>.ObservableCollection RegexOptions
        {
            get
            {
                if(regexOptions == null)
                    regexOptions = new EnumIO<RegexOptions>.ObservableCollection(RegularStore.Store.RegexOptions);
                RegularStore.Store.RegexOptions = RegexOptionsValue = regexOptions.Value;
                OnPropertyChanged(nameof(MatchString));
                OnPropertyChanged(nameof(CodeString));
                return regexOptions;
            }
        }

        ICommand copy;
        /// <summary>
        /// Команда копирования в буфер обмена строки регулярного выражения, подготовленного для использования в коде C#
        /// </summary>
        public ICommand Copy
        {
            get => copy ?? (copy = new RelayCommand(() => { Clipboard.SetText(RegularStore.Store.GetCodeString()); }, () => true));
        }

        ICommand load;
        /// <summary>
        /// Команда загрузки входного текста из файла
        /// </summary>
        public ICommand Load
        {
            get => load ?? (load = new RelayCommand(
                () => 
                {
                    OpenFileDialog openDialog = new OpenFileDialog();
                    if(openDialog.ShowDialog().Value)
                    {
                        string s = "";
                        try
                        {
                            s = File.ReadAllText(openDialog.FileName);
                        }
                        catch(Exception ex)
                        {
                            s = ex.Message;
                        }

                        this.SourceString = s;
                    }
                }, 
                () => true));
        }

        ICommand save;
        /// <summary>
        /// Команда сохранения входного текста в файл
        /// </summary>
        public ICommand Save
        {
            get => save ?? (save = new RelayCommand(
                () =>
                {
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
                    if(saveDialog.ShowDialog().Value)
                    {
                        try
                        {
                           File.WriteAllText(saveDialog.FileName, this.SourceString);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Проблемы при записи в файл!");
                        }
                    }
                },
                () => this.SourceString != null));
        }

    }
}
