//
//  SXUnity.m
//  DiscoveryApp
//
//  Created by Sergey Ivonchik on 3/14/17.
//  Copyright © 2017 Sergey Ivonchik. All rights reserved.
//

#import <Foundation/Foundation.h>

void UnitySendMessage(const char *object, const char *message, const char *args) {
  NSLog(@"Discovery::UnitySendMessage(%s, %s, %s)", object, message, args);
}
