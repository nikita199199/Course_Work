using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System.Collections.Generic;
using Android.Util;
using System.IO;
using Java.IO;
using System.Linq;
using Android.Support.V4.Content;
using Android.Content.PM;
using Android.Support.V4.App;
using Android;
using Android.Locations;
using Android.Views.InputMethods;
using System;
using Android.Support.Design.Widget;
using Android.Graphics.Drawables;
using Android.Graphics;
namespace Proba
{
    // Задаём название, тему и иконку приложения.
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", Icon = "@drawable/icon_APP", ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {
        // Лист InfoObject, то есть обьекты всех спортплощадок.
        List<InfoObject> parsePlaces = new List<InfoObject>();
        // Словарь, связывающий id маркера со списком людей.
        static Dictionary<string, List<string>> markerNames = new Dictionary<string, List<string>>();
        // Задаём id для разрешения к геолокации пользователя.
        int FIND_LOCATION_PERMISSION_CODE = 100;
        // Координаты местоположения пользователя.
        LatLng myPosition;
        // Время регистрации пользователя.
        string timeRegistration = null;
        // Задаём некоторую область для ограничения карты при использовании.
        LatLngBounds Moscow = new LatLngBounds(new LatLng(55.55, 37.2), new LatLng(55.9, 38));
        // Задаём область для наведения на неё при открытии приложения
        LatLngBounds zoomPlace = new LatLngBounds(new LatLng(55.73, 37.58), new LatLng(55.735, 37.585));
        // Объект Google карты.
        GoogleMap myMap;
        /// <summary>
        /// Первый метод, с которого начинается выполнение MainActivity.
        /// Производится первоначальная настройка activity, создаются объекты визуального интерфейса,
        /// этот метод получает объект Bundle, который содержит прежнее состояние activity, если оно было сохранено,
        /// Если activity заново создается, то данный объект имеет значение null. Если же activity уже ранее была создана,
        /// но находилась в приостановленном состоянии, то bundle содержит связанную с activity информацию.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Xamarin.Essentials устанавливает версию 28.0.0.3 всех требуемых библиотек Xamarin.Android.Support.
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            SetTabHost();
            // Загрузка карты.
            var map = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            map.GetMapAsync(this);
            SetThemes();
            ReadCsv();
            CheckPermission(Manifest.Permission.AccessFineLocation, FIND_LOCATION_PERMISSION_CODE);
            GetListSearch();
        }

        /// <summary>
        /// Метод запускается, когда карта готова к использованию и предоставляет экземпляр, не являющийся нулевым GoogleMap.
        /// </summary>
        /// <param name="map"></param>
        public void OnMapReady(GoogleMap map)
        {
            myMap = map;
            // Подключение индикатора текущего местоположения (синий круг).
            map.UiSettings.MyLocationButtonEnabled = true;
            SetMyLocation(map);
            // Возможность приближения.
            map.UiSettings.ZoomControlsEnabled = true;
            // Включение компаса.
            map.UiSettings.CompassEnabled = true;
            // Приближение на определенное место.
            map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(zoomPlace.Center, 15));
            // Ограничение для камеры до границ Москвы и ближайших окраин.
            map.SetLatLngBoundsForCameraTarget(Moscow);
            SetMarkers(map);
            // Переопределение информационного окна для маркеров.
            map.SetInfoWindowAdapter(new MyInfoWindowAdapter(this, LayoutInflaterService));
            // Привязка метода к событию клика на информационное окно.
            map.InfoWindowClick += MapOnInfoWindowClick;
        }

        /// <summary>
        /// Обработчик события нажатия на информационное окно.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapOnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            // Инициализация маркера, на который нажал пользователь.
            Marker myMarker = e.Marker;
            // Лист с именами людей, зарегистрировавшихся на определенную спортплощадку.
            List<string> names = markerNames[myMarker.Id];
            // ContextWrapper, который позволяет изменять тему из того, что находится в завернутый контекст.
            ContextThemeWrapper wrapper = new ContextThemeWrapper(ApplicationContext, Resource.Style.AppTheme);
            // Преобразование содержимого layout-файла во View-элемент с помощью inflate.
            LayoutInflater inflater = (LayoutInflater)wrapper.GetSystemService(LayoutInflaterService);
            LinearLayout layout = inflater.Inflate(Resource.Layout.Registration, null) as LinearLayout;
            Android.App.AlertDialog.Builder dialogBuilder = new Android.App.AlertDialog.Builder(this);
            // Инициализируем объект View созданным xml файлом для регистрации.
            View dialogView = inflater.Inflate(Resource.Layout.Registration, null);
            // Устанавливаем в диалог созданный объект View.
            dialogBuilder.SetView(dialogView);
            ListView listView = dialogView.FindViewById<ListView>(Resource.Id.listView1);
            TextInputEditText userInput = dialogView.FindViewById<TextInputEditText>(Resource.Id.textInputEditTextRegistration1);
            userInput.Hint = GetString(Resource.String.user_input);
            TextView timeDisplay = dialogView.FindViewById<TextView>(Resource.Id.textViewRegistration);
            timeDisplay.Text = GetString(Resource.String.time_input);
            ImageButton RegistrationButton = dialogView.FindViewById<ImageButton>(Resource.Id.registrationBut);
            RegistrationButton.SetImageBitmap(BitmapFromVector(ApplicationContext, Resource.Drawable.time, 0.08));
            RegistrationButton.SetBackgroundColor(Color.White);
            // Привязка метода к событию клика.
            RegistrationButton.Click += TimeSelectOnClick;
            TextView textArea = dialogView.FindViewById(Resource.Id.textView111) as TextView;
            textArea.Text = GetString(Resource.String.fullness_area);
            SetProgressBar(names, dialogView);
            // ArrayAdapter представляет собой простейший адаптер, который связывает список данных с набором элементов TextView.
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Resource.Layout.simple_dropdown, names);
            listView.Adapter = adapter;
            SetAnswerUserButtons(names, dialogBuilder, userInput, adapter);
            // Создание диалога с пользователем.
            Android.App.AlertDialog alertDialog = dialogBuilder.Create();
            // Показ диалога.
            alertDialog.Show();
        }

        /// <summary>
        /// Установка меню в виде TabHost
        /// </summary>
        private void SetTabHost()
        {
            // Инициализация меню.
            var tabs = FindViewById(Resource.Id.tabHost) as TabHost;
            // Инициализация контейнера вкладок.
            tabs.Setup();
            TabHost.TabSpec spec;
            // Создание новой вкладке, присвоение ей тега и названия.
            spec = tabs.NewTabSpec("1").SetIndicator("Карта");
            // Размещение во вкладке контента с элементами управления
            spec.SetContent(Resource.Id.linearLayout);
            // Добавление вкладки в меню
            tabs.AddTab(spec);
            spec = tabs.NewTabSpec("2").SetIndicator("Поиск");
            spec.SetContent(Resource.Id.linearLayout3);
            tabs.AddTab(spec);
            spec = tabs.NewTabSpec("3").SetIndicator("Настройки", GetDrawable(Resource.Drawable.phone));
            spec.SetContent(Resource.Id.linearLayout2);
            tabs.AddTab(spec);
            // Назначение ведущей вкладки
            tabs.SetCurrentTabByTag("1");
        }

        /// <summary>
        /// Возможность установки нескольких тем:дневного и ночного виденья.
        /// </summary>
        private void SetThemes()
        {
            // Инициализация кнопок.
            RadioButton light = FindViewById<RadioButton>(Resource.Id.StyleLight);
            light.Text = "Дневная тема";
            RadioButton dark = FindViewById<RadioButton>(Resource.Id.StyleDark);
            dark.Text = "Ночная тема";
            // Подвязка событий на клик.
            light.Click += delegate { ChangeThemes(Resource.Raw.StyleLight); };
            dark.Click += delegate { ChangeThemes(Resource.Raw.StyleDark); };

        }

        /// <summary>
        /// Установление соединения с возможностью замены тем
        /// </summary>
        /// <param name="id"></param>
        private void ChangeThemes(int id)
        {
            try
            {
                bool success = myMap.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(this, id));
                if (!success)
                {
                    Log.Error("Error", "Style parsing failed");
                }
            }
            catch (System.Exception ex)
            {
                Log.Error("Error", ex.Message);
            }
        }

        /// <summary>
        /// Добавление инициализированных объектов спортплощадок в список.
        /// </summary>
        private void ReadCsv()
        {
            // Открытие потока данных для чтения базы данных спортивных площадок.
            Stream stream = Resources.OpenRawResource(Resource.Raw.data_base);
            // BufferedReader при считывании данных использует специальную область — буфер, куда “складывает” прочитанные символы. 
            // Когда эти символы понадобятся нам в программе — они будут взяты из буфера, а не напрямую из источника данных, а это экономит очень много ресурсов. 
            // InputStreamReader не только получает данные из потока. Он еще и преобразует байтовые потоки в символьные.
            BufferedReader reader = new BufferedReader(new InputStreamReader(stream));
            string svcLine;
            reader.ReadLine();
            // Считываем отдельно каждую строку и делаем так, пока строки есть.
            while ((svcLine = reader.ReadLine()) != null)
            {
                try
                {
                    // Разобьём полученную строку на подстроки по ; .
                    string[] info = svcLine.Split(';');
                    // Первая подстрока это адрес.
                    string adress = info[0];
                    // Вторая подстрока это сайт.
                    string website = info[1];
                    // Третья подстрока это телефон.
                    string helpPhone = info[2];
                    // Четвертая подстрока это площадь.
                    string dimensions = info[3];
                    // Пятая подстрока это строка с координатами.
                    string coord = info[4];
                    // Алфавит для нахождения координат из строки.
                    char[] alphavite = { ' ', ']', '}' };
                    int index = 0;
                    for (int i = 0; i < coord.Length; i++)
                    {
                        // Нахождения индекса первого вхождения цифры.
                        if (int.TryParse(coord[i].ToString(), out index))
                        {
                            index = i;
                            break;
                        }
                    }
                    // Удаляем из изначальной строки все до первой цифры.
                    string str = coord.Remove(0, index);
                    // Из полученной строки удаляем все лишние символы,разделяем две координаты, заменяем точку на запятую и парсим в дабл с обрезкой до 6 символов.
                    double[] twoCoordinates = str.Trim(alphavite).Split(',').Select(x => x.Replace('.', ',')).Select(x => double.Parse(x)).Select(x => x.ToString("F6")).Select(x => double.Parse(x)).ToArray();
                    LatLng coordinates = new LatLng(twoCoordinates[1], twoCoordinates[0]);
                    // Добавляем создаваемый объект в список всех спортплощадок.
                    parsePlaces.Add(new InfoObject(adress, website, helpPhone, dimensions, coordinates));
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Проверка на разрешение приложению к геолокации устройства.
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="requestCode"></param>
        public void CheckPermission(string permission, int requestCode)
        {
            if (ContextCompat.CheckSelfPermission(this, permission) == Permission.Denied)
                ActivityCompat.RequestPermissions(this, new string[] { permission }, requestCode);
        }

        /// <summary>
        /// Проверка, что запрос разрешения не был прерван
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="permissions"></param>
        /// <param name="grantResults"></param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (requestCode == FIND_LOCATION_PERMISSION_CODE)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    Toast.MakeText(this, "Разрешение на геолокацию предоставлено", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "Разрешение на геолокацию не предоставлено", ToastLength.Short).Show();
                }
            }
        }

        /// <summary>
        /// Задание возможности поиска спортплощадки по адресу.
        /// </summary>
        private void GetListSearch()
        {
            // Инициализация AutoCompleteTextView.
            var search = FindViewById(Resource.Id.autoCompleteTextView1) as AutoCompleteTextView;
            // ArrayAdapter представляет собой простейший адаптер, который связывает список данных с набором элементов TextView.
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Resource.Layout.simple_dropdown, InfoObject.GetListAdress());
            // Связываем AutoCompleteTextView с адаптером, чтобы при вводе строки отображались только те адреса, где есть данная подстрока. 
            search.Adapter = adapter;
            var buttonSearch = FindViewById(Resource.Id.buttonSearch) as ImageButton;
            // Установка изображения для кнопки поиска.
            buttonSearch.SetImageBitmap(BitmapFromVector(ApplicationContext, Resource.Drawable.search_map, 0.08));
            // Задний фон кнопки делаем в цвет темы приложения.
            buttonSearch.SetBackgroundColor(Color.Rgb(250, 250, 250));
            // Привязываем события по нажатию на кнопку поиска.
            buttonSearch.Click += delegate { ChangeCameraPosition(search.Text); };
        }

        /// <summary>
        /// Перемещение фокуса на страницу с картой и наведение на соответствующий адресу объект.
        /// </summary>
        /// <param name="adress"></param>
        private void ChangeCameraPosition(string adress)
        {
            // Инициализация текста, вводимого в строку поиска.
            var autoCompleteTextView = FindViewById(Resource.Id.autoCompleteTextView1) as AutoCompleteTextView;
            try
            {
                // Инициализируем место по адресу.
                InfoObject place = parsePlaces[InfoObject.GetPosition(adress)];
                var tabs = FindViewById(Resource.Id.tabHost) as TabHost;
                // Перемещаем фокус на первую вкладку.
                tabs.SetCurrentTabByTag("1");
                // Приближение на выбранный по адресу объект.
                myMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(place.coordinates, 18));
                var buttonSearch = FindViewById(Resource.Id.buttonSearch) as ImageButton;
                autoCompleteTextView.Text = null;
                // Скрытие системной клавиатуры.
                InputMethodManager keyboard = (InputMethodManager)GetSystemService(InputMethodService);
                keyboard.HideSoftInputFromWindow(buttonSearch.WindowToken, HideSoftInputFlags.NotAlways);
                // Удаления фокуса с набора текста в поиске.
                autoCompleteTextView.ClearFocus();
            }
            catch (Exception)
            {
                autoCompleteTextView.Text = null;
                Toast.MakeText(this, "Такого адреса не существует", ToastLength.Short).Show();
            }
        }

        /// <summary>
        /// Установка пользовательской геолокации.
        /// </summary>
        /// <param name="map"></param>
        private void SetMyLocation(GoogleMap map)
        {
            // Проверка на наличие прав использование геоданных устройства.
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                // Отображение метки местоположения на карте.
                map.MyLocationEnabled = true;
                // Можно получить доступ к системной службе расположения с помощью экземпляра класса LocationManager.
                // LocationManager — это специальный класс, который позволяет взаимодействовать с системной службой расположения и вызывать методы для нее.
                // Приложение может получить ссылку на LocationManager, вызвав GetSystemService и передав тип службы, как показано ниже.
                LocationManager locationManager = (LocationManager)GetSystemService(LocationService);
                Criteria criteria = new Criteria();
                // Возвращает имя поставщика, которое наилучшим образом соответствует заданным критериям.
                string provider = locationManager.GetBestProvider(criteria, true);
                Location location = null;
                if (provider != null)
                    // Возвращает последнее известное исправление местоположения, полученное от данного поставщика.
                    location = locationManager.GetLastKnownLocation(provider);
                if (location != null)
                {
                    double latitude = location.Latitude;
                    double longitude = location.Longitude;
                    myPosition = new LatLng(latitude, longitude);
                }
            }
        }

        /// <summary>
        /// Установка маркеров на Google карту.
        /// </summary>
        /// <param name="map"></param>
        private void SetMarkers(GoogleMap map)
        {
            MarkerOptions[] markers = new MarkerOptions[parsePlaces.Count];
            for (int i = 0; i < parsePlaces.Count; i++)
            {
                markers[i] = new MarkerOptions().SetPosition(parsePlaces[i].coordinates).SetIcon(BitmapDescriptorFactory.FromBitmap(BitmapFromVector(ApplicationContext, Resource.Drawable.mark_map_blue, 3)));
                markers[i].SetTitle("Спортивная площадка" + "&" + parsePlaces[i].adress + "&" + parsePlaces[i].dimensions + "&" + parsePlaces[i].helpPhone + "&" + parsePlaces[i].website);
                Marker mark = map.AddMarker(markers[i]);
                // Добавление в Dictionary нового листа с именами по полученному id маркера.
                markerNames.Add(mark.Id, new List<string>());
            }
        }

        /// <summary>
        /// Получение из xml файла векторного изображения.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vectorResId"></param>
        /// <param name="X"></param>
        /// <returns></returns>
        public static Bitmap BitmapFromVector(Android.Content.Context context, int vectorResId, double X)
        {
            Drawable vectorDrawable = ContextCompat.GetDrawable(context, vectorResId);
            vectorDrawable.SetBounds(0, 0, (int)(vectorDrawable.IntrinsicWidth * X), (int)(vectorDrawable.IntrinsicHeight * X));
            Bitmap bitmap = Bitmap.CreateBitmap((int)(vectorDrawable.IntrinsicWidth * X), (int)(vectorDrawable.IntrinsicHeight * X), Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            vectorDrawable.Draw(canvas);
            return bitmap;
        }

        /// <summary>
        /// Отобразить фрагмент диалогового окна TimePicker пользователю.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void TimeSelectOnClick(object sender, EventArgs eventArgs)
        {
            TimePickerFragment frag = TimePickerFragment.NewInstance(
                // Обновляет отображение времени действия, используя переданное значение времени.
                delegate (DateTime time)
                {
                    timeRegistration = time.ToShortTimeString();
                });
            // Запуск фрагмента диалогового окна для пользователя.
            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }

        /// <summary>
        /// В зависимости от заполненности площадки смена цвета progress bar.
        /// </summary>
        /// <param name="names"></param>
        /// <param name="dialogView"></param>
        private static void SetProgressBar(List<string> names, View dialogView)
        {
            ProgressBar bar = dialogView.FindViewById(Resource.Id.progressBar1) as ProgressBar;
            for (int i = 0; i < names.Count; i++)
            {
                bar.Progress += 10;
                if (bar.Progress < 50) bar.ProgressDrawable.SetColorFilter(Color.Green, PorterDuff.Mode.Multiply);
                if ((bar.Progress >= 50) && (bar.Progress <= 80)) bar.ProgressDrawable.SetColorFilter(Color.Yellow, PorterDuff.Mode.Multiply); ;
                if (bar.Progress > 80) bar.ProgressDrawable.SetColorFilter(Color.Red, PorterDuff.Mode.Multiply);
            }
        }

        /// <summary>
        /// Установка кнопок для регистрации или её отмены, а также обработка событий нажатия на них.
        /// </summary>
        /// <param name="names"></param>
        /// <param name="dialogBuilder"></param>
        /// <param name="userInput"></param>
        /// <param name="adapter"></param>
        private void SetAnswerUserButtons(List<string> names, Android.App.AlertDialog.Builder dialogBuilder, TextInputEditText userInput, ArrayAdapter<string> adapter)
        {
            // Установка кнопки завершения регистрации.
            dialogBuilder.SetPositiveButton(Resource.String.dialog_registration, (senderAlert, args) =>
            {
                if ((timeRegistration != null) && (userInput.Text != ""))
                {
                    Toast.MakeText(this, GetString(Resource.String.registration_yes), ToastLength.Short).Show();
                    names.Add(userInput.Text + GetString(Resource.String.registration_time) + " " + timeRegistration);
                    adapter.Add(userInput.Text + GetString(Resource.String.registration_time) + " " + timeRegistration);
                    timeRegistration = null;
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.registration_mistake), ToastLength.Short).Show();
                }

            });
            // Установка кнопки отмены регистрации.
            dialogBuilder.SetNegativeButton(GetString(Resource.String.registration_cancel), (senderAlert, args) =>
            { Toast.MakeText(this, GetString(Resource.String.registration_canceled), ToastLength.Short).Show(); });
        }
    }
}