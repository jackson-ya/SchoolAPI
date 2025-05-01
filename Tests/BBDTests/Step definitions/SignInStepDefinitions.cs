using System;
using AventStack.ExtentReports;
using BackEndAutomation.Rest.Calls;
using BackEndAutomation.Rest.DataManagement;
using BackEndAutomation.Utilities;
using Reqnroll;
using RestSharp;

namespace BackEndAutomation
{
    [Binding]
    public class UserSignInStepDefinitions
    {
        private readonly RestCalls _restCalls;
        private readonly ResponseDataExtractors _extractResponseData;
        private readonly ScenarioContext _scenarioContext;
        private readonly ExtentTest _test;

        public UserSignInStepDefinitions(ScenarioContext scenarioContext, RestCalls restCalls, ResponseDataExtractors extractResponseData)
        {
            _scenarioContext = scenarioContext;
            _restCalls = restCalls;
            _extractResponseData = extractResponseData;
            _test = scenarioContext.Get<ExtentTest>(ContextKeys.ExtentTestKey);
        }

        [Given("user signs in with {string} username and {string} password.")]
        public void UserSignIn_(string username, string password)
        {
            _test.Info($"Attempting user sign-in with username: {username}.");

            RestResponse response = _restCalls.SignInUserCall(username, password);
            string tokenValue = _extractResponseData.Extractor(response.Content, JsonIdentifierKeys.AccessTokenKey);
            _scenarioContext.Add(ContextKeys.UserTokenKey, tokenValue);
            _scenarioContext.Add(ContextKeys.UserNameKey, username);

            Console.WriteLine(response.Content);
            _test.Pass($"User '{username}' successfully signed in. Access token retrieved.");
        }

        [Then("validate that the user is signed in.")]
        public void ValidateUserIsSignedIn_()
        {
            string username = _scenarioContext.Get<string>(ContextKeys.UserNameKey);
            bool isTokenExtracted = string.IsNullOrEmpty(_scenarioContext.Get<string>(ContextKeys.UserTokenKey));

            Utilities.UtilitiesMethods.AssertEqual(
                false,
                isTokenExtracted,
                $"Sign-in validation failed: Token not extracted or user '{username}' is not signed in.",
                _scenarioContext);

            _test.Pass($"Validation passed: User '{username}' is signed in and token is present.");
        }
    }
}