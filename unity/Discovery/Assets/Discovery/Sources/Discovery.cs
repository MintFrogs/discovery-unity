﻿// Copyright (c) 2017 Sergey Ivonchik
// This code is licensed under MIT license (See LICENSE for details)

using System;
using JetBrains.Annotations;
using UnityEngine;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace MintFrogs.Discovery
{
  public class Discovery : MonoBehaviour
  {
    private const string GameObjectName = "MfDiscoveryService";
    private const string LogFmt = "Discovery::{0}";
    private const string DiscoveryClazz = "com.mintfrogs.discovery.android.Discovery";
    private const string DiscoverySettingsClazz = "com.mintfrogs.discovery.android.Settings";

    private Settings currentSettings;
    private AndroidJavaObject currentDiscoveryObject;

    public delegate void LocationHandler(Location location);
    public delegate void ErrorHandler(string error);
    public delegate void LocationStatusHandler(bool isEnabled);
    public delegate void LocationPermissionsHandler(bool isAllowed);

    public event LocationHandler OnLocationUpdate;
    public event ErrorHandler OnLocationError;
    public event LocationStatusHandler OnLocationStatusUpdate;
    public event LocationPermissionsHandler OnLocationPermissionsUpdate;

    // --------------------------------------------------------------------------------------------
    // Public Interface
    // --------------------------------------------------------------------------------------------

    public static Discovery Instance { get; private set; }

    public void Initialize(Settings settings)
    {
      currentSettings = settings;

      if (RichUnity.IsAnyEditor())
      {
        return;
      }

#if UNITY_ANDROID
      InitializeAndroidImpl();
#elif UNITY_IOS
      SXDiscoveryInitialize(settings.SerializeAsAppleNativeString());
#else
      Debug.Log("Discovery::Initialize(settings)");
#endif
    }

    public bool IsInitialized()
    {
      return null != currentSettings;
    }

    public void StartUpdates()
    {
      if (RichUnity.IsAnyEditor())
      {
        if (null != OnLocationUpdate)
        {
          OnLocationUpdate(Location.Default());
        }

        return;
      }

#if UNITY_ANDROID
      StartAndroidImpl();
#elif UNITY_IOS
      SXDiscoveryStart();
#endif
    }

    public void StopUpdates()
    {
      if (RichUnity.IsAnyEditor())
      {
        return;
      }

#if UNITY_ANDROID
      StopAndroidImpl();
#elif UNITY_IOS
      SXDiscoveryStop();
#else
      Debug.Log("Discovery::Stop()");
#endif
    }

    public bool IsStarted()
    {
#if UNITY_ANDROID
      return IsStartedAndroidImpl();
#elif UNITY_IOS
      return SXDiscoveryIsStarted();
#else
      Debug.Log("Discovery::IsStarted()");
      return true;
#endif
    }

    public void QueryLocationServicesEnabled()
    {
      if (RichUnity.IsAnyEditor() && null != OnLocationStatusUpdate)
      {
        OnLocationStatusUpdate(Input.location.isEnabledByUser);
        return;
      }

#if UNITY_ANDROID
      QueryLocationServicesEnabledAndroidImpl();
#elif UNITY_IOS
      SXDisciveryIsLocationEnabled();
#endif
    }

    public void RequestLocationPermissions()
    {
      if (RichUnity.IsAnyEditor() && null != OnLocationPermissionsUpdate)
      {
        OnLocationPermissionsUpdate(true);
        return;
      }

#if UNITY_ANDROID
      RequestLocationPermissionsAndroidImpl();
#elif UNITY_IOS
      // XXX: iOS will ask permissions automatically while starting updates.
      OnInnerLocationPermissionsResult("true");
#else
      OnInnerLocationPermissionsResult("true");
#endif
    }

    public bool HasLocationPermissions()
    {
#if UNITY_ANDROID
      return HasLocationPermissionsAndroidImpl();
#endif
      // TODO: Implement properly for iOS
      return true;
    }

    // --------------------------------------------------------------------------------------------
    // Private Methods
    // --------------------------------------------------------------------------------------------

    [UsedImplicitly]
    private void Awake()
    {
      if (Instance != null && Instance != this)
      {
        Destroy(gameObject);
      }
      else
      {
        if (name != GameObjectName)
        {
          throw new SystemException("Discover object name must be: " + GameObjectName);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
      }
    }

    [UsedImplicitly]
    private void OnInnerLocationUpdate(string serializedLocation)
    {
      if (null != OnLocationUpdate)
      {
        OnLocationUpdate(Location.Parse(serializedLocation));
      }
    }

    [UsedImplicitly]
    private void OnInnerLocationError(string error)
    {
      if (null != OnLocationError)
      {
        OnLocationError(error);
      }
    }

    [UsedImplicitly]
    private void OnInnerLocationPermissionsResult(string result)
    {
      if (null != OnLocationPermissionsUpdate)
      {
        OnLocationPermissionsUpdate(result == "true");
      }
    }

    [UsedImplicitly]
    private void OnInnerLocationResolutionCallback(string result)
    {
      if (null != OnLocationStatusUpdate)
      {
        OnLocationStatusUpdate(result == "true");
      }
    }

    // --------------------------------------------------------------------------------------------
    // Android Impl
    // --------------------------------------------------------------------------------------------

    private void InitializeAndroidImpl()
    {
#if UNITY_ANDROID
      var interval = currentSettings.Interval;
      var fastestInterval = currentSettings.FastestInterval;
      var accuracy = currentSettings.Accuracy;

      var settingsObject = new AndroidJavaObject(DiscoverySettingsClazz, interval, fastestInterval, accuracy);
      var discoveryObject = new AndroidJavaClass(DiscoveryClazz).CallStatic<AndroidJavaObject>("getInstance", settingsObject, null);

      currentDiscoveryObject = discoveryObject;
#endif
    }

    private void StartAndroidImpl()
    {
#if UNITY_ANDROID
      if (null != currentDiscoveryObject)
      {
        currentDiscoveryObject.Call("start");
      }
      else
      {
        Debug.LogWarning(string.Format(LogFmt, "not initialized"));
      }
#endif
    }

    private void StopAndroidImpl()
    {
#if UNITY_ANDROID
      if (null != currentDiscoveryObject)
      {
        currentDiscoveryObject.Call("stop");
      }
      else
      {
        Debug.LogWarning(string.Format(LogFmt, "not initialized"));
      }
#endif
    }

    private bool IsStartedAndroidImpl()
    {
#if UNITY_ANDROID
      return null != currentDiscoveryObject && currentDiscoveryObject.Call<bool>("isStarted");
#else
      return false;
#endif
    }

    private void QueryLocationServicesEnabledAndroidImpl()
    {
#if UNITY_ANDROID
      if (null != currentDiscoveryObject)
      {
        currentDiscoveryObject.Call("isLocationServicesEnabled", null, true);
      }
      else
      {
        Debug.LogWarning(string.Format(LogFmt, "not initialized"));
      }
#endif
    }

    private void RequestLocationPermissionsAndroidImpl()
    {
#if UNITY_ANDROID
      if (null != currentDiscoveryObject)
      {
        currentDiscoveryObject.Call("requestLocationPermissions");
      }
      else
      {
        Debug.LogWarning(string.Format(LogFmt, "not initialized"));
      }
#endif
    }

    private bool HasLocationPermissionsAndroidImpl()
    {
#if UNITY_ANDROID
      if (null != currentDiscoveryObject)
      {
        return currentDiscoveryObject.Call<bool>("hasLocationPermissions");
      }

      Debug.LogWarning(string.Format(LogFmt, "not initialized"));
      return false;
#endif

      return false;
    }

    // --------------------------------------------------------------------------------------------
    // iOS Impl
    // --------------------------------------------------------------------------------------------

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void SXDiscoveryInitialize(string settings);

    [DllImport("__Internal")]
    private static extern void SXDiscoveryStart();

    [DllImport("__Internal")]
    private static extern void SXDiscoveryStop();

    [DllImport("__Internal")]
    private static extern bool SXDiscoveryIsStarted();

    [DllImport("__Internal")]
    private static extern bool SXDisciveryIsLocationEnabled();
#endif
  }
}
