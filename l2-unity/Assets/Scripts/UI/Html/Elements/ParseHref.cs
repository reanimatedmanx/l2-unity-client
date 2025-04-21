using UnityEngine;

public class ParseHref : IElementsUI
{
    private string _name;
    private string _action;

    public string Name { get => _name; }
    public string Action { get => _action; }
    public ParseHref(string href , string actions)
   {
        _name = href;
        _action = clearBypass(actions);
   }

   private string clearBypass(string href)
   {
        string pass1 = href.Replace("bypass", "").Trim();
        string pass2 = pass1.Replace("-h", "").Trim();
        return pass2;
   }
}
