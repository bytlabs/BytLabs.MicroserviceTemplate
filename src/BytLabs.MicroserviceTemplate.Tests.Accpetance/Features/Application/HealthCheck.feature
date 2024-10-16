Feature: Health Check
  As a developer
  I want to verify the health endpoint of the application
  So that I can ensure the application is running properly

  Scenario: Application health endpoint should return healthy
    Given the application client is initialized
    When I check the health endpoint
    Then the response should indicate the application is healthy
