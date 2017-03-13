// Copyright (c) 2017 Sergey Ivonchik
// This code is licensed under MIT license (See LICENSE for details)

using JetBrains.Annotations;
using UnityEngine;

namespace MintFrogs.Discovery
{
  public class Discovery : MonoBehaviour
  {
    public const int DefaultUpdateInterval = 1000;

    private const string DiscoveryClazz = "com.mintfrogs.discovery.android.Discovery";
    private const string DiscoverySettingsClazz = "com.mintfrogs.discovery.android.Settings";

    [SerializeField]
    private int interval;

    [SerializeField]
    private int accurancy;

    private AndroidJavaObject currentDiscoverObject;

    public delegate void LocationHandler(Location location);
    public event LocationHandler OnLocationUpdated;

    public void Initialize()
    {
      InitializeAndroidImpl();
    }

    public void StartUpdates()
    {
      if (null != currentDiscoverObject)
      {
        currentDiscoverObject.Call("start");
      }
    }

    public void StopUpdates()
    {
      if (null != currentDiscoverObject)
      {
        currentDiscoverObject.Call("stop");
      }
    }

    [UsedImplicitly]
    private void Awake()
    {
      DontDestroyOnLoad(gameObject);
    }

    [UsedImplicitly]
    private void OnLocationUpdate(string serializedLocation)
    {
      if (null != OnLocationUpdated)
      {
        OnLocationUpdated(Location.Parse(serializedLocation));
      }
    }

    private void InitializeAndroidImpl()
    {
      var settingsObject = new AndroidJavaObject(DiscoverySettingsClazz);
      var discoveryObject = new AndroidJavaObject(DiscoveryClazz, settingsObject);

      currentDiscoverObject = discoveryObject;
    }
  }
}
