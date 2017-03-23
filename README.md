# Discrovery

Simple Unity plugin which allows you to easily access iOS/Android location services.
Substitutes `Input.location` to receive more accurate and predictable results and
also manage permissions requests.

## Usage

Advanced usage sequence depends on a platform, because iOS/Android permissions
requests differ from each other, also you always should take into account details about
when you want to request native/in-house permissions from users.

1. Add GameObject to your main scene (or initial loading scene) and add script
`Discovery.cs` to that object

2. Initialize `Discovery` instance

```csharp
Discovery.Instance.Initialize(new Settings());
```

3. Subscribe to main events

```csharp
// Catches location updates with interval specified
// in Settings object while initializing Discovery.
Discovery.Instance.OnLocationUpdate += OnLocationUpdate;

// Fires when something wrong happens durning execution
Discovery.Instance.OnLocationError += OnLocationError;

// Fires in response to QueryLocationServicesEnabled request
Discovery.Instance.OnLocationStatusUpdate += OnLocationStatusUpdate;

// Fires in response to RequestLocationPermissions request
Discovery.Instance.OnLocationPermissionsUpdate += OnLocationPermissionsUpdate;
```

4. Start/Stop instance when you need to receive updates

```csharp
Discovery.Instance.StartUpdates();
Discovery.Instance.StopUpdates();
```

### iOS


TODO


### Android


TODO

## License

Copyright (c) 2017 Sergey Ivonchik

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
