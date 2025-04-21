using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CreatureMessage : SystemMessage
{
    private string sendName;
    private string text;
    private string color;
    public CreatureMessage(string sendName , string text , string color) : base(null, null)
    {
        this.sendName = sendName;
        this.text = " "+text;
        this.color = color;
    }

    public override string ToString()
    {
        return "<color="+color+">" + sendName+":"+text+"</color>";
    }
}
