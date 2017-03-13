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
    private Discovery currentDiscivery;

    [UsedImplicitly]
    private void Start()
    {
      currentDiscivery.OnLocationUpdated += OnLocationUpdated;
    }

    [UsedImplicitly]
    private void Stop()
    {
      currentDiscivery.OnLocationUpdated -= OnLocationUpdated;
    }

    public void OnInnerStartClick()
    {
      outputText.text = "starting...";
      startButton.interactable = false;
      stopButton.interactable = true;
      currentDiscivery.StartUpdates();
    }

    public void OnInnerStopClick()
    {
      outputText.text = "stopping...";
      startButton.interactable = true;
      stopButton.interactable = false;
      currentDiscivery.StopUpdates();
    }

    public void OnLocationUpdated(Location location)
    {
      var now = DateTime.Now.ToString();
      outputText.text = now + ": " + location;
    }
  }
}
