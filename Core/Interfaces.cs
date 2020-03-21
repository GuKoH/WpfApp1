using System;
using System.Collections.Generic;
using System.Text;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using AngleSharp;

namespace WpfApp1.Core
{
    /// <summary>
    /// Class HtmlLoader
    /// </summary>
    class HtmlLoader
    {
        readonly HttpClient client;
        readonly string url;
     
        int currentPage;
        public HtmlLoader(IParserSettings settings)
        {
            
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.136 YaBrowser/20.2.4.143 Yowser/2.5 Safari/537.36");
            url = $"{settings.BaseUrl}{settings.UrlList[0]}";
        }
       
        public async Task<string> GetSourceByList(string Url)
        {
            //string currentUrl = url.Replace("{CurrentId}", Url);
            HttpResponseMessage responseMessage = await client.GetAsync(Url); //поменять на currentUrl
            string source = default;

            if (responseMessage != null && responseMessage.StatusCode == HttpStatusCode.OK)
            {
                source = await responseMessage.Content.ReadAsStringAsync();
            }
            return source;
        }
      
    }
    /// <summary>
    /// Class TaskHtmlLoader
    /// </summary>
    class TaskHtmlLoader
    {
        readonly HttpClient client;
        readonly string url;
        readonly List<string> UrlList;
        int currentPage;
       
        public TaskHtmlLoader(IParserTaskSettings settings)
        {
            currentPage = settings.StartPoint;
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.136 YaBrowser/20.2.4.143 Yowser/2.5 Safari/537.36");
            url = $"{settings.BaseUrl}{settings.Postfix}{settings.StartPoint}";
        }

        public async Task<string> GetSourceByPage(int page)
        {
            string currentUrl = url.Replace(currentPage.ToString(), page.ToString());
            HttpResponseMessage responseMessage = await client.GetAsync(currentUrl); //поменять на currentUrl
            string source = default;

            if (responseMessage != null && responseMessage.StatusCode == HttpStatusCode.OK)
            {
                source = await responseMessage.Content.ReadAsStringAsync();
            }
            return source;
        }

    }
    /// <summary>
    /// Интерфейс парсера
    /// </summary>
    interface IParser<T> where T : class
    {
        T Parse(IHtmlDocument document);
        T ParseName(IHtmlDocument document);
    }

    /// <summary>
    /// интефейс ParserTask
    /// </summary>
    /// <typeparam name="T">SomeParam</typeparam>
    interface IParserTask<T> where T : class
    {
        T TaskParser(IHtmlDocument document);
    }
    interface IParserTaskSettings
    {
         string BaseUrl { get; set; }
         string Postfix { get; set; }
         int StartPoint { get; set; }
         int EndPoint { get; set; }
    }

    /// <summary>
    /// Интерфейс настроек парсера
    /// </summary>
    interface IParserSettings
    {
         string BaseUrl { get; set; }
         string[] UrlList { get; set; }
         int EndPoint { get; set; }
    }

    /// <summary>
    /// Class TaskParser
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class TaskParser<T> where T : class
    {
        TaskHtmlLoader loader;
        bool isActive;
        IParserTask<T> parserTask;
        IParserTaskSettings parserTaskSettings;
        public IParserTask<T> ParserTask
        {
            get { return parserTask; }
            set { parserTask = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
        }
        public IParserTaskSettings Settings
        {
            get { return parserTaskSettings; }
            set
            {
                parserTaskSettings = value; //Новые настройки парсера
                loader = new TaskHtmlLoader(value); //сюда помещаются настройки для загрузчика кода страницы
            }
        }
        public event Action<object> OnComplitedTask;
        public event Action<object, T> OnNewTask;
        public TaskParser(IParserTask<T> parserTask)
        {
            this.parserTask = parserTask;
        }
        public void StartTask()
        {
            isActive = true;
            Tasker();
        }
    
        public void Stop()
        {
            isActive = false;
        }
        public async void Tasker()
        {
            for (int i = parserTaskSettings.StartPoint; i <= parserTaskSettings.EndPoint; i++)
            {
                if (isActive)
                {
                    string source = await loader.GetSourceByPage(i);
                    HtmlParser taskParser = new HtmlParser();
                    IHtmlDocument document = await taskParser.ParseDocumentAsync(source);
                    T Task = parserTask.TaskParser(document);
                    OnNewTask?.Invoke(this, Task);

                }
               
            }
            OnComplitedTask?.Invoke(this);
            isActive = false;
        }

    }

    /// <summary>
    /// Сlass ParseWorker
    /// </summary>
    /// <typeparam name="T"></typeparam>
   class ParseWorker<T> where T : class
    {
        IParser<T> parser;
        
        IParserSettings parserSettings;
        HtmlLoader loader;
        bool isActive;

        public IParser<T> Parser
        {
            get { return parser; }
            set {parser = value; }
        }
        public IParserSettings Settings
        {
            get { return parserSettings; }
            set 
            {
                parserSettings = value;
                loader = new HtmlLoader(value); 
            }
        }
       
        public bool IsActive
        {
            get { return isActive; }
        }

        public event Action<object, T, T> OnNewData;
        public event Action<object> OnComplited;
      

      
        public ParseWorker(IParser<T> parser)
        {
            this.parser = parser;
        }
        public void Start()
        {
            isActive = true;
            Worker();
        }
        public void Stop()
        {
            isActive = false;
        }
        public async void Worker()
        {
            for (int i = 0; i < parserSettings.EndPoint; i++)
            {
                if (isActive)
                {
                    string source = await loader.GetSourceByList(parserSettings.UrlList[i]); //здесь должно быть не так
                  
                    HtmlParser domParser = new HtmlParser();
                    IHtmlDocument document = await domParser.ParseDocumentAsync(source);
                    T result = parser.Parse(document);
                    T compName = parser.ParseName(document);
                    OnNewData?.Invoke(this, result, compName);
                }
                
            }
            OnComplited?.Invoke(this);
            isActive = false;
        }
      
     }
}
