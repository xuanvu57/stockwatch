# stockwatch
The cross-flatform app is being built by .NET with MAUI and C#

### Feature:
- Monitoring a symbol in real-time and alert when the price is higher or lower the threshold
- Analyze the price history to find the potential symbols that match to the expectation
- Save or remove favorite symbols

# Project structure
## stockwatch
- The main project that contains the app UI and handle user behavior
- Include some service that relate to UI and mobile platforms' functions

## Domain
- The project that contains the models of the app

## Application
- The project that contains the business logic of the app
- It contains the models that are used to transfer data between the UI and the domain
- It also includes settings for applications

## Infrastructure
- The project that implements logic code to access the database
- It contains logic code to integrate with 3rd parties

# Integration

## Database
- Data is stored in Firestore Database (Firebase)
- Credential to access Firebase can be found at appsettings.json

## SSI integration
- The app uses Fast Connect API (SSI stock company) to read stock data in realtime
- The API key can be found at appsettings.json

# Emulator

## Access the folder with `FileSystem.AppDataDirectory` in the Android Emulator
- In Visual Studio, go to Tools/Android/Android Adb Command Prompt ...
- Enter `Adb shell`
- Enter `run-as com.txv.stockwatch`
- Enter `cd files`
- Enter `ls` and you should find all your files

### View file with Adb shell command
- Enter `cat your-file-name.ext`