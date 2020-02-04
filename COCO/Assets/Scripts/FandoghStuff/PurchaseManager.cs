using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PurchaseManager
{
    // return amount of added fangooghs
    // otherwise -1
    int MakePurchase(int amount, int price);
}
