using UnityEngine;

[System.Serializable]
public class Status {
    //object locker = new();

     private double _hp;
     private double _mp;
    //public double Hp { get => _hp; set => _hp = value; }

    public double GetHp()
    {
        //lock (locker)
        //{
            return _hp;
        //}
    }

    public void SetHp(double Hp)
    {
        //lock (locker)
        //{
            this._hp = Hp;
        //}
    }

    public double GetMp()
    {
        //lock (locker)
        //{
        return _mp;
        //}
    }

    public void SetMp(double Hp)
    {
        //lock (locker)
        //{
        this._mp = Hp;
        //}
    }
    //public double Mp { get => _mp; set => _mp = value; }
}