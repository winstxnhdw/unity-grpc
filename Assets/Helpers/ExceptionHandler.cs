using System;
using UnityEngine;

public class ExceptionHandler {
    public static void Handle<T>(string message, Action action) where T : Exception => ExceptionHandler.Handle<T>(message, action, out _);

    public static void Handle<T>(string message, Action action, out bool isError) where T : Exception {
        isError = false;

        try {
            action();
        }

        catch (T) {
            isError = true;
            Debug.Log(message);
        }
    }
}
