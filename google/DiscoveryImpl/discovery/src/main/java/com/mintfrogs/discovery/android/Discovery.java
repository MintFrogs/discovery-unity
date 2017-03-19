package com.mintfrogs.discovery.android;

import android.Manifest.permission;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.content.IntentSender;
import android.content.pm.PackageManager;
import android.location.Location;
import android.os.Build;
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
import com.google.android.gms.common.api.PendingResult;
import com.google.android.gms.common.api.ResultCallback;
import com.google.android.gms.common.api.Status;
import com.google.android.gms.location.LocationListener;
import com.google.android.gms.location.LocationRequest;
import com.google.android.gms.location.LocationServices;
import com.google.android.gms.location.LocationSettingsRequest;
import com.google.android.gms.location.LocationSettingsResult;
import com.google.android.gms.location.LocationSettingsStatusCodes;
import com.unity3d.player.UnityPlayer;

import java.util.ArrayList;
import java.util.Date;

public class Discovery implements ConnectionCallbacks, OnConnectionFailedListener, LocationListener {
  public static final String TAG = Discovery.class.getSimpleName();

  public static final String UNITY_OBJECT = "MfDiscoveryService";
  public static final String UNITY_UPDATE_CALLBACK = "OnInnerLocationUpdate";
  public static final String UNITY_ERROR_CALLBACK = "OnInnerLocationError";
  public static final String UNITY_PERMISSIONS_CALLBACK = "OnInnerLocationPermissionsResult";
  public static final String UNITY_RESOLUTION_CALLBACK = "OnInnerLocationResolutionCallback";

  public static int RESOLUTION_REQUEST = 0x0100;
  public static int PERMISSIONS_REQUEST = 0x0200;

  private static final String PERMISSION_ERROR = "missing-permission";
  private static final String CONNECTION_ERROR = "connection-error";

  private Settings mSettings;
  private GoogleApiClient mClient;
  private Activity mActivity;
  private boolean mStarted;
  private boolean mResolveOnConnect;
  private OnLocationEnabledListener mLastResolveListener;
  private boolean mLastResolveIndicator;

  public interface OnLocationEnabledListener {
    void onLocationEnabledResult(boolean isEnabled);
  }

  public static boolean isUnityEnv() {
    try {
      Class.forName("com.unity3d.player.UnityPlayer");
      return true;
    } catch (ClassNotFoundException e) {
      return false;
    }
  }

  public Discovery(Settings settings, @Nullable Activity atv) {
    mSettings = settings;
    mActivity = null == atv ? UnityPlayer.currentActivity : atv;
    mClient = newClientInstance(mActivity.getApplicationContext());
  }

  public void start() {
    Log.i(TAG, "starting... \\w" + mSettings);

    if (null != mClient) {
      if (mClient.isConnected()) {
        startUpdates();
      } else {
        mClient.connect();
      }
    }
  }

  public void stop() {
    Log.i(TAG, "stopping... \\w" + mSettings);
    stopUpdates();

    if (null != mClient) {
      mClient.disconnect();
    }
  }

  public boolean isStarted() {
    return mStarted;
  }

  public void isLocationServicesEnabled(@Nullable final OnLocationEnabledListener listener, final boolean isResolution) {
    Log.i(TAG, "request-location-services-availability: /listener: " +
        listener + " /isResolve: " + isResolution + " /client: " + mClient.isConnected());

    mLastResolveListener = listener;
    mLastResolveIndicator = isResolution;
    mResolveOnConnect = false;

    if (!mClient.isConnected()) {
      mResolveOnConnect = true;
      mClient.connect();

      return;
    }

    LocationSettingsRequest.Builder builder = new LocationSettingsRequest.Builder();
    builder.addLocationRequest(newLocationRequestInstance());

    PendingResult<LocationSettingsResult> result;
    result = LocationServices.SettingsApi.checkLocationSettings(mClient, builder.build());

    result.setResultCallback(new ResultCallback<LocationSettingsResult>() {
      @Override
      public void onResult(@NonNull LocationSettingsResult settingsResult) {
        Log.i(TAG, "location-result-callback: " + settingsResult);

        Status locationStatus = settingsResult.getStatus();
        int statusCode = locationStatus.getStatusCode();

        if (LocationSettingsStatusCodes.SUCCESS == statusCode) {
          notifyResolutionListeners(listener, true);
        } else if (LocationSettingsStatusCodes.RESOLUTION_REQUIRED == statusCode) {
          try {
            if (isResolution) {
              locationStatus.startResolutionForResult(mActivity, RESOLUTION_REQUEST);
            } else {
              notifyResolutionListeners(listener, false);
            }
          } catch (IntentSender.SendIntentException e) {
            notifyResolutionListeners(listener, false);
          }
        } else {
          notifyResolutionListeners(listener, false);
        }
      }
    });
  }

