# SSOiFrameDemo
Single Sign On in Azure AD B2C with one remote app in an iframe that can also receive data

## Step 1
In Azure AD B2C add a new App Registrations the MainApp.
1. In App Registrations start a New Registration
2. Name the app something like "MainApp"
3. Select "Web" platform with Redirect URL https://localhost:44331/Auth/LoginCallbackSSO
4. Press "Register"
5. Goto the Authentication section
6. Set the "Front-channel logout URL" to https://localhost:44331/Auth/LogoutCallbackSSO
7. Check "Access tokens"
8. Check "ID tokens"
9. Save

## Step 2
In Azure AD B2C add a new App Registrations the FrameApp.
1. In App Registrations start a New Registration
2. Name the app something like "FrameApp"
3. Select "Web" platform with Redirect URL https://localhost:44331/Auth/LoginCallbackSSO
4. Press "Register"
5. Goto the Authentication section
6. Set the "Front-channel logout URL" to https://localhost:44331/Auth/LogoutCallbackSSO
7. Check "Access tokens"
8. Check "ID tokens"
9. Save

## Step 3
Open appsettings.json in both MainApp and FrameApp and update the parameters for your Azure B2C instance
- AzureDirectoryName
- AzureB2CApplicationID (this is the "Application (client) ID" in the Overview of each App Registration
- AzureB2CPolicyName
- OpenIDLoginCallbackUrl
- OpenIDLogoutCallbackUrl

## Step 4
Set both the MainApp and FrameApp to start at the same time.  In Visual Studio goto the Solution Explorer and right click the solution for a context menu then select "Properties".  Here you can chose "Multiple Startup Projects"
