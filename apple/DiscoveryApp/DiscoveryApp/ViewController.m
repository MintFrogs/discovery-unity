//  Created by Sergey Ivonchik on 3/13/17.
//  Copyright © 2017 Sergey Ivonchik. All rights reserved.
//

#import "ViewController.h"
#import "SXDiscovery.h"

void UnitySendMessage(const char *object, const char *message, const char *args) {
  NSLog(@"Discovery[Dummy]::UnitySendMessage(%s, %s, %s)", object, message, args);
}

@interface ViewController ()
@end

@implementation ViewController
- (void)viewDidLoad {
  [super viewDidLoad];
  
  SXDiscoverySettings *settings = [[SXDiscoverySettings alloc] init];
  settings.updateInterval = 1500;
  settings.accuracy = kCLLocationAccuracyBestForNavigation;
  
  SXDiscoveryWarpper *wrapper = [SXDiscoveryWarpper sharedInstance];
  [wrapper initialize:settings];
}

- (void)viewWillAppear:(BOOL)animated {
  SXDiscoveryWarpper *wapper = [SXDiscoveryWarpper sharedInstance];
  
  if ([wapper isLocationServicesEnabled]) {
    [wapper start];
    [wapper performSelector:@selector(stop) withObject:nil afterDelay:5.0];
  } else {
    NSLog(@"Disabled");
  }
}

- (void)viewWillDisappear:(BOOL)animated {
  [[SXDiscoveryWarpper sharedInstance] stop];
}

- (void)didReceiveMemoryWarning {
  [super didReceiveMemoryWarning];
}
@end
