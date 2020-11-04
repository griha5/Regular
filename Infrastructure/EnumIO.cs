using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Regular.Infrastructure
{
    /// <summary>
    /// Класс предназначен для возможности работы с перечислениями как набором чек-бокс контролов
    /// </summary>
    /// <typeparam name="T">Тип перечисления</typeparam>
    public static class EnumIO<T>
        where T : Enum
    {
        /// <summary>
        /// Представление данного перечисления в виде OR-выражения
        /// </summary>
        /// <param name="t">Исходное перечисление</param>
        /// <returns>Лист простейших перечислений составляющий через OR исходное перечисление</returns>
        public static List<T> ToOr(T t)
        {
            if((int)(object)t == 0)
                return new List<T>() { t };
            else
            {
                List<T> temp = new List<T>();
                foreach(T b in Enum.GetValues(typeof(T)).Cast<T>())
                {
                    if(((int)(object)b & (int)(object)t) != 0)
                        temp.Add(b);
                }
                return temp;
            }
        }

        /// <summary>
        /// Представление данного перечисления в виде OR-выражения в виде C#-строки
        /// </summary>
        /// <param name="t">Исходное перечисление</param>
        /// <returns>C#-строка простейших перечислений составляющий через OR исходное перечисление</returns>
        public static string ToCS(T t)
        { 
            StringBuilder sb = new StringBuilder();
            foreach(T b in ToOr(t))
            {
                sb.Append($"{typeof(T).Name}.{b} | ");
            }
            sb.Length -= 3;
            return sb.ToString();                                   
        }

        /// <summary>
        /// Создание чек-бокс контроллов для даннонго типа перечисления. Каждый чек-бокс будет привязян. 
        /// </summary>
        /// <param name="sourceViewModel">Объект привязки (вью-модель)</param>
        /// <param name="path">Путь привязки</param>
        public static List<CheckBox> CheckBoxes(object sourceViewModel, string path)
        {
            List<CheckBox> boxes = new List<CheckBox>();
            int i = 0;
            foreach(T ro in Enum.GetValues(typeof(T)))
            {
                CheckBox ch = new CheckBox() { Content = ro.ToString(), Margin = new Thickness(5) };
                Binding b = new Binding();
                b.Source = sourceViewModel;
                b.Path = new PropertyPath($"{path}[{i++}]");
                b.Mode = BindingMode.TwoWay;
                ch.SetBinding(CheckBox.IsCheckedProperty, b);
                boxes.Add(ch);
            }
            return boxes;
        }

        /// <summary>
        /// Класс отслеживаемой коллекция из элементарных состояний перечисления 
        /// Данная коллекция привязывается к чек-боксам и обеспечивает логику переключения состояний перечисления
        /// </summary>
        public class ObservableCollection : ObservableCollection<bool>
        {
            public ObservableCollection(T t)
                : base(Enum.GetValues(typeof(T))
                      .OfType<T>().Cast<int>()
                      .Select(b => (int)(object)t == 0 ? b == 0 : (b & (int)(object)t) != 0))
            {

                enumValues = Enum.GetValues(typeof(T)).OfType<T>().Cast<int>().ToArray();
                nullKey = Array.IndexOf(enumValues, 0);
            }

            int[] enumValues;
            int nullKey;

            public T Value
            {
                get
                {
                    int c = 0;
                    for(int i = 0; i < enumValues.Length; i++)
                        if(this[i])
                            c |= enumValues[i];
                    return (T)(object)c;
                }
            }

            protected override void SetItem(int index, bool item)
            {
                if(index == nullKey && item)
                {
                    base.SetItem(index, item);
                    for(int i = 0; i < this.Count; i++)
                        if(i != nullKey)
                            this[i] = false;
                    return;
                }
                else if(index != nullKey && this[nullKey] && item)
                {
                    base.SetItem(index, item);
                    this[nullKey] = false;
                    return;
                }
                else if(index != nullKey && !item && (int)(object)this.Value == enumValues[index])

                {
                    base.SetItem(index, item);
                    this[nullKey] = true;
                    return;
                }
                else if(index == nullKey && !item && (int)(object)this.Value == 0)
                {
                    return;
                }

                base.SetItem(index, item);
            }
        }
}
}
