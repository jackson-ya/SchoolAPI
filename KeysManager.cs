using AventStack.ExtentReports;
using Reqnroll;
using System;

namespace BackEndAutomation.Utilities
{
    public static class ContextKeys
    {
        public const string UserTokenKey = "UserToken";
        public const string UserNameKey = "Username";
        public const string MessageKey = "Message";
        public const string DetailKey = "Detail";
        public const string RoleKey = "Role";
        public const string ClassIdKey = "ClassID";
        public const string ClassNameKey = "ClassName";
        public const string StudentIdKey = "StudentID";
        public const string StudentNameKey = "StudentName";
        public const string GradeKey = "Grade";
        public const string AllGradesKey = "AllGrades";
        public const string SubjectKey = "Subject";
        public const string NewGradeKey = "NewGrade";
        public const string ExtentTestKey = "ExtentTest";
        public const string ParentUsernameKey = "ParentUsername";
    }

    public static class JsonIdentifierKeys
    {
        public const string MessageKey = "message";
        public const string DetailKey = "detail";
        public const string AccessTokenKey = "access_token";
        public const string StudentIdKey = "student_id";
        public const string ClassIdKey = "class_id";
    }
}
