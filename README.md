# stockwatch

## Access the folder with `FileSystem.AppDataDirectory` in the Android Emulator
- In Visual Studio, go to Tools/Android/Android Adb Command Prompt ...
- Enter `Adb shell`
- Enter `run-as com.your.packagename`
- Enter `cd files`
- Enter `ls` and you should find all your files

### View file with Adb shell command
- Enter `cat your-file-name.ext`
