using Android.Content;
using Android.Gms.Maps.Model;
using Android.Views;
using Android.Widget;
using static Android.Gms.Maps.GoogleMap;

namespace Proba
{
    class MyInfoWindowAdapter : Java.Lang.Object, IInfoWindowAdapter
    {
        Context ApplicationContext;
        string LayoutInflaterService;
        public MyInfoWindowAdapter(MainActivity context, string service)
        {
            ApplicationContext = context;
            LayoutInflaterService = service;
        }

        /// <summary>
        /// Реализация интерфейса IInfoWindowAdapter:позволяет настроить содержимое окна, но при этом сохранить рамку и фон окна по умолчанию.
        /// Вызывается если getInfoWindow вернет null.
        /// </summary>
        /// <param name="marker"></param>
        /// <returns></returns>
        public View GetInfoContents(Marker marker)
        {
            return null;
        }

        /// <summary>
        /// Позволяет предоставить представление, которое будет использоваться для всего информационного окна.
        /// </summary>
        /// <param name="marker"></param>
        /// <returns></returns>
        public View GetInfoWindow(Marker marker)
        {
            // Получаем Title маркера.
            string str = marker.Title;
            // Разбиваем его на подстроки, получаем все нужные нам данные.
            string[] info = str.Split('&');
            string adress = info[1];
            string area = info[2];
            string phone = info[3];
            string website = info[4];
            // ContextWrapper, который позволяет изменять тему из того, что находится в завернутый контекст.
            ContextThemeWrapper wrapper = new ContextThemeWrapper(ApplicationContext, Resource.Style.AppTheme);
            // Преобразование содержимого layout-файла во View-элемент с помощью inflate.
            LayoutInflater inflater = (LayoutInflater)wrapper.GetSystemService(LayoutInflaterService);
            LinearLayout layout = inflater.Inflate(Resource.Layout.InfoWindow, null) as LinearLayout;
            TextView textAdress = layout.FindViewById(Resource.Id.textViewAdress) as TextView;
            TextView textPhone = layout.FindViewById(Resource.Id.textViewPhone) as TextView;
            TextView textBrowser = layout.FindViewById(Resource.Id.textViewBrowser) as TextView;
            TextView textHomeArea = layout.FindViewById(Resource.Id.textViewHomeArea) as TextView;
            Button buttonRegistr = layout.FindViewById(Resource.Id.buttonRegistration) as Button;
            buttonRegistr.Text = ApplicationContext.GetString(Resource.String.button_registration);
            // Установка стиля для информационного окна
            layout.SetBackgroundResource(Resource.Drawable.styleInfoWindow);
            textAdress.Text = adress;
            textPhone.Text = phone;
            textBrowser.Text = website;
            textHomeArea.Text = area;
            return layout;

        }

    }
}