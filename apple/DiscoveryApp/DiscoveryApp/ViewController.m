//  Created by Sergey Ivonchik on 3/13/17.
//  Copyright © 2017 Sergey Ivonchik. All rights reserved.
//

#import "ViewController.h"
#import "SXDiscovery.h"

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
  [[SXDiscoveryWarpper sharedInstance] start];
  [[SXDiscoveryWarpper sharedInstance] performSelector:@selector(stop) withObject:nil afterDelay:5.0];
}

- (void)viewWillDisappear:(BOOL)animated {
  [[SXDiscoveryWarpper sharedInstance] stop];
}

- (void)didReceiveMemoryWarning {
  [super didReceiveMemoryWarning];
}
@end
