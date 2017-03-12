package com.mintfrogs.discovery.android;

import android.Manifest.permission;
import android.content.Context;
import android.content.pm.PackageManager;
import android.location.Location;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.v4.app.ActivityCompat;

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.common.api.GoogleApiClient.ConnectionCallbacks;
import com.google.android.gms.common.api.GoogleApiClient.OnConnectionFailedListener;
import com.google.android.gms.location.LocationListener;
import com.google.android.gms.location.LocationRequest;
import com.google.android.gms.location.LocationServices;
import com.unity3d.player.UnityPlayer;

public class Discovery implements ConnectionCallbacks, OnConnectionFailedListener, LocationListener {
  private DiscoverySettings mSettings;
  private GoogleApiClient mClient;
  private Context mContext;

  public Discovery(DiscoverySettings settings) {
    mSettings = settings;
    mContext = UnityPlayer.currentActivity.getApplicationContext();
    mClient = newClientInstance(mContext);
  }

  public void start() {

  }

  public void stop() {

  }

  @SuppressWarnings("MissingPermission")
  public void queryLastLocation() {
    if (hasPermissions()) {
      Location location = LocationServices.FusedLocationApi.getLastLocation(mClient);
    }
  }

  @Override
  public void onConnected(@Nullable Bundle bundle) {

  }

  @Override
  public void onConnectionSuspended(int i) {

  }

  @Override
  public void onConnectionFailed(@NonNull ConnectionResult connectionResult) {

  }

  @Override
  public void onLocationChanged(Location location) {

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
      LocationServices.FusedLocationApi.requestLocationUpdates(mClient, newLocationRequestInstance(), this);
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
}
