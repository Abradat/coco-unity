using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePurchaseManager : PurchaseManager
{
    public int MakePurchase(int amount, int price)
    {
        return amount;
    }
}
