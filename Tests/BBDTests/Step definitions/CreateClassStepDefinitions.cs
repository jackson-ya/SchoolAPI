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
    public class CreateClassStepDefinitions
    {
        private readonly RestCalls _restCalls;
        private readonly ResponseDataExtractors _extractResponseData;
        private readonly ScenarioContext _scenarioContext;
        private readonly ExtentTest _test;

        public CreateClassStepDefinitions(ScenarioContext scenarioContext, RestCalls restCalls, ResponseDataExtractors extractResponseData)
        {
            _scenarioContext = scenarioContext;
            _restCalls = restCalls;
            _extractResponseData = extractResponseData;
            _test = scenarioContext.Get<ExtentTest>(ContextKeys.ExtentTestKey);
        }
        [When("teacher creates a class with {string} classname, {string} subject_1, {string} subject_2 and {string} subject_3.")]
        public void TeacherCreateClass_(string classname, string subject_1, string subject_2, string subject_3)
        {
            _test.Info($"Attempting to create class '{classname}' with subjects: '{subject_1}', '{subject_2}', '{subject_3}'.");

            string token = _scenarioContext.Get<string>(ContextKeys.UserTokenKey);
            RestResponse response = _restCalls.CraeteClassCall(classname, subject_1, subject_2, subject_3, token);
            string message = _extractResponseData.Extractor(response.Content, JsonIdentifierKeys.MessageKey);
            string classID = _extractResponseData.Extractor(response.Content, JsonIdentifierKeys.ClassIdKey);
            _scenarioContext.Add(ContextKeys.MessageKey, message);
            _scenarioContext.Add(ContextKeys.ClassIdKey, classID);
            _scenarioContext.Add(ContextKeys.ClassNameKey, classname);

            Console.WriteLine(response.Content);
            _test.Pass($"Class '{classname}' created successfully. Response message: \"{message}\".");
        }

        [Then("validate class is created {string}.")]
        public void ValidateClassIsCreated_(string expectedMessage)
        {

            string actualMessage = _scenarioContext.Get<string>(ContextKeys.MessageKey);
            string class_id = _scenarioContext.Get<string>(ContextKeys.ClassIdKey);
            string classname = _scenarioContext.Get<string>(ContextKeys.ClassNameKey);
            bool isClassIdExtracted = string.IsNullOrEmpty(_scenarioContext.Get<string>(ContextKeys.ClassIdKey));

            Utilities.UtilitiesMethods.AssertEqual(
                false,
                isClassIdExtracted,
                $"Validation failed: Class ID was not returned, indicating '{classname}' may not have been created.",
                _scenarioContext);

            Utilities.UtilitiesMethods.AssertEqual(
                expectedMessage,
                actualMessage,
                $"Validation failed: Expected message mismatch when creating class '{classname}'.",
                _scenarioContext);

            _test.Pass($"Validation passed: Class '{classname}' successfully created with ID '{class_id}'.");
        }
    }
}
