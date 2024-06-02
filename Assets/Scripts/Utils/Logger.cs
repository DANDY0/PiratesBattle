using UnityEngine;

namespace Utils
{
    public static class Logger
    {
        
        public static void Log(string message, string type = "")
        {
            if(!Constants.EnableLogs)
                return;
            
            Debug.Log($"type: {type},message: {message}");
        }
        
    }
}