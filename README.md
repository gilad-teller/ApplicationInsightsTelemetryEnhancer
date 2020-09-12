# Application Insights Telemetry Enhancer
Add data to Microsoft Azure Application Insights

## Features
- Add Application Insights Operation Id as a response header to alll of your app's responses
- Add request and response body to Application Insights Request Telemetry. You will see the body of every incomming API call, and the HTML of every page view.
- Add request and response body to Application Insights Dependency Telemetry. You will see the body of every API call your app makes to external APIs.

## ASP.Net support
Only .Net Core 2.2 and .Net Core 3.1 are currently supported

## Installing
You can search for this package in the Package Manager UI in Visual Studio, or see other installation instructions here:
https://www.nuget.org/packages/ApplicationInsightsTelemetryEnhancer

## Usage
### Operation Id Response Header
To add the operatrion id response header, add this line to the `Configure` method in `Startup.cs`
```C#
app.UseOperationIdHeader();
```

The default header name is `Operation-Id`
In order to customize it, you can add a configuration section to your `appsettings.json` file:
```JSON
"OperationIdHeader": {
  "HeaderName": "HeaderConfig"
}
```
After adding the configuration section, pass it in the following way in `ConfigureServices` method in `Startup.cs`:
```C#
services.AddOperationIdHeader(Configuration.GetSection("OperationIdHeader"));
```
Another way to customize the response header, is directrly in the `ConfigureServices` method:
```C#
services.AddOperationIdHeader(options =>
{
  options.HeaderName = "HeaderOptions";
});
```

### Dependency Telemetry Enhancer
To add the request and response body of every external HTTP call, add this line to the `ConfigureServices` method in `Startup.cs`
```C#
services.AddDependencyTelemetryEnhancer();
```

The default custom dimentions are "Request" and "Response".
In order to customize them, you can add a configuration section to your `appsettings.json` file:
```JSON
"DependencyTelemetryEnhancer": {
    "RequestPropertyKey": "RequestConfig",
    "ResponsePropertyKey": "ResponseConfig"
}
```
After adding the configuration section, pass it in the following way in `ConfigureServices` method in `Startup.cs`:
```C#
services.AddDependencyTelemetryEnhancer(Configuration.GetSection("DependencyTelemetryEnhancer"));
```
Another way to customize the custom dimentions, is directrly in the `ConfigureServices` method:
```C#
services.AddDependencyTelemetryEnhancer(options =>
{
    options.RequestPropertyKey = "RequestOptions";
    options.ResponsePropertyKey = "ResponseOptions";
});
```

### Request Telemetry Enhancer
To add the request and response body of every incomming HTTP call, add this line to the `Configure` method in `Startup.cs`
```C#
app.UseRequestTelemetryEnhancer();
```

The default custom dimentions are "Request" and "Response".
In order to customize them, you can add a configuration section to your `appsettings.json` file:
```JSON
"RequestTelemetryEnhancer": {
    "RequestPropertyKey": "RequestConfig",
    "ResponsePropertyKey": "ResponseConfig"
}
```
After adding the configuration section, pass it in the following way in `ConfigureServices` method in `Startup.cs`:
```C#
services.AddRequestTelemetryEnhancer(Configuration.GetSection("RequestTelemetryEnhancer"));
```
Another way to customize the custom dimentions, is directrly in the `ConfigureServices` method:
```C#
services.AddRequestTelemetryEnhancer(options =>
{
    options.RequestPropertyKey = "RequestOptions";
    options.ResponsePropertyKey = "ResponseOptions";
});
```
