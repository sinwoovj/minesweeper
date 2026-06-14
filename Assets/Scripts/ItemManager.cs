using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public void UseRecoveryItem()
    {
        PlayerManager.Instance.UseItem(0);
    }
}
