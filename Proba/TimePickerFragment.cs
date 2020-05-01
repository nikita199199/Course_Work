using System;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Widget;

namespace Proba
{
    public class TimePickerFragment : DialogFragment, TimePickerDialog.IOnTimeSetListener
    {
        public static readonly string TAG = "MyTimePickerFragment";
        // timeSelectedHandler действие инициализируется пустым делегатом для предотвращения исключений со ссылкой null.
        Action<DateTime> timeSelectedHandler = delegate { };

        /// <summary>
        /// Метод NewInstance фабрики вызывается для создания нового TimePickerFragment.
        /// Этот метод принимает обработчик Action<DateTime>, который вызывается, когда пользователь нажимает кнопку ОК в TimePickerDialog.
        /// </summary>
        /// <param name="onTimeSelected"></param>
        /// <returns></returns>
        public static TimePickerFragment NewInstance(Action<DateTime> onTimeSelected)
        {
            TimePickerFragment frag = new TimePickerFragment
            {
                timeSelectedHandler = onTimeSelected
            };
            return frag;
        }

        /// <summary>
        /// Когда фрагмент будет отображаться, Android вызывает метод DialogFragment OnCreateDialog.
        /// Этот метод создает новый объект TimePickerDialog и инициализирует его с помощью действия,
        /// объекта обратного вызова (текущего экземпляра TimePickerFragment) и текущего времени.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        /// <returns></returns>
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currentTime = DateTime.Now;
            bool is24HourFormat = true;
            TimePickerDialog dialog = new TimePickerDialog
                (Activity, this, currentTime.Hour, currentTime.Minute, is24HourFormat);
            return dialog;
        }

        /// <summary>
        /// Когда пользователь изменяет параметр времени в диалоговом окне TimePicker, вызывается метод OnTimeSet.
        /// OnTimeSet создает объект DateTime, используя текущую дату и слияния за время (час и минуту), выбранное пользователем.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="hourOfDay"></param>
        /// <param name="minute"></param>
        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            DateTime currentTime = DateTime.Now;
            DateTime selectedTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hourOfDay, minute, 0);
            Log.Debug(TAG, selectedTime.ToLongTimeString());
            // Этот DateTime объект передается в timeSelectedHandler, зарегистрированный в объекте TimePickerFragment во время создания.
            // OnTimeSet вызывает этот обработчик, чтобы обновить отображение времени действия до выбранного времени.
            timeSelectedHandler(selectedTime);
        }
    }
}