using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngleSharp.Html.Dom;
using System.Threading.Tasks;
using AngleSharp.Dom;

namespace WpfApp1.Core
{

    /// <summary>
    /// Class targetParser
    /// </summary>
    class targetParser :IParser<string[]>
    {
        public string[] Parse(IHtmlDocument document)
        {
            //List<string> list = new List<string>();
            //IEnumerable<IElement> items = document.QuerySelectorAll("div.data");
            //    //.Where(item => item.ClassName != null && item.ClassName.Contains("data"));

            //foreach (var item in items)
            //{
            //    list.Add(item.TextContent);
            //}

            List<string> list = new List<string>();
     
            IEnumerable<IElement> captions = document.QuerySelectorAll("div.caption");
            IEnumerable<IElement> datas = document.QuerySelectorAll("div.data");
            for (int i = 0; i < captions.Count(); i++)
            {
                
                list.Add(captions.ElementAt(i).TextContent);
                list.Add(datas.ElementAt(i).TextContent);
                
            }
            
            return list.ToArray();
        }
        public string[] ParseName(IHtmlDocument document)
        {
            List<string> list = new List<string>();
            IEnumerable<IElement> items = document.All.Where( m=> m.LocalName =="h2");
            //.Where(item => item.ClassName != null && item.ClassName.Contains("data"));

            foreach (var item in items)
            {
                list.Add(item.TextContent);
            }
            return list.ToArray();
        }
    }

    /// <summary>
    /// class targetParserSettings
    /// </summary>
    class targetParserSettings : IParserSettings
    {
        public targetParserSettings(string baseUrl, string[] urlList, int endPoint)
        {
            BaseUrl = baseUrl;
            UrlList = urlList;
            
            EndPoint = endPoint;
        }

        public string BaseUrl { get; set; }
        public string[] UrlList { get; set; }
        public int EndPoint { get; set; }
        
    }

    
    class TaskParserSettings : IParserTaskSettings
    {
        public TaskParserSettings(string baseUrl,string postFix,int stPoint,int endPoint)
        {
            BaseUrl = baseUrl;
            Postfix = postFix;
            StartPoint = stPoint;
            EndPoint = endPoint;
        }
            public string BaseUrl { get; set; }
            public string Postfix { get; set; }
            public int StartPoint { get; set; }
            public int EndPoint { get; set; }
            
    
    }


    /// <summary>
    /// Class taskParser
    /// </summary>
    class taskParser : IParserTask<string[]>
    {
        public string[] TaskParser(IHtmlDocument document)
        {
            List<string> list = new List<string>();
            IEnumerable<IElement> items = document.QuerySelectorAll("h3 a[href]");//.OfType<IHtmlAnchorElement>(); //((IHtmlAnchorElement)m).Href  
                                                                                 // var items2 = items.Select(m => ((IHtmlAnchorElement)m).Href).ToList();
            foreach (IHtmlAnchorElement item in items)
            {
                list.Add(item.Href);
            }
            return list.ToArray();
            //return items2.ToArray();
        }

    }

    class taskParserkorabel : IParserTask<string[]>
    {
        public string[] TaskParser(IHtmlDocument document)
        {
            List<string> list = new List<string>();
            IEnumerable<IElement> items = document.QuerySelectorAll("h2 a[href]");//.OfType<IHtmlAnchorElement>(); //((IHtmlAnchorElement)m).Href  
                                                                                  // var items2 = items.Select(m => ((IHtmlAnchorElement)m).Href).ToList();
            foreach (IHtmlAnchorElement item in items)
            {
                list.Add(item.Href);
            }
            return list.ToArray();
            //return items2.ToArray();
        }

    }

    class targetParserKor : IParser<string[]>
    {
        public string[] Parse(IHtmlDocument document)
        {
            //List<string> list = new List<string>();
            //IEnumerable<IElement> items = document.QuerySelectorAll("div.data");
            //    //.Where(item => item.ClassName != null && item.ClassName.Contains("data"));

            //foreach (var item in items)
            //{
            //    list.Add(item.TextContent);
            //}

            List<string> list = new List<string>();

            IEnumerable<IElement> captions = document.QuerySelectorAll("td.info-text span b");
           // IEnumerable<IElement> datas = document.QuerySelectorAll("div.data");
            for (int i = 0; i < captions.Count(); i++)
            {

                list.Add(captions.ElementAt(i).TextContent);
               // list.Add(datas.ElementAt(i).TextContent);

            }

            return list.ToArray();
        }
        public string[] ParseName(IHtmlDocument document)
        {
            List<string> list = new List<string>();
            IEnumerable<IElement> items = document.All.Where(m => m.LocalName == "h2");
            //.Where(item => item.ClassName != null && item.ClassName.Contains("data"));

            foreach (var item in items)
            {
                list.Add(item.TextContent);
            }
            return list.ToArray();
        }
    }


}
