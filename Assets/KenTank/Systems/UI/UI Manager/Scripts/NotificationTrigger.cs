using UnityEngine;

namespace KenTank.Systems.UI
{
    public class NotificationTrigger : MonoBehaviour
    {
        public static void Show(string msg)
        {
            NotificationCenter.instance.Show(msg);
        }
    }
}