// Copyright (c) 2017 Sergey Ivonchik
// This code is licensed under MIT license (See LICENSE for details)

namespace MintFrogs.Discovery
{
  public class Settings
  {
    public const int DefaultInterval = 1500;
    public const int DefaultFastestInterval = 1000;

    public const int AccuracyBalanced = 102;
    public const int AccuracyHigh = 100;
    public const int AccuracyLow = 104;

    public Settings(int interval, int fastestInterval, int accuracy)
    {
      Interval = interval;
      FastestInterval = fastestInterval;
      Accuracy = accuracy;
    }

    public Settings() : this(DefaultInterval, DefaultFastestInterval, AccuracyBalanced)
    {
    }

    public int Interval { get; private set; }

    public int FastestInterval { get; private set; }

    public int Accuracy { get; private set; }
  }
}
