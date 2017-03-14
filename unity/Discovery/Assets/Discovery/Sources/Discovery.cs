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
    private AndroidJavaObject currentDiscoverObject;

    public delegate void LocationHandler(Location location);
    public delegate void ErrorHandler(string error);

    public event LocationHandler OnLocationUpdate;
    public event ErrorHandler OnLocationError;

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

      currentDiscoverObject = discoveryObject;
#endif
    }

    private void StartAndroidImpl()
    {
#if UNITY_ANDROID
      if (null != currentDiscoverObject)
      {
        currentDiscoverObject.Call("start");
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
      if (null != currentDiscoverObject)
      {
        currentDiscoverObject.Call("stop");
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
      return null != currentDiscoverObject && currentDiscoverObject.Call<bool>("isStarted");
#else
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
