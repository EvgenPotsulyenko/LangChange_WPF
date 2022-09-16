using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Wpf_multiLang
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Какие языки доступны
        private static List<CultureInfo> _Languages = new List<CultureInfo>();

        public static List<CultureInfo> Languages 
        {
            get
            {
                return _Languages;
            }
        }

        public App()
        {
            InitializeComponent();
            App.LanguageChanged += App_LanguageChanged;

            _Languages.Clear();
            _Languages.Add(new CultureInfo("en-US")); //Нейтральная культура для этого проекта
            _Languages.Add(new CultureInfo("uk-UA"));

            Language = Wpf_multiLang.Properties.Settings.Default.DefaultLanguage;
        }


        //Евент для оповещения всех окон приложения
        public static event EventHandler LanguageChanged;

        public static CultureInfo Language
        {
            get
            {
                return System.Threading.Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (value == System.Threading.Thread.CurrentThread.CurrentUICulture) return;

                //1. Меняем язык приложения:
                System.Threading.Thread.CurrentThread.CurrentUICulture = value;

                //2. Создаём ResourceDictionary для новой культуры
                ResourceDictionary dict = new ResourceDictionary();
                switch (value.Name)
                {
                    case "uk-UA":
                        dict.Source = new Uri(String.Format("Resources/Lang/lang.{0}.xaml", value.Name), UriKind.Relative);
                        break;
                    default:
                        dict.Source = new Uri("Resources/Lang/lang.xaml", UriKind.Relative);
                        break;
                }

                //3. Находим старую ResourceDictionary и удаляем его и добавляем новую ResourceDictionary
                ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                              where d.Source != null && d.Source.OriginalString.StartsWith("Resources/Lang/lang.")
                                              select d).First();
                if (oldDict != null)
                {
                    int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                    Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }

                //4. Вызываем евент для оповещения всех окон.
                LanguageChanged(Application.Current, new EventArgs());
            }
        }

        private void App_LanguageChanged(Object sender, EventArgs e)
        {
            Wpf_multiLang.Properties.Settings.Default.DefaultLanguage = Language;
            Wpf_multiLang.Properties.Settings.Default.Save();
        }



    }
}
