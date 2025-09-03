using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallet : IWallet
{
    private UserDataManager userDataManager => Managers.Data.UserDataManager;
    public int PlayerMoney => userDataManager.CurrentData.gold;
    public event Action<int> OnMoneyChanged;

    public bool TrySpend(int amount)
    {
        if (amount < 0 || PlayerMoney < amount)
        {
            return false;
        }

        userDataManager.CurrentData.gold -= amount;
        OnMoneyChanged?.Invoke(PlayerMoney);

        return true;
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        userDataManager.CurrentData.gold += amount;
        OnMoneyChanged?.Invoke(PlayerMoney);
    }
}
