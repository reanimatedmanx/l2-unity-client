using FMOD;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class MultiSellList : ServerPacket
{
    private List<MultiSellData> _listMultisell;
    
    private int _listId;
    private int _page;
    private int _pageSize;
    private int _finished;
    private bool _isStackable;
    private int _size;

    public List<MultiSellData> GetMultiSell() { return _listMultisell; }
    public MultiSellList(byte[] d) : base(d)
    {
        _listMultisell = new List<MultiSellData>();
        Parse();
    }

    public override void Parse()
    {
        
        _listId = ReadI();
        _page = ReadI();
        _finished = ReadI();
        _pageSize = ReadI();
        _size = ReadI();

        for (int i = 0; i < _size; i++)
        {
            int index = ReadI();
            int unk1 = ReadI();
            int unk2 = ReadI();
            _isStackable = ReadB() == 1;
            int sizeProducts = ReadSh();
            int sizeIngredients = ReadSh();

            List<Product> listProducts = CreateProductList(sizeProducts);
            List<Ingredient> listIngredient = CreateIngredientList(sizeIngredients);
            _listMultisell.Add(new MultiSellData(listProducts, listIngredient));
        }
        UnityEngine.Debug.Log("");

    }

    private List<Product> CreateProductList(int sizeProducts)
    {
        List < Product > list = new List < Product >(sizeProducts);
        for (int pIndex = 0; pIndex < sizeProducts; pIndex++)
        {
            int itemId = ReadSh();
            int bodyPart = ReadI();
            int type2 = ReadSh();
            int itemCount = ReadI();
            int enchantLevel = ReadSh();

            //not working. server send 0
            int augmentId = ReadI();
            int manaLeft = ReadI();

            Product product = new Product(0, 0, itemCount, type2, 0, bodyPart, enchantLevel, 0, itemId);
            list.Add(product);
        }

        return list;
    }

    private List<Ingredient> CreateIngredientList(int sizeIngredient)
    {
        List<Ingredient> list = new List<Ingredient>(sizeIngredient);

        for (int pIndex = 0; pIndex < sizeIngredient; pIndex++)
        {
            int itemId = ReadSh();
            int type2 = ReadSh();
            int itemCount = ReadI();
            int enchantLevel = ReadSh();

            //not working. server send 0
            int augmentId = ReadI();
            int manaLeft = ReadI();

            Ingredient product = new Ingredient(itemId, type2, itemCount, enchantLevel);
            list.Add(product);
        }

        return list;
    }

}

public class MultiSellData
{
    private List<Ingredient> _ingredients;
    private List<Product> _products;
    public MultiSellData(List<Product> products , List<Ingredient> ingredients)
    {
        _ingredients = ingredients;
        _products = products;
    }
    public List<Ingredient> IngredientList { get => _ingredients; }
    public List<Product> ProductList { get => _products; }
}

public class Ingredient
{
    private int _itemId;

    private int _type2;
    private int _itemCount;
    private int _enchantLevel;

    public Ingredient(int itemId ,  int type2 , int itemCount , int enchantLevel)
    {
        _itemId = itemId;
        _type2 = type2;
        _itemCount = itemCount;
        _enchantLevel = enchantLevel;
    }
}
