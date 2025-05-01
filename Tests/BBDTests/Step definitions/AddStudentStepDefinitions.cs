using System;
using System.Reactive.Subjects;
using AventStack.ExtentReports;
using BackEndAutomation.Rest.Calls;
using BackEndAutomation.Rest.DataManagement;
using BackEndAutomation.Utilities;
using Reqnroll;
using RestSharp;

namespace BackEndAutomation
{
    [Binding]
    public class AddStudentStepDefinitions
    {
        private readonly RestCalls _restCalls;
        private readonly ResponseDataExtractors _extractResponseData;
        private readonly ScenarioContext _scenarioContext;
        private readonly ExtentTest _test;

        public AddStudentStepDefinitions(ScenarioContext scenarioContext, RestCalls restCalls, ResponseDataExtractors extractResponseData)
        {
            _scenarioContext = scenarioContext;
            _restCalls = restCalls;
            _extractResponseData = extractResponseData;
            _test = scenarioContext.Get<ExtentTest>(ContextKeys.ExtentTestKey);
        }
        [When("teacher add student with {string} name and {string} class id.")]
        public void TeacherAddStudent(string studentName, string class_id)
        {
            _test.Info($"Sending request to add student '{studentName}' to class ID '{class_id}'.");

            string token = _scenarioContext.Get<string>(ContextKeys.UserTokenKey);
            RestResponse response = _restCalls.AddStudentCall(studentName, class_id, token);
            string message = _extractResponseData.Extractor(response.Content, JsonIdentifierKeys.MessageKey);
            string studentID = _extractResponseData.Extractor(response.Content, JsonIdentifierKeys.StudentIdKey);
            _scenarioContext.Add(ContextKeys.MessageKey, message);
            _scenarioContext.Add(ContextKeys.StudentNameKey, studentName);
            _scenarioContext.Add(ContextKeys.StudentIdKey, studentID);
            _scenarioContext.Add(ContextKeys.ClassIdKey, class_id);

            Console.WriteLine(response.Content);
            _test.Pass($"Student '{studentName}' (ID: {studentID}) was successfully added to class '{class_id}'. API response: \"{message}\".");
        }

        [Then("validate that student is added {string}.")]
        public void ValidateStudentIsAdded_(string expectedMessage)
        {
            string actualMessage = _scenarioContext.Get<string>(ContextKeys.MessageKey);
            string class_id = _scenarioContext.Get<string>(ContextKeys.ClassIdKey);
            string studentName = _scenarioContext.Get<string>(ContextKeys.StudentNameKey);
            bool isStudentIdExtracted = string.IsNullOrEmpty(_scenarioContext.Get<string>(ContextKeys.StudentIdKey));

            Utilities.UtilitiesMethods.AssertEqual(
                false,
                isStudentIdExtracted,
                $"Failed to extract Student ID. This suggests that student '{studentName}' may not have been successfully added.",
                _scenarioContext);

            Utilities.UtilitiesMethods.AssertEqual(
                expectedMessage,
                actualMessage,
                $"API response did not match expected result while adding student '{studentName}'.",
                _scenarioContext);

            _test.Pass($"Validation passed: Student '{studentName}' has been successfully added to class '{class_id}'.");
        }

        [Then("validate that student is not added {string}.")]
        public void ValidateStudentIsNotAdded_(string expectedMessage)
        {
            string actualMessage = _scenarioContext.Get<string>(ContextKeys.MessageKey);
            string class_id = _scenarioContext.Get<string>(ContextKeys.ClassIdKey);
            string studentName = _scenarioContext.Get<string>(ContextKeys.StudentNameKey);

            Utilities.UtilitiesMethods.AssertEqual(
                expectedMessage,
                actualMessage,
                $"Validation failed: Expected error message when adding student '{studentName}' to class '{class_id}' was not returned.",
                _scenarioContext);

            _test.Pass($"Validation passed: Student '{studentName}' was correctly not added to class '{class_id}' as expected.");
        }

    }
}