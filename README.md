# xamarin-image-size-test
Repro for Images throwing OOM on Xamarin Forms

# Getting Started
1. Open the App.cs in the `ImageBrowser` project and uncomment one of the `MainPage =` statements.
2. Debug on a low memory device to see the issue faster (Lumia 520 is great for this) or a 512 MB emulator image
3. Start scrolling the images down to the bottom

In the default behavior, you'll eventually get an unhanded exception. It may not be an actual OOM but it might say "The image is unrecognized." See the MSDN [docs](http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.media.imaging.bitmapsource.setsourceasync.aspx) that show how this can happen if the app is close to its memory limit.

Switch to the Optimized page and you can scroll to the bottom of the list fine. 
