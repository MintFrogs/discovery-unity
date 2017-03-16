// Copyright (c) 2017 Sergey Ivonchik
// This code is licensed under MIT license (See LICENSE for details)

using JetBrains.Annotations;
using UnityEngine;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace MintFrogs.Discovery
{
  public class Discovery : MonoBehaviour
  {
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

#if UNITY_ANDROID
      InitializeAndroidImpl();
#elif UNITY_IOS
      SXDiscoveryInitialize(settings.SerializeAsAppleNativeString());
#else
      Debug.Log("Discovery::Initialize(settings)");
#endif
    }

    public void StartUpdates()
    {
#if UNITY_ANDROID
      StartAndroidImpl();
#elif UNITY_IOS
      SXDiscoveryStart();
#else
      if (null != OnLocationUpdate)
      {
        OnLocationUpdate(Location.Default());
      }
#endif
    }

    public void StopUpdates()
    {
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
      return false;
#endif
    }

    public void QueryLocationServicesEnabled()
    {
#if UNITY_ANDROID
      QueryLocationServicesEnabledAndroidImpl();
#endif
    }

    public void RequestLocationPermissions()
    {
#if UNITY_ANDROID
      RequestLocationPermissionsAndroidImpl();
#endif
    }

    public bool HasLocationPermissions()
    {
#if UNITY_ANDROID
      return HasLocationPermissionsAndroidImpl();
#else
      return UnityEngine.Input.location.isEnabledByUser;
#endif
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
      var discoveryObject = new AndroidJavaObject(DiscoveryClazz, settingsObject, null);

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
#endif
  }
}
