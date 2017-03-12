package com.mintfrogs.discovery.android;

public class DiscoverySettings {
  public static final int DEFAULT_UPDATE_INTERVAL = 1000;

  private int mUpdateInterval;

  public DiscoverySettings(int updateInterval) {
    mUpdateInterval = updateInterval;
  }

  public int getUpdateInterval() {
    return mUpdateInterval;
  }
}
