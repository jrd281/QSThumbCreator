# QSThumbCreator
A WPF/C# app used to automate thumbnail creation for QlikSense

## Prerequisites
- The application as of now is only compatible with **Chrome 83**
- A valid QliKSense login will be required to interface with the QRS REST API

## How To Use

1. [Download release 1.1.0.0](https://qsthumbcreator.s3.amazonaws.com/releases/QSThumbCreator+1.1.0.0.zip)
2. Unzip the file
3. Run the .exe file named QSThumbCreator.exe
4. Follow the prompts.

 &nbsp;&nbsp;&nbsp;&nbsp;Throughout the prompts, the user will be presented with two options.
 1. Create the thumbs **only** and save to a local folder
 2. Create the thumbs and push to a selected content library on the QlikSense server
 &nbsp;&nbsp;&nbsp;&nbsp;  Option #2 requires ContentAdmin privileges
 ## To-Do
 -  [ ] Set the thumbnail for an app derived from it''s first sheet
 -  [ ] Set the thumbnail on the sheet itself

 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Both To-Dos, I believe require the usage of the QlikEngine API which seems to be strangely documented

## Incorporated Libraries of Note

- &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[AutoItX.Dotnet](https://www.nuget.org/packages/AutoItX.Dotnet/)
- &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[ModernWpf](https://github.com/Kinnara/ModernWpf)
- &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[qlik_rest_sdk](https://github.com/kolsrud/qlik_rest_sdk)
- &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[Selenium WebDriver](https://www.selenium.dev/)
- &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[Selenium WebDriver ChromeDriver](https://github.com/jsakamoto/nupkg-selenium-webdriver-chromedriver/)

## License

MIT