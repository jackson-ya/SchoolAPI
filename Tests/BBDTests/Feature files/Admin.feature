Feature: Admin

  Admin authentication and user management.

Background:
	Given user signs in with "admin1" username and "admin123" password.

Scenario: Sign in and receive a token
	Then validate that the user is signed in.

@Positive_flow
Scenario Outline: Create user
	When admin creates a user with "<username>" username, "<password>" password, and "<role>" role.
	Then validate user is created. "created successfully" 

Examples:
	| title              | username  | password  | role      | 
	| CREATING TEACHER   | brrrrrrs  | teacher11 | teacher   | 
	| CREATING MODERATOR | moderator | moderator | moderator | 
	| CREATING PARENT    | parent1   | parent1   | parent    | 

@Negative_flow
Scenario Outline: Try to create user that exists
	When admin try to create existing user with "<username>" username, "<password>" password, and "<role>" role.
	Then validate user is already created "<message>".

Examples:
	| title            | username  | password  | role    | message                 |
	| CREATING TEACHER | teacher11 | teacher11 | teacher | Username already exists |

@Positive_flow
Scenario Outline: Connecting parent to student
	When admin connect parent "<parent_username>" to student with id: "<student_id>".
	Then validate parent is connected to student "<message>".

Examples:
	| parent_username | student_id                           | message                  |
	| parent1         | 43bac5dc-ecba-4826-8b8d-204cecd07b18 | Parent linked to student |

    

