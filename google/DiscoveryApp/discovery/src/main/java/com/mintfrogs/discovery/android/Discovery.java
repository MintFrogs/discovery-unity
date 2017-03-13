package com.mintfrogs.discovery.android;

import android.Manifest.permission;
import android.content.Context;
import android.content.pm.PackageManager;
import android.location.Location;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.v4.app.ActivityCompat;
import android.text.TextUtils;
import android.util.Log;

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.common.api.GoogleApiClient.ConnectionCallbacks;
import com.google.android.gms.common.api.GoogleApiClient.OnConnectionFailedListener;
import com.google.android.gms.location.LocationListener;
import com.google.android.gms.location.LocationRequest;
import com.google.android.gms.location.LocationServices;
import com.unity3d.player.UnityPlayer;

import java.util.ArrayList;

public class Discovery implements ConnectionCallbacks, OnConnectionFailedListener, LocationListener {
  private static final String TAG = Discovery.class.getSimpleName();
  private static final String UNITY_OBJECT = "MfDiscoveryService";
  private static final String UNITY_UPDATE_CALLBACK = "OnLocationUpdate";
  private static final String UNITY_ERROR_CALLBACK = "OnLocationError";

  private Settings mSettings;
  private GoogleApiClient mClient;
  private Context mContext;

  public Discovery(Settings settings, @Nullable Context context) {
    mSettings = settings;
    mContext = null == context ? UnityPlayer.currentActivity.getApplicationContext() : context;
    mClient = newClientInstance(mContext);
  }

  public void start() {
    Log.i(TAG, "starting... \\w" + mClient);

    if (null != mClient) {
      mClient.connect();
    }
  }

  public void stop() {
    Log.i(TAG, "stopping... \\w" + mClient);

    stopUpdates();

    if (null != mClient) {
      mClient.disconnect();
    }
  }

  @SuppressWarnings("MissingPermission")
  public String queryLastLocation() {
    if (hasPermissions()) {
      Location location = LocationServices.FusedLocationApi.getLastLocation(mClient);
      return serializeLocationAsString(location);
    }

    return "";
  }

  @Override
  public void onConnected(@Nullable Bundle bundle) {
    Log.i(TAG, "connected... \\w" + mClient + " bundle: " + bundle);
    startUpdates();
  }

  @Override
  public void onConnectionSuspended(int i) {
    Log.i(TAG, "suspended" + "[" + i + "]" + "... \\w" + mClient);
    stopUpdates();
  }

  @Override
  public void onConnectionFailed(@NonNull ConnectionResult result) {
    Log.i(TAG, "connection failed... \\w" + mClient + " result: " + result);
    stopUpdates();
  }

  @Override
  public void onLocationChanged(Location location) {
    String args = serializeLocationAsString(location);

    if (isUnityEnv()) {
      UnityPlayer.UnitySendMessage(UNITY_OBJECT, UNITY_UPDATE_CALLBACK, args);
    } else {
      Log.w(TAG, "onLocationChanged[null]: " + args);
    }
  }

  private GoogleApiClient newClientInstance(Context ctx) {
    GoogleApiClient.Builder builder = new GoogleApiClient.Builder(ctx);
    builder.addConnectionCallbacks(this);
    builder.addOnConnectionFailedListener(this);
    builder.addApi(LocationServices.API);
    builder.build();

    return builder.build();
  }

  private LocationRequest newLocationRequestInstance() {
    LocationRequest request = new LocationRequest();
    request.setInterval(mSettings.getUpdateInterval());
    request.setFastestInterval(mSettings.getUpdateInterval());
    return request;
  }

  @SuppressWarnings("MissingPermission")
  private void startUpdates() {
    if (hasPermissions()) {
      LocationRequest req = newLocationRequestInstance();
      LocationServices.FusedLocationApi.requestLocationUpdates(mClient, req, this);
    }
  }

  private void stopUpdates() {
    LocationServices.FusedLocationApi.removeLocationUpdates(mClient, this);
  }

  private boolean hasPermissions() {
    int isFineGranted = ActivityCompat.checkSelfPermission(mContext, permission.ACCESS_FINE_LOCATION);
    int isCoarseGranted = ActivityCompat.checkSelfPermission(mContext, permission.ACCESS_COARSE_LOCATION);

    return !(isFineGranted != PackageManager.PERMISSION_GRANTED &&
        isCoarseGranted != PackageManager.PERMISSION_GRANTED);
  }

  private String serializeLocationAsString(@Nullable Location location) {
    if (null == location) {
      return "";
    }

    ArrayList<String> items = new ArrayList<>();
    items.add(Double.toString(location.getLatitude()));
    items.add(Double.toString(location.getLongitude()));
    items.add(Double.toString(location.getAltitude()));
    items.add(Float.toString(location.getBearing()));
    items.add(Float.toString(location.getAccuracy()));

    return TextUtils.join(";", items);
  }

  private boolean isUnityEnv() {
    try {
      Class.forName("com.unity3d.player.UnityPlayer");
      return true;
    } catch (ClassNotFoundException e) {
      return false;
    }
  }
}
