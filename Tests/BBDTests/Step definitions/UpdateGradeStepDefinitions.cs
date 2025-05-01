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
    public class UpdateGradeStepDefinitions
    {
        private readonly RestCalls _restCalls;
        private readonly ResponseDataExtractors _extractResponseData;
        private readonly ScenarioContext _scenarioContext;
        private readonly ExtentTest _test;

        public UpdateGradeStepDefinitions(ScenarioContext scenarioContext, RestCalls restCalls, ResponseDataExtractors extractResponseData)
        {
            _scenarioContext = scenarioContext;
            _restCalls = restCalls;
            _extractResponseData = extractResponseData;
            _test = scenarioContext.Get<ExtentTest>(ContextKeys.ExtentTestKey);
        }
        [Then("teacher update grade to {string}.")]
        public void TeacherUpdateGrade_(int newGrade)
        {
            int grade = _scenarioContext.Get<int>(ContextKeys.GradeKey);
            string student_id = _scenarioContext.Get<string>(ContextKeys.StudentIdKey);
            string subject = _scenarioContext.Get<string> (ContextKeys.SubjectKey);

            _test.Info($"Updating grade from {grade} to {newGrade} for student ID: {student_id} in subject: {subject}.");

            string token = _scenarioContext.Get<string>(ContextKeys.UserTokenKey);
            RestResponse response = _restCalls.AddGradeCall(newGrade, student_id, subject, token);
            string message = _extractResponseData.Extractor(response.Content, JsonIdentifierKeys.MessageKey);
            _scenarioContext[ContextKeys.MessageKey] = message;
            _scenarioContext.Add(ContextKeys.NewGradeKey, newGrade);

            Console.WriteLine(response.Content);
            _test.Pass($"Successfully updated grade from {grade} to {newGrade} for student ID: {student_id} in subject: {subject}. Response message: {message}.");
        }

        [Then("validate that grade is updated {string}.")]
        public void ValidateGradeIsUpdated_(string expectedMessage)
        {
            string actualMessage = _scenarioContext.Get<string>(ContextKeys.MessageKey);
            string student_id = _scenarioContext.Get<string>(ContextKeys.StudentIdKey);
            int grade = _scenarioContext.Get<int>(ContextKeys.GradeKey);
            string subject = _scenarioContext.Get<string>(ContextKeys.SubjectKey);
            int newGrade = _scenarioContext.Get<int>(ContextKeys.NewGradeKey);

            Utilities.UtilitiesMethods.AssertEqual(
                expectedMessage,
                actualMessage,
                $"Grade update failed: Expected message did not match actual when updating grade from {grade} to {newGrade} for student ID: {student_id} in subject: {subject}.",
                _scenarioContext);

            _test.Pass($"Validation passed: Grade successfully updated from {grade} to {newGrade} for student ID: {student_id} in subject: {subject}.");
        }
    }
}