  @SuppressLint("NewApi")
  public void requestLocationPermissions() {
    if (!hasLocationPermissions()) {
      mActivity.requestPermissions(new String[]{
          permission.ACCESS_FINE_LOCATION, permission.ACCESS_COARSE_LOCATION
      }, PERMISSIONS_REQUEST);
    }
  }

  public boolean hasLocationPermissions() {
    boolean hasPermissions = true;

    if (Build.VERSION.SDK_INT >= 23) {
      int isFineGranted = ActivityCompat.checkSelfPermission(mActivity, permission.ACCESS_FINE_LOCATION);
      int isCoarseGranted = ActivityCompat.checkSelfPermission(mActivity, permission.ACCESS_COARSE_LOCATION);

      hasPermissions = !(isFineGranted != PackageManager.PERMISSION_GRANTED &&
          isCoarseGranted != PackageManager.PERMISSION_GRANTED);
    }

    Log.i(TAG, "has-permissions[" + Build.VERSION.SDK_INT + "]: " + hasPermissions);
    return hasPermissions;
  }

  @SuppressWarnings("MissingPermission")
  public String queryLastLocation() {
    if (hasLocationPermissions()) {
      Location location = LocationServices.FusedLocationApi.getLastLocation(mClient);
      return serializeLocationAsString(location);
    }

    if (isUnityEnv()) {
      UnityPlayer.UnitySendMessage(UNITY_OBJECT, UNITY_ERROR_CALLBACK, PERMISSION_ERROR);
    }

    return "";
  }

  @Override
  public void onConnected(@Nullable Bundle bundle) {
    Log.i(TAG, "connected[r/" + mResolveOnConnect + "]... \\w" + mClient + " bundle: " + bundle);

    if (mResolveOnConnect) {
      isLocationServicesEnabled(mLastResolveListener, mLastResolveIndicator);
    } else {
      startUpdates();
    }
  }

  @Override
  public void onConnectionSuspended(int i) {
    Log.i(TAG, "suspended" + "[" + i + "]" + "... \\w" + mClient);
    stopUpdates();
  }

  @Override
  public void onConnectionFailed(@NonNull ConnectionResult result) {
    Log.i(TAG, "connection failed... \\w" + mClient + " result: " + result);

    if (isUnityEnv()) {
      UnityPlayer.UnitySendMessage(UNITY_OBJECT, UNITY_ERROR_CALLBACK, CONNECTION_ERROR);
    }

    stopUpdates();
  }

  @Override
  public void onLocationChanged(Location location) {
    String args = serializeLocationAsString(location);

    if (isUnityEnv()) {
      long timeStamp = new Date().getTime();
      Log.i(TAG, "onLocationChanged[" + timeStamp + "]: " + args + " value: " + location);
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
    request.setFastestInterval(mSettings.getFastestUpdateInterval());
    request.setPriority(mSettings.getPriority());
    return request;
  }

  @SuppressWarnings("MissingPermission")
  private void startUpdates() {
    if (hasLocationPermissions()) {
      LocationRequest req = newLocationRequestInstance();
      LocationServices.FusedLocationApi.requestLocationUpdates(mClient, req, this);

      mStarted = true;
      Log.i(TAG, "starting-updates: " + req);
    } else {
      if (isUnityEnv()) {
        UnityPlayer.UnitySendMessage(UNITY_OBJECT, UNITY_ERROR_CALLBACK, PERMISSION_ERROR);
      }
    }
  }

  private void stopUpdates() {
    if (mStarted) {
      LocationServices.FusedLocationApi.removeLocationUpdates(mClient, this);
      mStarted = false;
    }
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

  private void notifyResolutionListeners(OnLocationEnabledListener listener, boolean isEnabled) {
    if (null != listener) {
      listener.onLocationEnabledResult(isEnabled);
    } else if (isUnityEnv()) {
      UnityPlayer.UnitySendMessage(UNITY_OBJECT, UNITY_RESOLUTION_CALLBACK, Boolean.toString(isEnabled));
    }
  }
}
