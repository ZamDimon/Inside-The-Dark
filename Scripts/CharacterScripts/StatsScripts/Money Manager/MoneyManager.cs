using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoneyManager : MonoBehaviour {
    public delegate void MoneyChanged(int value);
    public event MoneyChanged OnMoneyChanged;

    [SerializeField] private int startMoney;
    private int money;
    
    private void Awake() {
        AddMoney(startMoney);
    }

    public void AddMoney(int value) { 
        money += value;
        OnMoneyChanged?.Invoke(value);
    }

    public bool TryTakeMoney(int value) {
        if (value > money) 
            return false;
        
        money -= value;
        OnMoneyChanged?.Invoke(-value);
        return true;
    }

    public int GetMoney() => money;
}
