using HtmlAgilityPack;

using System.Collections.Generic;
using System.Security.Principal;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NpcHtmlMessage : ServerPacket
{

    private int _npcObjId;
    private string _html;
    private int _itemId;
    private IParse parce;
    public List<IElementsUI> Elements()
    {
        return parce.GetElements();
    }


    public NpcHtmlMessage(byte[] d) : base(d)
    {
        Parse();
    }

    public int GetNpcId()
    {
        return _npcObjId;
    }

    public override void Parse()
    {
        _npcObjId = ReadI();
        _html = ReadOtherS();
        _itemId = ReadI();
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(_html);
        parce = new ParseDetected();

        PrintNode(htmlDoc.DocumentNode  , parce);


    }

    static void PrintNode(HtmlNode node , IParse parce)
    {
        // Если узел текстовый, выводим его текст
        if (node.NodeType == HtmlNodeType.Text)
        {
           // Debug.Log("Какой то текст " + node.InnerText.Trim());

            parce.Parse(node.InnerText.Trim());
            //Console.WriteLine($"Text: '{node.InnerText.Trim()}'");
        }
        else
        {
            //Debug.Log("Какой то тэг  " + node.Name);

            // Проходим по дочерним узлам
            foreach (var child in node.ChildNodes)
            {
                parce.ParseNode(child);
                PrintNode(child, parce);
            }
        }
    }
}
