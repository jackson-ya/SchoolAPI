Feature: Parent

  Parent authentication and student grades view.

Background:
	Given user signs in with "parent1" username and "parent1" password.

Scenario: Sign in and receive a token
	Then validate that the user is signed in.

@Positive_flow
Scenario Outline: View student grades
	When parent view grades of student with id: "<student_id>".
	Then validate grades are visible.

Examples:
	| student_id                           |
	| 43bac5dc-ecba-4826-8b8d-204cecd07b18 |

@Positive_flow
Scenario Outline: Try to view student grades with invalid id
	When parent view grades of student with id: "<student_id>".
	Then validate student id is invalid "<message>".

Examples:
	| student_id         | message           |
	| invalid-student-id | Student not found |

@Negative_flow
Scenario Outline: Try to view student grades, who isn't connected to parent
	When parent view grades of student with id: "<student_id>".
	Then validate student is not linked to parent "<message>".

Examples:
	| student_id                           | message                              |
	| 5b238374-36b6-477b-9ff0-9333a0b194b1 | You can't view this student's grades |
