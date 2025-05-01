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
    public class CreateUserStepDefinitions
    {
        private readonly RestCalls _restCalls;
        private readonly ResponseDataExtractors _extractResponseData;
        private readonly ScenarioContext _scenarioContext;
        private readonly ExtentTest _test;

        public CreateUserStepDefinitions(ScenarioContext scenarioContext, RestCalls restCalls, ResponseDataExtractors extractResponseData)
        {
            _scenarioContext = scenarioContext;
            _restCalls = restCalls;
            _extractResponseData = extractResponseData;
            _test = scenarioContext.Get<ExtentTest>(ContextKeys.ExtentTestKey);
        }

        [When("admin creates a user with {string} username, {string} password, and {string} role.")]
        public void AdminCreateUser_(string username, string password, string role)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            username = $"{username}_{timestamp}";

            _test.Info($"Attempting to create a user with username: {username} and role: {role}.");

            string token = _scenarioContext.Get<string>(ContextKeys.UserTokenKey);
            RestResponse response = _restCalls.CraeteUserCall(username, password, role, token);
            string message = _extractResponseData.Extractor(response.Content, JsonIdentifierKeys.MessageKey);
            _scenarioContext.Add(ContextKeys.MessageKey, message);
            _scenarioContext.Add(ContextKeys.RoleKey, role);
            _scenarioContext[ContextKeys.UserNameKey] = username;

            Console.WriteLine(response.Content);
            _test.Pass($"User '{username}' with role '{role}' created successfully. Response message: {message}.");
        }

        [Then("validate user is created. {string}")]
        public void ValidateUserIsCreated_(string expectedMessage)
        {
            string role = _scenarioContext.Get<string>(ContextKeys.RoleKey);
            string actualMessage = _scenarioContext.Get<string>(ContextKeys.MessageKey);
            string username = _scenarioContext.Get<string>(ContextKeys.UserNameKey);
            expectedMessage = $"{role} '{username}' {expectedMessage}";

            Utilities.UtilitiesMethods.AssertEqual(
                expectedMessage,
                actualMessage,
                $"User creation failed for role '{role}' with username '{username}'.",
                _scenarioContext);

            _test.Pass($"Validation successful: User '{username}' with role '{role}' was created as expected.");
        }

        [When("admin try to create existing user with {string} username, {string} password, and {string} role.")]
        public void AdminTryToCreateExistingUser_(string username, string password, string role)
        {
            _test.Info($"Attempting to create an existing user with username: {username} and role: {role}.");

            string token = _scenarioContext.Get<string>(ContextKeys.UserTokenKey);
            RestResponse response = _restCalls.CraeteUserCall(username, password, role, token);
            string detail = _extractResponseData.Extractor(response.Content, JsonIdentifierKeys.DetailKey);
            _scenarioContext.Add(ContextKeys.DetailKey, detail);
            _scenarioContext.Add(ContextKeys.RoleKey, role);

            Console.WriteLine(response.Content);
            _test.Pass($"Received expected error when trying to create existing user '{username}'. Response message: {detail}.");
        }

        [Then("validate user is already created {string}.")]
        public void ValidateUserIsAlreadyCreated_(string expectedMessage)
        {
            string role = _scenarioContext.Get<string>(ContextKeys.RoleKey);
            string actualMessage = _scenarioContext.Get<string>(ContextKeys.DetailKey);

            Utilities.UtilitiesMethods.AssertEqual(
                expectedMessage,
                actualMessage,
                $"Validation failed: Expected duplicate user creation error for role '{role}', but got a different message.",
                _scenarioContext);

            _test.Pass($"Validation successful: Duplicate user for role '{role}' was correctly identified by the system.");
        }

    }
}