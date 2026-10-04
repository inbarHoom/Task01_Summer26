using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[InitializeOnLoad]
public static class UnityRemoteInputFix
{
    private static MethodInfo removeMessageHandler;
    private static Delegate remoteMessageHandler;

    static UnityRemoteInputFix()
    {
        EditorApplication.update += Connect;
        AssemblyReloadEvents.beforeAssemblyReload += Disconnect;
        EditorApplication.quitting += Disconnect;
    }

    private static void Connect()
    {
        Type remoteType = Assembly.Load("UnityEditor.GenericRemoteModule")
            .GetType("UnityEditor.Remote.GenericRemote");
        Type runtimeType = typeof(InputSystem).Assembly
            .GetType("UnityEngine.InputSystem.LowLevel.NativeInputRuntime");

        if (remoteType == null || runtimeType == null) return;

        object runtime = runtimeType
            .GetField("instance", BindingFlags.Public | BindingFlags.Static)
            ?.GetValue(null);
        remoteMessageHandler = runtimeType
            .GetField("m_UnityRemoteMessageHandler", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(runtime) as Delegate;

        if (remoteMessageHandler == null) return;

        MethodInfo addMessageHandler = remoteType.GetMethod("AddMessageHandler");
        removeMessageHandler = remoteType.GetMethod("RemoveMessageHandler");
        addMessageHandler?.Invoke(null, new object[] { remoteMessageHandler });
        EditorApplication.update -= Connect;
        Debug.Log("Unity Remote touch connected to the Input System.");
    }

    private static void Disconnect()
    {
        EditorApplication.update -= Connect;

        if (removeMessageHandler != null && remoteMessageHandler != null)
            removeMessageHandler.Invoke(null, new object[] { remoteMessageHandler });

        removeMessageHandler = null;
        remoteMessageHandler = null;
    }
}
