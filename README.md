# AgileEngine Photos Search API

This project is a C# Web API with a Background Worker that regularly pulls the AgileEngine photos and stores them in Memory Cache, so then they can be queried using the following endpoints:

* `/api/images` - Will retrieve the details of every photo stored in the memory cache

* `/api/images/{imageID}` - Will retrieve the details of the photo that matches with the ID sent in the URL

* `/api/search/?author={author}&camera={camera}&tags={tags}` - Will retrieve the details of the photos that match with the querystring parameters. They are all optional, and while **author** and **camera** need to be an exact match, you can introduce multiple **tags** separated by a blank space and the API will retrieve the details of every photo that contains all of the tags you shared. For example: `/api/search?tags=%23photo%20%23today` or `api/search?camera=Olympus%20Tough%20TG-6&tags=%23photooftheday%20%23photography`.

## The Background Worker

There is a background worker setup to refresh the cache every **10 minutes** - You can update this setting from the **appsettings.json** file, by modifiying the property `RefreshCachePeriodInMinutes` to the desired amount of minutes.

All information is pulled directly from the AgileEngine API, and later stored in the [MemoryCache](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.memorycache?view=dotnet-plat-ext-5.0), along with the AuthToken used to authenticate the requests to avoid requesting it again until it expires.

The background worker starts running as soon as the App is launched, so you will need to wait a few minutes until the cache is loaded for the first time.

## How to run the project

You will need to have VisualStudio installed first, clone this repository and update the **appsettings.json** file, by setting the `ApiKey` property with a valid ApiKey to consume the AgileEngine API.

After that, you can run the solution and once it's running, you will see a Web Application that allows you to run and test the endpoints with a user-friendly UI - but you can always send the requests directly using the browser.
