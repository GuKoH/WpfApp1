using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp1.Core
{

    /// <summary>
    /// Class targetParser
    /// </summary>
    internal class targetParser : IParser<string[]>
    {
        public string[] Parse(IHtmlDocument document, string target)
        {
            List<string> list = new List<string>();
            if (target == "http://morehod.ru/")
            {

                IEnumerable<IElement> captions = document.QuerySelectorAll("div.caption");
                IEnumerable<IElement> datas = document.QuerySelectorAll("div.data");
                for (int i = 0; i < captions.Count(); i++)
                {

                    list.Add(captions.ElementAt(i).TextContent);
                    list.Add(datas.ElementAt(i).TextContent);

                }
            }
            else if (target == "https://www.korabel.ru/")
            {
                IEnumerable<IElement> captions = document.QuerySelectorAll("td.info-text span b");

                for (int i = 0; i < captions.Count(); i++)
                {
                    list.Add(captions.ElementAt(i).TextContent);

                }
            }
            return list.ToArray();
        }
        public string[] ParseName(IHtmlDocument document, string target)
        {
            List<string> list = new List<string>();
            if (target == "http://morehod.ru/")
            {

                IEnumerable<IElement> items = document.All.Where(m => m.LocalName == "h2");

                foreach (IElement item in items)
                {
                    list.Add(item.TextContent);
                }
            }
            else if (target == "https://www.korabel.ru/")
            {
                IEnumerable<IElement> items = document.All.Where(m => m.LocalName == "h2");

                foreach (IElement item in items)
                {
                    list.Add(item.TextContent);
                }
            }


            return list.ToArray();
        }
    }

    /// <summary>
    /// class targetParserSettings
    /// </summary>
    internal class targetParserSettings : IParserSettings
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

    internal class TaskParserSettings : IParserTaskSettings
    {
        public TaskParserSettings(string baseUrl, string postFix, int stPoint, int endPoint)
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
    internal class taskParser : IParserTask<string[]>
    {
        public string[] TaskParser(IHtmlDocument document, string target)
        {
            List<string> list = new List<string>();
            if (target == "http://morehod.ru/")
            {


                IEnumerable<IElement> items = document.QuerySelectorAll("h3 a[href]");//.OfType<IHtmlAnchorElement>(); //((IHtmlAnchorElement)m).Href  
                                                                                      // var items2 = items.Select(m => ((IHtmlAnchorElement)m).Href).ToList();
                foreach (IHtmlAnchorElement item in items)
                {
                    list.Add(item.Href);
                }
            }
            else if (target == "https://www.korabel.ru/")
            {
                IEnumerable<IElement> items = document.QuerySelectorAll("h2 a[href]");//.OfType<IHtmlAnchorElement>(); //((IHtmlAnchorElement)m).Href  
                                                                                      // var items2 = items.Select(m => ((IHtmlAnchorElement)m).Href).ToList();
                foreach (IHtmlAnchorElement item in items)
                {
                    list.Add(item.Href);
                }
            }
            return list.ToArray();
            //return items2.ToArray();
        }

    }


}
