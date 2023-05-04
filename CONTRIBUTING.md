## Contributing

### Looking for issues to work on?

Issues marked `triage approved` are a good place to start.

You can also check the `help wanted` tag to find other issues to help with.

If you're interested in working on a fix, leave a comment to let everyone know and to help avoid duplicated effort from others.

### Rules

1. **DO** make sure your issue exists before submitting a new one. Your issue might be closed if it's a duplicate.
2. **DO** write a good description for your pull request. More detail is better. Describe why the change is being made and why you have chosen a particular solution. Describe any manual testing you performed to validate your change.
3. **DO** check for additional occurrences of the same problem in other parts of the codebase before submitting your PR.
4. **DO NOT** submit a PR unless it is linked to an Issue marked triage approved. This enables us to have a discussion on the idea before anyone invests time in an implementation.
5. **DO NOT** merge multiple changes into one PR unless they have the same root cause.
6. **DO NOT** submit pure formatting/typo changes to code that has not been modified otherwise.

### Running the app

#### Requirements
- Requires at least version 1809 (build 17763) to run.
- Visual Studio 2019 or later, with Windows 11 SDK for UWP (build 22000)

#### Steps to build

1. Clone the project, this can be done by running `git clone https://github.com/itsWindows11/Carpeddit.git` in your favourite terminal.
2. Launch `Carpeddit.sln` (the solution file).
3. Create a reddit API app by [clicking on this link](https://reddit.com/prefs/apps), scrolling down until you find the "Create application" or "Create another application button". Set the name to `Carpeddit`, the redirect URL to `http://127.0.0.1:3000/reddit-callback` and the type to `web app` and click on "create app". Copy the client ID and secret for use in the next step.
4. Modify the ApiConstants.cs file to include the following, and put the client ID and secret in their respective properties:
   ```
   namespace Carpeddit.Common.Constants
   {
      public static class APIConstants
      {
          public const string ClientId = "INSERT_CLIENT_ID";
          public const string ClientSecret = "INSERT_CLIENT_SECRET";
          public const string RedirectUri = "https://itswin11.netlify.app/apps/carpeddit/auth";
      }
   }
   ```
4. Press F5 or the run button.
