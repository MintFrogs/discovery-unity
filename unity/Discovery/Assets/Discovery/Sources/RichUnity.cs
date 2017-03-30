// Copyright (c) 2017 Sergey Ivonchik
// This code is licensed under MIT license (See LICENSE for details)

using UnityEngine;

namespace MintFrogs.Discovery
{
  public static class RichUnity
  {
    public static bool IsAnyEditor()
    {
      return Application.platform == RuntimePlatform.OSXEditor ||
        Application.platform == RuntimePlatform.LinuxEditor ||
        Application.platform == RuntimePlatform.WindowsEditor;
    }

    public static bool IsAndroid()
    {
      return Application.platform == RuntimePlatform.Android;
    }

    public static bool IsApple()
    {
      return Application.platform == RuntimePlatform.IPhonePlayer;
    }
  }
}
