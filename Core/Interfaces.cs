using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WpfApp1.Core
{
    /// <summary>
    /// Class HtmlLoader
    /// </summary>
    internal class HtmlLoader
    {
        private readonly HttpClient _client;
        private readonly string _url;


        public HtmlLoader(IParserSettings settings)
        {

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.136 YaBrowser/20.2.4.143 Yowser/2.5 Safari/537.36");
            _url = $"{settings.BaseUrl}{settings.UrlList[0]}";
        }

        public async Task<string> GetSourceByList(string url)
        {
            //string currentUrl = url.Replace("{CurrentId}", Url);
            HttpResponseMessage responseMessage = await _client.GetAsync(url).ConfigureAwait(true); //поменять на currentUrl
            string source = default;

            if (responseMessage != null && responseMessage.StatusCode == HttpStatusCode.OK)
            {
                source = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(true);
            }
            return source;
        }

    }

    /// <summary>
    /// Class TaskHtmlLoader
    /// </summary>
    internal class TaskHtmlLoader
    {
        private readonly HttpClient _client;
        private readonly string _url;
        private readonly int _currentPage;

        public TaskHtmlLoader(IParserTaskSettings settings)
        {
            _currentPage = settings.StartPoint;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.136 YaBrowser/20.2.4.143 Yowser/2.5 Safari/537.36");
            _url = $"{settings.BaseUrl}{settings.Postfix}{settings.StartPoint}";
        }

        public async Task<string> GetSourceByPage(int page)
        {
            string currentUrl = _url.Replace(_currentPage.ToString(CultureInfo.CurrentCulture), page.ToString(CultureInfo.CurrentCulture));
            HttpResponseMessage responseMessage = await _client.GetAsync(currentUrl).ConfigureAwait(true); //поменять на currentUrl
            string source = default;

            if (responseMessage != null && responseMessage.StatusCode == HttpStatusCode.OK)
            {
                source = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(true);
            }
            return source;
        }

    }

    /// <summary>
    /// Интерфейс парсера
    /// </summary>
    internal interface IParser<T> where T : class
    {
        T Parse(IHtmlDocument document, string target);
        T ParseName(IHtmlDocument document, string target);
    }

    /// <summary>
    /// интефейс ParserTask
    /// </summary>
    /// <typeparam name="T">SomeParam</typeparam>
    internal interface IParserTask<T> where T : class
    {
        T TaskParser(IHtmlDocument document, string target);
    }

    internal interface IParserTaskSettings
    {
        string BaseUrl { get; set; }
        string Postfix { get; set; }
        int StartPoint { get; set; }
        int EndPoint { get; set; }
    }

    /// <summary>
    /// Интерфейс настроек парсера
    /// </summary>
    internal interface IParserSettings
    {
        string BaseUrl { get; set; }
        string[] UrlList { get; set; }
        int EndPoint { get; set; }
    }

    /// <summary>
    /// Class TaskParser
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class TaskParser<T> where T : class
    {
        private TaskHtmlLoader _loader;
        private bool _isActive;
        private IParserTask<T> _parserTask;
        private IParserTaskSettings _parserTaskSettings;
        public IParserTask<T> ParserTask
        {
            get => _parserTask;
            set => _parserTask = value;
        }

        public bool IsActive => _isActive;
        public IParserTaskSettings Settings
        {
            get => _parserTaskSettings;
            set
            {
                _parserTaskSettings = value; //Новые настройки парсера
                _loader = new TaskHtmlLoader(value); //сюда помещаются настройки для загрузчика кода страницы
            }
        }
        public event Action<object> OnComplitedTask;
        public event Action<object, T> OnNewTask;
        public TaskParser(IParserTask<T> parserTask)
        {
            this._parserTask = parserTask;
        }
        public void StartTask()
        {
            _isActive = true;
            Tasker();
        }

        public void Stop()
        {
            _isActive = false;
        }
        public async void Tasker()
        {
            for (int i = _parserTaskSettings.StartPoint; i <= _parserTaskSettings.EndPoint; i++)
            {
                if (_isActive)
                {
                    string source = await _loader.GetSourceByPage(i).ConfigureAwait(true);
                    HtmlParser taskParser = new HtmlParser();
                    IHtmlDocument document = await taskParser.ParseDocumentAsync(source).ConfigureAwait(true);
                    T Task = _parserTask.TaskParser(document, _parserTaskSettings.BaseUrl);
                    OnNewTask?.Invoke(this, Task);

                }

            }
            OnComplitedTask?.Invoke(this);
            _isActive = false;
        }

    }

    /// <summary>
    /// Сlass ParseWorker
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ParseWorker<T> where T : class
    {
        private IParser<T> _parser;
        private IParserSettings _parserSettings;
        private HtmlLoader _loader;
        private bool _isActive;

        public IParser<T> Parser
        {
            get => _parser;
            set => _parser = value;
        }
        public IParserSettings Settings
        {
            get => _parserSettings;
            set
            {
                _parserSettings = value;
                _loader = new HtmlLoader(value);
            }
        }

        public bool IsActive => _isActive;

        public event Action<object, T, T> OnNewData;
        public event Action<object> OnComplited;



        public ParseWorker(IParser<T> parser)
        {
            this._parser = parser;
        }
        public void Start()
        {
            _isActive = true;
            Worker();
        }
        public void Stop()
        {
            _isActive = false;
        }
        public async void Worker()
        {
            for (int i = 0; i < _parserSettings.EndPoint; i++)
            {
                if (_isActive)
                {
                    string source = await _loader.GetSourceByList(_parserSettings.UrlList[i]).ConfigureAwait(true); //здесь должно быть не так

                    HtmlParser domParser = new HtmlParser();
                    IHtmlDocument document = await domParser.ParseDocumentAsync(source).ConfigureAwait(true);
                    T result = _parser.Parse(document, _parserSettings.BaseUrl);
                    T compName = _parser.ParseName(document, _parserSettings.BaseUrl);
                    OnNewData?.Invoke(this, result, compName);
                }

            }
            OnComplited?.Invoke(this);
            _isActive = false;
        }

    }
}
