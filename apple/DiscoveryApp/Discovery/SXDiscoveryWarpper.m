//  Created by Sergey Ivonchik on 3/13/17.
//  Copyright © 2017 Sergey Ivonchik. All rights reserved.
//

#import "SXDiscovery.h"
#import "SXDiscoveryWarpper.h"
#import "SXDiscoverySettings.h"
#import <CoreLocation/CoreLocation.h>

static const char *kUnityObject = "MfDiscoveryService";
static const char *kUnityLocationUpdate = "OnInnerLocationUpdate";
static const char *kUnityLocationError = "OnInnerLocationError";
static const char *kUnityLocationStatusUpdate = "OnInnerLocationResolutionCallback";

static const char *kLocationPermisssionsError = "missing-permission";
static const char *kLocationConnectionError = "connection-error";

@interface SXDiscoveryWarpper () <CLLocationManagerDelegate>
@property (nonatomic, assign) BOOL started;
@property (nonatomic, strong) CLLocationManager *manager;
@property (nonatomic, strong) SXDiscoverySettings *settings;

- (NSString *)locationAsString:(CLLocation *)location;
@end

@implementation SXDiscoveryWarpper
+ (instancetype)sharedInstance
{
  static SXDiscoveryWarpper *instance = nil;
  static dispatch_once_t token;
  
  dispatch_once(&token, ^{
    instance = [[SXDiscoveryWarpper alloc] init];
  });
  
  return instance;
}

- (void)initialize:(SXDiscoverySettings *)settings {
  self.settings = settings;
  
  if (nil == self.manager) {
    CLAuthorizationStatus sx = [CLLocationManager authorizationStatus];
    
    if (kCLAuthorizationStatusDenied == sx || kCLAuthorizationStatusRestricted == sx) {
      NSLog(@"Discovery: has no permissions to location service");
      UnitySendMessage(kUnityObject, kUnityLocationError, kLocationPermisssionsError);
    } else if (kCLAuthorizationStatusNotDetermined == sx) {
      self.manager = [[CLLocationManager alloc] init];
      self.manager.delegate = self;
      self.manager.desiredAccuracy = self.settings.accuracy;
      
      [self.manager requestWhenInUseAuthorization];
    } else {
      self.manager = [[CLLocationManager alloc] init];
      self.manager.delegate = self;
      self.manager.desiredAccuracy = self.settings.accuracy;
    }
  } else {
    if (self.isStarted) {
      [self stop];
      [self initialize:settings];
    }
  }
}

- (void)start {
  if (nil != self.manager) {
    NSLog(@"Discovery: starting...");
    [self.manager startUpdatingLocation];
    self.started = true;
  }
}

- (void)stop {
  if (nil != self.manager) {
    NSLog(@"Discovery: stopping...");
    [self.manager stopUpdatingLocation];
    self.started = false;
  }
}

- (BOOL)isStarted {
  return self.started;
}

- (BOOL)isLocationServicesEnabled {
  BOOL isEnabled = [CLLocationManager locationServicesEnabled];
  UnitySendMessage(kUnityObject, kUnityLocationStatusUpdate, isEnabled ? "true" : "false");
  return isEnabled;
}

- (void)locationManager:(CLLocationManager *)manager didChangeAuthorizationStatus:(CLAuthorizationStatus)sx {
  if (kCLAuthorizationStatusDenied == sx || kCLAuthorizationStatusRestricted == sx) {
    NSLog(@"Discovery: has no permissions to location service");
    UnitySendMessage(kUnityObject, kUnityLocationError, kLocationPermisssionsError);
  } else {
    NSLog(@"Discovery: location status changes -> %d", sx);
  }
}

- (void)locationManager:(CLLocationManager *)manager didUpdateLocations:(NSArray *)locations {
  CLLocation *location = [locations lastObject];
  NSDate *timeStamp = location.timestamp;
  NSTimeInterval tx = [timeStamp timeIntervalSinceNow];
  NSString *locationString = [self locationAsString:location];
  
  NSLog(@"Discovery:didUpdateLocation[%f/%@]-> %@", tx, locationString, location);
  UnitySendMessage(kUnityObject, kUnityLocationUpdate, [locationString UTF8String]);
}

- (void)locationManager:(CLLocationManager *)manager didFailWithError:(NSError *)error {
  self.started = false;
  NSLog(@"Discovery: error->%@", [error description]);
  UnitySendMessage(kUnityObject, kUnityLocationError, kLocationConnectionError);
}

- (NSString *)locationAsString:(CLLocation *)location {
  NSString *lat = [@(location.coordinate.latitude) stringValue];
  NSString *lng = [@(location.coordinate.longitude) stringValue];
  NSString *alt = [@(location.altitude) stringValue];
  NSString *brg = [@(location.course) stringValue];
  NSString *acc = [@(location.verticalAccuracy) stringValue];
  
  NSArray *items = @[lat, lng, alt, brg, acc];
  return [items componentsJoinedByString:@";"];
}
@end
