using HtmlAgilityPack;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParseDetected : IParse
{

    //public List<IElementsUI> _elements = new List<IElementsUI>();
    //HashSet<string> _uniqueTexts = new HashSet<string>();
    private Dictionary<string, IElementsUI> _elements = new Dictionary<string, IElementsUI>();
    private int _count = 0;
    public List<IElementsUI> GetElements()
    {
        return _elements.Values.ToList();
    }

    public void Parse(string text)
    {
       //ParseLabel label = new ParseLabel(text);
        //_elements.Add(label);
    }
    
    public void ParseNode(HtmlNode node)
    {
        try
        {
            if (node.Name.Equals("#text"))
            {
                // ParseLabel label = new ParseLabel(node.InnerText);
                if (!_elements.ContainsKey(node.InnerText))
                {
                    _elements.Add(node.InnerText, new ParseLabel(node.InnerText));
                }

            }
            else if (node.Name.Equals("br") | node.Name.Equals("br1"))
            {
                //if (!_elements.ContainsKey(node.InnerText))
                //{
                _elements.Add("br" + _count++, new ParseBr());
                //}
            }
            else if (node.Name.Equals("a"))
            {

                var href = node.Attributes.FirstOrDefault();
                var name = node.InnerText;

                if (!_elements.ContainsKey(name))
                {
                    _elements.Add(node.InnerText, new ParseHref(name, href.Value));
                }
                else
                {
                    AddDictNotUniq(node.InnerText, name, href.Value);
                }

            }
            else if (node.Name.Equals("font"))
            {
                //_elements.Add("br" + _count++, new ParseFontColor());
                var fontColor = node.Attributes.FirstOrDefault();
                string color = fontColor.Value;
                string name = fontColor.Name;
                string text = node.InnerText;
                ParseFontColor parseFontColor = new ParseFontColor(color, text);

                if(!AddHrefColor(text, parseFontColor, _elements))
                {
                    _elements.Add(text, parseFontColor);
                }
                
                //Debug.Log("");
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("HtmlMessage->ParseNode: error " + ex.ToString());
        }
       
    }

    private bool AddHrefColor(string textId , ParseFontColor color , Dictionary<string, IElementsUI> dict)
    {
        if (dict.ContainsKey(textId))
        {
           IElementsUI element =  dict[textId];
            
            if(element != null && element.GetType() == typeof(ParseHref))
            {
                var href = (ParseHref)element;
                href.SetColor(color);
                return true;
            }
        }
        return false;
    }

    private void AddDictNotUniq(string nameName , string name , string hrefValue)
    {
        _elements.Remove(name);
        _elements.Add(nameName, new ParseHref(name, hrefValue));
    }

    
}
