using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Proba
{
    class InfoObject
    {
        static List<string> listAdress = new List<string>();
        // Автореализуемое свойство для адреса места. 
        public string adress { get; }
        // Автореализуемое свойство для адреса сайта. 
        public string website { get; }
        // Автореализуемое свойство для номера телефона. 
        public string helpPhone { get; }
        // Автореализуемое свойство для площади и длины места. 
        public string dimensions { get; }
        // Автореализуемое свойство для координат места. 
        public LatLng coordinates { get; }

        public InfoObject(string adress, string website, string helpPhone, string dimensions, LatLng coordinates)
        {
            this.adress = adress;
            this.website = website;
            this.helpPhone = helpPhone;
            this.dimensions = dimensions;
            this.coordinates = coordinates;
            listAdress.Add(adress);
        }
        /// <summary>
        /// По адресу получаем индекс объекта в списке
        /// </summary>
        /// <param name="adress"></param>
        /// <returns></returns>
        public static int GetPosition(string adress)
        {
            return listAdress.IndexOf(adress);
        }

        /// <summary>
        /// Получить лист спортплощадок
        /// </summary>
        /// <returns></returns>
        public static List<string> GetListAdress()
        {
            return listAdress;
        }
    }
}