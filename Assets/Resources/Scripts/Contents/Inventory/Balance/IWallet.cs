using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWallet
{
    int PlayerMoney { get; }
    bool TrySpend(int amount);
    void AddMoney(int amount);
    event Action<int> OnMoneyChanged;
}
