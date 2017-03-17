// Copyright (c) 2017 Sergey Ivonchik
// This code is licensed under MIT license (See LICENSE for details)

using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace MintFrogs.Discovery
{
  public class ExampleBehaviour : MonoBehaviour
  {
    [SerializeField]
    private Text outputText;

    [SerializeField]
    private Button startButton;

    [SerializeField]
    private Button stopButton;

    [SerializeField]
    private Discovery currentDiscovery;

    [UsedImplicitly]
    private void Start()
    {
      currentDiscovery.Initialize(new Settings());
      currentDiscovery.OnLocationUpdate += OnLocationUpdate;
      currentDiscovery.OnLocationError += OnLocationError;
      currentDiscovery.OnLocationStatusUpdate += OnLocationStatusUpdate;
      currentDiscovery.OnLocationPermissionsUpdate += OnLocationPermissionsUpdate;
    }

    [UsedImplicitly]
    private void Stop()
    {
      currentDiscovery.OnLocationUpdate -= OnLocationUpdate;
      currentDiscovery.OnLocationError -= OnLocationError;
      currentDiscovery.OnLocationStatusUpdate -= OnLocationStatusUpdate;
      currentDiscovery.OnLocationPermissionsUpdate -= OnLocationPermissionsUpdate;
    }

    public void OnInnerStartClick()
    {
      outputText.text = "quering...";
      currentDiscovery.QueryLocationServicesEnabled();
    }

    public void OnInnerStopClick()
    {
      outputText.text = "stopping...";
      startButton.interactable = true;
      stopButton.interactable = false;
      currentDiscovery.StopUpdates();
      outputText.text = "Output Text";
    }

    public void OnInnerUpdateLocationStatus()
    {
      outputText.text = "quering status...";
      currentDiscovery.QueryLocationServicesEnabled();
    }

    public void OnInnerUpdateLocationPermissions()
    {
      outputText.text = "quering permissions... has: [" + currentDiscovery.HasLocationPermissions() + "]";
      currentDiscovery.RequestLocationPermissions();
    }

    public void OnLocationUpdate(Location location)
    {
      var now = DateTime.Now.ToString();
      outputText.text = now + ": " + location;
    }

    public void OnLocationError(string error)
    {
      outputText.text = "error: " + error;
    }

    public void OnLocationStatusUpdate(bool isEnabled)
    {
      outputText.text = "onLocationStatusUpdate: " + isEnabled;

      if (isEnabled)
      {
        if (currentDiscovery.HasLocationPermissions())
        {
          startButton.interactable = false;
          stopButton.interactable = true;
          currentDiscovery.StartUpdates();
        }
        else
        {
          currentDiscovery.RequestLocationPermissions();
        }
      }
      else
      {
        outputText.text = "onLocationStatusUpdate: Disabled";
      }
    }

    public void OnLocationPermissionsUpdate(bool isAllowed)
    {
      outputText.text = "onLocationPermissionsUpdate: " + isAllowed;

      if (isAllowed)
      {
        startButton.interactable = false;
        stopButton.interactable = true;
        currentDiscovery.StartUpdates();
      }
      else
      {
        outputText.text = "onLocationPermissionsUpdate: Not Allowed";
      }
    }
  }
}
