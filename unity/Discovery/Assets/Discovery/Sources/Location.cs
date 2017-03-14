// Copyright (c) 2017 Sergey Ivonchik
// This code is licensed under MIT license (See LICENSE for details)

using System.Collections.Generic;

namespace MintFrogs.Discovery
{
  public class Location
  {
    private readonly double lat;
    private readonly double lng;
    private readonly double altitude;
    private readonly float bearing;
    private readonly float accurency;

    public Location(double lat, double lng, double altitude, float bearing, float accurency)
    {
      this.lat = lat;
      this.lng = lng;
      this.altitude = altitude;
      this.bearing = bearing;
      this.accurency = accurency;
    }

    public static Location Parse(string serializedLocation)
    {
      if (string.IsNullOrEmpty(serializedLocation))
      {
        return null;
      }

      var parts = serializedLocation.Split(';');
      var lat = double.Parse(parts[0]);
      var lng = double.Parse(parts[1]);
      var alt = double.Parse(parts[2]);
      var brg = float.Parse(parts[3]);
      var acc = float.Parse(parts[4]);

      return new Location(lat, lng, alt, brg, acc);
    }

    public static Location Default()
    {
      return new Location(53.9, 27.5667, 0, 0, 0);
    }

    public double Lat
    {
      get { return lat; }
    }

    public double Lng
    {
      get { return lng; }
    }

    public double Altitude
    {
      get { return altitude; }
    }

    public float Bearing
    {
      get { return bearing; }
    }

    public float Accurency
    {
      get { return accurency; }
    }

    public override string ToString()
    {
      var items = new List<string>
      {
        lat.ToString(),
        lng.ToString(),
        altitude.ToString(),
        bearing.ToString(),
        accurency.ToString()
      };

      return string.Join(";", items.ToArray());
    }
  }
}
