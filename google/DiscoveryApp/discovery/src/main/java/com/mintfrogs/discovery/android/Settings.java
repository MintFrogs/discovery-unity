package com.mintfrogs.discovery.android;

import com.google.android.gms.location.LocationRequest;

public class Settings {
  public static final int DEFAULT_UPDATE_INTERVAL = 1500;
  public static final int DEFAULT_PRIORITY = LocationRequest.PRIORITY_BALANCED_POWER_ACCURACY;

  private int mUpdateInterval;
  private int mFastestUpdateInterval;
  private int mPriority;

  public Settings(int updateInterval, int fastestUpdateInterval, int priority) {
    mUpdateInterval = updateInterval;
    mFastestUpdateInterval = fastestUpdateInterval;
    mPriority = priority;
  }

  public Settings() {
    this(DEFAULT_UPDATE_INTERVAL, DEFAULT_UPDATE_INTERVAL, DEFAULT_PRIORITY);
  }

  public int getUpdateInterval() {
    return mUpdateInterval;
  }

  public int getFastestUpdateInterval() {
    return mFastestUpdateInterval;
  }

  public int getPriority() {
    return mPriority;
  }
}
