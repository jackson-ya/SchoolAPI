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
    public class ParentViewGradesStepDefinitions
    {
        private readonly RestCalls _restCalls;
        private readonly ResponseDataExtractors _extractResponseData;
        private readonly ScenarioContext _scenarioContext;
        private readonly ExtentTest _test;

        public ParentViewGradesStepDefinitions(ScenarioContext scenarioContext, RestCalls restCalls, ResponseDataExtractors extractResponseData)
        {
            _scenarioContext = scenarioContext;
            _restCalls = restCalls;
            _extractResponseData = extractResponseData;
            _test = scenarioContext.Get<ExtentTest>(ContextKeys.ExtentTestKey);
        }
        [When("parent view grades of student with id: {string}.")]
        public void ParentViewGrades_(string student_id)
        {
            _test.Info($"Parent is attempting to view grades for student with ID: {student_id}.");

            string token = _scenarioContext.Get<string>(ContextKeys.UserTokenKey);
            RestResponse response = _restCalls.ViewGradesCall(student_id, token);
            string allGrades = _extractResponseData.ExtractAllGrades(response.Content);
            string detail = _extractResponseData.Extractor(response.Content, JsonIdentifierKeys.DetailKey);
            _scenarioContext.Add(ContextKeys.StudentIdKey, student_id);
            _scenarioContext.Add(ContextKeys.AllGradesKey, allGrades);
            _scenarioContext.Add(ContextKeys.DetailKey, detail);

            Console.WriteLine(response.Content);
            _test.Pass($"Grades successfully retrieved for student ID: {student_id}. Extracted grades: {allGrades}.");
        }

        [Then("validate grades are visible.")]
        public void ValidateGradesAreVisible_()
        {
            string allGrades = _scenarioContext.Get<string>(ContextKeys.AllGradesKey);
            bool areGradesExtracted = !string.IsNullOrEmpty(_scenarioContext.Get<string>(ContextKeys.AllGradesKey));

            Utilities.UtilitiesMethods.AssertEqual(
                true,
                areGradesExtracted,
                "Grade extraction failed: No grades found or student has no assigned grades.",
                _scenarioContext);

            _test.Pass($"Validation successful: Grades are visible for the student. Grades: {allGrades}.");
        }

        [Then("validate student id is invalid {string}.")]
        public void ValidateStudentIdIsInvalid_(string expectedMessage)
        {
            string actualMessage = _scenarioContext.Get<string>(ContextKeys.DetailKey);

            Utilities.UtilitiesMethods.AssertEqual(
                expectedMessage,
                actualMessage,
                $"Validation failed: Expected error message for invalid student ID not received.",
                _scenarioContext);

            _test.Pass($"Validation successful: System correctly handled invalid student ID. Message: {expectedMessage}.");
        }

        [Then("validate student is not linked to parent {string}.")]
        public void ThenValidateStudentIsNotLinkedToParent_(string expectedMessage)
        {
            string actualMessage = _scenarioContext.Get<string>(ContextKeys.DetailKey);

            Utilities.UtilitiesMethods.AssertEqual(
                expectedMessage,
                actualMessage,
                $"Validation failed: System did not return expected message for unlinked student-parent relation.",
                _scenarioContext);

            _test.Pass($"Validation successful: Student is correctly not linked to the parent. Message: {expectedMessage}.");
        }

    }
}