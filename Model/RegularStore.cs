using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Regular.Infrastructure;

namespace Regular.Model
{
    /// <summary>
    /// Класс-модель для хранения и рабоы с регулярными выражениями
    /// </summary>
    public class RegularStore
    {
        public static RegularStore Store = new RegularStore();
        private readonly ConfigurationData Data;

        private RegularStore()
        {
            this.Data = ConfigurationData.Data;
        }

        /// <summary>
        /// Регулярное выражение
        /// </summary>
        public string RegularString
        {
            get => Data.GetData(nameof(RegularString));
            set { Data.SetData(nameof(RegularString), value); }
        }

        /// <summary>
        /// Входной текст, к которому применяется регулярное выражение
        /// </summary>
        public string SourceString
        {
            get => Data.GetData(nameof(SourceString));
            set { Data.SetData(nameof(SourceString), value); }
        }

        /// <summary>
        /// Опции применяемые для регулярного выражения
        /// </summary>
        public RegexOptions RegexOptions
        {
            get
            {
                try { return (RegexOptions)Enum.Parse(typeof(RegexOptions), Data.GetData(nameof(RegexOptions), RegexOptions.None.ToString())); }
                catch { return RegexOptions.None; }
            }
            set { Data.SetData(nameof(RegexOptions), value.ToString()); }
        }

        /// <summary>
        /// Строка регулярного выражения подготовленная для испольсования в коде C# 
        /// </summary>
        /// <returns></returns>
        public string GetCodeString()
        {
            return string.Format($"new Regex(@\"{this.RegularString.Replace("\"","\"\"")}\", {EnumIO<RegexOptions>.ToCS(this.RegexOptions)})");
        }

        /// <summary>
        /// Результат применения регулярного выражения к входному тексту при данных опциях
        /// </summary>
        /// <returns></returns>
        public string GetMatchString()
        {

            Regex r = new Regex(RegularString, RegexOptions);
            Match m = r.Match(SourceString);
            StringBuilder sb = new StringBuilder();
            int i = 0;
            while(m.Success && i<=100)
            {
                
                int c = 0;
                string ms = $"----------- Match {i++} ";
                foreach(Group g in m.Groups)
                {
                    if(c == 0)
                        sb.Append(ms);
                    else
                        sb.Append(new string('-', ms.Length));
                    sb.AppendLine($"--- Group[\"{g.Name}\"] Group[{c++}]");
                    sb.AppendLine(g.Value);
                }
                m = m.NextMatch();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Сохранение настроек в файле кофигурации приложения
        /// </summary>
        public void SaveSettings()
        {
            this.Data.SaveData();
        }
    }
}
