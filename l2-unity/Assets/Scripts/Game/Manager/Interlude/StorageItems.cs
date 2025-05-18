using System.Collections.Generic;

public class StorageItems
{
    private static StorageItems instance;
    private object _sync = new object();
    private ItemInstance[] items;
    private List<Shortcut> shortCuts;
    private bool showWindow;
    private StorageItems()
    { }

    public static StorageItems getInstance()
    {
        if (instance == null)
        {
            instance = new StorageItems();
        }

        return instance;
    }
    public void AddShortCuts(List<Shortcut> shortCuts)
    {
        lock (_sync)
        {
            if (this.shortCuts != null)
            {
                this.shortCuts.Clear();
            }

            this.shortCuts = shortCuts;
        }
       
    }
    public void AddItems(ItemInstance[] itemsParce)
    {
        lock (_sync)
        {
            items = null;
            items = itemsParce;
        } 
    }

    public void AddShow(bool show)
    {
        showWindow = show;
    }

    public ItemInstance[]  GetItems()
    {
        lock (_sync)
        {
            if (items != null)
            {
                return items;
            }
            return new ItemInstance[0];
        }
   
    }

    public List<Shortcut> GetShortCuts()
    {
        lock (_sync)
        {
            if (shortCuts != null)
            {
                return shortCuts;
            }
            return new List<Shortcut>();
        }

    }

    public bool GetShowWindow()
    {
        return showWindow;
    }
}
