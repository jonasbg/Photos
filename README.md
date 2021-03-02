# Jonas Photo Utility
This is a simple utiliy. Spin it up with docker and mount a volume to `/data`. The image will then give you a list of the images for the first day it will found. For instance, if there is photos from today, only those will be shown. But if theres no photos for a week, then that day last week will be shown.

## How to start
The easiest way is to use the docker image. The image is optimized for Raspberry Pi arm64 architecture.
```bash
docker run -p 8080:8080 -v -d ./pictures:/data jonasbg/photos
```

Go to `localhost:8080/Photos` to retrieve the list of photos found.
Use index to get the image: `localhost:8080/Photos/1` (the index starts at 1)

## Integration
Integrate it with what you would like. Personally I've integrated it with Apple iOS Shortcuts app to show me the latest set of images for a given directory.

[Download iOS Shortcut here](https://www.icloud.com/shortcuts/2fbc1089421e4fd696e3518d84593967)
