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

      // XXX: First, you request permissions, then query for
      // location availability. For iOS, first step will return
      // TRUE (Always) and when you start services, iOS automatically
      // will ask for permissions.

      // XXX: Also, for Android, when you ask if services enabled,
      // resolution window will be shown to the user, for iOS, it
      // will simply return status of services availability.

      // XXX: Main case here: Android - prmissions BEFORE availability,
      // on iOS AFTER.
      if (currentDiscovery.HasLocationPermissions())
      {
        currentDiscovery.QueryLocationServicesEnabled();
      }
      else
      {
        currentDiscovery.RequestLocationPermissions();
      }
    }

    public void OnInnerStopClick()
    {
      outputText.text = "stopping...";
      startButton.interactable = true;
      stopButton.interactable = false;
      currentDiscovery.StopUpdates();
      outputText.text = "Output Text";
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
        startButton.interactable = false;
        stopButton.interactable = true;
        currentDiscovery.StartUpdates();
      }
      else
      {
        startButton.interactable = true;
        stopButton.interactable = false;
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
        currentDiscovery.QueryLocationServicesEnabled();
      }
      else
      {
        outputText.text = "onLocationPermissionsUpdate: Not Allowed";
      }
    }
  }
}
