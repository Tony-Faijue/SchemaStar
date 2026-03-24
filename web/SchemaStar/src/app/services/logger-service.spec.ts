import { TestBed } from '@angular/core/testing';

import { LoggerService, LogLevel } from './logger-service';
import { environment } from '../../../environment';

describe('LoggerService', () => {
  let service: LoggerService;
  let originalLogLevel: LogLevel;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LoggerService);
    originalLogLevel = environment.logLevel;  //Get Environment LogLevel
    
    //Spy on console methods and prevent them from displaying in the console
    spyOn(console, 'debug');
    spyOn(console, 'log');
    spyOn(console, 'warn');
    spyOn(console, 'error');
  });

  afterEach(() => {
    environment.logLevel = originalLogLevel; //Set back to the original Environment LogLevel
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should not display DEBUG messages when Log Level is set to WARN ', () => {
    //Arrange
    environment.logLevel = LogLevel.warn;
    //Act
    service.debug("Debug should not show up");
    //Assert
    expect(console.debug).not.toHaveBeenCalled();
  });

   it('should display LOG messages when Log Level is set to INFO ', () => {
    //Arrange
    environment.logLevel = LogLevel.info;
    //Act
    service.log("Log should show up");
    //Assert
    expect(console.log).toHaveBeenCalledWith("Log should show up");
  });

   it('should display ERROR messages when Log Level is set to INFO ', () => {
    //Arrange
    environment.logLevel = LogLevel.info;
    //Act
    service.error("Error should show up");
    //Assert
    expect(console.error).toHaveBeenCalledWith("Error should show up");
  });

  it('should not display ANY messages when Log Level is set to NONE ', () => {
    //Arrange
    environment.logLevel = LogLevel.none;
    //Act
    service.fatal("Fatal not should show up");
    //Assert
    expect(console.error).not.toHaveBeenCalled();
  });
});
