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
    }

    [UsedImplicitly]
    private void Stop()
    {
      currentDiscovery.OnLocationUpdate -= OnLocationUpdate;
      currentDiscovery.OnLocationError -= OnLocationError;
    }

    public void OnInnerStartClick()
    {
      outputText.text = "starting...";
      startButton.interactable = false;
      stopButton.interactable = true;
      currentDiscovery.StartUpdates();
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
  }
}
